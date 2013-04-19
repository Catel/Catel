// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders.Logic
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;

    using Catel.Data;
    using Data;
    using IoC;
    using Logging;
    using MVVM;
    using MVVM.Views;
    using Reflection;
    using Windows.Threading;

#if NETFX_CORE
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows.Controls;

    using UIEventArgs = System.EventArgs;
#endif

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
    public abstract class LogicBase : ObservableObject
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The view model instances currently held by this provider. This value should only be used
        /// inside the <see cref="ViewModel"/> property. For accessing the view model, use the 
        /// <see cref="ViewModel"/> property.
        /// </summary>
        private IViewModel _viewModel;

        /// <summary>
        /// The view manager.
        /// </summary>
        private readonly IViewManager _viewManager;

        /// <summary>
        /// Boolean representing whether this is the first validation after the control has been loaded.
        /// </summary>
        private bool _isFirstValidationAfterLoaded = true;

        /// <summary>
        /// The view model factory used to create the view model instances.
        /// </summary>
        private readonly IViewModelFactory _viewModelFactory;

#if !NET
        private bool _isFirstLayoutUpdatedAfterUnloadedEvent;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="LogicBase" /> class.
        /// </summary>
        static LogicBase()
        {
            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicBase"/> class.
        /// </summary>
        /// <param name="targetControl">The target control.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetControl"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        protected LogicBase(FrameworkElement targetControl, Type viewModelType, IViewModel viewModel = null)
        {
            Argument.IsNotNull("targetControl", targetControl);
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.ImplementsInterface("viewModelType", viewModelType, typeof(IViewModel));

            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<LogicBase>();

            Log.Debug("Constructing behavior '{0}' for '{1}' with unique id '{2}'", GetType().Name, targetControl.GetType().Name, UniqueIdentifier);

            var serviceLocator = ServiceLocator.Default;

            _viewManager = serviceLocator.ResolveType<IViewManager>();
            _viewModelFactory = serviceLocator.ResolveType<IViewModelFactory>();

            TargetControl = targetControl;
            ViewModelType = viewModelType;
            ViewModel = viewModel;

#if SL4 || SL5
            TargetControl.FixUILanguageBug();
#endif

            ViewModelBehavior = (viewModel != null) ? LogicViewModelBehavior.Injected : LogicViewModelBehavior.Dynamic;

            if (ViewModel != null)
            {
                SetDataContext(ViewModel);
            }

            // Very impoortant to exit here in design mode
            if (Catel.Environment.IsInDesignMode)
            {
                return;
            }

#if NET
            TargetControl.Loaded += OnTargetControlLoadedInternal;
#else
            // Note: in silverlight, use LayoutUpdated instead of Loaded because the order is down => top instead of top => down
            TargetControl.LayoutUpdated += (sender, e) =>
            {
                if (_isFirstLayoutUpdatedAfterUnloadedEvent)
                {
                    _isFirstLayoutUpdatedAfterUnloadedEvent = false;
                    return;
                }

                OnTargetControlLoadedInternal(sender, new UIEventArgs());
            };
#endif

            TargetControl.Unloaded += OnTargetControlUnloadedInternal;

            // This also subscribes to DataContextChanged, don't double subscribe
            TargetControl.SubscribeToAllDependencyProperties(OnTargetControlPropertyChangedInternal);

#if NET
            IsTargetControlLoaded = TargetControl.IsLoaded;
#endif

            Log.Debug("Constructed behavior '{0}' for '{1}'", GetType().Name, TargetControl.GetType().Name);
        }
        #endregion

        #region Properties
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
                if (_viewModel == value)
                {
                    return;
                }

                OnViewModelChanging();

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                    _viewModel.Canceled -= OnViewModelCanceled;
                    _viewModel.Saved -= OnViewModelSaved;
                    _viewModel.Closed -= OnViewModelClosed;
                }

                _viewModel = value;

                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                    _viewModel.Canceled += OnViewModelCanceled;
                    _viewModel.Saved += OnViewModelSaved;
                    _viewModel.Closed += OnViewModelClosed;

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

                ViewModelChanged.SafeInvoke(this);

                RaisePropertyChanged("ViewModel");

                if ((_viewModel != null) && IsTargetControlLoaded)
                {
                    _viewModel.InitializeViewModel();
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
        /// Gets the type of the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the target control of this MVVM provider.
        /// </summary>
        /// <value>The target control.</value>
        protected internal FrameworkElement TargetControl { get; set; }

        /// <summary>
        /// Gets the type of the target control.
        /// </summary>
        /// <value>The type of the target control.</value>
        protected Type TargetControlType
        {
            get { return TargetControl.GetType(); }
        }

        /// <summary>
        /// Gets a value indicating whether the target control is loaded or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the target control is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsTargetControlLoaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the control can be loaded. This is very useful in non-WPF classes where
        /// the <c>LayoutUpdated</c> is used instead of the <c>Loaded</c> event.
        /// <para />
        /// If this value is <c>true</c>, this logic implementation can call the <see cref="OnTargetControlLoaded"/> when
        /// the control is loaded. Otherwise, the call will be ignored.
        /// </summary>
        /// <remarks>
        /// This value is introduced for Windows Phone because a navigation backwards still leads to a call to
        /// <c>LayoutUpdated</c>. To prevent new view models from being created, this property can be overridden by 
        /// such logic implementations.
        /// </remarks>
        /// <value><c>true</c> if this instance can control be loaded; otherwise, <c>false</c>.</value>
        protected virtual bool CanControlBeLoaded { get { return true; } }
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
        public event EventHandler<EventArgs> ViewModelCanceled;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> has been saved.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelSaved;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> has been closed.
        /// </summary>
        public event EventHandler<ViewModelClosedEventArgs> ViewModelClosed;

        /// <summary>
        /// Occurs when a property on the <see cref="TargetControl"/> has changed.
        /// </summary>
        public event EventHandler<DependencyPropertyValueChangedEventArgs> TargetControlPropertyChanged;
        #endregion

        #region Methods
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
            // It might be possible that a view model is already set, so use it if the datacontext is a view model
            var dataContextAsIViewModel = TargetControl.DataContext as IViewModel;
            if ((dataContextAsIViewModel != null) && (dataContextAsIViewModel.GetType() == ViewModelType))
            {
                return dataContextAsIViewModel;
            }

            return ConstructViewModelUsingArgumentOrDefaultConstructor(TargetControl.DataContext);
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
        /// Called when a property on the view model has changed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModelPropertyChanged(IViewModel viewModel, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when the <see cref="TargetControl"/> has just been loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method will call the <see cref="OnTargetControlLoaded"/> which can be overriden for custom 
        /// behavior. This method is required to protect from duplicate loaded events.
        /// </remarks>
        private void OnTargetControlLoadedInternal(object sender, UIEventArgs e)
        {
            // Don't do this again (another bug in WPF: OnLoaded is called more than OnUnloaded)
            if (IsTargetControlLoaded)
            {
                return;
            }

            if (!CanControlBeLoaded)
            {
                Log.Debug("Received Loaded or LayoutUpdated, but CanControlBeLoaded is false thus not treating the control as loaded");
                return;
            }

            Log.Debug("Target control '{0}' is loaded", TargetControl.GetType().Name);

            var view = TargetControl as IView;
            if (view == null)
            {
                Log.Warning("Cannot register view '{0}' in the view manager because it does not implement IView", TargetControl.GetType().FullName);
            }
            else
            {
                _viewManager.RegisterView(view);
            }

            IsTargetControlLoaded = true;

            OnTargetControlLoaded(sender, e);

#if !NET
            // According to the documentation, no visual tree is garantueed in the Loaded event of the user control.
            // However, as a solution the documentation says you need to manually call ApplyTemplate, so let's do that.
            // For more info, see http://msdn.microsoft.com/en-us/library/ms596558(vs.95)
            var targetControl = TargetControl as Control;
            if (targetControl != null)
            {
                (targetControl).ApplyTemplate();
            }
#endif

            var targetControlAsIViewModelContainer = TargetControl as IViewModelContainer;
            if (targetControlAsIViewModelContainer != null)
            {
                ViewToViewModelMappingHelper.InitializeViewToViewModelMappings(targetControlAsIViewModelContainer);
            }

#if NETFX_CORE
            TargetControl.Dispatcher.BeginInvoke(() =>
#else
            TargetControl.Dispatcher.BeginInvoke((ThreadStart)delegate
#endif
            {
                if (ViewModel != null)
                {
                    // Initialize the view model. The view model itself is responsible to prevent double initialization
                    ViewModel.InitializeViewModel();

                    // Revalidate since the control already initialized the view model before the control
                    // was visible, therefore the WPF engine does not show warnings and errors
                    var viewModelAsViewModelBase = ViewModel as ViewModelBase;
                    if (viewModelAsViewModelBase != null)
                    {
                        viewModelAsViewModelBase.Validate(true, false);
                    }
                    else
                    {
                        ViewModel.ValidateViewModel(true, false);
                    }

                    _isFirstValidationAfterLoaded = true;
                }
            });
        }

        /// <summary>
        /// Called when the <see cref="TargetControl"/> has just been loaded.
        /// <para />
        /// The base implementation will try to create a view model based on the current DataContext and
        /// set it as the DataContext of the <see cref="TargetControl"/>. To create custom logic for
        /// view model creation, override this method and do not call the base.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnTargetControlLoaded(object sender, UIEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = CreateViewModelByUsingDataContextOrConstructor();
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetControl"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method will call the <see cref="OnTargetControlUnloaded"/> which can be overriden for custom 
        /// behavior. This method is required to protect from duplicate unloaded events.
        /// </remarks>
        private void OnTargetControlUnloadedInternal(object sender, UIEventArgs e)
        {
            // Don't do this again (another bug in WPF: OnLoaded is called more than OnUnloaded)
            if (!IsTargetControlLoaded)
            {
                return;
            }

#if !NET
            _isFirstLayoutUpdatedAfterUnloadedEvent = true;
#endif

            Log.Debug("Target control '{0}' is unloaded", TargetControl.GetType().Name);

            var view = TargetControl as IView;
            if (view == null)
            {
                Log.Warning("Cannot unregister view '{0}' in the view manager because it does not implement IView", TargetControl.GetType().FullName);
            }
            else
            {
                _viewManager.UnregisterView(view);
            }

            IsTargetControlLoaded = false;
            _isFirstValidationAfterLoaded = true;

            OnTargetControlUnloaded(sender, e);

            var targetControlAsIViewModelContainer = TargetControl as IViewModelContainer;
            if (targetControlAsIViewModelContainer != null)
            {
                ViewToViewModelMappingHelper.UninitializeViewToViewModelMappings(targetControlAsIViewModelContainer);
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetControl"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnTargetControlUnloaded(object sender, UIEventArgs e)
        {   
        }

        /// <summary>
        /// Called when the <c>DataContext</c> property of the <see cref="TargetControl"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnTargetControlDataContextChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            Log.Debug("DataContext of TargetControl '{0}' has changed to '{1}'", TargetControl.GetType().Name, ObjectToStringHelper.ToTypeString(TargetControl.DataContext));

            var dataContext = TargetControl.DataContext;

            if (dataContext == null)
            {
                return;
            }

            if (BindingHelper.IsSentinelObject(dataContext))
            {
                return;
            }

            if (ViewModel == dataContext)
            {
                return;
            }

            if (dataContext.GetType().IsAssignableFromEx(ViewModelType))
            {
                // Use the view model from the data context, probably set manually
                ViewModel = (IViewModel)dataContext;
            }
        }

        /// <summary>
        /// Called when a property on the <see cref="TargetControl"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void OnTargetControlPropertyChangedInternal(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            if (!((FrameworkElement)sender).IsRealDependencyProperty(e.PropertyName))
            {
                // Ignore, this is a wrapper
                return;
            }

            //Log.Debug("Property '{0}' of TargetControl '{1}' has changed", e.PropertyName, TargetControl.GetType().Name);

            if (string.Equals(e.PropertyName, "DataContext", StringComparison.OrdinalIgnoreCase))
            {
                OnTargetControlDataContextChanged(sender, e);
            }

            OnTargetControlPropertyChanged(sender, e);
        }

        /// <summary>
        /// Called when a property on the <see cref="TargetControl"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyValueChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnTargetControlPropertyChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            TargetControlPropertyChanged.SafeInvoke(this, e);      
        }

        /// <summary>
        /// Called when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModelPropertyChanged.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public virtual void OnViewModelCanceled(object sender, EventArgs e)
        {
            ViewModelCanceled.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public virtual void OnViewModelSaved(object sender, EventArgs e)
        {
            ViewModelSaved.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.MVVM.ViewModelClosedEventArgs"/> instance containing the event data.</param>
        public virtual void OnViewModelClosed(object sender, ViewModelClosedEventArgs e)
        {
            ViewModelClosed.SafeInvoke(this, e);
        }

        /// <summary>
        /// Validates the view model.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="ViewModel"/> is valid; otherwise <c>false</c>.</returns>
        public virtual bool ValidateViewModel()
        {
            if (ViewModel == null)
            {
                return false;
            }

            bool result = ViewModel.ValidateViewModel(_isFirstValidationAfterLoaded, false);    

            _isFirstValidationAfterLoaded = false;

            return result;
        }

        /// <summary>
        /// Cancels the view model.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully canceled; otherwise <c>false</c>.</returns>
        public virtual bool CancelViewModel()
        {
            if (ViewModel == null)
            {
                return false;
            }

            return ViewModel.CancelViewModel();
        }

        /// <summary>
        /// Cancels and closes the view model.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully canceled; otherwise <c>false</c>.</returns>
        public bool CancelAndCloseViewModel()
        {
            bool result = CancelViewModel();
            if (!result)
            {
                return result;
            }

            CloseViewModel(result);

            return result;
        }

        /// <summary>
        /// Saves the view model.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully saved; otherwise <c>false</c>.</returns>
        public virtual bool SaveViewModel()
        {
            if (ViewModel == null)
            {
                return false;
            }

            return ViewModel.SaveViewModel();
        }

        /// <summary>
        /// Saves and closes the view model. If the saving fails, the view model is not closed.
        /// </summary>
        /// <returns><c>true</c> if the view model is successfully saved; otherwise <c>false</c>.</returns>
        public bool SaveAndCloseViewModel()
        {
            bool result = SaveViewModel();
            if (!result)
            {
                return result;
            }

            CloseViewModel(result);

            return result;
        }

        /// <summary>
        /// Closes the view model.
        /// </summary>
        public virtual void CloseViewModel(bool? result)
        {
            if (ViewModel != null)
            {
                ViewModel.CloseViewModel(result);
                ViewModel = null;
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

            var determineViewModelInstanceEventArgs = new DetermineViewModelInstanceEventArgs(injectionObject);
            DetermineViewModelInstance.SafeInvoke(this, determineViewModelInstanceEventArgs);
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

            var determineViewModelTypeEventArgs = new DetermineViewModelTypeEventArgs(injectionObject);
            DetermineViewModelType.SafeInvoke(this, determineViewModelTypeEventArgs);
            if (determineViewModelTypeEventArgs.ViewModelType != null)
            {
                Log.Info("ViewModelType is overriden by the DetermineViewModelType event, using '{0}' instead of '{1}'",
                    determineViewModelTypeEventArgs.ViewModelType.FullName, viewModelType.FullName);

                viewModelType = determineViewModelTypeEventArgs.ViewModelType;
            }

            if (_viewModelFactory.CanReuseViewModel(TargetControlType, viewModelType, injectionObject as IViewModel))
            {
                Log.Info("DataContext of type '{0}' is allowed to be reused by view '{1}', using the current DataContext as view model",
                    viewModelType.FullName, TargetControlType.FullName);

                return (IViewModel)injectionObject;
            }

            Log.Debug("Using IViewModelFactory '{0}' to instantiate the view model", _viewModelFactory.GetType().FullName);

            var viewModelInstance = _viewModelFactory.CreateViewModel(viewModelType, injectionObject);
            
            Log.Debug("Used IViewModelFactory to instantiate view model, the factory did{0} return a valid view model", 
                (viewModelInstance != null) ? string.Empty : " NOT");

            return viewModelInstance;
        }
        #endregion
    }
}