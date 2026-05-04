namespace Catel.MVVM;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Catel.Logging;
using Catel.Services;
using Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// View model base for MVVM implementations. This class is based on the <see cref="ModelBase" />, and supports all
/// common interfaces used by WPF.
/// </summary>
/// <remarks>This view model base does not add any services.</remarks>
public abstract partial class ViewModelBase : ValidatableModelBase, IViewModel
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ViewModelBase));

    /// <summary>
    /// Gets the view model manager.
    /// </summary>
    /// <value>The view model manager.</value>
    protected readonly IViewModelManager _viewModelManager;

    /// <summary>
    /// The backing field for the title property.
    /// </summary>
    private string _title = string.Empty;

    private readonly IObjectIdGenerator<IViewModel, int> _objectIdGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
    /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
    protected ViewModelBase(IServiceProvider serviceProvider)
        : base()
    {
        ServiceProvider = serviceProvider;

        // Hack for now, we need to get rid of this
        _viewModelManager = serviceProvider.GetRequiredService<IViewModelManager>();

        if (CatelEnvironment.IsInDesignMode)
        {
            ViewModelCommandManager = default!;
            _objectIdGenerator = default!;
            return;
        }

        var type = GetType();

        _objectIdGenerator = serviceProvider.GetRequiredService<IObjectIdGenerator<IViewModel, int>>();
        UniqueIdentifier = GetObjectId(_objectIdGenerator);

        Logger.LogDebug("Creating view model of type '{TypeName}' with unique identifier {UniqueIdentifier}", type.Name, BoxingCache.GetBoxedValue(UniqueIdentifier));

        ViewModelCommandManager = new MVVM.ViewModelCommandManager(this);
        ViewModelCommandManager.AddHandler(async (viewModel, propertyName, command, commandParameter) =>
            {
                var eventArgs = new CommandExecutedEventArgs((ICatelCommand)command, commandParameter, propertyName);

                await CommandExecutedAsync.SafeInvokeAsync(this, eventArgs);
            });

        InvalidateCommandsOnPropertyChanged = true;

        _viewModelManager.RegisterViewModelInstance(this);
    }

    /// <summary>
    /// Gets the service provider for this object.
    /// </summary>
    [ExcludeFromValidation]
    protected IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Occurs when the view model has been initialized.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? InitializedAsync;

    /// <summary>
    /// Occurs when a command on the view model has been executed.
    /// </summary>
    public event AsyncEventHandler<CommandExecutedEventArgs>? CommandExecutedAsync;

    /// <summary>
    /// Occurs when the view model is about to be saved.
    /// </summary>
    public event AsyncEventHandler<SavingEventArgs>? SavingAsync;

    /// <summary>
    /// Occurs when the view model is saved successfully.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? SavedAsync;

    /// <summary>
    /// Occurs when the view model is about to be canceled.
    /// </summary>
    public event AsyncEventHandler<CancelingEventArgs>? CancelingAsync;

    /// <summary>
    /// Occurs when the view model is canceled.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? CanceledAsync;

    /// <summary>
    /// Occurs when the view model is being closed.
    /// </summary>
    public event AsyncEventHandler<EventArgs>? ClosingAsync;

    /// <summary>
    /// Occurs when the view model has just been closed.
    /// </summary>
    public event AsyncEventHandler<ViewModelClosedEventArgs>? ClosedAsync;

    /// <summary>
    /// Gets the unique identifier of the view model.
    /// </summary>
    /// <value>The unique identifier.</value>
    [ExcludeFromValidation]
    public int UniqueIdentifier { get; private set; }

    /// <summary>
    /// Gets the <see cref="ViewModelCommandManager"/> of this view model.
    /// </summary>
    /// <value>The <see cref="ViewModelCommandManager"/>.</value>
    [ExcludeFromValidation]
    protected internal IViewModelCommandManager ViewModelCommandManager { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the commands should automatically be invalidated on a property change.
    /// <para />
    /// If this property is <c>false</c>, properties should either be invalidated by the .NET Framework or by a manual
    /// call to the <see cref="IViewModelCommandManager.InvalidateCommands(bool)"/> method.
    /// </summary>
    /// <value>
    /// <c>true</c> if the commands should automatically be invalidated on a property change; otherwise, <c>false</c>.
    /// </value>
    [ExcludeFromValidation]
    protected bool InvalidateCommandsOnPropertyChanged { get; set; }

    /// <summary>
    /// Defines the maximum allowed time used by the save, cancel and close actions of the view model.
    /// <para/>
    /// The default value is <see cref="IViewModelExtensions.ViewModelActionAwaitTimeoutInMilliseconds"/>
    /// </summary>
    [ExcludeFromValidation]
    protected internal int ViewModelActionAwaitTimeoutInMilliseconds { get; set; } = IViewModelExtensions.ViewModelActionAwaitTimeoutInMilliseconds;

    /// <summary>
    /// Gets a value indicating whether this object is currently initializing.
    /// </summary>
    /// <value>
    /// <c>true</c> if this object is currently initializing; otherwise, <c>false</c>.
    /// </value>
    [ExcludeFromValidation]
    protected internal bool IsInitializing { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this object is initialized.
    /// </summary>
    /// <value>
    /// <c>true</c> if this object is initialized; otherwise, <c>false</c>.
    /// </value>
    [ExcludeFromValidation]
    protected internal bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is currently canceling.
    /// </summary>
    [ExcludeFromValidation]
    protected internal bool IsCanceling { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is currently saving.
    /// </summary>
    [ExcludeFromValidation]
    protected internal bool IsSaving { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is closing.
    /// </summary>
    /// <value><c>true</c> if this instance is closing; otherwise, <c>false</c>.</value>
    protected internal bool IsClosing { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
    /// <see cref="CancelViewModelAsync"/>, <see cref="SaveViewModelAsync"/> or <see cref="CloseViewModelAsync"/>
    /// will have no effect.
    /// </summary>
    /// <value><c>true</c> if the view model is closed; otherwise, <c>false</c>.</value>
    [ExcludeFromValidation]
    public bool IsClosed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is saved at least once.
    /// </summary>
    /// <value><c>true</c> if this instance is saved at least once; otherwise, <c>false</c>.</value>
    [ExcludeFromValidation]
    public bool IsSaved { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is canceled at least once.
    /// </summary>
    /// <value><c>true</c> if this instance is canceled at least once; otherwise, <c>false</c>.</value>
    [ExcludeFromValidation]
    public bool IsCanceled { get; private set; }

    /// <summary>
    /// Gets the title of the view model.
    /// </summary>
    /// <value>The title.</value>
    [ExcludeFromValidation]
    public virtual string Title
    {
        get { return _title; }
        protected set
        {
            _title = value;

            RaisePropertyChanged(nameof(Title));
        }
    }

    /// <summary>
    /// Converts the object to a string.
    /// </summary>
    /// <returns>System.String.</returns>
    public override string ToString()
    {
        return $"{GetType().FullName} (ID = {UniqueIdentifier})";
    }

    /// <summary>
    /// Called when a property value has changed.
    /// </summary>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (IsSaving || IsCanceling || IsClosing || IsClosed)
        {
            return;
        }

        if (string.IsNullOrEmpty(e.PropertyName))
        {
            return;
        }

        base.OnPropertyChanged(e);

        if (InvalidateCommandsOnPropertyChanged)
        {
            ViewModelCommandManager.InvalidateCommands();
        }
    }

    /// <summary>
    /// Cancels the editing of the data.
    /// </summary>
    /// <returns>
    /// <c>true</c> if successful; otherwise <c>false</c>.
    /// </returns>
    protected virtual Task<bool> CancelAsync()
    {
        return Task<bool>.FromResult(true);
    }

    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <returns>
    /// <c>true</c> if successful; otherwise <c>false</c>.
    /// </returns>
    protected virtual Task<bool> SaveAsync()
    {
        return Task<bool>.FromResult(true);
    }

    /// <summary>
    /// Called when the view model is about to be closed.
    /// <para />
    /// This method also raises the <see cref="ClosingAsync"/> event.
    /// </summary>
    protected virtual Task OnClosingAsync()
    {
        return ClosingAsync.SafeInvokeAsync(this);
    }

    /// <summary>
    /// Closes this instance. Always called after the <see cref="CancelAsync"/> of <see cref="SaveAsync"/> method.
    /// </summary>
    protected virtual Task CloseAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the view model has just been closed.
    /// <para />
    /// This method also raises the <see cref="ClosedAsync"/> event.
    /// </summary>
    /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
    protected virtual Task OnClosedAsync(bool? result)
    {
        var eventArgs = new ViewModelClosedEventArgs(this, result);
        return ClosedAsync.SafeInvokeAsync(this, eventArgs);
    }

    /// <summary>
    /// Initializes the view model. Normally the initialization is done in the constructor, but sometimes this must be delayed
    /// to a state where the associated UI element (user control, window, ...) is actually loaded.
    /// <para />
    /// This method is called as soon as the associated UI element is loaded.
    /// </summary>
    /// <returns>The task.</returns>
    /// <remarks>It's not recommended to implement the initialization of properties in this method. The initialization of properties
    /// should be done in the constructor. This method should be used to start the retrieval of data from a web service or something
    /// similar.
    /// <para />
    /// During unit tests, it is recommended to manually call this method because there is no external container calling this method.</remarks>
    public async Task InitializeViewModelAsync()
    {
        if (!IsInitializing && !IsInitialized)
        {
            ((IFreezable)this).Unfreeze();

            IsInitializing = true;

            await InitializeAsync();
            await InitializedAsync.SafeInvokeAsync(this);

            IsInitializing = false;
            IsInitialized = true;
        }
    }

    /// <summary>
    /// Initializes the view model. Normally the initialization is done in the constructor, but sometimes this must be delayed
    /// to a state where the associated UI element (user control, window, ...) is actually loaded.
    /// <para />
    /// This method is called as soon as the associated UI element is loaded.
    /// </summary>
    /// <remarks>
    /// It's not recommended to implement the initialization of properties in this method. The initialization of properties
    /// should be done in the constructor. This method should be used to start the retrieval of data from a web service or something
    /// similar.
    /// <para />
    /// During unit tests, it is recommended to manually call this method because there is no external container calling this method.
    /// </remarks>
    protected virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cancels the editing of the data.
    /// </summary>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public async Task<bool> CancelViewModelAsync()
    {
        if (IsClosing || IsClosed)
        {
            return false;
        }

        IsCanceling = true;

        var eventArgs = new CancelingEventArgs();
        await CancelingAsync.SafeInvokeAsync(this, eventArgs);

        if (eventArgs.Cancel)
        {
            Logger.LogDebug("Canceling of view model '{TypeName}' is canceled via the Canceling event", GetType());
            IsCanceling = false;
            return false;
        }

        var cancel = await CancelAsync();

        Logger.LogDebug(cancel ? "Canceled view model '{TypeName}'" : "Failed to cancel view model '{TypeName}'", GetType());
        if (!cancel)
        {
            IsCanceling = false;
            return false;
        }

        Logger.LogDebug("Canceled view model '{TypeName}'", GetType());

        await CanceledAsync.SafeInvokeAsync(this);

        IsCanceled = true;
        IsCanceling = false;

        return true;
    }

    /// <summary>
    /// Saves the data.
    /// </summary>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    public async Task<bool> SaveViewModelAsync()
    {
        if (IsSaving || IsCanceling || IsClosing || IsClosed)
        {
            return false;
        }

        IsSaving = true;

        // Force validation before saving
        Validate(true);

        if (!IsValidationSuspended)
        {
            var validationContext = ((IValidatable)this).ValidationContext;
            if (validationContext.HasErrors)
            {
                IsSaving = false;

                return false;
            }
        }

        var eventArgs = new SavingEventArgs();
        await SavingAsync.SafeInvokeAsync(this, eventArgs);

        if (eventArgs.Cancel)
        {
            IsSaving = false;

            Logger.LogDebug("Saving of view model '{TypeName}' is canceled via the Saving event", GetType());
            return false;
        }

        var saved = await SaveAsync();

        Logger.LogDebug(saved ? "Saved view model '{TypeName}'" : "Failed to save view model '{TypeName}'", GetType());

        if (saved)
        {
            await SavedAsync.SafeInvokeAsync(this);

            IsSaved = true;
        }

        IsSaving = false;

        return saved;
    }

    /// <summary>
    /// Closes this instance. Always called after the <see cref="CancelAsync"/> of <see cref="SaveAsync"/> method.
    /// </summary>
    /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
    public async Task CloseViewModelAsync(bool? result)
    {
        if (IsClosed)
        {
            return;
        }

        IsClosing = true;

        await OnClosingAsync();

        _viewModelManager.UnregisterAllModels(this);

        await CloseAsync();

        // Note: important to set *before* calling the event (the handler might need to check
        // if the vm is closed)
        IsClosing = false;
        IsClosed = true;
        IsInitialized = false;

        ((IFreezable)this).Freeze();

        await OnClosedAsync(result);

        var type = GetType();

        Logger.LogDebug("Closed view model '{TypeName}'", type);

        _viewModelManager.UnregisterViewModelInstance(this);

        _objectIdGenerator.ReleaseIdentifier(UniqueIdentifier);
    }

    /// <summary>
    /// Gets the object id. 
    /// </summary>
    /// <param name="objectIdGenerator">The object id generator</param>
    /// <returns>The object id</returns>
    protected virtual int GetObjectId(IObjectIdGenerator<IViewModel, int> objectIdGenerator)
    {
        return objectIdGenerator.GetUniqueIdentifier();
    }
}
