// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    using Collections;
    using Data;
    using IoC;
    using Auditing;
    using Logging;
    using Memento;
    using Messaging;
    using Reflection;
    using Services;

#if NET40 || SILVERLIGHT && !WINDOWS_PHONE
    using System.ComponentModel.DataAnnotations;
#endif

    #region Enums
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
    /// View model base for MVVM implementations. This class is based on the <see cref="ModelBase"/>, and supports all
    /// common interfaces used by WPF.
    /// </summary>
    /// <remarks>This view model base does not add any services. The technique specific implementation should take care of that
    /// (such as WPF, Silverlight, etc).</remarks>
    [AllowNonSerializableMembers]
    public abstract partial class ViewModelBase : ModelBase, IViewModel, INotifyableViewModel, IRelationalViewModel
    {
        /// <summary>
        /// Available clean up models for a model.
        /// </summary>
        private enum ModelCleanUpMode
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

        /// <summary>
        /// The dispatcher service used to dispatch all calls.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly IDispatcherService _dispatcherService;

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
        protected static readonly ViewModelManager ViewModelManager;

        /// <summary>
        /// Mappings from view model properties to models and their properties.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ViewModelToModelMapping> _viewModelToModelMap = new Dictionary<string, ViewModelToModelMapping>();

        /// <summary>
        /// Dictionary of properties that are decorated with the <see cref="ValidationToViewModelAttribute"/>. These properties should be
        /// updated after each validation sequence.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ValidationToViewModelAttribute> _validationSummaries = new Dictionary<string, ValidationToViewModelAttribute>();

#if NET
        /// <summary>
        /// The cached property descriptors. If the property is empty, the property descriptors are yet to be built. Otherwise
        /// this list can be used to return the cached property descriptors.
        /// </summary>
        [field: NonSerialized]
        private PropertyDescriptorCollection _propertyDescriptors;
#endif

#if NET
        /// <summary>
        /// List of view model properties that are implemented as properties and can be ignored by reflection.
        /// </summary>
        [field: NonSerialized]
#endif
        private static readonly HashSet<string> _viewModelImplementedProperties;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ViewModelBase" /> class.
        /// </summary>
        static ViewModelBase()
        {
            ViewModelManager = new ViewModelManager();

            var serviceLocator = IoC.ServiceLocator.Default;
            serviceLocator.RegisterInstance<IViewModelManager>(ViewModelManager);

            var properties = (from property in typeof(ViewModelBase).GetPropertiesEx(false)
                              select property.Name);
            _viewModelImplementedProperties = new HashSet<string>(properties);
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
        protected ViewModelBase(bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false)
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

            AuditingHelper.RegisterViewModel(this);

            Log.Debug("Creating view model of type '{0}' with unique identifier {1}", GetType().Name, UniqueIdentifier);

            _ignoreMultipleModelsWarning = ignoreMultipleModelsWarning;

            if (serviceLocator != null)
            {
                Log.Debug("Using a custom instance of the IServiceLocator");

                ServiceLocator = serviceLocator;
            }
            else
            {
                Log.Debug("Using the default instance of the IServiceLocator");

                // We always need an IoC provider
                ServiceLocator = IoC.ServiceLocator.Default;
            }

            Log.Debug("Registering view model services");

            RegisterViewModelServices(ServiceLocator);

            Log.Debug("Registered view model services");

            _dispatcherService = ServiceLocator.ResolveType<IDispatcherService>();

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

            if (ServiceLocator.IsTypeRegistered<IValidatorProvider>())
            {
                var validatorProvider = ServiceLocator.ResolveType<IValidatorProvider>();
                Validator = validatorProvider.GetValidator(GetType());
                if (Validator != null)
                {
                    Log.Debug("Found validator '{0}' for view model '{1}'", Validator.GetType().Name, GetType().Name);
                }
            }

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
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets the view model construction time, which is used to get unique instances of view models.
        /// </summary>
        /// <value>The view model construction time.</value>
        public DateTime ViewModelConstructionTime { get; private set; }

        /// <summary>
        /// Gets the parent view model.
        /// </summary>
        /// <value>The parent view model.</value>
        protected IViewModel ParentViewModel { get; private set; }

        /// <summary>
        /// Gets the <see cref="ViewModelCommandManager"/> of this view model.
        /// </summary>
        /// <value>The <see cref="ViewModelCommandManager"/>.</value>
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
        protected bool InvalidateCommandsOnPropertyChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all validation should be deferred until the first call to <see cref="SaveViewModel"/>.
        /// <para />
        /// If this value is <c>true</c>, all validation will be suspended. As soon as the first call is made to the <see cref="SaveViewModel"/>,
        /// the validation will no longer be suspended and activated.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the validation should be deferred; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If this value is used, it must be set as first property in the view model because the validation kicks in immediately
        /// when properties change.
        /// </remarks>
        protected bool DeferValidationUntilFirstSaveCall
        {
            get
            {
                return HideValidationResults;
            }
            set
            {
                HideValidationResults = value;
                RaisePropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the models as soon as they are initialized. This means that
        /// as soon as a model value is set, the view model checks whether the entity already contains errors.
        /// <para />
        /// If this value is <c>true</c>, the errors will immediately be returned for mappings on the model. Otherwise, the errors
        /// will only become available when a value is entered and then being undone.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the models should be validated on initialization; otherwise, <c>false</c>.
        /// </value>
        protected bool ValidateModelsOnInitialization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether models that implement <see cref="IEditableObject"/> are supported correctly.
        /// </summary>
        /// <value>
        /// <c>true</c> if models that implement <see cref="IEditableObject"/> are supported correctly; otherwise, <c>false</c>.
        /// </value>
        private bool SupportIEditableObject { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is currently saving.
        /// </summary>
        protected bool IsSaving { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RaisePropertyChanged"/> will be dispatched using
        /// the <see cref="IDispatcherService"/>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        protected bool DispatchPropertyChangedEvent { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
        /// <see cref="CancelViewModel"/>, <see cref="SaveViewModel"/> or <see cref="CloseViewModel"/>
        /// will have no effect.
        /// </summary>
        /// <value><c>true</c> if the view model is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public virtual string Title
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a value indicating whether this object contains any field or business errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
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

                return ChildViewModels.Any(childViewModel => childViewModel.HasDirtyModel);
            }
        }

        /// <summary>
        /// Gets the service locator that provides all the implementations for interfaces required by the view-model.
        /// </summary>
        /// <value>The service locator.</value>
        protected IServiceLocator ServiceLocator { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the properties with attributes.
        /// </summary>
        private void InitializePropertiesWithAttributes()
        {
            var viewModelType = GetType();

            var metaData = InitializeViewModelMetaData(viewModelType);

            _modelObjectsInfo.AddRange(metaData.Models);
            _viewModelToModelMap.AddRange(metaData.Mappings);
            _validationSummaries.AddRange(metaData.Validations);

#if NET
            _propertyDescriptors = new PropertyDescriptorCollection(null);
            foreach (ViewModelPropertyDescriptor propertyDescriptor in metaData.PropertyDescriptors)
            {
                // Since this is a dynamically exposed property, it must be registered
                var propertyName = propertyDescriptor.Name;
                if (!IsPropertyRegistered(propertyName))
                {
                    // Make sure that this is not a view model property itself
                    if (!_viewModelImplementedProperties.Contains(propertyName))
                    {
                        var propertyData = RegisterProperty(propertyName, propertyDescriptor.PropertyType);
                        InitializePropertyAfterConstruction(propertyData);                        
                    }
                }

                _propertyDescriptors.Add(ViewModelPropertyDescriptorFactory.CreatePropertyDescriptor(this, propertyDescriptor));
            }
#endif
        }

        /// <summary>
        /// Initializes the view model meta data.
        /// <para />
        /// This method only initializes the meta data once per view model type. If a type is already initialized,
        /// this method will immediately return.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>ViewModelMetadata.</returns>
        /// <exception cref="InvalidOperationException">The ExposeAttribute can only be used on a property that is also decorated with the ModelAttribute.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        private static ViewModelMetadata InitializeViewModelMetaData(Type viewModelType)
        {
            if (_metaData.ContainsKey(viewModelType))
            {
                Log.Debug("Metadata for view model '{0}' is already determined once, re-using existing metadata", viewModelType.FullName);
                return _metaData[viewModelType];
            }

            var properties = new List<PropertyInfo>();
            properties.AddRange(viewModelType.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true)));

#if NET
            var propertyDescriptors = new List<ViewModelPropertyDescriptor>();

            // All already registered properties
            var existingProperties = viewModelType.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false));
            foreach (var existingProperty in existingProperties)
            {
                propertyDescriptors.Add(new ViewModelPropertyDescriptor(null, existingProperty.Name, existingProperty.PropertyType));
            }
#endif

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

                #region Expose attributes
#if NET
                var exposeAttributes = propertyInfo.GetCustomAttributesEx(typeof(ExposeAttribute), true);
                if (exposeAttributes.Length > 0)
                {
                    if (!modelObjectsInfo.ContainsKey(propertyInfo.Name))
                    {
                        Log.Error("The ExposeAttribute can only be used on a property that is also decorated with the ModelAttribute");
                        throw new InvalidOperationException("The ExposeAttribute can only be used on a property that is also decorated with the ModelAttribute");
                    }

                    foreach (ExposeAttribute exposeAttribute in exposeAttributes)
                    {
                        // Manual mapping, treat the same as a ViewModelToModelAttribute
                        if (!viewModelToModelMap.ContainsKey(exposeAttribute.PropertyName))
                        {
                            viewModelToModelMap.Add(exposeAttribute.PropertyName, new ViewModelToModelMapping(exposeAttribute.PropertyName, propertyInfo.Name, exposeAttribute.PropertyNameOnModel, exposeAttribute.Mode));
                        }

                        var modelPropertyInfo = propertyInfo.PropertyType.GetPropertyEx(exposeAttribute.PropertyNameOnModel);
                        if (modelPropertyInfo == null)
                        {
                            string error = string.Format("The property '{0}' is not available on model '{1}' so cannot be mapped",
                                exposeAttribute.PropertyNameOnModel, propertyInfo.PropertyType.Name);

                            Log.Error(error);
                            throw new InvalidOperationException(error);
                        }

                        if ((modelPropertyInfo.GetSetMethod() == null) && (exposeAttribute.Mode != ViewModelToModelMode.OneWay))
                        {
                            string error = string.Format("The property '{0}' is read-only on model '{1}', but the mode is not OneWay",
                                exposeAttribute.PropertyNameOnModel, propertyInfo.PropertyType.Name);

                            Log.Error(error);
                            throw new InvalidOperationException(error);
                        }

                        propertyDescriptors.Add(ViewModelPropertyDescriptorFactory.CreatePropertyDescriptor(null, exposeAttribute.PropertyName, modelPropertyInfo.PropertyType));
                    }
                }
#endif
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

#if NET
            _metaData.Add(viewModelType, new ViewModelMetadata(viewModelType, modelObjectsInfo, viewModelToModelMap, validationSummaries, propertyDescriptors));
#else
            _metaData.Add(viewModelType, new ViewModelMetadata(viewModelType, modelObjectsInfo, viewModelToModelMap, validationSummaries));
#endif

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

                var modelPropertyPropertyInfo = modelPropertyType.GetPropertyEx(mapping.ValueProperty);
                if (modelPropertyPropertyInfo == null)
                {
                    Log.Warning("Mapped viewmodel property '{0}' to model property '{1}' is invalid because property '{1}' is not found on the model '{2}'.\n\n" +
                        "If the property is defined in a sub-interface, reflection does not return it as a valid property. If this is the case, you can safely ignore this warning",
                        mapping.ViewModelProperty, mapping.ValueProperty, mapping.ModelProperty);
                    //throw new PropertyNotFoundInModelException(mapping.ViewModelProperty, mapping.ModelProperty, mapping.ValueProperty);
                    // Disabled because a property defined in an interface is not detected by FlattenHierarchy
                }
                else
                {
                    if (viewModelPropertyType != modelPropertyPropertyInfo.PropertyType)
                    {
                        Log.Warning("Property '{0}' mapped on model property '{1}' is of the wrong type ('{2}'), should be '{3}'",
                            mapping.ViewModelProperty, mapping.ValueProperty, viewModelPropertyType, modelPropertyPropertyInfo.PropertyType);
                    }
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

            lock (_modelObjects)
            {
                foreach (var modelInfo in _modelObjectsInfo)
                {
                    _modelObjects.Add(modelInfo.Key, null);
                }
            }

            if (!Catel.Environment.IsInDesignMode)
            {
                if (SupportIEditableObject)
                {
                    lock (_modelObjects)
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
            }

            ValidateViewModelToModelMappings();

            if (!_ignoreMultipleModelsWarning)
            {
                lock (_modelObjects)
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
        /// Gets all models that are decorated with the <see cref="ModelAttribute"/>.
        /// </summary>
        /// <returns>Array of models.</returns>
        protected object[] GetAllModels()
        {
            return _modelObjects.Values.ToArray();
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

                    if (PropertyHelper.TrySetPropertyValue(model, mapping.ValueProperty, value))
                    {
                        Log.Debug("Updated property '{0}' on model type '{1}' to '{2}'", mapping.ValueProperty, model.GetType().Name, ObjectToStringHelper.ToString(value));
                    }
                    else
                    {
                        Log.Warning("Failed to set property '{0}' on model type '{1}'", mapping.ValueProperty, model.GetType().Name);
                    }
                }
            }

            Log.Debug("Updated all explicit view model to model mappings");
        }

        /// <summary>
        /// Raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// <para/>
        /// This is the one and only method that actually raises the <see cref="ObservableObject.PropertyChanged"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void RaisePropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            if (DispatchPropertyChangedEvent)
            {
                _dispatcherService.BeginInvoke(() => base.RaisePropertyChanged(sender, e));
            }
            else
            {
                base.RaisePropertyChanged(sender, e);
            }
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

            lock (_modelObjects)
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
                        if (string.CompareOrdinal(mapping.ModelProperty, e.PropertyName) == 0)
                        {
                            if (newModelValue != null)
                            {
                                // We have a new model, ignore OneWayToSource
                                if (mapping.Mode != ViewModelToModelMode.OneWayToSource)
                                {
                                    var value = PropertyHelper.GetPropertyValue(newModelValue, mapping.ValueProperty);
                                    SetValue(mapping.ViewModelProperty, value, true, ValidateModelsOnInitialization);
                                }
                            }
                            else
                            {
                                // Always restore default value when a model becomes null
                                var propertyData = GetPropertyData(mapping.ViewModelProperty);
                                var value = propertyData.GetDefaultValue();
                                SetValue(mapping.ViewModelProperty, value, true, ValidateModelsOnInitialization);
                            }
                        }
                    }
                }
            }

            // If we are validating, don't map view model values back to the model
            if (!IsValidating)
            {
                if (_viewModelToModelMap.ContainsKey(e.PropertyName))
                {
                    lock (_modelObjects)
                    {
                        ViewModelToModelMapping mapping = _viewModelToModelMap[e.PropertyName];
                        object model = _modelObjects[mapping.ModelProperty];
                        if (model != null)
                        {
                            object viewModelValue = GetValue(e.PropertyName);
                            object modelValue = PropertyHelper.GetPropertyValue(model, mapping.ValueProperty);
                            if (!ObjectHelper.AreEqualReferences(viewModelValue, modelValue))
                            {
                                object valueToSet = viewModelValue;
                                string propertyToSet = mapping.ValueProperty;

#if !WINDOWS_PHONE && !NET35
                                if (_modelErrorInfo.ContainsKey(mapping.ModelProperty))
                                {
                                    _modelErrorInfo[mapping.ModelProperty].ClearDefaultErrors(mapping.ValueProperty);
                                }
#endif

                                // Only TwoWay or OneWayToSource mappings should be mapped
                                if ((mapping.Mode == ViewModelToModelMode.TwoWay) || (mapping.Mode == ViewModelToModelMode.OneWayToSource))
                                {
                                    if (PropertyHelper.TrySetPropertyValue(model, propertyToSet, valueToSet))
                                    {
                                        Log.Debug("Updated property '{0}' on model type '{1}' to '{2}'", propertyToSet, model.GetType().Name, ObjectToStringHelper.ToString(valueToSet));
                                    }
                                    else
                                    {
                                        Log.Warning("Failed to set property '{0}' on model type '{1}'", propertyToSet, model.GetType().Name);
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
                if (string.CompareOrdinal(mapping.ValueProperty, e.PropertyName) == 0)
                {
                    // Check if this is the right model (duplicate mappings might exist)
                    if (_modelObjects[mapping.ModelProperty] == sender)
                    {
                        // Only map properties in TwoWay or OneWay mode
                        if ((mapping.Mode == ViewModelToModelMode.TwoWay) || (mapping.Mode == ViewModelToModelMode.OneWay))
                        {
                            object viewModelValue = GetValue(mapping.ViewModelProperty);
                            object modelValue = PropertyHelper.GetPropertyValue(sender, e.PropertyName);
                            if (!ObjectHelper.AreEqualReferences(viewModelValue, modelValue))
                            {
                                SetValue(mapping.ViewModelProperty, modelValue);
                            }
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
        }

        /// <summary>
        /// Called when the object is validating.
        /// </summary>
        protected override void OnValidating()
        {
            base.OnValidating();

            lock (_modelObjects)
            {
                foreach (KeyValuePair<string, object> model in _modelObjects)
                {
                    if (model.Value == null) continue;

                    var modelValueAsModelBaseBase = model.Value as ModelBase;
                    if (modelValueAsModelBaseBase != null)
                    {
                        modelValueAsModelBaseBase.Validate();
                    }
                }
            }

            lock (ChildViewModels)
            {
                var previousValue = _childViewModelsHaveErrors;

                _childViewModelsHaveErrors = false;

                foreach (IViewModel childViewModel in ChildViewModels)
                {
                    childViewModel.ValidateViewModel();
                    if (childViewModel.HasErrors)
                    {
                        _childViewModelsHaveErrors = true;
                        RaisePropertyChanged(() => HasErrors);
                    }
                }

                if (!_childViewModelsHaveErrors && (_childViewModelsHaveErrors != previousValue))
                {
                    RaisePropertyChanged(() => HasErrors);
                }
            }
        }

        /// <summary>
        /// Called when the object is validating the fields.
        /// </summary>
        protected override void OnValidatingFields()
        {
            base.OnValidatingFields();

            // Map all field errors and warnings from the model to this viewmodel
            foreach (KeyValuePair<string, ViewModelToModelMapping> viewModelToModelMap in _viewModelToModelMap)
            {
                ViewModelToModelMapping mapping = viewModelToModelMap.Value;
                var model = GetValue(mapping.ModelProperty);
                string modelProperty = mapping.ValueProperty;

                bool hasSetFieldError = false;
                bool hasSetFieldWarning = false;

                // IDataErrorInfo
                var dataErrorInfo = model as IDataErrorInfo;
                if (dataErrorInfo != null)
                {
                    if (!string.IsNullOrEmpty(dataErrorInfo[modelProperty]))
                    {
                        SetFieldValidationResult(FieldValidationResult.CreateError(mapping.ViewModelProperty, dataErrorInfo[modelProperty]));

                        hasSetFieldError = true;
                    }
                }

                // IDataWarningInfo
                var dataWarningInfo = model as IDataWarningInfo;
                if (dataWarningInfo != null)
                {
                    if (!string.IsNullOrEmpty(dataWarningInfo[modelProperty]))
                    {
                        SetFieldValidationResult(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, dataWarningInfo[modelProperty]));

                        hasSetFieldWarning = true;
                    }
                }

                // INotifyDataErrorInfo & INotifyDataWarningInfo
                if (_modelErrorInfo.ContainsKey(mapping.ModelProperty))
                {
                    var modelErrorInfo = _modelErrorInfo[mapping.ModelProperty];

                    if (!hasSetFieldError)
                    {
                        foreach (string error in modelErrorInfo.GetErrors(modelProperty))
                        {
                            if (!string.IsNullOrEmpty(error))
                            {
                                SetFieldValidationResult(FieldValidationResult.CreateError(mapping.ViewModelProperty, error));
                                break;
                            }
                        }
                    }

                    if (!hasSetFieldWarning)
                    {
                        foreach (string warning in modelErrorInfo.GetWarnings(modelProperty))
                        {
                            if (!string.IsNullOrEmpty(warning))
                            {
                                SetFieldValidationResult(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, warning));
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the object is validating the business rules.
        /// </summary>
        protected override void OnValidatingBusinessRules()
        {
            base.OnValidatingBusinessRules();

            lock (_modelObjects)
            {
                foreach (KeyValuePair<string, object> modelObject in _modelObjects)
                {
                    // IDataErrorInfo
                    var dataErrorInfo = modelObject.Value as IDataErrorInfo;
                    if ((dataErrorInfo != null) && !string.IsNullOrEmpty(dataErrorInfo.Error))
                    {
                        SetBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(dataErrorInfo.Error));
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = modelObject.Value as IDataWarningInfo;
                    if ((dataWarningInfo != null) && !string.IsNullOrEmpty(dataWarningInfo.Warning))
                    {
                        SetBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(dataWarningInfo.Warning));
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo
                    if (_modelErrorInfo.ContainsKey(modelObject.Key))
                    {
                        var modelErrorInfo = _modelErrorInfo[modelObject.Key];

                        foreach (string error in modelErrorInfo.GetErrors(string.Empty))
                        {
                            SetBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(error));
                        }

                        foreach (string warning in modelErrorInfo.GetWarnings(string.Empty))
                        {
                            SetBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(warning));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the object is validated.
        /// </summary>
        protected override void OnValidated()
        {
            bool updatedValidationSummaries = false;

            foreach (var validationSummaryInfo in _validationSummaries)
            {
                IValidationSummary validationSummary;
                if (validationSummaryInfo.Value.UseTagToFilter)
                {
                    validationSummary = this.GetValidationSummary(validationSummaryInfo.Value.IncludeChildViewModels, validationSummaryInfo.Value.Tag);
                }
                else
                {
                    validationSummary = this.GetValidationSummary(validationSummaryInfo.Value.IncludeChildViewModels);
                }

                PropertyHelper.SetPropertyValue(this, validationSummaryInfo.Key, validationSummary);

                updatedValidationSummaries = true;
            }

            if (updatedValidationSummaries)
            {
                ViewModelCommandManager.InvalidateCommands();
            }

            base.OnValidated();
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

            var modelAsINotifyPropertyChanged = model as INotifyPropertyChanged;
            if (modelAsINotifyPropertyChanged != null)
            {
                modelAsINotifyPropertyChanged.PropertyChanged += OnModelPropertyChangedInternal;
            }

            _modelsDirtyFlags[modelProperty] = false;
            _modelErrorInfo[modelProperty] = new ModelErrorInfo(model);

            _modelErrorInfo[modelProperty].Updated += OnModelErrorInfoUpdated;

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

#if (NET || SL4 || SL5) && !NET35
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
        /// 	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool Cancel() { return true; }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool Save() { return true; }

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
        protected virtual void Close() { }

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
        /// 	<c>true</c> if a specific property is registered as a model; otherwise, <c>false</c>.
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
        /// Gets the service of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <param name="tag">The tag.</param>
        /// <returns>Service object or <c>null</c> if the service is not found.</returns>
        public T GetService<T>(object tag = null)
        {
            return (T)ServiceLocator.ResolveType(typeof(T), tag);
        }

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

                MessageMediatorHelper.SubscribeRecipient(this);

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
        /// Validates the specified notify changed properties only.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, a validation is forced (even if the object knows it is already validated).</param>
        /// <param name="notifyChangedPropertiesOnly">if set to <c>true</c> only the properties for which the warnings or errors have been changed
        /// will be updated via <see cref="INotifyPropertyChanged.PropertyChanged"/>; otherwise all the properties that
        /// had warnings or errors but not anymore and properties still containing warnings or errors will be updated.</param>
        /// <returns>
        /// 	<c>true</c> if validation succeeds; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is useful when the view model is initialized before the window, and therefore WPF does not update the errors and warnings.
        /// </remarks>
        public bool ValidateViewModel(bool force = false, bool notifyChangedPropertiesOnly = true)
        {
            if (IsClosed)
            {
                return true;
            }

            Validate(force, notifyChangedPropertiesOnly);

            if (DeferValidationUntilFirstSaveCall)
            {
                return true;
            }

            return !HasErrors;
        }

        /// <summary>
        /// Cancels the editing of the data.
        /// </summary>
        public bool CancelViewModel()
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

            bool cancel = Cancel();

            Log.Info(cancel ? "Canceled view model '{0}'" : "Failed to cancel view model '{0}'", GetType());
            if (!cancel)
            {
                return false;
            }

            if (SupportIEditableObject)
            {
                lock (_modelObjects)
                {
                    foreach (KeyValuePair<string, object> modelKeyValuePair in _modelObjects)
                    {
                        UninitializeModel(modelKeyValuePair.Key, modelKeyValuePair.Value, ModelCleanUpMode.CancelEdit);
                    }
                }
            }

            Log.Info("Canceled view model '{0}'", GetType());

            Canceled.SafeInvoke(this);

            return true;
        }

        /// <summary>
        /// Cancels the editing of the data, but also closes the view model in the same call.
        /// </summary>
        public bool CancelAndCloseViewModel()
        {
            bool result = CancelViewModel();
            if (result)
            {
                CloseViewModel(false);
            }

            return result;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        public bool SaveViewModel()
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

            bool saved = Save();

            Log.Info(saved ? "Saved view model '{0}'" : "Failed to save view model '{0}'", GetType());

            if (saved)
            {
                if (SupportIEditableObject)
                {
                    lock (_modelObjects)
                    {
                        foreach (KeyValuePair<string, object> modelKeyValuePair in _modelObjects)
                        {
                            UninitializeModel(modelKeyValuePair.Key, modelKeyValuePair.Value, ModelCleanUpMode.EndEdit);
                        }
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
        /// <returns>
        /// 	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        public bool SaveAndCloseViewModel()
        {
            bool result = SaveViewModel();
            if (result)
            {
                CloseViewModel(true);
            }

            return result;
        }

        /// <summary>
        /// Closes this instance. Always called after the <see cref="Cancel"/> of <see cref="Save"/> method.
        /// </summary>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        public void CloseViewModel(bool? result)
        {
            if (IsClosed)
            {
                return;
            }

            OnClosing();

            ViewModelManager.UnregisterAllModels(this);

            Close();

            SuspendValidation = true;

            MessageMediatorHelper.UnsubscribeRecipient(this);

            IsClosed = true;

            OnClosed(result);

            Log.Info("Closed view model '{0}'", GetType());

            ViewModelManager.UnregisterViewModelInstance(this);
        }
        #endregion
    }
}
