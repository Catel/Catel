// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Threading.Tasks;
    using Catel.Data;
    using Logging;
    using Threading;

    /// <summary>
    /// Extension methods for <see cref="IViewModel"/>.
    /// </summary>
    public static class IViewModelExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the result of the view model by checking the <see cref="IViewModel.IsSaved"/> and <see cref="IViewModel.IsCanceled"/> properties.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns><c>true</c> if the view model is saved; <c>false</c> if the view model is canceled; otherwise <c>null</c>.</returns>
        public static bool? GetResult(this IViewModel viewModel)
        {
            if (viewModel != null)
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

        /// <summary>
        /// Saves the data, but also closes the view model in the same call if the save succeeds.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public static async Task<bool> SaveAndCloseViewModelAsync(this IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var viewModelBase = viewModel as ViewModelBase;
            if (viewModelBase != null)
            {
                var exitAfterBlock = false;

                if (viewModelBase.IsSaving)
                {
                    exitAfterBlock = true;

                    if (!await viewModelBase.AwaitSavingAsync())
                    {
                        return false;
                    }
                }

                if (viewModelBase.IsClosing)
                {
                    exitAfterBlock = true;

                    await viewModelBase.AwaitClosingAsync();
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
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public static async Task<bool> CancelAndCloseViewModelAsync(this IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var viewModelBase = viewModel as ViewModelBase;
            if (viewModelBase != null)
            {
                var exitAfterBlock = false;

                if (viewModelBase.IsCanceling)
                {
                    exitAfterBlock = true;

                    if (!await viewModelBase.AwaitCancelingAsync())
                    {
                        return false;
                    }
                }

                if (viewModelBase.IsClosing)
                {
                    exitAfterBlock = true;

                    await viewModelBase.AwaitClosingAsync();
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
                Log.Error(ex, $"Failed to await saving of view model '{BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier)}'");
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
                Log.Error(ex, $"Failed to await canceling of view model '{BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier)}'");
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
            var tcs = new TaskCompletionSource<object>();

            var closedHandler = new AsyncEventHandler<ViewModelClosedEventArgs>(async (sender, e) =>
            {
                tcs.TrySetResult(BoxingCache.GetBoxedValue(true));
            });

            viewModel.ClosedAsync += closedHandler;

            try
            {
                await tcs.Task.AwaitWithTimeoutAsync(timeout);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to await closing of view model '{BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier)}'");
                throw;
            }
            finally
            {
                viewModel.ClosedAsync -= closedHandler;
            }
        }
    }
}
