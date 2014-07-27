// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
    using Catel.Services;

    using Collections;
    using Data;
    using IoC;
    using Auditing;
    using Logging;
    using Messaging;
    using Reflection;
    using ViewModels;

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

    /// <summary>
    /// Available view model events that can be retrieved via the <see cref="InterestedInAttribute"/>.
    /// </summary>
    public enum ViewModelEvent
    {
        /// <summary>
        /// Saving event, invoked when a view model is about to be saved.
        /// </summary>
        Saving,

        /// <summary>
        /// Saved event, invoked when a view model has been saved successfully.
        /// </summary>
        Saved,

        /// <summary>
        /// Canceling event, invoked when a view model is about to be canceled.
        /// </summary>
        Canceling,

        /// <summary>
        /// Canceled event, invoked when a view model has been canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// Closed event, invoked when the view model is closed.
        /// </summary>
        Closed
    }
    #endregion

    /// <summary>
    /// View model base for MVVM implementations. This class is based on the <see cref="ModelBase" />, and supports all
    /// common interfaces used by WPF.
    /// </summary>
    /// <remarks>This view model base does not add any services. The technique specific implementation should take care of that
    /// (such as WPF, Silverlight, etc).</remarks>
    public abstract partial class ViewModelBase : ModelBase, IViewModel, INotifyableViewModel, IRelationalViewModel
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
        private static readonly Dictionary<Type, ViewModelMetadata> _metaData = new Dictionary<Type, ViewModelMetadata>();

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
        /// Value indicating whether the view model is already initialized via a call to <see cref="InitializeViewModel" />.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _isViewModelInitialized;

        /// <summary>
        /// Value indicating whether the view model attributes are initialized. 
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _areViewModelAttributesIntialized;

        /// <summary>
        /// Value indicating whether the specified models are dirty.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, bool> _modelsDirtyFlags = new Dictionary<string, bool>();

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

#if NET
        /// <summary>
        /// The view model properties by type.
        /// </summary>
        [field: NonSerialized]
        private static readonly Dictionary<Type, HashSet<string>> _viewModelPropertiesByType = new Dictionary<Type, HashSet<string>>();
#endif
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
        protected ViewModelBase() :
            this(true, false, false)
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
            : this(null, supportIEditableObject, ignoreMultipleModelsWarning, skipViewModelAttributesInitialization) { }

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
            ViewModelConstructionTime = DateTime.Now;

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            AuditingHelper.RegisterViewModel(this);

            Log.Debug("Creating view model of type '{0}' with unique identifier {1}", GetType().Name, UniqueIdentifier);

#if NET
            var viewModelType = GetType();
            if (!_viewModelPropertiesByType.ContainsKey(viewModelType))
            {
                var properties = (from property in viewModelType.GetPropertiesEx()
                                  select property.Name);
                _viewModelPropertiesByType[viewModelType] = new HashSet<string>(properties);
            }
#endif

            _ignoreMultipleModelsWarning = ignoreMultipleModelsWarning;

            if (serviceLocator == null)
            {
                serviceLocator = ServiceLocator.Default;
            }

#if !XAMARIN
            DependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
            _dispatcherService = DependencyResolver.Resolve<IDispatcherService>();
#endif

            // In silverlight, automatically invalidate commands when property changes
#if !WINDOWS_PHONE && !NET35
            ValidateModelsOnInitialization = true;
#endif

            InvalidateCommandsOnPropertyChanged = true;

            ViewModelCommandManager = MVVM.ViewModelCommandManager.Create(this);
            ViewModelCommandManager.AddHandler((viewModel, propertyName, command, commandParameter) =>
                CommandExecuted.SafeInvoke(this, new CommandExecutedEventArgs((ICatelCommand)command, commandParameter, propertyName)));

            SupportIEditableObject = supportIEditableObject;

            // Temporarily suspend validation, will be enabled at the end of constructor again
            SuspendValidation = true;

            if (!skipViewModelAttributesInitialization)
            {
                InitializeViewModelAttributes();
            }

            ViewModelManager.RegisterViewModelInstance(this);

            object[] interestedInAttributes = GetType().GetCustomAttributesEx(typeof(InterestedInAttribute), true);
            foreach (InterestedInAttribute interestedInAttribute in interestedInAttributes)
            {
                ViewModelManager.AddInterestedViewModelInstance(interestedInAttribute.ViewModelType, this);
            }

            InitializeThrottling();

            // Enable validation again like we promised some lines of code ago
            SuspendValidation = false;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view model has been initialized.
        /// </summary>
        public new event EventHandler<EventArgs> Initialized;

        /// <summary>
        /// Occurs when a command on the view model has been executed.
        /// </summary>
        public event EventHandler<CommandExecutedEventArgs> CommandExecuted;

        /// <summary>
        /// Occurs when the view model is about to be saved.
        /// </summary>
        public event EventHandler<SavingEventArgs> Saving;

        /// <summary>
        /// Occurs when the view model is saved successfully.
        /// </summary>
        public event EventHandler<EventArgs> Saved;

        /// <summary>
        /// Occurs when the view model is about to be canceled.
        /// </summary>
        public event EventHandler<CancelingEventArgs> Canceling;

        /// <summary>
        /// Occurrs when the view model is canceled.
        /// </summary>
        public event EventHandler<EventArgs> Canceled;

        /// <summary>
        /// Occurs when the view model is being closed.
        /// </summary>
        public event EventHandler<EventArgs> Closing;

        /// <summary>
        /// Occurs when the view model has just been closed.
        /// </summary>
        public event EventHandler<ViewModelClosedEventArgs> Closed;
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
        protected IViewModelCommandManager ViewModelCommandManager { get; private set; }

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
        /// Gets a value indicating whether this instance is currently saving.
        /// </summary>
        [ExcludeFromValidation]
        protected bool IsSaving { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
        /// <see cref="CancelViewModel"/>, <see cref="SaveViewModel"/> or <see cref="CloseViewModel"/>
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
            get
            {
                return _title;
            }
            protected set
            {
                _title = value;

                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object contains any field or business errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        public new bool HasErrors
        {
            get { return base.HasErrors || _childViewModelsHaveErrors; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a dirty model.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a dirty model; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        public virtual bool HasDirtyModel
        {
            get
            {
                var hasDirtyModels = (from dirtyModelFlag in _modelsDirtyFlags
                                      where dirtyModelFlag.Value
                                      select dirtyModelFlag).Any();
                if (hasDirtyModels)
                {
                    return true;
                }

                lock (ChildViewModels)
                {
                    return ChildViewModels.Any(childViewModel => childViewModel.HasDirtyModel);
                }
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

            var metaData = InitializeViewModelMetaData(viewModelType);

            lock (_modelLock)
            {
                _modelObjectsInfo.AddRange(metaData.Models);
            }

            _viewModelToModelMap.AddRange(metaData.Mappings);
            _validationSummaries.AddRange(metaData.Validations);
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
        private static ViewModelMetadata InitializeViewModelMetaData(Type viewModelType)
        {
            if (_metaData.ContainsKey(viewModelType))
            {
                return _metaData[viewModelType];
            }

            var properties = new List<PropertyInfo>();
            properties.AddRange(viewModelType.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true)));

            var modelObjectsInfo = new Dictionary<string, ModelInfo>();
            var viewModelToModelMap = new Dictionary<string, ViewModelToModelMapping>();
            var validationSummaries = new Dictionary<string, ValidationToViewModelAttribute>();

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

                    if (!viewModelToModelMap.ContainsKey(propertyInfo.Name))
                    {
                        viewModelToModelMap.Add(propertyInfo.Name, new ViewModelToModelMapping(propertyInfo.Name, viewModelToModelAttribute));
                    }
                }
                #endregion

                #region ValidationToViewModel attributes
                var validationToViewModelAttribute = propertyInfo.GetCustomAttributeEx(typeof(ValidationToViewModelAttribute), true) as ValidationToViewModelAttribute;
                if (validationToViewModelAttribute != null)
                {
                    if (propertyInfo.PropertyType != typeof(IValidationSummary))
                    {
                        string error = string.Format("A property decorated with the ValidationToViewModel attribute must be of type IValidationSummary, but '{0}' is not", propertyInfo.Name);
                        Log.Error(error);
                        throw new InvalidOperationException(error);
                    }

                    validationSummaries.Add(propertyInfo.Name, validationToViewModelAttribute);

                    Log.Debug("Registered property '{0}' as validation summary", propertyInfo.Name);
                }
                #endregion
            }

            _metaData.Add(viewModelType, new ViewModelMetadata(viewModelType, modelObjectsInfo, viewModelToModelMap, validationSummaries));

            return _metaData[viewModelType];
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
                    Log.Error("There is no model '{0}' registered with the model attribute, so the ViewModelToModel attribute on property '{1}' is invalid",
                        mapping.ModelProperty, mapping.ViewModelProperty);

                    throw new ModelNotRegisteredException(mapping.ModelProperty, mapping.ViewModelProperty);
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
                        //throw new PropertyNotFoundInModelException(mapping.ViewModelProperty, mapping.ModelProperty, mapping.ValueProperty);
                        // Disabled because a property defined in an interface is not detected by FlattenHierarchy
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
                            var modelKeyValuePairValueAsModelBaseBase = modelKeyValuePair.Value as ModelBase;
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
            if (ParentViewModel != parentViewModel)
            {
                ParentViewModel = parentViewModel;

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

                if (!ChildViewModels.Contains(childViewModel))
                {
                    ChildViewModels.Add(childViewModel);

                    childViewModel.PropertyChanged += OnChildViewModelPropertyChanged;
                    childViewModel.Closed += OnChildViewModelClosed;
                }

                if (childViewModel.HasErrors || childViewModel.HasWarnings)
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
            if ((e.PropertyName == "HasErrors") || (e.PropertyName == "HasWarnings"))
            {
                Validate();
            }
        }

        /// <summary>
        /// Called when the child view model is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnChildViewModelClosed(object sender, EventArgs e)
        {
            ((IRelationalViewModel)this).UnregisterChildViewModel((IViewModel)sender);
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
                childViewModel.Closed -= OnChildViewModelClosed;

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
                        if (PropertyHelper.TrySetPropertyValue(model, mapping.ValueProperties[i], modelValues[i]))
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
            if (IsClosed)
            {
                return;
            }

            if (string.IsNullOrEmpty(e.PropertyName))
            {
                return;
            }

            lock (_modelLock)
            {
                if (_modelObjects.ContainsKey(e.PropertyName))
                {
                    // Clean up old model
                    var oldModelValue = _modelObjects[e.PropertyName];
                    if (oldModelValue != null)
                    {
                        UninitializeModel(e.PropertyName, oldModelValue, ModelCleanUpMode.CancelEdit);
                    }

                    var newModelValue = GetValue(e.PropertyName);
                    _modelObjects[e.PropertyName] = newModelValue;

                    if (newModelValue != null)
                    {
                        InitializeModel(e.PropertyName, newModelValue);
                    }

                    // Since the model has been changed, copy all values from the model to the view model
                    foreach (KeyValuePair<string, ViewModelToModelMapping> viewModelToModelMap in _viewModelToModelMap)
                    {
                        ViewModelToModelMapping mapping = viewModelToModelMap.Value;
                        IViewModelToModelConverter converter = mapping.Converter;
                        if (string.CompareOrdinal(mapping.ModelProperty, e.PropertyName) == 0)
                        {
                            var values = new object[mapping.ValueProperties.Length];
                            if (newModelValue != null)
                            {
                                // We have a new model, ignore OneWayToSource
                                if (mapping.Mode == ViewModelToModelMode.OneWayToSource)
                                    continue;

                                for (var index = 0; index < mapping.ValueProperties.Length; index++)
                                {
                                    var property = mapping.ValueProperties[index];
                                    values[index] = PropertyHelper.GetPropertyValue(newModelValue, property);
                                }
                            }
                            else
                            {
                                // Always restore default value when a model becomes null
                                for (var index = 0; index < mapping.ValueProperties.Length; index++)
                                {
                                    var property = mapping.ValueProperties[index];
                                    var propertyData = GetPropertyData(property);
                                    values[index] = propertyData.GetDefaultValue();
                                }
                            }

                            values[0] = converter.Convert(values, this);

                            SetValue(mapping.ViewModelProperty, values[0], true, ValidateModelsOnInitialization);
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
                        ViewModelToModelMapping mapping = _viewModelToModelMap[e.PropertyName];
                        object model = _modelObjects[mapping.ModelProperty];
                        if (model != null)
                        {
                            object viewModelValue = GetValue(e.PropertyName);
                            //object modelValue = PropertyHelper.GetPropertyValue(model, mapping.ValueProperties);
                            //if (!ObjectHelper.AreEqualReferences(viewModelValue, modelValue))
                            //{
                                string[] propertiesToSet = mapping.ValueProperties;

#if !WINDOWS_PHONE && !NET35
                                if (_modelErrorInfo.ContainsKey(mapping.ModelProperty))
                                {
                                    mapping.ValueProperties.ForEach(_modelErrorInfo[mapping.ModelProperty].ClearDefaultErrors);
                                }
#endif

                                // Only TwoWay or OneWayToSource mappings should be mapped
                                if ((mapping.Mode == ViewModelToModelMode.TwoWay) || (mapping.Mode == ViewModelToModelMode.OneWayToSource))
                                {
                                    var valuesToSet = mapping.Converter.ConvertBack(viewModelValue, this);
                                    if (propertiesToSet.Length != valuesToSet.Length)
                                    {
                                        Log.Error("Properties - values count mismatch, properties '{0}', values '{1}'",
                                            string.Join(", ", propertiesToSet), string.Join(", ", valuesToSet));
                                    }
                                    for (int index = 0; index < propertiesToSet.Length && index < valuesToSet.Length; index++)
                                    {
                                        if (PropertyHelper.TrySetPropertyValue(model, propertiesToSet[index], valuesToSet[index]))
                                        {
                                            Log.Debug("Updated property '{0}' on model type '{1}' to '{2}'", propertiesToSet[index], model.GetType().Name, ObjectToStringHelper.ToString(valuesToSet[index]));
                                        }
                                        else
                                        {
                                            Log.Warning("Failed to set property '{0}' on model type '{1}'", propertiesToSet[index], model.GetType().Name);
                                        }
                                    }
                                }
                            //}
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
        /// Called when a property has changed for a view model type that the current view model is interested in. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <remarks>
        /// This method should only be called by Catel so the <see cref="ManagedViewModel"/> can invoke it. This method is only used as a pass-through
        /// to the actual <see cref="OnViewModelPropertyChanged"/> method.
        /// </remarks>
        void INotifyableViewModel.ViewModelPropertyChanged(IViewModel viewModel, string propertyName)
        {
            Log.Debug("A view model ('{0}') the current view model ('{1}') is interested in has changed a property ('{2}')",
                viewModel.GetType(), GetType(), propertyName);

            OnViewModelPropertyChanged(viewModel, propertyName);
        }

        /// <summary>
        /// Called when a property has changed for a view model type that the current view model is interested in. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnViewModelPropertyChanged(IViewModel viewModel, string propertyName)
        {
        }

        /// <summary>
        /// Called when a command for a view model type that the current view model is interested in has been executed. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter used during the execution.</param>
        /// <remarks>
        /// This method should only be called by Catel so the <see cref="ManagedViewModel"/> can invoke it. This method is only used as a pass-through
        /// to the actual <see cref="OnViewModelCommandExecuted"/> method.
        /// </remarks>
        void INotifyableViewModel.ViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
            Log.Debug("A view model ('{0}') the current view model ('{1}') is interested in has executed a command with tag '{2}'",
                viewModel.GetType(), GetType(), ObjectToStringHelper.ToString(command.Tag));

            OnViewModelCommandExecuted(viewModel, command, commandParameter);
        }

        /// <summary>
        /// Called when a command for a view model type that the current view model is interested in has been executed. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter used during the execution.</param>
        protected virtual void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
        }

        /// <summary>
        /// Views the model event.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewModelEvent">The view model event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method should only be called by Catel so the <see cref="ManagedViewModel"/> can invoke it. This method is only used as a pass-through
        /// to the actual <see cref="OnViewModelEvent"/> method.
        /// </remarks>
        void INotifyableViewModel.ViewModelEvent(IViewModel viewModel, ViewModelEvent viewModelEvent, EventArgs e)
        {
            Log.Debug("A view model ('{0}') the current view model ('{1}') is interested in has raised an event ('{2}')",
                viewModel.GetType(), GetType(), viewModelEvent.ToString());

            OnViewModelEvent(viewModel, viewModelEvent, e);
        }

        /// <summary>
        /// Called when an event for a view model type that the current view model is interested in has been raised. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewModelEvent">The view model event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModelEvent(IViewModel viewModel, ViewModelEvent viewModelEvent, EventArgs e)
        {
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
            foreach (KeyValuePair<string, ViewModelToModelMapping> map in _viewModelToModelMap)
            {
                ViewModelToModelMapping mapping = map.Value;
                IViewModelToModelConverter converter = mapping.Converter;
                if (converter.ShouldConvert(e.PropertyName))
                {
                    // Check if this is the right model (duplicate mappings might exist)
                    if (_modelObjects[mapping.ModelProperty] == sender)
                    {
                        // Only map properties in TwoWay or OneWay mode
                        if ((mapping.Mode == ViewModelToModelMode.TwoWay) || (mapping.Mode == ViewModelToModelMode.OneWay))
                        {
                            var values = new object[mapping.ValueProperties.Length];
                            for (var index = 0; index < mapping.ValueProperties.Length; index++)
                            {
                                var property = mapping.ValueProperties[index];
                                values[index] = PropertyHelper.GetPropertyValue(sender, property);
                            }

                            var convertedValue = mapping.Converter.Convert(values, this);
                            SetValue(mapping.ViewModelProperty, convertedValue);
                        }
                    }
                }
            }

            var modelNameOnViewModel = (from modelObject in _modelObjects
                                        where ObjectHelper.AreEqualReferences(modelObject.Value, sender)
                                        select modelObject.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(modelNameOnViewModel))
            {
                _modelsDirtyFlags[modelNameOnViewModel] = true;
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

            UninitializeModel(modelProperty, model, modelCleanUpMode);
            InitializeModel(modelProperty, model);
        }

        /// <summary>
        /// Initializes a model by subscribing to all events.
        /// </summary>
        /// <param name="modelProperty">The name of the model property.</param>
        /// <param name="model">The model.</param>
        private void InitializeModel(string modelProperty, object model)
        {
            if (model != null)
            {
                ViewModelManager.RegisterModel(this, model);
            }

            _modelsDirtyFlags[modelProperty] = false;
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

#if (NET || SL5) && !NET35
            if (ValidateModelsOnInitialization)
            {
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);

                System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);
                _modelErrorInfo[modelProperty].InitializeDefaultErrors(validationResults);
            }
#endif
        }

        /// <summary>
        /// Uninitializes a model by unsubscribing from all events.
        /// </summary>
        /// <param name="modelProperty">The name of the model property.</param>
        /// <param name="model">The model.</param>
        /// <param name="modelCleanUpMode">The model clean up mode.</param>
        private void UninitializeModel(string modelProperty, object model, ModelCleanUpMode modelCleanUpMode)
        {
            if (model != null)
            {
                ViewModelManager.UnregisterModel(this, model);
            }

            if (_modelErrorInfo.ContainsKey(modelProperty))
            {
                var modelErrorInfo = _modelErrorInfo[modelProperty];
                modelErrorInfo.Updated -= OnModelErrorInfoUpdated;
                modelErrorInfo.CleanUp();

                _modelErrorInfo.Remove(modelProperty);
            }

            if (SupportIEditableObject)
            {
                if (_modelObjectsInfo[modelProperty].SupportIEditableObject)
                {
                    switch (modelCleanUpMode)
                    {
                        case ModelCleanUpMode.CancelEdit:
                            try
                            {
                                EditableObjectHelper.CancelEditObject(model);
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(ex, "Failed to cancel the edit of object for model '{0}'", modelProperty);
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
        ///	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected virtual Task<bool> Cancel() { return Task.Factory.StartNew(() => true); }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected virtual Task<bool> Save() { return Task.Factory.StartNew(() => true); }

        /// <summary>
        /// Called when the view model is about to be closed.
        /// <para />
        /// This method also raises the <see cref="Closing"/> event.
        /// </summary>
        protected virtual void OnClosing()
        {
            Closing.SafeInvoke(this);
        }

        /// <summary>
        /// Closes this instance. Always called after the <see cref="Cancel"/> of <see cref="Save"/> method.
        /// </summary>
        protected virtual Task Close() { return Task.Factory.StartNew(() => { }); }

        /// <summary>
        /// Called when the view model has just been closed.
        /// <para />
        /// This method also raises the <see cref="Closed"/> event.
        /// </summary>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        protected virtual void OnClosed(bool? result)
        {
            Closed.SafeInvoke(this, new ViewModelClosedEventArgs(this, result));
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
        /// <remarks>
        /// It's not recommended to implement the initialization of properties in this method. The initialization of properties
        /// should be done in the constructor. This method should be used to start the retrieval of data from a web service or something
        /// similar.
        /// <para />
        /// During unit tests, it is recommended to manually call this method because there is no external container calling this method.
        /// </remarks>
        public void InitializeViewModel()
        {
            if (!_isViewModelInitialized)
            {
                _isViewModelInitialized = true;

                //MessageMediatorHelper.SubscribeRecipient(this);

                Initialize();

                Initialized.SafeInvoke(this);
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
        protected virtual void Initialize() { }

        /// <summary>
        /// Cancels the editing of the data.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public async Task<bool> CancelViewModel()
        {
            if (IsClosed)
            {
                return false;
            }

            var eventArgs = new CancelingEventArgs();
            Canceling.SafeInvoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("Canceling of view model '{0}' is canceled via the Canceling event", GetType());
                return false;
            }

            var cancel = await Cancel();

            Log.Info(cancel ? "Canceled view model '{0}'" : "Failed to cancel view model '{0}'", GetType());
            if (!cancel)
            {
                return false;
            }

            lock (_modelLock)
            {
                foreach (KeyValuePair<string, object> modelKeyValuePair in _modelObjects)
                {
                    UninitializeModel(modelKeyValuePair.Key, modelKeyValuePair.Value, ModelCleanUpMode.CancelEdit);
                }
            }

            Log.Info("Canceled view model '{0}'", GetType());

            Canceled.SafeInvoke(this);

            return true;
        }

        /// <summary>
        /// Cancels the editing of the data, but also closes the view model in the same call.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public async Task<bool> CancelAndCloseViewModel()
        {
            var result = await CancelViewModel();
            if (result)
            {
                await CloseViewModel(false);
            }

            return result;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public async Task<bool> SaveViewModel()
        {
            if (IsClosed)
            {
                return false;
            }

            IsSaving = true;

            if (DeferValidationUntilFirstSaveCall)
            {
                DeferValidationUntilFirstSaveCall = false;
            }

            // Force validation before saving
            if (!ValidateViewModel(true, false))
            {
                IsSaving = false;

                return false;
            }

            var eventArgs = new SavingEventArgs();
            Saving.SafeInvoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                IsSaving = false;

                Log.Info("Saving of view model '{0}' is canceled via the Saving event", GetType());
                return false;
            }

            var saved = await Save();

            Log.Info(saved ? "Saved view model '{0}'" : "Failed to save view model '{0}'", GetType());

            if (saved)
            {
                lock (_modelLock)
                {
                    foreach (KeyValuePair<string, object> modelKeyValuePair in _modelObjects)
                    {
                        UninitializeModel(modelKeyValuePair.Key, modelKeyValuePair.Value, ModelCleanUpMode.EndEdit);
                    }
                }

                Saved.SafeInvoke(this);
            }

            IsSaving = false;

            return saved;
        }

        /// <summary>
        /// Saves the data, but also closes the view model in the same call if the save succeeds.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public async Task<bool> SaveAndCloseViewModel()
        {
            bool result = await SaveViewModel();
            if (result)
            {
                await CloseViewModel(true);
            }

            return result;
        }

        /// <summary>
        /// Closes this instance. Always called after the <see cref="Cancel"/> of <see cref="Save"/> method.
        /// </summary>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        public async Task CloseViewModel(bool? result)
        {
            if (IsClosed)
            {
                return;
            }

            UninitializeThrottling();

            OnClosing();

            ViewModelManager.UnregisterAllModels(this);

            await Close();

            SuspendValidation = true;

            IsClosed = true;

            OnClosed(result);

            Log.Info("Closed view model '{0}'", GetType());

            ViewModelManager.UnregisterViewModelInstance(this);
        }
        #endregion
    }
}
