// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIViewController.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MonoTouch.UIKit
{
    using System;
    using System.ComponentModel;
    using global::MonoTouch.Foundation;
    using IoC;
    using Logging;
    using MVVM;
    using MVVM.Providers;
    using MVVM.Views;

    /// <summary>
    /// UI View controller implementation that automatically takes care of view models.
    /// </summary>
    [Register("UIViewController")]
    public class UIViewController : global::MonoTouch.UIKit.UIViewController, IPage
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IViewModelLocator _viewModelLocator;

        private readonly PageLogic _logic;
        private object _dataContext;

        private BindingContext _bindingContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="UIViewController"/> class.
        /// </summary>
        static UIViewController()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            _viewModelLocator = dependencyResolver.Resolve<IViewModelLocator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIViewController"/> class.
        /// </summary>
        public UIViewController()
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            var viewModelType = GetViewModelType();
            if (viewModelType is null)
            {
                Log.Debug("GetViewModelType() returned null, using the ViewModelLocator to resolve the view model");

                viewModelType = _viewModelLocator.ResolveViewModel(GetType());
                if (viewModelType is null)
                {
                    const string error = "The view model of the view could not be resolved. Use either the GetViewModelType() method or IViewModelLocator";
                    Log.Error(error);
                    throw new NotSupportedException(error);
                }
            }

            _logic = new PageLogic(this, viewModelType);
            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
                OnPropertyChanged(e);

                PropertyChanged?.Invoke(this, e);
            };

            _logic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged?.Invoke(this, e);
            };

            _logic.DetermineViewModelInstance += (sender, e) =>
            {
                e.ViewModel = GetViewModelInstance(e.DataContext);
            };

            _logic.DetermineViewModelType += (sender, e) =>
            {
                e.ViewModelType = GetViewModelType(e.DataContext);
            };

            _logic.ViewLoading += (sender, e) => ViewLoading?.Invoke(this);
            _logic.ViewLoaded += (sender, e) => ViewLoaded?.Invoke(this);
            _logic.ViewUnloading += (sender, e) => ViewUnloading?.Invoke(this);
            _logic.ViewUnloaded += (sender, e) => ViewUnloaded?.Invoke(this);
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                _dataContext = value;

                DataContextChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<PageLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model container should prevent the 
        /// creation of a view model.
        /// <para />
        /// This property is very useful when using views in transitions where the view model is no longer required.
        /// </summary>
        /// <value><c>true</c> if the view model container should prevent view model creation; otherwise, <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "ViewModelLifetimeManagement.FullyManual", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public bool PreventViewModelCreation
        {
            get { return _logic.GetValue<PageLogic, bool>(x => x.PreventViewModelCreation); }
            set { _logic.SetValue<PageLogic>(x => x.PreventViewModelCreation = value); }
        }

        /// <summary>
        /// Gets or sets a the view model lifetime management.
        /// <para />
        /// By default, this value is <see cref="ViewModelLifetimeManagement"/>.
        /// </summary>
        /// <value>
        /// The view model lifetime management.
        /// </value>
        public ViewModelLifetimeManagement ViewModelLifetimeManagement
        {
            get { return _logic.GetValue<PageLogic, ViewModelLifetimeManagement>(x => x.ViewModelLifetimeManagement); }
            set { _logic.SetValue<PageLogic>(x => x.ViewModelLifetimeManagement = value); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel
        {
            get { return _logic.GetValue<PageLogic, IViewModel>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets the parent of the view.
        /// </summary>
        /// <value>The parent.</value>
        object IView.Parent
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view is enabled.
        /// </summary>
        /// <value><c>true</c> if the view is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property on the container has changed.
        /// </summary>
        /// <remarks>
        /// This event makes it possible to externally subscribe to property changes of a view
        /// (mostly the container of a view model) because the .NET Framework does not allows us to.
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> property has changed.
        /// </summary>
        public event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;

        /// <summary>
        /// Occurs when the view model container is loading.
        /// </summary>
        public event EventHandler<EventArgs> ViewLoading;

        /// <summary>
        /// Occurs when the view model container is loaded.
        /// </summary>
        public event EventHandler<EventArgs> ViewLoaded;

        /// <summary>
        /// Occurs when the view model container starts unloading.
        /// </summary>
        public event EventHandler<EventArgs> ViewUnloading;

        /// <summary>
        /// Occurs when the view model container is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> ViewUnloaded;

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> Unloaded;

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        public event EventHandler<EventArgs> DataContextChanged;
        #endregion

        #region Methods
        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();

            ViewModelChanged?.Invoke(this);
            RaisePropertyChanged(nameof(ViewModel));

            if (_bindingContext != null)
            {
                _bindingContext.DetermineIfBindingsAreRequired(ViewModel);
            }
        }

        /// <summary>
        /// Raises the <c>PropertyChanged</c> event.
        /// </summary>
        /// <param name="propertyName">The property name to raise the event for.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when the bindings must be added. This can happen
        /// <para />
        /// Normally the binding system would take care of this.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns><c>true</c> if the bindings were successfully added.</returns>
        protected virtual void AddBindings(BindingContext bindingContext, IViewModel viewModel)
        {
        }

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            RaiseViewModelChanged();

            Loaded?.Invoke(this);

            InitializeBindingContext();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            // Note: call *after* base so NavigationAdapter always gets called
            Unloaded?.Invoke(this);

            UninitializeBindingContext();
        }

        private void InitializeBindingContext()
        {
            if (_bindingContext != null)
            {
                UninitializeBindingContext();
            }

            _bindingContext = new BindingContext();
            _bindingContext.BindingUpdateRequired += OnBindingUpdateRequired;
            _bindingContext.DetermineIfBindingsAreRequired(ViewModel);
        }

        private void UninitializeBindingContext()
        {
            if (_bindingContext is null)
            {
                return;
            }

            _bindingContext.BindingUpdateRequired -= OnBindingUpdateRequired;
            _bindingContext.Clear();
            _bindingContext = null;
        }

        private void OnBindingUpdateRequired(object sender, EventArgs e)
        {
            AddBindings(_bindingContext, ViewModel);
        }

        /// <summary>
        /// Gets the type of the view model. If this method returns <c>null</c>, the view model type will be retrieved by naming 
        /// convention using the <see cref="IViewModelLocator"/> registered in the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        protected virtual Type GetViewModelType()
        {
            return null;
        }

        /// <summary>
        /// Gets the type of the view model at runtime based on the <see cref="DataContext"/>. If this method returns 
        /// <c>null</c>, the earlier determined view model type will be used instead.
        /// </summary>
        /// <param name="dataContext">The data context. This value can be <c>null</c>.</param>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks>
        /// Note that this method is only called when the <see cref="DataContext"/> changes.
        /// </remarks>
        protected virtual Type GetViewModelType(object dataContext)
        {
            return null;
        }

        /// <summary>
        /// Gets the instance of the view model at runtime based on the <see cref="DataContext"/>. If this method returns 
        /// <c>null</c>, the logic will try to construct the view model by itself.
        /// </summary>
        /// <param name="dataContext">The data context. This value can be <c>null</c>.</param>
        /// <returns>The instance of the view model or <c>null</c> in case it should be auto created.</returns>
        /// <remarks>
        /// Note that this method is only called when the <see cref="DataContext"/> changes.
        /// </remarks>
        protected virtual IViewModel GetViewModelInstance(object dataContext)
        {
            return null;
        }

        /// <summary>
        /// Called when a dependency property on this control has changed.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when a property on the current <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <remarks>
        /// This method does not implement any logic and saves a developer from subscribing/unsubscribing
        /// to the <see cref="ViewModelChanged"/> event inside the same user control.
        /// </remarks>
        protected virtual void OnViewModelChanged()
        {
        }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected bool ValidateData()
        {
            return _logic.ValidateViewModel();
        }

        /// <summary>
        /// Discards all changes made by this window.
        /// </summary>
        protected void DiscardChanges()
        {
            _logic.CancelViewModel();
        }

        /// <summary>
        /// Applies all changes made by this window.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected bool ApplyChanges()
        {
            return _logic.SaveViewModel();
        }
        #endregion
    }
}
