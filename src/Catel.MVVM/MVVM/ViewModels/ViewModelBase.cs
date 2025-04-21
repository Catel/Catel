﻿namespace Catel.MVVM
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Auditing;
    using Collections;
    using Data;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Reflection;
    using Services;

    /// <summary>
    /// View model base for MVVM implementations. This class is based on the <see cref="ModelBase" />, and supports all
    /// common interfaces used by WPF.
    /// </summary>
    /// <remarks>This view model base does not add any services.</remarks>
    public abstract partial class ViewModelBase : ValidatableModelBase, IRelationalViewModel
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary containing the view model metadata of a view model type so it has to be calculated only once.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ViewModelMetadata> _metaData = new ConcurrentDictionary<Type, ViewModelMetadata>();

        /// <summary>
        /// The object adapter responsible for the property mappings.
        /// </summary>
        protected readonly IObjectAdapter _objectAdapter;

        /// <summary>
        /// The dispatcher service used to dispatch all calls.
        /// </summary>
        private readonly IDispatcherService _dispatcherService;

        /// <summary>
        /// Value indicating whether the multiple modules warning should be ignored.
        /// </summary>
        private readonly bool _ignoreMultipleModelsWarning;

        /// <summary>
        /// Value indicating whether the view model attributes are initialized. 
        /// </summary>
        private bool _areViewModelAttributesInitialized;

        private readonly object _modelLock = new object();

        /// <summary>
        /// Dictionary of available models inside the view model.
        /// </summary>
        private readonly Dictionary<string, object?> _modelObjects = new Dictionary<string, object?>();

        /// <summary>
        /// Dictionary with info about the available models inside the view model.
        /// </summary>
        private readonly Dictionary<string, ModelInfo> _modelObjectsInfo = new Dictionary<string, ModelInfo>();

        /// <summary>
        /// Dictionary with data error info about a specific model.
        /// </summary>
        private readonly Dictionary<string, ModelErrorInfo> _modelErrorInfo = new Dictionary<string, ModelErrorInfo>();

        /// <summary>
        /// List of child view models which can be registered by the <c>RegisterChildViewModel</c> method.
        /// </summary>
        internal readonly List<IViewModel> ChildViewModels = new List<IViewModel>();

        /// <summary>
        /// Value to determine whether child view models have errors or not.
        /// </summary>
        private bool _childViewModelsHaveErrors;

        /// <summary>
        /// Gets the view model manager.
        /// </summary>
        /// <value>The view model manager.</value>
        protected readonly IViewModelManager _viewModelManager;

        /// <summary>
        /// Mappings from view model properties to models and their properties.
        /// </summary>
        private readonly Dictionary<string, ViewModelToModelMapping> _viewModelToModelMap = new Dictionary<string, ViewModelToModelMapping>();

        private readonly AuditingWrapper? _auditingWrapper;

        /// <summary>
        /// The backing field for the title property.
        /// </summary>
        private string _title;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected ViewModelBase(IServiceProvider serviceProvider)
            : this(serviceProvider, true, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// <para/>
        /// This constructor allows the injection of a custom <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to inject.</param>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="InitializeViewModelAttributes"/>.</param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ViewModelBase(IServiceProvider serviceProvider, bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base()
        {
            _viewModelManager = serviceProvider.GetRequiredService<IViewModelManager>();
            _dispatcherService = serviceProvider.GetRequiredService<IDispatcherService>();

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            _ignoreMultipleModelsWarning = ignoreMultipleModelsWarning;

            _throttlingTimer = new Windows.Threading.DispatcherTimerEx(_dispatcherService);

            var type = GetType();


            Log.Debug("Creating view model of type '{0}' with unique identifier {1}", type.Name, BoxingCache.GetBoxedValue(UniqueIdentifier));

            ValidateModelsOnInitialization = true;

            ViewModelCommandManager = MVVM.ViewModelCommandManager.Create(this);
            ViewModelCommandManager.AddHandler(async (viewModel, propertyName, command, commandParameter) =>
                {
                    var eventArgs = new CommandExecutedEventArgs((ICatelCommand)command, commandParameter, propertyName);

                    await CommandExecutedAsync.SafeInvokeAsync(this, eventArgs);
                });

            DeferValidationUntilFirstSaveCall = true;
            InvalidateCommandsOnPropertyChanged = true;
            SupportIEditableObject = supportIEditableObject;

            // Temporarily suspend validation
            using (SuspendValidations(false))
            {
                if (!skipViewModelAttributesInitialization)
                {
                    InitializeViewModelAttributes();
                }

                _viewModelManager.RegisterViewModelInstance(this);

                InitializeThrottling();
            }

            // As a last step, enable the auditors (we don't need change notifications of previous properties, etc)
            var auditingManager = serviceProvider.GetService<IAuditingManager>();
            if (auditingManager is not null)
            {
                _auditingWrapper = new AuditingWrapper(auditingManager, serviceProvider.GetRequiredService<IObjectAdapter>(), this);
            }
        }

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
        /// Gets the service provider for this object.
        /// </summary>
        [ExcludeFromValidation]
        protected IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Gets the parent view model.
        /// </summary>
        /// <value>The parent view model.</value>
        [ExcludeFromValidation]
        public IViewModel? ParentViewModel { get; private set; }

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
        /// Gets or sets a value indicating whether models that implement <see cref="IEditableObject"/> are supported correctly.
        /// </summary>
        /// <value>
        /// <c>true</c> if models that implement <see cref="IEditableObject"/> are supported correctly; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        private bool SupportIEditableObject { get; set; }

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
        /// Gets a value indicating whether this object contains any field or business errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        public override bool HasErrors
        {
            get
            {
                if (base.HasErrors)
                {
                    return true;
                }

                if (_childViewModelsHaveErrors)
                {
                    return true;
                }

                return false;
            }
        }

        partial void InitializeThrottling();

        partial void UninitializeThrottling();

        /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToString()
        {
            return $"{GetType().FullName} (ID = {UniqueIdentifier})";
        }

        /// <summary>
        /// Initializes the properties with attributes.
        /// </summary>
        private void InitializePropertiesWithAttributes()
        {
            var viewModelType = GetType();

            var metaData = GetViewModelMetaData(viewModelType);

            lock (_modelLock)
            {
                _modelObjectsInfo.AddRange(metaData.Models);
            }

            _viewModelToModelMap.AddRange(metaData.Mappings);
        }

        /// <summary>
        /// Initializes the view model meta data.
        /// <para />
        /// This method only initializes the meta data once per view model type. If a type is already initialized,
        /// this method will immediately return.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>ViewModelMetadata.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        private ViewModelMetadata GetViewModelMetaData(Type viewModelType)
        {
            return _metaData.GetOrAdd(viewModelType, CreateViewModelMetaData);
        }

        private ViewModelMetadata CreateViewModelMetaData(Type viewModelType)
        {
            var properties = new List<PropertyInfo>();
            var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(true, false, true);
            properties.AddRange(viewModelType.GetPropertiesEx(bindingFlags));

            var modelObjectsInfo = new Dictionary<string, ModelInfo>();
            var viewModelToModelMap = new Dictionary<string, ViewModelToModelMapping>();

            var modelNames = (from propertyInfo in properties
                              where propertyInfo.IsDecoratedWithAttribute<ModelAttribute>()
                              select propertyInfo.Name).ToList();

            // Important: iterate twice, models first, they are needed to get the model type
            foreach (var propertyInfo in properties)
            {
                var modelAttribute = propertyInfo.GetCustomAttributeEx<ModelAttribute>(true);
                if (modelAttribute is not null)
                {
                    modelObjectsInfo.Add(propertyInfo.Name, new ModelInfo(propertyInfo, modelAttribute));
                }
            }

            // View model to model mappings
            foreach (var propertyInfo in properties)
            {
                var viewModelToModelAttribute = propertyInfo.GetCustomAttributeEx<ViewModelToModelAttribute>(true);
                if (viewModelToModelAttribute is not null)
                {
                    if (string.IsNullOrEmpty(viewModelToModelAttribute.Property))
                    {
                        // Assume the property name in the model is the same as in the view model
                        viewModelToModelAttribute.Property = propertyInfo.Name;
                    }

                    if (string.IsNullOrWhiteSpace(viewModelToModelAttribute.Model))
                    {
                        Log.Debug("ViewToViewModel is missing the Model name, searching for the right model automatically");

                        if (modelNames.Count != 1)
                        {
                            throw Log.ErrorAndCreateException<InvalidOperationException>($"It is only possible to automatically select the right model if there is 1 model. There are '{modelNames.Count.ToString()}' models, so please specify the model name explicitly.");
                        }

                        viewModelToModelAttribute.Model = modelNames[0];
                    }

                    if (!viewModelToModelMap.ContainsKey(propertyInfo.Name))
                    {
                        var modelProperty = modelObjectsInfo[viewModelToModelAttribute.Model];

                        viewModelToModelMap.Add(propertyInfo.Name, new ViewModelToModelMapping(ServiceProvider, propertyInfo, modelProperty.PropertyType, viewModelToModelAttribute));
                    }
                }
            }

            // Validation (1 time for all view models of the same type)
            foreach (var viewModelToModelMapping in viewModelToModelMap)
            {
                var mapping = viewModelToModelMapping.Value;
                if (!modelObjectsInfo.ContainsKey(mapping.ModelProperty))
                {
                    throw Log.ErrorAndCreateException(msg => new ModelNotRegisteredException(mapping.ModelProperty, mapping.ViewModelProperty),
                        "There is no model '{0}' registered with the model attribute, so the ViewModelToModel attribute on property '{1}' is invalid",
                        mapping.ModelProperty, mapping.ViewModelProperty);
                }

                var viewModelPropertyType = viewModelToModelMapping.Value.ViewModelPropertyType;
                var modelPropertyType = viewModelToModelMapping.Value.ModelPropertyType;
                var modelPropertyPropertyTypes = new Type[mapping.ValueProperties.Length];

                for (var i = 0; i < mapping.ValueProperties.Length; i++)
                {
                    var valueProperty = mapping.ValueProperties[i];

                    var modelPropertyPropertyInfo = modelPropertyType.GetPropertyEx(valueProperty);
                    if (modelPropertyPropertyInfo is null)
                    {
                        Log.Warning("Mapped viewmodel property '{0}' to model property '{1}' is invalid because property '{1}' is not found on the model '{2}'.\n\n" +
                                "If the property is defined in a sub-interface, reflection does not return it as a valid property. If this is the case, you can safely ignore this warning",
                            mapping.ViewModelProperty, valueProperty, mapping.ModelProperty);

                        modelPropertyPropertyTypes[i] = typeof(object);
                    }
                    else
                    {
                        modelPropertyPropertyTypes[i] = modelPropertyPropertyInfo.PropertyType;
                    }
                }

                if (!mapping.Converter.CanConvert(modelPropertyPropertyTypes, viewModelPropertyType, viewModelType))
                {
                    Log.Warning("Property '{0}' mapped on model properties '{1}' cannot be converted via given converter '{2}'",
                        mapping.ViewModelProperty, string.Join(", ", mapping.ValueProperties), mapping.ConverterType);
                }
            }

            return new ViewModelMetadata(viewModelType, modelObjectsInfo, viewModelToModelMap);
        }

        /// <summary>
        /// Initializes the view model attributes, such as the <see cref="ModelAttribute"/> and
        /// <see cref="ViewModelToModelAttribute"/>.
        /// <para />
        /// This method is automatically invoked by the constructor. Sometimes, dynamic properties
        /// are registered after the constructor. Therefore, it is possible to skip the initialization
        /// of the attributes and handle this manually.
        /// </summary>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected void InitializeViewModelAttributes()
        {
            if (_areViewModelAttributesInitialized)
            {
                return;
            }

            _areViewModelAttributesInitialized = true;

            using (SuspendValidations(false))
            {
                InitializePropertiesWithAttributes();

                lock (_modelLock)
                {
                    foreach (var modelInfo in _modelObjectsInfo)
                    {
                        _modelObjects.Add(modelInfo.Key, null);
                    }
                }

                if (SupportIEditableObject)
                {
                    lock (_modelLock)
                    {
                        foreach (var modelKeyValuePair in _modelObjects)
                        {
                            if (_modelObjectsInfo[modelKeyValuePair.Key].SupportIEditableObject)
                            {
                                if (modelKeyValuePair.Value is not null)
                                {
                                    EditableObjectHelper.BeginEditObject(modelKeyValuePair.Value);
                                }
                            }
                        }
                    }
                }

                if (!_ignoreMultipleModelsWarning)
                {
                    lock (_modelLock)
                    {
                        if (_modelObjects.Count > 1)
                        {
                            Log.Warning("The view model {0} implements {1} models.\n\n" +
                                        "Normally, a view model only implements 1 model so make sure you are using the Model attribute correctly. If the Model attribute is used correctly (on models only), this warning can be ignored by using a constructor overload.",
                                GetType().Name, BoxingCache.GetBoxedValue(_modelObjects.Count));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the new parent view model of this view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        void IRelationalViewModel.SetParentViewModel(IViewModel? parentViewModel)
        {
            if (!ObjectHelper.AreEqualReferences(ParentViewModel, parentViewModel))
            {
                ParentViewModel = parentViewModel;

                var parentVm = parentViewModel as ViewModelBase;
                if (parentVm is not null)
                {
                    var value = DeferValidationUntilFirstSaveCall;
                    var parentVmValue = parentVm.DeferValidationUntilFirstSaveCall;

                    if (value != parentVmValue)
                    {
                        Log.Debug($"DeferValidationUntilFirstCall is '{BoxingCache.GetBoxedValue(value)}', but overriding value using parent view model value '{BoxingCache.GetBoxedValue(parentVmValue)}'");

                        DeferValidationUntilFirstSaveCall = parentVmValue;
                    }
                }

                RaisePropertyChanged(nameof(ParentViewModel));
            }
        }

        /// <summary>
        /// Registers a child view model to this view model. When a view model is registered as a child view model, it will
        /// receive all notifications from this view model and be notified of any validation changes.
        /// </summary>
        /// <param name="childViewModel">The child view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="childViewModel"/> is <c>null</c>.</exception>
        void IRelationalViewModel.RegisterChildViewModel(IViewModel childViewModel)
        {
            ArgumentNullException.ThrowIfNull(childViewModel);

            lock (ChildViewModels)
            {
                if (ObjectHelper.AreEqualReferences(this, childViewModel))
                {
                    return;
                }

                var childViewModels = ChildViewModels;
                if (!childViewModels.Contains(childViewModel))
                {
                    childViewModels.Add(childViewModel);

                    childViewModel.PropertyChanged += OnChildViewModelPropertyChanged;
                    childViewModel.ClosedAsync += OnChildViewModelClosedAsync;
                }

                var validate = false;

                // The ViewModelBase.HasErrors has a diff implementation than IModelValidation, this might (or should) be changed
                // but this is the easiest and most reliable way to make it work now
                var viewModelBase = childViewModel as ViewModelBase;
                if (viewModelBase is not null && viewModelBase.HasErrors)
                {
                    validate = true;
                }
                else
                {
                    var validationContext = ((IValidatableModel)childViewModel).ValidationContext;
                    if (validationContext is not null)
                    {
                        if (validationContext.HasErrors || validationContext.HasWarnings)
                        {
                            validate = true;
                        }
                    }
                }

                if (validate)
                {
                    Validate();
                }
            }
        }

        /// <summary>
        /// Called when a property has changed on the child view model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
#pragma warning disable CTL0003 // Fix method name to match some property raising NotifyPropertyChanged event
        private void OnChildViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
#pragma warning restore CTL0003 // Fix method name to match some property raising NotifyPropertyChanged event
        {
            if (e.PropertyName == nameof(HasErrors) || e.PropertyName == nameof(HasWarnings))
            {
                Validate();
            }

            if (InvalidateCommandsOnPropertyChanged)
            {
                ViewModelCommandManager.InvalidateCommands();
            }
        }

        /// <summary>
        /// Called when the child view model is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private Task OnChildViewModelClosedAsync(object? sender, EventArgs e)
        {
            var viewModel = sender as IViewModel;
            if (viewModel is not null)
            {
                ((IRelationalViewModel)this).UnregisterChildViewModel(viewModel);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Unregisters the child view model. This means that the child view model will no longer receive any notifications
        /// from this view model as parent view model, nor will it be included in any validation calls in this view model.
        /// </summary>
        /// <param name="childViewModel">The child.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="childViewModel"/> is <c>null</c>.</exception>
        void IRelationalViewModel.UnregisterChildViewModel(IViewModel childViewModel)
        {
            ArgumentNullException.ThrowIfNull(childViewModel);

            lock (ChildViewModels)
            {
                var index = ChildViewModels.IndexOf(childViewModel);
                if (index == -1)
                {
                    return;
                }

                childViewModel.PropertyChanged -= OnChildViewModelPropertyChanged;
                childViewModel.ClosedAsync -= OnChildViewModelClosedAsync;

                ChildViewModels.RemoveAt(index);

                // #2036: revalidate
                Validate();
            }
        }

        /// <summary>
        /// Gets the child view models of this view model.
        /// </summary>
        /// <returns>An enumerable of current child view models.</returns>
        protected IEnumerable<IViewModel> GetChildViewModels()
        {
            lock (ChildViewModels)
            {
                return ChildViewModels.ToArray();
            }
        }

        /// <summary>
        /// Gets all models that are decorated with the <see cref="ModelAttribute"/>.
        /// </summary>
        /// <returns>Array of models.</returns>
        protected object[] GetAllModels()
        {
            lock (_modelLock)
            {
                return _modelObjects.Values.Where(x => x is not null).Cast<object>().ToArray();
            }
        }

        /// <summary>
        /// Updates the view model to model mappings that are defined as <see cref="ViewModelToModelMode.Explicit"/>.
        /// </summary>
        protected void UpdateExplicitViewModelToModelMappings()
        {
            Log.Debug("Updating all explicit view model to model mappings");

            var explicitMappings = (from mapping in _viewModelToModelMap
                                    where mapping.Value.Mode == ViewModelToModelMode.Explicit
                                    select mapping.Value);

            foreach (var mapping in explicitMappings)
            {
                var model = _modelObjects[mapping.ModelProperty];
                if (model is not null)
                {
                    var value = GetValue<object>(mapping.ViewModelProperty);
                    var modelValues = mapping.Converter.ConvertBack(value, this);
                    for (var i = 0; i < mapping.ValueProperties.Length; i++)
                    {
                        if (_objectAdapter.TrySetMemberValue(model, mapping.ValueProperties[i], modelValues[i]))
                        {
                            Log.Debug("Updated property '{0}' on model type '{1}' to '{2}'", mapping.ValueProperties, model.GetType().Name, ObjectToStringHelper.ToString(value));
                        }
                        else
                        {
                            Log.Warning("Failed to set property '{0}' on model type '{1}'", mapping.ValueProperties, model.GetType().Name);
                        }
                    }
                }
            }

            Log.Debug("Updated all explicit view model to model mappings");
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

            lock (_modelLock)
            {
                if (_modelObjects.ContainsKey(e.PropertyName))
                {
                    // Clean up old model
                    var oldModelValue = _modelObjects[e.PropertyName];
                    if (oldModelValue is not null)
                    {
                        UninitializeModelInternal(e.PropertyName, oldModelValue, ModelCleanUpMode.CancelEdit);
                    }

                    var newModelValue = GetValue<object?>(e.PropertyName);
                    _modelObjects[e.PropertyName] = newModelValue;

                    if (newModelValue is not null)
                    {
                        InitializeModelInternal(e.PropertyName, newModelValue);
                    }

                    // Since the model has been changed, copy all values from the model to the view model
                    foreach (var viewModelToModelMap in _viewModelToModelMap)
                    {
                        var mapping = viewModelToModelMap.Value;
                        var converter = mapping.Converter;
                        if (string.CompareOrdinal(mapping.ModelProperty, e.PropertyName) == 0)
                        {
                            var values = new object?[mapping.ValueProperties.Length];

                            if (newModelValue is not null)
                            {
                                // We have a new model, ignore OneWayToSource
                                if (mapping.Mode == ViewModelToModelMode.OneWayToSource)
                                {
                                    continue;
                                }

                                for (var index = 0; index < mapping.ValueProperties.Length; index++)
                                {
                                    var property = mapping.ValueProperties[index];

                                    if (_objectAdapter.TryGetMemberValue(newModelValue, property, out object? memberValue))
                                    {
                                        values[index] = memberValue;
                                    }
                                }
                            }
                            else
                            {
                                var property = mapping.ViewModelProperty;
                                var propertyData = GetPropertyData(property);
                                values[0] = propertyData.GetDefaultValue();
                            }

                            values[0] = converter.Convert(values, this);

                            SetValue(mapping.ViewModelProperty, values[0], true);
                        }
                    }
                }
            }

            // If we are validating, don't map view model values back to the model
            if (!IsValidating)
            {
                var validate = false;

                if (_viewModelToModelMap.TryGetValue(e.PropertyName, out var mapping))
                {
                    lock (_modelLock)
                    {
                        var model = _modelObjects[mapping.ModelProperty];
                        if (model is not null)
                        {
                            var modelInfo = _modelObjectsInfo[mapping.ModelProperty];
                            if (!modelInfo.IsCanceling)
                            {
                                var viewModelValue = GetValue<object>(e.PropertyName);
                                var propertiesToSet = mapping.ValueProperties;

                                if (_modelErrorInfo.TryGetValue(mapping.ModelProperty, out var modelErrorInfo))
                                {
                                    mapping.ValueProperties.ForEach(modelErrorInfo.ClearDefaultErrors);
                                }

                                // Only TwoWay, OneWayToSource mappings should be mapped
                                if ((mapping.Mode == ViewModelToModelMode.TwoWay) || (mapping.Mode == ViewModelToModelMode.OneWayToSource))
                                {
                                    var valuesToSet = mapping.Converter.ConvertBack(viewModelValue, this);
                                    if (propertiesToSet.Length != valuesToSet.Length)
                                    {
                                        Log.Error("Properties - values count mismatch, properties '{0}', values '{1}'",
                                            string.Join(", ", propertiesToSet), string.Join(", ", valuesToSet));
                                    }

                                    for (var index = 0; index < propertiesToSet.Length && index < valuesToSet.Length; index++)
                                    {
                                        try
                                        {
                                            mapping.IgnoredProperties.AddRange(propertiesToSet);

                                            var propertyToUpdate = propertiesToSet[index];
                                            var valueToSet = valuesToSet[index];

                                            // Check equality first, see https://github.com/Catel/Catel/issues/2164
                                            // If we fail, just set it anyways (keep old behavior)
                                            if (_objectAdapter.TryGetMemberValue<object?>(model, propertyToUpdate, out var currentValue))
                                            {
                                                if (ObjectHelper.AreEqual(currentValue, valueToSet))
                                                {
                                                    continue;
                                                }
                                            }

                                            if (_objectAdapter.TrySetMemberValue(model, propertyToUpdate, valueToSet))
                                            {
                                                Log.Debug("Updated property '{0}' on model type '{1}' to '{2}'", propertiesToSet[index], model.GetType().Name, ObjectToStringHelper.ToString(valueToSet));

                                                // Force validation, see https://github.com/Catel/Catel/issues/1108
                                                validate = true;
                                            }
                                            else
                                            {
                                                Log.Warning("Failed to set property '{0}' on model type '{1}'", propertiesToSet[index], model.GetType().Name);
                                            }
                                        }
                                        finally
                                        {
                                            foreach (var propertyToSet in propertiesToSet)
                                            {
                                                mapping.IgnoredProperties.Remove(propertyToSet);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Log.Warning("Value for model property '{0}' is null, cannot map properties from view model to model", mapping.ModelProperty);
                        }
                    }
                }

                if (validate)
                {
                    // Model was updated successfully, make sure to revalidate, fixes https://github.com/Catel/Catel/issues/1108
                    Validate(true);
                }
            }

            if (InvalidateCommandsOnPropertyChanged)
            {
                ViewModelCommandManager.InvalidateCommands();
            }
        }

        /// <summary>
        /// Called when a property on one of the registered models has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method will also raise for properties that are not mapped on the view model.
        /// </remarks>
        protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the PropertyChanged event of a Model.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelPropertyChangedInternal(object? sender, PropertyChangedEventArgs e)
        {
            foreach (var map in _viewModelToModelMap)
            {
                var mapping = map.Value;

                var converter = mapping.Converter;
                if (converter.ShouldConvert(e.PropertyName))
                {
                    var model = _modelObjects[mapping.ModelProperty];
                    if (model is null)
                    {
                        continue;
                    }

                    // Check if this is the right model (duplicate mappings might exist)
                    if (ReferenceEquals(model, sender))
                    {
                        var propertyName = e.PropertyName ?? string.Empty;
                        if (mapping.IgnoredProperties.Contains(propertyName))
                        {
                            continue;
                        }

                        // Only OneWay, TwoWay or Explicit (yes, only VM => M is explicit) should be mapped
                        if ((mapping.Mode == ViewModelToModelMode.TwoWay) ||
                            (mapping.Mode == ViewModelToModelMode.OneWay) ||
                            (mapping.Mode == ViewModelToModelMode.Explicit))
                        {
                            var values = new object?[mapping.ValueProperties.Length];

                            for (var index = 0; index < mapping.ValueProperties.Length; index++)
                            {
                                var property = mapping.ValueProperties[index];

                                if (_objectAdapter.TryGetMemberValue(model, property, out object? modelValue))
                                {
                                    values[index] = modelValue;
                                }
                            }

                            var convertedValue = mapping.Converter.Convert(values, this);
                            SetValue(mapping.ViewModelProperty, convertedValue);
                        }
                    }
                }
            }

            OnModelPropertyChanged(sender, e);

            Validate();
        }

        /// <summary>
        /// Resets the model by calling uninitializing and initializing the model again. This means that if the model
        /// supports 
        /// <see cref="IEditableObject"/>, it will be reset.
        /// </summary>
        /// <param name="modelProperty">The model property.</param>
        /// <param name="modelCleanUpMode">The model clean up mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="modelProperty"/> is <c>null</c>.</exception>
        protected void ResetModel(string modelProperty, ModelCleanUpMode modelCleanUpMode)
        {
            ArgumentNullException.ThrowIfNull(modelProperty);

            var model = GetValue<IModel>(modelProperty);

            UninitializeModelInternal(modelProperty, model, modelCleanUpMode);
            InitializeModelInternal(modelProperty, model);
        }

        /// <summary>
        /// Initializes a model by subscribing to all events.
        /// </summary>
        /// <param name="modelProperty">The name of the model property.</param>
        /// <param name="model">The model.</param>
        private void InitializeModelInternal(string modelProperty, object model)
        {
            if (model is null)
            {
                return;
            }

            _viewModelManager.RegisterModel(this, model);

            var modelAsINotifyPropertyChanged = model as INotifyPropertyChanged;
            if (modelAsINotifyPropertyChanged is not null)
            {
                modelAsINotifyPropertyChanged.PropertyChanged += OnModelPropertyChangedInternal;
            }

            var modelErrorInfo = new ModelErrorInfo(model);
            modelErrorInfo.Updated += OnModelErrorInfoUpdated;

            var modelInfo = _modelObjectsInfo[modelProperty];

            _modelErrorInfo[modelProperty] = modelErrorInfo;

            if (ValidateModelsOnInitialization)
            {
                if (modelInfo.SupportValidation)
                {
                    var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                    var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);

                    System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);
                    modelErrorInfo.InitializeDefaultErrors(validationResults);
                }
            }

            if (SupportIEditableObject)
            {
                if (modelInfo.SupportIEditableObject)
                {
                    EditableObjectHelper.BeginEditObject(model);
                }
            }

            InitializeModel(modelProperty, model);
        }

        /// <summary>
        /// Called when a model initialized.
        /// </summary>
        /// <param name="modelProperty">The name of the model property.</param>
        /// <param name="model">The model.</param>
        protected virtual void InitializeModel(string modelProperty, object model)
        {
        }

        /// <summary>
        /// Uninitializes a model by unsubscribing from all events.
        /// </summary>
        /// <param name="modelProperty">The name of the model property.</param>
        /// <param name="model">The model.</param>
        /// <param name="modelCleanUpMode">The model clean up mode.</param>
        private void UninitializeModelInternal(string modelProperty, object model, ModelCleanUpMode modelCleanUpMode)
        {
            if (model is null)
            {
                return;
            }

            _viewModelManager.UnregisterModel(this, model);

            if (_modelErrorInfo.TryGetValue(modelProperty, out var modelErrorInfo))
            {
                modelErrorInfo.Updated -= OnModelErrorInfoUpdated;
                modelErrorInfo.CleanUp();

                _modelErrorInfo.Remove(modelProperty);
            }

            if (SupportIEditableObject)
            {
                var modelInfo = _modelObjectsInfo[modelProperty];
                if (modelInfo.SupportIEditableObject)
                {
                    switch (modelCleanUpMode)
                    {
                        case ModelCleanUpMode.CancelEdit:
                            try
                            {
                                modelInfo.IsCanceling = true;

                                EditableObjectHelper.CancelEditObject(model);
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(ex, "Failed to cancel the edit of object for model '{0}'", modelProperty);
                            }
                            finally
                            {
                                modelInfo.IsCanceling = false;
                            }
                            break;

                        case ModelCleanUpMode.EndEdit:
                            try
                            {
                                EditableObjectHelper.EndEditObject(model);
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(ex, "Failed to end the edit of object for model '{0}'", modelProperty);
                            }
                            break;
                    }
                }
            }

            var modelAsINotifyPropertyChanged = model as INotifyPropertyChanged;
            if (modelAsINotifyPropertyChanged is not null)
            {
                modelAsINotifyPropertyChanged.PropertyChanged -= OnModelPropertyChangedInternal;
            }

            UninitializeModel(modelProperty, model, modelCleanUpMode);
        }

        /// <summary>
        /// Called when a model uninitialized.
        /// </summary>
        /// <param name="modelProperty">The name of the model property.</param>
        /// <param name="model">The model.</param>
        /// <param name="modelCleanUpMode">The model clean up mode.</param>
        protected virtual void UninitializeModel(string modelProperty, object model, ModelCleanUpMode modelCleanUpMode)
        {
        }

        /// <summary>
        /// Called when the <see cref="ModelErrorInfo.Updated"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnModelErrorInfoUpdated(object? sender, EventArgs e)
        {
            Validate(true);
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
        /// Determines whether a specific property is registered as a model.
        /// </summary>
        /// <param name="name">The name of the registered model.</param>
        /// <returns>
        /// <c>true</c> if a specific property is registered as a model; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsModelRegistered(string name)
        {
            if (!IsPropertyRegistered(name))
            {
                return false;
            }

            return _modelObjects.ContainsKey(name);
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
                Log.Info("Canceling of view model '{0}' is canceled via the Canceling event", GetType());
                IsCanceling = false;
                return false;
            }

            var cancel = await CancelAsync();

            Log.Info(cancel ? "Canceled view model '{0}'" : "Failed to cancel view model '{0}'", GetType());
            if (!cancel)
            {
                IsCanceling = false;
                return false;
            }

            lock (_modelLock)
            {
                foreach (var modelKeyValuePair in _modelObjects)
                {
                    var model = modelKeyValuePair.Value;
                    if (model is not null)
                    {
                        UninitializeModelInternal(modelKeyValuePair.Key, model, ModelCleanUpMode.CancelEdit);
                    }
                }
            }

            Log.Info("Canceled view model '{0}'", GetType());

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

            if (DeferValidationUntilFirstSaveCall)
            {
                DeferValidationUntilFirstSaveCall = false;
            }

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

                Log.Info("Saving of view model '{0}' is canceled via the Saving event", GetType());
                return false;
            }

            var saved = await SaveAsync();

            Log.Info(saved ? "Saved view model '{0}'" : "Failed to save view model '{0}'", GetType());

            if (saved)
            {
                lock (_modelLock)
                {
                    foreach (var modelKeyValuePair in _modelObjects)
                    {
                        var model = modelKeyValuePair.Value;
                        if (model is not null)
                        {
                            UninitializeModelInternal(modelKeyValuePair.Key, model, ModelCleanUpMode.EndEdit);
                        }
                    }
                }

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

            UninitializeThrottling();

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

            Log.Info("Closed view model '{0}'", type);

            _viewModelManager.UnregisterViewModelInstance(this);
        }
    }
}
