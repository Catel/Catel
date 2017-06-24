// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Auditing;
    using Collections;
    using Data;
    using IoC;
    using Logging;
    using Reflection;
    using Services;
    using System.Collections.ObjectModel;
    using Threading;

#if !PCL
    using System.Collections.Concurrent;
#endif

    #region Enums
    /// <summary>
    /// Available clean up models for a model.
    /// </summary>
    public enum ModelCleanUpMode
    {
        /// <summary>
        /// Call <see cref="IEditableObject.CancelEdit"/>.
        /// </summary>
        CancelEdit,

        /// <summary>
        /// Call <see cref="IEditableObject.EndEdit"/>.
        /// </summary>
        EndEdit
    }
    #endregion

    /// <summary>
    /// View model base for MVVM implementations. This class is based on the <see cref="ModelBase" />, and supports all
    /// common interfaces used by WPF.
    /// </summary>
    /// <remarks>This view model base does not add any services.</remarks>
    public abstract partial class ViewModelBase : ValidatableModelBase, IRelationalViewModel, IUniqueIdentifyable
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary containing the view model metadata of a view model type so it has to be calculated only once.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif

#if !PCL
        private static readonly ConcurrentDictionary<Type, ViewModelMetadata> _metaData = new ConcurrentDictionary<Type, ViewModelMetadata>();
#else
        private static readonly Dictionary<Type, ViewModelMetadata> _metaData = new Dictionary<Type, ViewModelMetadata>();
#endif

#if !XAMARIN
        /// <summary>
        /// The dispatcher service used to dispatch all calls.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly IDispatcherService _dispatcherService;
#endif

        /// <summary>
        /// Value indicating whether the multiple modules warning should be ignored.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly bool _ignoreMultipleModelsWarning;

        /// <summary>
        /// Value indicating whether the view model attributes are initialized. 
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _areViewModelAttributesIntialized;

#if NET
        [field: NonSerialized]
#endif
        private readonly object _modelLock = new object();

        /// <summary>
        /// Dictionary of available models inside the view model.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, object> _modelObjects = new Dictionary<string, object>();

        /// <summary>
        /// Dictionary with info about the available models inside the view model.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ModelInfo> _modelObjectsInfo = new Dictionary<string, ModelInfo>();

        /// <summary>
        /// Dictionary with data error info about a specific model.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ModelErrorInfo> _modelErrorInfo = new Dictionary<string, ModelErrorInfo>();

        /// <summary>
        /// List of child view models which can be registed by the <c>RegisterChildViewModel</c> method.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        internal readonly List<IViewModel> ChildViewModels = new List<IViewModel>();

        /// <summary>
        /// Value to determine whether child view models have errors or not.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _childViewModelsHaveErrors;

        /// <summary>
        /// Gets the view model manager.
        /// </summary>
        /// <value>The view model manager.</value>
#if NET
        [field: NonSerialized]
#endif
        protected static readonly IViewModelManager ViewModelManager;

        /// <summary>
        /// Mappings from view model properties to models and their properties.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ViewModelToModelMapping> _viewModelToModelMap = new Dictionary<string, ViewModelToModelMapping>();

        /// <summary>
        /// The backing field for the title property.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private string _title;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ViewModelBase" /> class.
        /// </summary>
        static ViewModelBase()
        {
            var serviceLocator = IoC.ServiceLocator.Default;
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();
            ViewModelManager = serviceLocator.ResolveType<IViewModelManager>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected ViewModelBase()
            : this(true, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">
        /// if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="InitializeViewModelAttributes"/>.
        /// </param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected ViewModelBase(bool supportIEditableObject, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false)
            : this(null, supportIEditableObject, ignoreMultipleModelsWarning, skipViewModelAttributesInitialization)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// <para/>
        /// This constructor allows the injection of a custom <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="serviceLocator">The service locator to inject. If <c>null</c>, the <see cref="Catel.IoC.ServiceLocator.Default"/> will be used.</param>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="InitializeViewModelAttributes"/>.</param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected ViewModelBase(IServiceLocator serviceLocator, bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false,
            bool skipViewModelAttributesInitialization = false)
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<ViewModelBase>();
            ViewModelConstructionTime = FastDateTime.Now;

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            var type = GetType();

            Log.Debug("Creating view model of type '{0}' with unique identifier {1}", type.Name, UniqueIdentifier);

            _ignoreMultipleModelsWarning = ignoreMultipleModelsWarning;

#if !XAMARIN
            if (serviceLocator == null)
            {
                serviceLocator = ServiceLocator.Default;
            }

            DependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
            _dispatcherService = DependencyResolver.Resolve<IDispatcherService>();
#endif

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

            // Temporarily suspend validation, will be enabled at the end of constructor again
            SuspendValidation = true;

            if (!skipViewModelAttributesInitialization)
            {
                InitializeViewModelAttributes();
            }

            ViewModelManager.RegisterViewModelInstance(this);

            InitializeThrottling();

            // Enable validation again like we promised some lines of code ago
            SuspendValidation = false;

            // As a last step, enable the auditors (we don't need change notifications of previous properties, etc)
            AuditingHelper.RegisterViewModel(this);
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view model has been initialized.
        /// </summary>
        public event AsyncEventHandler<EventArgs> InitializedAsync;

        /// <summary>
        /// Occurs when a command on the view model has been executed.
        /// </summary>
        public event AsyncEventHandler<CommandExecutedEventArgs> CommandExecutedAsync;

        /// <summary>
        /// Occurs when the view model is about to be saved.
        /// </summary>
        public event AsyncEventHandler<SavingEventArgs> SavingAsync;

        /// <summary>
        /// Occurs when the view model is saved successfully.
        /// </summary>
        public event AsyncEventHandler<EventArgs> SavedAsync;

        /// <summary>
        /// Occurs when the view model is about to be canceled.
        /// </summary>
        public event AsyncEventHandler<CancelingEventArgs> CancelingAsync;

        /// <summary>
        /// Occurrs when the view model is canceled.
        /// </summary>
        public event AsyncEventHandler<EventArgs> CanceledAsync;

        /// <summary>
        /// Occurs when the view model is being closed.
        /// </summary>
        public event AsyncEventHandler<EventArgs> ClosingAsync;

        /// <summary>
        /// Occurs when the view model has just been closed.
        /// </summary>
        public event AsyncEventHandler<ViewModelClosedEventArgs> ClosedAsync;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the unique identifier of the view model.
        /// </summary>
        /// <value>The unique identifier.</value>
        [ExcludeFromValidation]
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets the view model construction time, which is used to get unique instances of view models.
        /// </summary>
        /// <value>The view model construction time.</value>
        [ExcludeFromValidation]
        public DateTime ViewModelConstructionTime { get; private set; }

        /// <summary>
        /// Gets the parent view model.
        /// </summary>
        /// <value>The parent view model.</value>
        [ExcludeFromValidation]
        public IViewModel ParentViewModel { get; private set; }

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
        /// Gets a value indicating whether this object is currently initializing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is currently initializing; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        protected bool IsInitializing { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is initialized; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        protected bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is currently canceling.
        /// </summary>
        [ExcludeFromValidation]
        protected bool IsCanceling { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is currently saving.
        /// </summary>
        [ExcludeFromValidation]
        protected bool IsSaving { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is closing.
        /// </summary>
        /// <value><c>true</c> if this instance is closing; otherwise, <c>false</c>.</value>
        protected bool IsClosing { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
        /// <see cref="CancelViewModelAsync"/>, <see cref="SaveViewModelAsync"/> or <see cref="CloseViewModelAsync"/>
        /// will have no effect.
        /// </summary>
        /// <value><c>true</c> if the view model is closed; otherwise, <c>false</c>.</value>
        [ExcludeFromValidation]
        public bool IsClosed { get; private set; }

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

        /// <summary>
        /// Gets the dependency resolver.
        /// </summary>
        /// <value>The dependency resolver.</value>
        [ExcludeFromValidation]
        protected IDependencyResolver DependencyResolver { get; private set; }
        #endregion

        #region Methods
        partial void InitializeThrottling();

        partial void UninitializeThrottling();

        /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string ToString()
        {
            return string.Format("{0} (ID = {1})", GetType().FullName, UniqueIdentifier);
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
        private static ViewModelMetadata GetViewModelMetaData(Type viewModelType)
        {
#if PCL
            lock (_metaData)
            {
                ViewModelMetadata data = null;
                if (!_metaData.TryGetValue(viewModelType, out data))
                {
                    data = CreateViewModelMetaData(viewModelType);
                    _metaData[viewModelType] = data;
                }

                return data;
            }
#else
            return _metaData.GetOrAdd(viewModelType, CreateViewModelMetaData);
#endif
        }

        private static ViewModelMetadata CreateViewModelMetaData(Type viewModelType)
        {
            var properties = new List<PropertyInfo>();
            var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(true, false, true);
            properties.AddRange(viewModelType.GetPropertiesEx(bindingFlags));

            var modelObjectsInfo = new Dictionary<string, ModelInfo>();
            var viewModelToModelMap = new Dictionary<string, ViewModelToModelMapping>();

            var modelNames = (from propertyInfo in properties
                              where propertyInfo.IsDecoratedWithAttribute<ModelAttribute>()
                              select propertyInfo.Name).ToList();

            foreach (var propertyInfo in properties)
            {
                #region Model attributes
                var modelAttribute = propertyInfo.GetCustomAttributeEx(typeof(ModelAttribute), true) as ModelAttribute;
                if (modelAttribute != null)
                {
                    modelObjectsInfo.Add(propertyInfo.Name, new ModelInfo(propertyInfo.Name, modelAttribute));
                }
                #endregion

                #region ViewModelToModel attributes
                var viewModelToModelAttribute = propertyInfo.GetCustomAttributeEx(typeof(ViewModelToModelAttribute), true) as ViewModelToModelAttribute;
                if (viewModelToModelAttribute != null)
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
                            throw Log.ErrorAndCreateException<InvalidOperationException>("It is only possible to automatically select the right model if there is 1 model. There are '{0}' models, so please specify the model name explicitly.", modelNames.Count);
                        }

                        viewModelToModelAttribute.Model = modelNames[0];
                    }

                    if (!viewModelToModelMap.ContainsKey(propertyInfo.Name))
                    {
                        viewModelToModelMap.Add(propertyInfo.Name, new ViewModelToModelMapping(propertyInfo.Name, viewModelToModelAttribute));
                    }
                }
                #endregion
            }

            return new ViewModelMetadata(
                viewModelType,
                modelObjectsInfo,
                viewModelToModelMap);
        }

        /// <summary>
        /// Validates the view model to model mappings.
        /// </summary>
        /// <exception cref="ModelNotRegisteredException">A property is mapped to a model that does not exists.</exception>
        private void ValidateViewModelToModelMappings()
        {
            foreach (var viewModelToModelMapping in _viewModelToModelMap)
            {
                var mapping = viewModelToModelMapping.Value;
                if (!IsModelRegistered(mapping.ModelProperty))
                {
                    throw Log.ErrorAndCreateException(msg => new ModelNotRegisteredException(mapping.ModelProperty, mapping.ViewModelProperty),
                        "There is no model '{0}' registered with the model attribute, so the ViewModelToModel attribute on property '{1}' is invalid",
                        mapping.ModelProperty, mapping.ViewModelProperty);
                }

                var viewModelPropertyType = GetPropertyData(mapping.ViewModelProperty).Type;
                var modelPropertyType = GetPropertyData(mapping.ModelProperty).Type;
                var modelPropertyPropertyTypes = new List<Type>(mapping.ValueProperties.Length);

                foreach (var valueProperty in mapping.ValueProperties)
                {
                    var modelPropertyPropertyInfo = modelPropertyType.GetPropertyEx(valueProperty);
                    if (modelPropertyPropertyInfo == null)
                    {
                        Log.Warning("Mapped viewmodel property '{0}' to model property '{1}' is invalid because property '{1}' is not found on the model '{2}'.\n\n" +
                                "If the property is defined in a sub-interface, reflection does not return it as a valid property. If this is the case, you can safely ignore this warning",
                            mapping.ViewModelProperty, valueProperty, mapping.ModelProperty);
                    }
                    else
                    {
                        modelPropertyPropertyTypes.Add(modelPropertyPropertyInfo.PropertyType);
                    }
                }

                if (!mapping.Converter.CanConvert(modelPropertyPropertyTypes.ToArray(), viewModelPropertyType, GetType()))
                {
                    Log.Warning("Property '{0}' mapped on model properties '{1}' cannot be converted via given converter '{2}'",
                        mapping.ViewModelProperty, string.Join(", ", mapping.ValueProperties), mapping.ConverterType);
                }
            }
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
            if (_areViewModelAttributesIntialized)
            {
                return;
            }

            _areViewModelAttributesIntialized = true;

            SuspendValidation = true;

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
                            var modelKeyValuePairValueAsModelBaseBase = modelKeyValuePair.Value as IModel;
                            if ((modelKeyValuePairValueAsModelBaseBase == null) || !modelKeyValuePairValueAsModelBaseBase.IsInEditSession)
                            {
                                EditableObjectHelper.BeginEditObject(modelKeyValuePair.Value);
                            }
                        }
                    }
                }
            }

            ValidateViewModelToModelMappings();

            if (!_ignoreMultipleModelsWarning)
            {
                lock (_modelLock)
                {
                    if (_modelObjects.Count > 1)
                    {
                        Log.Warning("The view model {0} implements {1} models.\n\n" +
                                    "Normally, a view model only implements 1 model so make sure you are using the Model attribute correctly. If the Model attribute is used correctly (on models only), this warning can be ignored by using a constructor overload.",
                            GetType().Name, _modelObjects.Count);
                    }
                }
            }

            SuspendValidation = false;
        }

        /// <summary>
        /// Sets the new parent view model of this view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        void IRelationalViewModel.SetParentViewModel(IViewModel parentViewModel)
        {
            if (!ObjectHelper.AreEqualReferences(ParentViewModel, parentViewModel))
            {
                ParentViewModel = parentViewModel;

                var parentVm = parentViewModel as ViewModelBase;
                if (parentVm != null)
                {
                    DeferValidationUntilFirstSaveCall = parentVm.DeferValidationUntilFirstSaveCall;
                }

                RaisePropertyChanged("ParentViewModel");
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
            Argument.IsNotNull("childViewModel", childViewModel);

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
                if (viewModelBase != null && viewModelBase.HasErrors)
                {
                    validate = true;
                }
                else
                {
                    var validationContext = ((IValidatableModel)childViewModel).ValidationContext;
                    if (validationContext != null)
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
        private void OnChildViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
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
        private Task OnChildViewModelClosedAsync(object sender, EventArgs e)
        {
            ((IRelationalViewModel)this).UnregisterChildViewModel((IViewModel)sender);

            return TaskHelper.Completed;
        }

        /// <summary>
        /// Unregisters the child view model. This means that the child view model will no longer receive any notifications
        /// from this view model as parent view model, nor will it be included in any validation calls in this view model.
        /// </summary>
        /// <param name="childViewModel">The child.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="childViewModel"/> is <c>null</c>.</exception>
        void IRelationalViewModel.UnregisterChildViewModel(IViewModel childViewModel)
        {
            Argument.IsNotNull("childViewModel", childViewModel);

            lock (ChildViewModels)
            {
                int index = ChildViewModels.IndexOf(childViewModel);
                if (index == -1)
                {
                    return;
                }

                childViewModel.PropertyChanged -= OnChildViewModelPropertyChanged;
                childViewModel.ClosedAsync -= OnChildViewModelClosedAsync;

                ChildViewModels.RemoveAt(index);
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
                return _modelObjects.Values.ToArray();
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
                if (model != null)
                {
                    object value = GetValue(mapping.ViewModelProperty);
                    var modelValues = mapping.Converter.ConvertBack(value, this);
                    for (int i = 0; i < mapping.ValueProperties.Length; i++)
                    {
                        if (PropertyHelper.TrySetPropertyValue(model, mapping.ValueProperties[i], modelValues[i], false))
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
        /// <param name="e">The <see cref="AdvancedPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
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
                    if (oldModelValue != null)
                    {
                        UninitializeModelInternal(e.PropertyName, oldModelValue, ModelCleanUpMode.CancelEdit);
                    }

                    var newModelValue = GetValue(e.PropertyName);
                    _modelObjects[e.PropertyName] = newModelValue;

                    if (newModelValue != null)
                    {
                        InitializeModelInternal(e.PropertyName, newModelValue);
                    }

                    // Since the model has been changed, copy all values from the model to the view model
                    foreach (KeyValuePair<string, ViewModelToModelMapping> viewModelToModelMap in _viewModelToModelMap)
                    {
                        var mapping = viewModelToModelMap.Value;
                        var converter = mapping.Converter;
                        if (string.CompareOrdinal(mapping.ModelProperty, e.PropertyName) == 0)
                        {
                            var values = new object[mapping.ValueProperties.Length];
                            if (newModelValue != null)
                            {
                                // We have a new model, ignore OneWayToSource
                                if (mapping.Mode == ViewModelToModelMode.OneWayToSource)
                                {
                                    continue;
                                }

                                for (var index = 0; index < mapping.ValueProperties.Length; index++)
                                {
                                    var property = mapping.ValueProperties[index];
                                    values[index] = PropertyHelper.GetPropertyValue(newModelValue, property, false);
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
                if (_viewModelToModelMap.ContainsKey(e.PropertyName))
                {
                    lock (_modelLock)
                    {
                        var mapping = _viewModelToModelMap[e.PropertyName];
                        var model = _modelObjects[mapping.ModelProperty];
                        if (model != null)
                        {
                            var modelInfo = _modelObjectsInfo[mapping.ModelProperty];
                            if (!modelInfo.IsCanceling)
                            {
                                var viewModelValue = GetValue(e.PropertyName);
                                var propertiesToSet = mapping.ValueProperties;

#if !XAMARIN_FORMS
                                if (_modelErrorInfo.ContainsKey(mapping.ModelProperty))
                                {
                                    mapping.ValueProperties.ForEach(_modelErrorInfo[mapping.ModelProperty].ClearDefaultErrors);
                                }
#endif

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

                                            if (PropertyHelper.TrySetPropertyValue(model, propertiesToSet[index], valuesToSet[index], false))
                                            {
                                                Log.Debug("Updated property '{0}' on model type '{1}' to '{2}'", propertiesToSet[index], model.GetType().Name, ObjectToStringHelper.ToString(valuesToSet[index]));
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
        protected virtual void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the PropertyChanged event of a Model.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelPropertyChangedInternal(object sender, PropertyChangedEventArgs e)
        {
            foreach (var map in _viewModelToModelMap)
            {
                var mapping = map.Value;

                var converter = mapping.Converter;
                if (converter.ShouldConvert(e.PropertyName))
                {
                    // Check if this is the right model (duplicate mappings might exist)
                    if (ReferenceEquals(_modelObjects[mapping.ModelProperty], sender))
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
                            var values = new object[mapping.ValueProperties.Length];
                            for (var index = 0; index < mapping.ValueProperties.Length; index++)
                            {
                                var property = mapping.ValueProperties[index];
                                values[index] = PropertyHelper.GetPropertyValue(sender, property, false);
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
            Argument.IsNotNull("modelProperty", modelProperty);

            var model = GetValue(modelProperty);

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
            if (model != null)
            {
                ViewModelManager.RegisterModel(this, model);
            }

            _modelErrorInfo[modelProperty] = new ModelErrorInfo(model);
            _modelErrorInfo[modelProperty].Updated += OnModelErrorInfoUpdated;

            var modelAsINotifyPropertyChanged = model as INotifyPropertyChanged;
            if (modelAsINotifyPropertyChanged != null)
            {
                modelAsINotifyPropertyChanged.PropertyChanged += OnModelPropertyChangedInternal;
            }

            if (SupportIEditableObject)
            {
                if (_modelObjectsInfo[modelProperty].SupportIEditableObject)
                {
                    if (model != null)
                    {
                        EditableObjectHelper.BeginEditObject(model);
                    }
                }
            }

#if NET
            if (ValidateModelsOnInitialization)
            {
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);

                System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);
                _modelErrorInfo[modelProperty].InitializeDefaultErrors(validationResults);
            }
#endif

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
            if (model != null)
            {
                ViewModelManager.UnregisterModel(this, model);
            }

            ModelErrorInfo modelErrorInfo;
            if (_modelErrorInfo.TryGetValue(modelProperty, out modelErrorInfo))
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
            if (modelAsINotifyPropertyChanged != null)
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
        private void OnModelErrorInfoUpdated(object sender, EventArgs e)
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
            return TaskHelper<bool>.FromResult(true);
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected virtual Task<bool> SaveAsync()
        {
            return TaskHelper<bool>.FromResult(true);
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
            return TaskHelper.Completed;
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
        #endregion

        #region Services
        /// <summary>
        /// Registers the default view model services.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        protected void RegisterViewModelServices(IServiceLocator serviceLocator)
        {
            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);
        }
        #endregion

        #region IViewModel Members
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
            return TaskHelper.Completed;
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
                    UninitializeModelInternal(modelKeyValuePair.Key, modelKeyValuePair.Value, ModelCleanUpMode.CancelEdit);
                }
            }

            Log.Info("Canceled view model '{0}'", GetType());

            await CanceledAsync.SafeInvokeAsync(this);

            IsCanceling = false;

            return true;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public async Task<bool> SaveViewModelAsync()
        {
            if (IsClosing || IsClosed)
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

            if (!SuspendValidation)
            {
                var validationContext = ((IValidatable) this).ValidationContext;
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
                    foreach (KeyValuePair<string, object> modelKeyValuePair in _modelObjects)
                    {
                        UninitializeModelInternal(modelKeyValuePair.Key, modelKeyValuePair.Value, ModelCleanUpMode.EndEdit);
                    }
                }

                await SavedAsync.SafeInvokeAsync(this);
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

            ViewModelManager.UnregisterAllModels(this);

            await CloseAsync();

            SuspendValidation = true;

            // Note: important to set *before* calling the event (the handler might need to check
            // if the vm is closed)
            IsClosing = false;
            IsClosed = true;

            await OnClosedAsync(result);

            Log.Info("Closed view model '{0}'", GetType());

            ViewModelManager.UnregisterViewModelInstance(this);
        }
        #endregion
    }
}