// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Caching;
    using Data;
    using IoC;
    using Logging;
    using MVVM;
    using Views;
    using Reflection;
    using Threading;
    using Catel.Services;

    /// <summary>
    /// Available unload behaviors.
    /// </summary>
    public enum UnloadBehavior
    {
        /// <summary>
        /// Closes the view model.
        /// </summary>
        CloseViewModel,

        /// <summary>
        /// Saves and closes the view model.
        /// </summary>
        SaveAndCloseViewModel,

        /// <summary>
        /// Cancels and closes the view model.
        /// </summary>
        CancelAndCloseViewModel
    }

    /// <summary>
    /// The available view model behaviors.
    /// </summary>
    public enum LogicViewModelBehavior
    {
        /// <summary>
        /// View model was injected thus will be stable during the lifetime of the view.
        /// </summary>
        Injected,

        /// <summary>
        /// View model is dynamic and will be automatically determined.
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// Base implementation of the behaviors, which defines all the different possible situations
    /// a behavior must implement / support to be a valid MVVM provider behavior.
    /// </summary>
    public abstract class LogicBase : ObservableObject, IViewLoadState, IUniqueIdentifyable
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static IViewModelFactory _viewModelFactory;
        private static readonly IViewModelLocator _viewModelLocator;
        private static readonly IViewManager _viewManager;
        private static readonly IViewPropertySelector _viewPropertySelector;
        private static readonly IViewContextService _viewContextService;
        private static readonly IObjectAdapter _objectAdapter;
        private static readonly Dictionary<Type, bool> _hasVmPropertyCache = new Dictionary<Type, bool>();

        /// <summary>
        /// The view model instances currently held by this provider. This value should only be used
        /// inside the <see cref="ViewModel"/> property. For accessing the view model, use the 
        /// <see cref="ViewModel"/> property.
        /// </summary>
        private IViewModel _viewModel;

        private bool _isFirstValidationAfterLoaded = true;

        /// <summary>
        /// The view load manager.
        /// </summary>
        protected static readonly IViewLoadManager ViewLoadManager;

        /// <summary>
        /// The lock object.
        /// </summary>
        protected readonly object _lockObject = new object();

        private IView _targetView;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="LogicBase"/> class.
        /// </summary>
        static LogicBase()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            _viewModelLocator = dependencyResolver.Resolve<IViewModelLocator>();
            _viewManager = dependencyResolver.Resolve<IViewManager>();
            _viewPropertySelector = dependencyResolver.Resolve<IViewPropertySelector>();
            _viewContextService = dependencyResolver.Resolve<IViewContextService>();
            _objectAdapter = dependencyResolver.Resolve<IObjectAdapter>();
            ViewLoadManager = dependencyResolver.Resolve<IViewLoadManager>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicBase"/> class.
        /// </summary>
        /// <param name="targetView">The target control.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetView"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        protected LogicBase(IView targetView, Type viewModelType = null, IViewModel viewModel = null)
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            Argument.IsNotNull("targetView", targetView);

            var targetViewType = targetView.GetType();

            if (!_hasVmPropertyCache.TryGetValue(targetViewType, out var hasVmProperty))
            {
                hasVmProperty = targetViewType.GetPropertyEx("VM") != null;
                _hasVmPropertyCache[targetViewType] = hasVmProperty;
            }

            HasVmProperty = hasVmProperty;

            if (viewModelType is null)
            {
                viewModelType = (viewModel != null) ? viewModel.GetType() : _viewModelLocator.ResolveViewModel(targetViewType);
                if (viewModelType is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>($"The view model of the view '{targetViewType.Name}' could not be resolved. Make sure to customize the IViewModelLocator or register the view and view model manually");
                }
            }

            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<LogicBase>();

            Log.Debug($"Constructing behavior '{GetType().Name}' for '{targetView.GetType().Name}' with unique id '{UniqueIdentifier}'");

            TargetView = targetView;
            ViewModelType = viewModelType;
            ViewModel = viewModel;

            ViewModelBehavior = (viewModel != null) ? LogicViewModelBehavior.Injected : LogicViewModelBehavior.Dynamic;
            ViewModelLifetimeManagement = ViewModelLifetimeManagement.Automatic;

            if (ViewModel != null)
            {
                SetDataContext(ViewModel);
            }

            Log.Debug("Subscribing to view events");

            ViewLoadManager.AddView(this);

            if (this.SubscribeToWeakGenericEvent<ViewLoadEventArgs>(ViewLoadManager, nameof(IViewLoadManager.ViewLoading), OnViewLoadedManagerLoadingInternal, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'ViewLoadManager.ViewLoading', going to subscribe without weak events");

                ViewLoadManager.ViewLoading += OnViewLoadedManagerLoadingInternal;
            }

            if (this.SubscribeToWeakGenericEvent<ViewLoadEventArgs>(ViewLoadManager, nameof(IViewLoadManager.ViewLoaded), OnViewLoadedManagerLoadedInternal, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'ViewLoadManager.ViewLoaded', going to subscribe without weak events");

                ViewLoadManager.ViewLoaded += OnViewLoadedManagerLoadedInternal;
            }

            if (this.SubscribeToWeakGenericEvent<ViewLoadEventArgs>(ViewLoadManager, nameof(IViewLoadManager.ViewUnloading), OnViewLoadedManagerUnloadingInternal, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'ViewLoadManager.ViewUnloading', going to subscribe without weak events");

                ViewLoadManager.ViewUnloading += OnViewLoadedManagerUnloadingInternal;
            }

            if (this.SubscribeToWeakGenericEvent<ViewLoadEventArgs>(ViewLoadManager, nameof(IViewLoadManager.ViewUnloaded), OnViewLoadedManagerUnloadedInternal, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'ViewLoadManager.ViewUnloaded', going to subscribe without weak events");

                ViewLoadManager.ViewUnloaded += OnViewLoadedManagerUnloadedInternal;
            }

            // Required so the ViewLoadManager can handle the rest
            targetView.Loaded += (sender, e) => Loaded?.Invoke(this, EventArgs.Empty);
            targetView.Unloaded += (sender, e) => Unloaded?.Invoke(this, EventArgs.Empty);

            TargetView.DataContextChanged += OnTargetViewDataContextChanged;

            Log.Debug("Subscribing to view properties");

            // This also subscribes to DataContextChanged, don't double subscribe
            var viewPropertiesToSubscribe = DetermineInterestingViewProperties();
            foreach (var viewPropertyToSubscribe in viewPropertiesToSubscribe)
            {
                TargetView.SubscribeToPropertyChanged(viewPropertyToSubscribe, OnTargetViewPropertyChanged);
            }

            Log.Debug($"Constructed behavior '{GetType().Name}' for '{TargetViewType?.Name}'");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the view model factory used to create the view model instances.
        /// </summary>
        protected IViewModelFactory ViewModelFactory
        {
            get
            {
                if (_viewModelFactory is null)
                {
                    var dependencyResolver = this.GetDependencyResolver();
                    _viewModelFactory = dependencyResolver.Resolve<IViewModelFactory>();
                }

                return _viewModelFactory;
            }
        }

        /// <summary>
        /// Gets the weak reference to the last known data context.
        /// </summary>
        protected WeakReference LastKnownDataContext { get; private set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        /// <remarks>
        /// When a new value is set, the old view model will be disposed.
        /// </remarks>
        public IViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            protected set
            {
                if (ReferenceEquals(_viewModel, value))
                {
                    return;
                }

                var oldViewModel = _viewModel;
                var newViewModel = value;

                Log.Debug($"Changing view model from '{oldViewModel?.GetType()}' to '{newViewModel?.GetType()}'");

                OnViewModelChanging();

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                    _viewModel.CanceledAsync -= OnViewModelCanceledAsync;
                    _viewModel.SavedAsync -= OnViewModelSavedAsync;
                    _viewModel.ClosedAsync -= OnViewModelClosedAsync;
                }

                _viewModel = value;

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                    _viewModel.CanceledAsync += OnViewModelCanceledAsync;
                    _viewModel.SavedAsync += OnViewModelSavedAsync;
                    _viewModel.ClosedAsync += OnViewModelClosedAsync;

                    // Must be in a try/catch because Silverlight sometimes throws out of range exceptions for bindings
                    try
                    {
                        SetDataContext(_viewModel);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Caught exception while setting DataContext to new value", ex);
                    }
                }

                OnViewModelChanged();

                ViewModelChanged?.Invoke(this, EventArgs.Empty);

                RaisePropertyChanged("ViewModel");

                if ((_viewModel != null) && IsTargetViewLoaded)
                {
                    // Target view is loaded, but it *could* be possible the container has not yet been registered. To
                    // make sure that the 
                    var targetViewAsViewModelContainer = TargetView as IViewModelContainer;
                    if (targetViewAsViewModelContainer != null)
                    {
                        ViewToViewModelMappingHelper.InitializeViewToViewModelMappings(targetViewAsViewModelContainer, _objectAdapter);
                    }

                    _viewModel.InitializeViewModelAsync();
                }
            }
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets the view model behavior.
        /// </summary>
        /// <value>The view model behavior.</value>
        public LogicViewModelBehavior ViewModelBehavior { get; private set; }

        /// <summary>
        /// Gets or sets the view model lifetime management.
        /// </summary>
        public ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets a value whether the target view has a 'VM' property available.
        /// </summary>
        public bool HasVmProperty { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the view model container should prevent the 
        /// creation of a view model.
        /// <para />
        /// This property is very useful when using views in transitions where the view model is no longer required.
        /// </summary>
        /// <value><c>true</c> if the view model container should prevent view model creation; otherwise, <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "ViewModelLifetimeManagement.FullyManual", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public bool PreventViewModelCreation { get; set; }

        /// <summary>
        /// Gets the target control of this MVVM provider.
        /// </summary>
        /// <value>The target control.</value>
        protected internal IView TargetView
        {
            get { return _targetView; }
            set
            {
                _targetView = value;
                TargetViewType = _targetView?.GetType();
            }
        }

        /// <summary>
        /// Gets the type of the target control.
        /// </summary>
        /// <value>The type of the target control.</value>
        protected Type TargetViewType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <c>null</c> DataContext should be ignored and no new view
        /// model should be created.
        /// <para />
        /// This property will automatically be set to <c>true</c> when a parent view model container invokes the
        /// <see cref="IViewLoadManager.ViewUnloading"/> event. It will be set to <c>false</c> again when the parent
        /// view model container invokes the <see cref="IViewLoadManager.ViewLoading"/>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if the <c>null</c> DataContext should be ignored; otherwise, <c>false</c>.</value>
        protected bool IgnoreNullDataContext { get; set; }

        /// <summary>
        /// Gets a value indicating whether the target control is loaded or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if the target control is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsTargetViewLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the control can be loaded. This is very useful in non-WPF classes where
        /// the <c>LayoutUpdated</c> is used instead of the <c>Loaded</c> event.
        /// <para />
        /// If this value is <c>true</c>, this logic implementation can call the <see cref="OnTargetViewLoadedAsync"/> when
        /// the control is loaded. Otherwise, the call will be ignored.
        /// </summary>
        /// <remarks>
        /// This value is introduced for Windows Phone because a navigation backwards still leads to a call to
        /// <c>LayoutUpdated</c>. To prevent new view models from being created, this property can be overridden by 
        /// such logic implementations.
        /// </remarks>
        /// <value><c>true</c> if this instance can control be loaded; otherwise, <c>false</c>.</value>
        protected virtual bool CanViewBeLoaded { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether this instance is loading.
        /// </summary>
        /// <value><c>true</c> if this instance is loading; otherwise, <c>false</c>.</value>
        protected bool IsLoading { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is unloading.
        /// </summary>
        /// <value><c>true</c> if this instance is unloading; otherwise, <c>false</c>.</value>
        protected bool IsUnloading { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current view model is being closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closing the current view model; otherwise, <c>false</c>.</value>
        protected bool IsClosingViewModel { get; private set; }

        private bool CanLoad
        {
            get
            {
                // Don't do this again (another bug in WPF: OnLoaded is called more than OnUnloaded)
                if (IsTargetViewLoaded)
                {
                    Log.Debug($"Cannot load target view '{TargetViewType?.Name}', view is already loaded");

                    return false;
                }

                if (!CanViewBeLoaded)
                {
                    Log.Debug($"Cannot load target view '{TargetViewType?.Name}', CanViewBeLoaded returned false");

                    return false;
                }

                return true;
            }
        }

        private bool CanUnload
        {
            get
            {
                // Don't do this again (another bug in WPF: OnLoaded is called more than OnUnloaded)
                if (!IsTargetViewLoaded)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// The view.
        /// </summary>
        IView IViewLoadState.View
        {
            get { return TargetView; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view model is about to construct a new view model. This event can be used to
        /// intercept and inject a dynamically instantiated view model.
        /// </summary>
        public event EventHandler<DetermineViewModelInstanceEventArgs> DetermineViewModelInstance;

        /// <summary>
        /// Occurs when the view model is about to construct a new view model. This event can be used to
        /// intercept and inject a dynamically determined view model type.
        /// </summary>
        public event EventHandler<DetermineViewModelTypeEventArgs> DetermineViewModelType;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> property has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the current <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> has been canceled.
        /// </summary>
        public event AsyncEventHandler<EventArgs> ViewModelCanceledAsync;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> has been saved.
        /// </summary>
        public event AsyncEventHandler<EventArgs> ViewModelSavedAsync;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> has been closed.
        /// </summary>
        public event AsyncEventHandler<ViewModelClosedEventArgs> ViewModelClosedAsync;

        /// <summary>
        /// Occurs when a property on the <see cref="TargetView"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> TargetViewPropertyChanged;

        /// <summary>
        /// Occurs when the view model container is loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view model container is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> Unloaded;
        #endregion

        #region Methods
        /// <summary>
        /// Determines the interesting view properties.
        /// </summary>
        /// <returns>A list of names with view properties to subscribe to.</returns>
        private List<string> DetermineInterestingViewProperties()
        {
            var targetViewType = TargetViewType;

            var finalProperties = new List<string>();

            if ((_viewPropertySelector is null) || (_viewPropertySelector.MustSubscribeToAllViewProperties(targetViewType)))
            {
                var viewProperties = TargetView.GetProperties();
                finalProperties.AddRange(viewProperties);
            }
            else
            {
                var propertiesToSubscribe = new HashSet<string>(_viewPropertySelector.GetViewPropertiesToSubscribeTo(targetViewType));
                if (!propertiesToSubscribe.Contains("DataContext"))
                {
                    propertiesToSubscribe.Add("DataContext");
                }

                foreach (var propertyToSubscribe in propertiesToSubscribe)
                {
                    if (!finalProperties.Contains(propertyToSubscribe))
                    {
                        finalProperties.Add(propertyToSubscribe);
                    }
                }
            }

            return finalProperties;
        }

        /// <summary>
        /// Gets the data context for the current view.
        /// </summary>
        /// <param name="view">The view to retrieve the data context from.</param>
        /// <returns>The data context.</returns>
        protected virtual object GetDataContext(IView view)
        {
            if (view is null)
            {
                return null;
            }

            return _viewContextService.GetContext(view);
        }

        /// <summary>
        /// Sets the data context of the target control.
        /// <para />
        /// This method is abstract because the real logic implementation knows how to set the data context (for example,
        /// by using an additional data context grid).
        /// </summary>
        /// <param name="newDataContext">The new data context.</param>
        protected abstract void SetDataContext(object newDataContext);

        /// <summary>
        /// Creates a view model by using data context or, if that is not possible, the constructor of the view model.
        /// </summary>
        protected IViewModel CreateViewModelByUsingDataContextOrConstructor()
        {
            var dataContext = GetDataContext(TargetView);

            // It might be possible that a view model is already set, so use it if the datacontext is a view model
            var dataContextAsIViewModel = dataContext as IViewModel;
            if ((dataContextAsIViewModel != null) && (dataContextAsIViewModel.GetType() == ViewModelType))
            {
                return dataContextAsIViewModel;
            }

            return ConstructViewModelUsingArgumentOrDefaultConstructor(dataContext);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> property is about to change.
        /// </summary>
        protected virtual void OnViewModelChanging()
        {
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> property has just been changed.
        /// </summary>
        protected virtual void OnViewModelChanged()
        {
        }

        /// <summary>
        /// Called when the view manager is unloading.
        /// <para />
        /// This method is public because the view loaded manager must be subscribed to as a weak event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        public void OnViewLoadedManagerLoadingInternal(object sender, ViewLoadEventArgs e)
        {
            OnViewLoadedManagerLoading(sender, e);
        }

        /// <summary>
        /// Called when the view manager is unloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewLoadedManagerLoading(object sender, ViewLoadEventArgs e)
        {
            if (ReferenceEquals(e.View, TargetView))
            {
                OnTargetViewLoadingInternal(TargetView, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the view manager is loaded.
        /// <para />
        /// This method is public because the view loaded manager must be subscribed to as a weak event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        public void OnViewLoadedManagerLoadedInternal(object sender, ViewLoadEventArgs e)
        {
            OnViewLoadedManagerLoaded(sender, e);
        }

        /// <summary>
        /// Called when the view manager is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewLoadedManagerLoaded(object sender, ViewLoadEventArgs e)
        {
            if (ReferenceEquals(e.View, TargetView))
            {
                OnTargetViewLoadedInternal(TargetView, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the view manager is unloading.
        /// <para />
        /// This method is public because the view loaded manager must be subscribed to as a weak event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        public void OnViewLoadedManagerUnloadingInternal(object sender, ViewLoadEventArgs e)
        {
            OnViewLoadedManagerUnloading(sender, e);
        }

        /// <summary>
        /// Called when the view manager is unloading.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewLoadedManagerUnloading(object sender, ViewLoadEventArgs e)
        {
            if (ReferenceEquals(e.View, TargetView))
            {
                OnTargetViewUnloadingInternal(TargetView, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the view manager is unloaded.
        /// <para />
        /// This method is public because the view loaded manager must be subscribed to as a weak event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        public void OnViewLoadedManagerUnloadedInternal(object sender, ViewLoadEventArgs e)
        {
            OnViewLoadedManagerUnloaded(sender, e);
        }

        /// <summary>
        /// Called when the view manager is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewLoadEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewLoadedManagerUnloaded(object sender, ViewLoadEventArgs e)
        {
            if (ReferenceEquals(e.View, TargetView))
            {
                OnTargetViewUnloadedInternal(TargetView, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetView"/> is about to be loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTargetViewLoadingInternal(object sender, EventArgs e)
        {
            if (!CanLoad)
            {
                return;
            }

            Log.Debug($"Target view '{TargetViewType?.Name}' is being loaded");

            IsLoading = true;
        }

        /// <summary>
        /// Called when the <see cref="TargetView"/> has just been loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method will call the <see cref="OnTargetViewLoadedAsync"/> which can be overriden for custom 
        /// behavior. This method is required to protect from duplicate loaded events.
        /// </remarks>
#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnTargetViewLoadedInternal(object sender, EventArgs e)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            if (!CanLoad)
            {
                return;
            }

            Log.Debug($"Target view '{TargetViewType?.Name}' has been loaded");

            var view = TargetView;
            if (view != null)
            {
                _viewManager.RegisterView(view);
            }

            IsTargetViewLoaded = true;

            var dataContext = GetDataContext(view);
            LastKnownDataContext = (dataContext != null) ? new WeakReference(dataContext) : null;

            await OnTargetViewLoadedAsync(sender, e);

            TargetView.EnsureVisualTree();

            var targetViewAsViewModelContainer = TargetView as IViewModelContainer;
            if (targetViewAsViewModelContainer != null)
            {
                ViewToViewModelMappingHelper.InitializeViewToViewModelMappings(targetViewAsViewModelContainer, _objectAdapter);
            }

            TargetView.Dispatch(() =>
            {
#pragma warning disable 4014
                // No need to await
                InitializeViewModelAsync();
#pragma warning restore 4014
            });

            IsLoading = false;
        }

        private async Task InitializeViewModelAsync()
        {
            var viewModel = ViewModel;
            if (ViewModel != null)
            {
                // Initialize the view model. The view model itself is responsible to prevent double initialization
                await viewModel.InitializeViewModelAsync();

                // Revalidate since the control already initialized the view model before the control
                // was visible, therefore the WPF engine does not show warnings and errors
                var viewModelAsViewModelBase = viewModel as ViewModelBase;
                if (viewModelAsViewModelBase != null)
                {
                    viewModelAsViewModelBase.Validate(true, false);
                }
                else
                {
                    viewModel.Validate(true);
                }

                _isFirstValidationAfterLoaded = true;
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetView" /> has just been loaded.
        /// <para />
        /// The base implementation will try to create a view model based on the current DataContext and
        /// set it as the DataContext of the <see cref="TargetView" />. To create custom logic for
        /// view model creation, override this method and do not call the base.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        /// <returns>Task.</returns>
        public virtual async Task OnTargetViewLoadedAsync(object sender, EventArgs e)
        {
            await CompleteViewModelClosingAsync();

            if (ViewModelLifetimeManagement == ViewModelLifetimeManagement.FullyManual)
            {
                Log.Debug($"View model lifetime management is set to '{ViewModelLifetimeManagement}', not creating view model on loaded event for '{TargetViewType?.Name}'");
                return;
            }

            if (ViewModel is null)
            {
                ViewModel = CreateViewModelByUsingDataContextOrConstructor();
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetView"/> is about to be unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTargetViewUnloadingInternal(object sender, EventArgs e)
        {
            if (!CanUnload)
            {
                return;
            }

            Log.Debug($"Target view '{TargetViewType?.Name}' is being unloaded");

            IsUnloading = true;
        }

        /// <summary>
        /// Called when the <see cref="TargetView"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method will call the <see cref="OnTargetViewUnloadedAsync"/> which can be overriden for custom 
        /// behavior. This method is required to protect from duplicate unloaded events.
        /// </remarks>
#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnTargetViewUnloadedInternal(object sender, EventArgs e)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            if (!CanUnload)
            {
                return;
            }

            Log.Debug($"Target view '{TargetViewType?.Name}' has been unloaded");

            var view = TargetView;
            if (view != null)
            {
                _viewManager.UnregisterView(view);
            }

            IsTargetViewLoaded = false;
            _isFirstValidationAfterLoaded = true;

            await OnTargetViewUnloadedAsync(sender, e);

            var targetViewAsViewModelContainer = TargetView as IViewModelContainer;
            if (targetViewAsViewModelContainer != null)
            {
                ViewToViewModelMappingHelper.UninitializeViewToViewModelMappings(targetViewAsViewModelContainer);
            }

            IsUnloading = false;
        }

        /// <summary>
        /// Called when the <see cref="TargetView" /> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        /// <returns>Task.</returns>
        public virtual Task OnTargetViewUnloadedAsync(object sender, EventArgs e)
        {
            return TaskHelper.Completed;
        }

        /// <summary>
        /// Gets a value indicating whether the specified arguments represent the current data context.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected bool IsCurrentDataContext(DataContextChangedEventArgs e)
        {
            if (e.AreEqual)
            {
                return true;
            }

            // CTL-891 Additional check for data context change
            var lastKnownDataContext = LastKnownDataContext;
            if (lastKnownDataContext != null && lastKnownDataContext.IsAlive)
            {
                if (ReferenceEquals(lastKnownDataContext.Target, e.NewContext))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the <c>DataContext</c> property of the <see cref="TargetView" /> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public virtual void OnTargetViewDataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            if (IsCurrentDataContext(e))
            {
                return;
            }

            var dataContext = GetDataContext(TargetView);

            Log.Debug($"DataContext of TargetView '{TargetViewType?.Name}' has changed to '{ObjectToStringHelper.ToTypeString(dataContext)}'");

            LastKnownDataContext = null;

            if (ReferenceEquals(dataContext, null))
            {
                return;
            }

            if (dataContext.IsSentinelBindingObject())
            {
                return;
            }

            // Here we have a data context that makes sense
            LastKnownDataContext = new WeakReference(dataContext);

            if (ReferenceEquals(ViewModel, dataContext))
            {
                return;
            }

            // Check if the VM is compatible
            if (_viewModelLocator.IsCompatible(TargetViewType, dataContext.GetType()))
            {
                // Use the view model from the data context, probably set manually
                ViewModel = (IViewModel)dataContext;
            }
        }

        /// <summary>
        /// Called when a property on the <see cref="TargetView" /> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnTargetViewPropertyChangedInternal(object sender, PropertyChangedEventArgs e)
        {
            OnTargetViewPropertyChanged(sender, e);
        }

        /// <summary>
        /// Called when a property on the <see cref="TargetView" /> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnTargetViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TargetViewPropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Called when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModelPropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public virtual Task OnViewModelCanceledAsync(object sender, EventArgs e)
        {
            return ViewModelCanceledAsync.SafeInvokeAsync(this, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public virtual Task OnViewModelSavedAsync(object sender, EventArgs e)
        {
            return ViewModelSavedAsync.SafeInvokeAsync(this, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.MVVM.ViewModelClosedEventArgs"/> instance containing the event data.</param>
        public virtual Task OnViewModelClosedAsync(object sender, ViewModelClosedEventArgs e)
        {
            return ViewModelClosedAsync.SafeInvokeAsync(this, e);
        }

        /// <summary>
        /// Validates the view model.
        /// </summary>
        public virtual void ValidateViewModel()
        {
            var vm = ViewModel;
            if (vm is null)
            {
                return;
            }

            vm.Validate(_isFirstValidationAfterLoaded);

            _isFirstValidationAfterLoaded = false;
        }

        /// <summary>
        /// Cancels the view model.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully canceled; otherwise <c>false</c>.</returns>
        public virtual Task<bool> CancelViewModelAsync()
        {
            if (ViewModel is null)
            {
                return TaskHelper<bool>.FromResult(true);
            }

            return ViewModel.CancelViewModelAsync();
        }

        /// <summary>
        /// Cancels and closes the view model.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully canceled; otherwise <c>false</c>.</returns>
        public async Task<bool> CancelAndCloseViewModelAsync()
        {
            if (!await CancelViewModelAsync())
            {
                return false;
            }

            await CloseViewModelAsync(true);

            return true;
        }

        /// <summary>
        /// Saves the view model.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully saved; otherwise <c>false</c>.</returns>
        public virtual Task<bool> SaveViewModelAsync()
        {
            var vm = ViewModel;
            if (vm is null)
            {
                return TaskHelper<bool>.FromResult(true);
            }

            return vm.SaveViewModelAsync();
        }

        /// <summary>
        /// Saves and closes the view model. If the saving fails, the view model is not closed.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully saved; otherwise <c>false</c>.</returns>
        public async Task<bool> SaveAndCloseViewModelAsync()
        {
            if (!await SaveViewModelAsync())
            {
                return false;
            }

            await CloseViewModelAsync(true);

            return true;
        }

        /// <summary>
        /// Closes the view model.
        /// </summary>
        public virtual Task CloseViewModelAsync(bool? result)
        {
            return CloseViewModelAsync(result, false);
        }

        /// <summary>
        /// Closes and disposes the view model.
        /// </summary>
        public async virtual Task CloseViewModelAsync(bool? result, bool dispose)
        {
            var vm = ViewModel;
            if (vm != null)
            {
                try
                {
                    lock (_lockObject)
                    {
                        IsClosingViewModel = true;
                    }

                    var isClosing = false;

                    var viewModelBase = vm as ViewModelBase;
                    if (viewModelBase != null)
                    {
                        isClosing = viewModelBase.IsClosing;
                    }

                    if (!isClosing && !vm.IsClosed)
                    {
                        await vm.CloseViewModelAsync(result);

                        if (dispose)
                        {
                            var disposable = vm as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }

                    ViewModel = null;
                }
                finally
                {
                    lock (_lockObject)
                    {
                        IsClosingViewModel = false;
                    }
                }
            }
        }

        /// <summary>
        /// Awaits until the closing of the view model is completed.
        /// </summary>
        /// <returns>Task.</returns>
        protected async Task CompleteViewModelClosingAsync()
        {
            while (true)
            {
                lock (_lockObject)
                {
                    if (!IsClosingViewModel)
                    {
                        return;
                    }
                }

                Log.Debug($"View '{TargetViewType}' is still closing the view model, awaiting completion");

                await TaskShim.Delay(5);
            }
        }

        /// <summary>
        /// Tries to construct the view model using the argument. If that fails, it will try to use
        /// the default constructor of the view model. If that is not available, <c>null</c> is returned.
        /// </summary>
        /// <param name="injectionObject">The object that is injected into the view model constructor.</param>
        /// <returns>
        /// Constructed view model or <c>null</c> if the view model could not be constructed.
        /// </returns>
        protected IViewModel ConstructViewModelUsingArgumentOrDefaultConstructor(object injectionObject)
        {
            return ConstructViewModelUsingArgumentOrDefaultConstructor(injectionObject, ViewModelType);
        }

        /// <summary>
        /// Tries to construct the view model using the argument. If that fails, it will try to use
        /// the default constructor of the view model. If that is not available, <c>null</c> is returned.
        /// </summary>
        /// <param name="injectionObject">The object that is injected into the view model constructor.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>Constructed view model or <c>null</c> if the view model could not be constructed.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        private IViewModel ConstructViewModelUsingArgumentOrDefaultConstructor(object injectionObject, Type viewModelType)
        {
            Argument.IsNotNull("viewModelType", viewModelType);

            if (ViewModelBehavior == LogicViewModelBehavior.Injected)
            {
                return ViewModel;
            }

            if (ViewModelLifetimeManagement == ViewModelLifetimeManagement.FullyManual)
            {
                Log.Debug($"View model lifetime management is set to '{ViewModelLifetimeManagement}', preventing view model creation for '{TargetViewType?.Name}'");
                return null;
            }

            // Can be removed soon, now managed via ViewModelLifetimeManagement
            if (PreventViewModelCreation)
            {
                Log.Info("ViewModel construction is prevented by the PreventViewModelCreation property");
                return null;
            }

            if (IgnoreNullDataContext && (injectionObject is null))
            {
                Log.Info("ViewModel construction is prevented by the IgnoreNullDataContext property");
                return null;
            }

            var determineViewModelInstanceHandler = DetermineViewModelInstance;
            if (determineViewModelInstanceHandler != null)
            {
                var determineViewModelInstanceEventArgs = new DetermineViewModelInstanceEventArgs(injectionObject);
                determineViewModelInstanceHandler(this, determineViewModelInstanceEventArgs);
                if (determineViewModelInstanceEventArgs.ViewModel != null)
                {
                    var viewModel = determineViewModelInstanceEventArgs.ViewModel;
                    Log.Info("ViewModel instance is overriden by the DetermineViewModelInstance event, using view model of type '{0}'", viewModel.GetType().Name);

                    return viewModel;
                }

                if (determineViewModelInstanceEventArgs.DoNotCreateViewModel)
                {
                    Log.Info("ViewModel construction is prevented by the DetermineViewModelInstance event (DoNotCreateViewModel is set to true)");
                    return null;
                }
            }

            var determineViewModelTypeHandler = DetermineViewModelType;
            if (determineViewModelTypeHandler != null)
            {
                var determineViewModelTypeEventArgs = new DetermineViewModelTypeEventArgs(injectionObject);
                determineViewModelTypeHandler(this, determineViewModelTypeEventArgs);
                if (determineViewModelTypeEventArgs.ViewModelType != null)
                {
                    Log.Info("ViewModelType is overriden by the DetermineViewModelType event, using '{0}' instead of '{1}'",
                        determineViewModelTypeEventArgs.ViewModelType.FullName, viewModelType.FullName);

                    viewModelType = determineViewModelTypeEventArgs.ViewModelType;
                }
            }

            var injectionObjectAsViewModel = injectionObject as IViewModel;
            if (injectionObjectAsViewModel != null)
            {
                var injectionObjectViewModelType = injectionObject.GetType();

                if (ViewModelFactory.CanReuseViewModel(TargetViewType, viewModelType, injectionObjectViewModelType, injectionObject as IViewModel))
                {
                    Log.Info("DataContext of type '{0}' is allowed to be reused by view '{1}', using the current DataContext as view model",
                             viewModelType.FullName, TargetViewType.FullName);

                    return (IViewModel)injectionObject;
                }
            }

            Log.Debug("Using IViewModelFactory '{0}' to instantiate the view model", ViewModelFactory.GetType().FullName);

            var viewModelInstance = ViewModelFactory.CreateViewModel(viewModelType, injectionObject);

            Log.Debug("Used IViewModelFactory to instantiate view model, the factory did{0} return a valid view model",
                (viewModelInstance != null) ? string.Empty : " NOT");

            return viewModelInstance;
        }
        #endregion
    }
}
