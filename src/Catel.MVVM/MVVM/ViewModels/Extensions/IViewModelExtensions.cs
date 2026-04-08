namespace Catel.MVVM;

using System;
using System.Threading.Tasks;
using Catel.Data;
using Logging;
using Microsoft.Extensions.Logging;
using Threading;

/// <summary>
/// Extension methods for <see cref="IViewModel"/>.
/// </summary>
public static class IViewModelExtensions
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(IViewModelExtensions));

    public static int ViewModelActionAwaitTimeoutInMilliseconds { get; set; } = 50;

    /// <summary>
    /// Determines whether the specified validation summary is outdated by checking the last modified date/time on the validation context.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="lastUpdated">The last updated ticks.</param>
    /// <param name="includeChildViewModelValidations">If set to <c>true</c>, all validation from all child view models should be gathered as well.</param>
    /// <returns><c>true</c> if the validation summary is outdated; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public static bool IsValidationSummaryOutdated(this IViewModel viewModel, long lastUpdated, bool includeChildViewModelValidations)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (((IValidatable)viewModel).ValidationContext.LastModifiedTicks > lastUpdated)
        {
            return true;
        }

        if (includeChildViewModelValidations)
        {
            if (viewModel is FeaturedViewModelBase featuredViewModelBase)
            {
                foreach (var childViewModel in featuredViewModelBase.ChildViewModels)
                {
                    var childAsViewModelBase = childViewModel as FeaturedViewModelBase;
                    if (childAsViewModelBase is not null)
                    {
                        if (IsValidationSummaryOutdated(childAsViewModelBase, lastUpdated, true))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the validation summary for the specified <paramref name="viewModel"/> and, if specified, the children as well.
    /// <para />
    /// This method does not filter on any tag.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="includeChildViewModelValidations">If set to <c>true</c>, all validation from all child view models should be gathered as well.</param>
    /// <returns>The validation summary.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public static IValidationSummary GetValidationSummary(this IViewModel viewModel, bool includeChildViewModelValidations)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var validationContext = GetNestedValidationContext(viewModel, includeChildViewModelValidations);

        return new ValidationSummary(validationContext);
    }

    /// <summary>
    /// Gets the validation summary for the specified <paramref name="viewModel"/> and, if specified, the children as well.
    /// <para/>
    /// This method also filters on the specified tag.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="includeChildViewModelValidations">If set to <c>true</c>, all validation from all child view models should be gathered as well.</param>
    /// <param name="tag">The tag.</param>
    /// <returns>The validation summary.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public static IValidationSummary GetValidationSummary(this IViewModel viewModel, bool includeChildViewModelValidations, object tag)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var validationContext = GetNestedValidationContext(viewModel, includeChildViewModelValidations);

        return new ValidationSummary(validationContext, tag);
    }

    /// <summary>
    /// Gets the nested validation context. If <paramref name="recursive"/> is <c>true</c>, the validation context returned
    /// will include all validation from all registered children.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="recursive">If set to <c>true</c>, the validation context will be merged with all children.</param>
    /// <returns>
    /// A combined <see cref="IValidationContext"/> of all the child view models and the <paramref name="viewModel"/> itself.
    /// </returns>
    /// <remarks>
    /// This method does not check for arguments for performance reasons and because it's private.
    /// </remarks>
    private static IValidationContext GetNestedValidationContext(IViewModel viewModel, bool recursive)
    {
        var validationContext = new ValidationContext();

        validationContext.SynchronizeWithContext(((IValidatable)viewModel).ValidationContext, true);

        if (recursive)
        {
            if (viewModel is FeaturedViewModelBase featuredViewModelBase)
            {
                foreach (var childViewModel in featuredViewModelBase.ChildViewModels)
                {
                    var childAsViewModelBase = childViewModel as FeaturedViewModelBase;
                    if (childAsViewModelBase is not null)
                    {
                        validationContext.SynchronizeWithContext(GetNestedValidationContext(childAsViewModelBase, true), true);
                    }
                }
            }
        }

        return validationContext;
    }

    /// <summary>
    /// Gets the result of the view model by checking the <see cref="IViewModel.IsSaved"/> and <see cref="IViewModel.IsCanceled"/> properties.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <returns><c>true</c> if the view model is saved; <c>false</c> if the view model is canceled; otherwise <c>null</c>.</returns>
    public static bool? GetResult(this IViewModel? viewModel)
    {
        if (viewModel is not null)
        {
            if (viewModel.IsSaved)
            {
                return true;
            }
            else if (viewModel.IsCanceled)
            {
                return false;
            }
        }

        return null;
    }

    private static int GetViewModelActionAwaitTimeout(this IViewModel viewModel)
    {
        var timeout = ViewModelActionAwaitTimeoutInMilliseconds;

        if (viewModel is ViewModelBase viewModelBase)
        {
            timeout = viewModelBase.ViewModelActionAwaitTimeoutInMilliseconds;
        }

        return timeout;
    }

    /// <summary>
    /// Saves the data, but also closes the view model in the same call if the save succeeds.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public static Task<bool> SaveAndCloseViewModelAsync(this IViewModel viewModel)
    {
        return SaveAndCloseViewModelAsync(viewModel, GetViewModelActionAwaitTimeout(viewModel));
    }

    /// <summary>
    /// Saves the data, but also closes the view model in the same call if the save succeeds.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public static async Task<bool> SaveAndCloseViewModelAsync(this IViewModel viewModel, int timeout)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var viewModelBase = viewModel as ViewModelBase;
        if (viewModelBase is not null)
        {
            var exitAfterBlock = false;

            if (viewModelBase.IsSaving)
            {
                exitAfterBlock = true;

                if (!await viewModelBase.AwaitSavingAsync(timeout))
                {
                    return false;
                }
            }

            if (viewModelBase.IsClosing)
            {
                exitAfterBlock = true;

                await viewModelBase.AwaitClosingAsync(timeout);
            }

            if (exitAfterBlock)
            {
                return true;
            }
        }

        var result = await viewModel.SaveViewModelAsync();
        if (result)
        {
            await viewModel.CloseViewModelAsync(true);
        }

        return result;
    }

    /// <summary>
    /// Cancels the editing of the data, but also closes the view model in the same call.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public static Task<bool> CancelAndCloseViewModelAsync(this IViewModel viewModel)
    {
        return CancelAndCloseViewModelAsync(viewModel, GetViewModelActionAwaitTimeout(viewModel));
    }

    /// <summary>
    /// Cancels the editing of the data, but also closes the view model in the same call.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public static async Task<bool> CancelAndCloseViewModelAsync(this IViewModel viewModel, int timeout)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var viewModelBase = viewModel as ViewModelBase;
        if (viewModelBase is not null)
        {
            var exitAfterBlock = false;

            if (viewModelBase.IsCanceling)
            {
                exitAfterBlock = true;

                if (!await viewModelBase.AwaitCancelingAsync(timeout))
                {
                    return false;
                }
            }

            if (viewModelBase.IsClosing)
            {
                exitAfterBlock = true;

                await viewModelBase.AwaitClosingAsync(timeout);
            }

            if (exitAfterBlock)
            {
                return true;
            }
        }

        var result = await viewModel.CancelViewModelAsync();
        if (result)
        {
            await viewModel.CloseViewModelAsync(false);
        }

        return result;
    }

    /// <summary>
    /// Awaits the saving of a the <see cref="ViewModelBase" />. This method should be used with care, and can hook into
    /// an existing save operation called on the <see cref="ViewModelBase" />.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The result of the external call to <see cref="ViewModelBase.SaveAsync"/>.</returns>
    public static async Task<bool> AwaitSavingAsync(this ViewModelBase viewModel, int timeout = 50)
    {
        // We should somehow have the task here to await, but we don't want to add a `SavingTask`
        // on the vm, so we will listen to events
        //
        // There is still a chance that we get a leak in case of:
        // 1. VM starts saving => IsSaving becomes true => Validation fails => no saving / saved events
        // 2. VM starts saving => IsSaving becomes true => Saving fails (exception or something)
        //
        // To "solve" this, we'll give the VM only 50ms to save itself, which is extremely reasonable
        var tcs = new TaskCompletionSource<bool>();

        var savingHandler = new AsyncEventHandler<SavingEventArgs>(async (sender, e) =>
        {
            if (e.Cancel)
            {
                tcs.TrySetResult(false);
            }
        });

        var savedHandler = new AsyncEventHandler<EventArgs>(async (sender, e) =>
        {
            tcs.TrySetResult(true);
        });

        viewModel.SavingAsync += savingHandler;
        viewModel.SavedAsync += savedHandler;

        try
        {
            await tcs.Task.AwaitWithTimeoutAsync(timeout);
            return tcs.Task.Result;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, $"Failed to await saving of view model '{viewModel.GetType().Name}', ID = '{viewModel.UniqueIdentifier}'");
            throw;
        }
        finally
        {
            viewModel.SavingAsync -= savingHandler;
            viewModel.SavedAsync -= savedHandler;
        }
    }

    /// <summary>
    /// Awaits the canceling of a the <see cref="ViewModelBase" />. This method should be used with care, and can hook into
    /// an existing cancel operation called on the <see cref="ViewModelBase" />.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The result of the external call to <see cref="ViewModelBase.CancelAsync"/>.</returns>
    public static async Task<bool> AwaitCancelingAsync(this ViewModelBase viewModel, int timeout = 50)
    {
        // We should somehow have the task here to await, but we don't want to add a `CancelingTask`
        // on the vm, so we will listen to events
        //
        // To "solve" this, we'll give the VM only 50ms to save itself, which is extremely reasonable
        var tcs = new TaskCompletionSource<bool>();

        var cancelingHandler = new AsyncEventHandler<CancelingEventArgs>(async (sender, e) =>
        {
            if (e.Cancel)
            {
                tcs.TrySetResult(false);
            }
        });

        var canceledHandler = new AsyncEventHandler<EventArgs>(async (sender, e) =>
        {
            tcs.TrySetResult(true);
        });

        viewModel.CancelingAsync += cancelingHandler;
        viewModel.CanceledAsync += canceledHandler;

        try
        {
            await tcs.Task.AwaitWithTimeoutAsync(timeout);
            return tcs.Task.Result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to await canceling of view model '{viewModel.GetType().Name}', ID = '{viewModel.UniqueIdentifier}'");
            throw;
        }
        finally
        {
            viewModel.CancelingAsync -= cancelingHandler;
            viewModel.CanceledAsync -= canceledHandler;
        }
    }

    /// <summary>
    /// Awaits the closing of a the <see cref="ViewModelBase" />. This method should be used with care, and can hook into
    /// an existing close operation called on the <see cref="ViewModelBase" />.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="timeout">The timeout.</param>
    public static async Task AwaitClosingAsync(this ViewModelBase viewModel, int timeout = 50)
    {
        // We should somehow have the task here to await, but we don't want to add a `ClosingTask`
        // on the vm, so we will listen to events
        //
        // To "solve" this, we'll give the VM only 50ms to save itself, which is extremely reasonable
        var tcs = new TaskCompletionSource<bool>();

        var closedHandler = new AsyncEventHandler<ViewModelClosedEventArgs>(async (sender, e) =>
        {
            tcs.TrySetResult(true);
        });

        viewModel.ClosedAsync += closedHandler;

        try
        {
            await tcs.Task.AwaitWithTimeoutAsync(timeout);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to await closing of view model '{viewModel.GetType().Name}', ID = '{viewModel.UniqueIdentifier}'");
            throw;
        }
        finally
        {
            viewModel.ClosedAsync -= closedHandler;
        }
    }
}
