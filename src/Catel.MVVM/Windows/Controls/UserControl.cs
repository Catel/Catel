﻿namespace Catel.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using MVVM.Providers;
    using MVVM.Views;
    using MVVM;

    using System.Windows;
    using System.Windows.Markup;
    using UIEventArgs = System.EventArgs;
    using Catel.Services;

    /// <summary>
    /// <see cref="UserControl"/> that supports MVVM by using a <see cref="IViewModel"/> typed parameter.
    /// If the user control is not constructed with the right view model by the developer, it will try to create
    /// the view model itself. It does this by keeping an eye on the <c>DataContext</c> property. If
    /// the property changes, the control will check the type of the DataContext and try to create the view model by using
    /// the DataContext value as the constructor. If the view model can be constructed, the DataContext of the UserControl will
    /// be replaced by the view model.
    /// </summary>
    /// <remarks>
    /// This control suffers a lot from the bugs, or features "by design" as Microsoft likes to call it, of WPF. Below are the most 
    /// common issues that this control suffers from:
    /// <list type="number">
    ///   <item>
    ///     <description>WPF sometimes invokes the Loaded multiple times, without invoking Unloaded.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public class UserControl : System.Windows.Controls.UserControl, IUserControl
    {
        private readonly UserControlLogic _logic;

        private event EventHandler<EventArgs>? _viewLoaded;
        private event EventHandler<EventArgs>? _viewUnloaded;
        private event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs>? _viewDataContextChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControl"/> class.
        /// </summary>
        /// <remarks>
        /// This method is required for design time support.
        /// </remarks>
        public UserControl(IServiceProvider serviceProvider, IViewModelWrapperService viewModelWrapperService,
            IDataContextSubscriptionService dataContextSubscriptionService)
            : this(null, serviceProvider, viewModelWrapperService, dataContextSubscriptionService) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControl"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="viewModelWrapperService">The view model wrapper service.</param>
        /// <param name="dataContextSubscriptionService">The data context subscription service.</param>
        /// <param name="viewModel">The view model.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public UserControl(IViewModel? viewModel, IServiceProvider serviceProvider, IViewModelWrapperService viewModelWrapperService,
            IDataContextSubscriptionService dataContextSubscriptionService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            this.FixBlurriness();

            _logic = new UserControlLogic(serviceProvider, viewModelWrapperService, this, null, viewModel);

            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, nameof(ActualWidth), StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, nameof(ActualHeight), StringComparison.InvariantCulture))
                {
                    PropertyChanged?.Invoke(this, e);
                }
            };

            _logic.ViewModelClosedAsync += OnViewModelClosedAsync;
            _logic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged?.Invoke(this, e);
            };

            Loaded += (sender, e) =>
            {
                _viewLoaded?.Invoke(this, EventArgs.Empty);

                OnLoaded(e);
            };

            Unloaded += (sender, e) =>
            {
                _viewUnloaded?.Invoke(this, EventArgs.Empty);

                OnUnloaded(e);
            };

            this.AddDataContextChangedHandler((sender, e) => _viewDataContextChanged?.Invoke(this, new Catel.MVVM.Views.DataContextChangedEventArgs(e.OldValue, e.NewValue)),
                dataContextSubscriptionService);
        }

        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<UserControlLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel? ViewModel
        {
            get { return _logic.GetValue<UserControlLogic, IViewModel?>(x => x.ViewModel); }
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
            get { return _logic.GetValue<UserControlLogic, ViewModelLifetimeManagement>(x => x.ViewModelLifetimeManagement); }
            set { _logic.SetValue<UserControlLogic>(x => x.ViewModelLifetimeManagement = value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether parent view model containers are supported. If supported,
        /// the user control will search for a <see cref="DependencyObject"/> that implements the <see cref="IViewModelContainer"/>
        /// interface. During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any parent control implementing the <see cref="IViewModelContainer"/> interface, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>false</c>.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if parent view model containers are supported; otherwise, <c>false</c>.
        /// </value>
        public bool SupportParentViewModelContainers
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.SupportParentViewModelContainers, true); }
            set { _logic.SetValue<UserControlLogic>(x => x.SupportParentViewModelContainers = value); }
        }

        /// <summary>
        /// Gets or sets a value for the <see cref="SupportParentViewModelContainers"/> property. This way, the behavior
        /// can be changed an entire application to prevent disabling it on every control.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        public static bool DefaultSupportParentViewModelContainersValue
        {
            get { return UserControlLogic.DefaultSupportParentViewModelContainersValue; }
            set { UserControlLogic.DefaultSupportParentViewModelContainersValue = value; }
        }

        /// <summary>
        /// Gets or sets the unload behavior when the data context of the target control changes.
        /// </summary>
        /// <value>The unload behavior.</value>
        public UnloadBehavior UnloadBehavior
        {
            get { return _logic.GetValue<UserControlLogic, UnloadBehavior>(x => x.UnloadBehavior, UnloadBehavior.SaveAndCloseViewModel); }
            set { _logic.SetValue<UserControlLogic>(x => x.UnloadBehavior = value); }
        }

        /// <summary>
        /// Gets or sets the default value for the <see cref="UnloadBehavior"/> property.
        /// <para />
        /// The default value is <see cref="Catel.MVVM.Providers.UnloadBehavior.SaveAndCloseViewModel"/>.
        /// </summary>
        /// <value>The unload behavior.</value>
        public static UnloadBehavior DefaultUnloadBehaviorValue
        {
            get { return UserControlLogic.DefaultUnloadBehaviorValue; }
            set { UserControlLogic.DefaultUnloadBehaviorValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to skip the search for an info bar message control. If not skipped,
        /// the user control will search for a the first <see cref="InfoBarMessageControl"/> that can be found. 
        /// During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any <see cref="InfoBarMessageControl"/>, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>true</c>.
        /// <para />
        /// The default value is determined by the <see cref="DefaultSkipSearchingForInfoBarMessageControlValue"/> property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the search for an info bar message control should be skipped; otherwise, <c>false</c>.
        /// </value>
        public bool SkipSearchingForInfoBarMessageControl
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.SkipSearchingForInfoBarMessageControl, false); }
            set { _logic.SetValue<UserControlLogic>(x => x.SkipSearchingForInfoBarMessageControl = value); }
        }

        /// <summary>
        /// Gets or sets a value for the <see cref="SkipSearchingForInfoBarMessageControl"/> property. This way, the behavior
        /// can be changed an entire application to prevent disabling it on every control.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>The default value.</value>
        /// <remarks>
        /// Internally this value uses the <see cref="UserControlLogic.DefaultSkipSearchingForInfoBarMessageControlValue"/> property.
        /// </remarks>
        public static bool DefaultSkipSearchingForInfoBarMessageControlValue
        {
            get { return UserControlLogic.DefaultSkipSearchingForInfoBarMessageControlValue; }
            set { UserControlLogic.DefaultSkipSearchingForInfoBarMessageControlValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create a <see cref="WarningAndErrorValidator"/> for the current
        /// control.
        /// <para />
        /// If a user control does not have any <see cref="InfoBarMessageControl"/> or equivalent control, it is useless to create
        /// a <see cref="WarningAndErrorValidator"/> for the current control.
        /// <para />
        /// The default value is determined by the <see cref="DefaultCreateWarningAndErrorValidatorForViewModelValue"/> property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="WarningAndErrorValidator"/> should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateWarningAndErrorValidatorForViewModel
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.CreateWarningAndErrorValidatorForViewModel, true); }
            set { _logic.SetValue<UserControlLogic>(x => x.CreateWarningAndErrorValidatorForViewModel = value); }
        }

        /// <summary>
        /// Gets or sets a value for the <see cref="CreateWarningAndErrorValidatorForViewModel"/> property. This way, the behavior
        /// can be changed an entire application to prevent disabling it on every control.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        public static bool DefaultCreateWarningAndErrorValidatorForViewModelValue
        {
            get { return UserControlLogic.DefaultCreateWarningAndErrorValidatorForViewModelValue; }
            set { UserControlLogic.DefaultCreateWarningAndErrorValidatorForViewModelValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user control should automatically be disabled when there is no
        /// active view model.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user control should automatically be disabled when there is no active view model; otherwise, <c>false</c>.
        /// </value>
        public bool DisableWhenNoViewModel
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.DisableWhenNoViewModel, false); }
            set { _logic.SetValue<UserControlLogic>(x => x.DisableWhenNoViewModel = value); }
        }

        /// <summary>
        /// Occurs when a property on the container has changed.
        /// </summary>
        /// <remarks>
        /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
        /// (mostly the container of a view model) because the .NET Framework does not allows us to.
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> property has changed.
        /// </summary>
        public event EventHandler<EventArgs>? ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs>? ViewModelPropertyChanged;

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        event EventHandler<EventArgs>? IView.Loaded
        {
            add { _viewLoaded += value; }
            remove { _viewLoaded -= value; }
        }

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        event EventHandler<EventArgs>? IView.Unloaded
        {
            add { _viewUnloaded += value; }
            remove { _viewUnloaded -= value; }
        }

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs>? IView.DataContextChanged
        {
            add { _viewDataContextChanged += value; }
            remove { _viewDataContextChanged -= value; }
        }

        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();

            ViewModelChanged?.Invoke(this, EventArgs.Empty);
            RaisePropertyChanged(nameof(ViewModel));

            if (_logic.HasVmProperty)
            {
                RaisePropertyChanged("VM");
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
        /// Adds a specified object as the child of a System.Windows.Controls.ContentControl.
        /// </summary>
        /// <param name="value">The object to add.</param>
        protected override void AddChild(object value)
        {
            // Fix for https://github.com/Catel/Catel/issues/1260, make sure to create the grid first (and we force it, this
            // might be a non-xaml (e.g. non-InitializeComponent) control
            if (!CatelEnvironment.IsInDesignMode)
            {
                var wrapper = _logic.CreateViewModelWrapper(true);
                if (wrapper is not null)
                {
                    // Pass on to the grid
                    ((IAddChild)Content).AddChild(value);
                    return;
                }
            }

            base.AddChild(value);
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
        /// Called when a property on the current <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has been closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>

        protected virtual Task OnViewModelClosedAsync(object sender, ViewModelClosedEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the user control is loaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoaded(UIEventArgs e)
        {
        }

        /// <summary>
        /// Called when the user control is unloaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnloaded(UIEventArgs e)
        {
        }

        /// <summary>
        /// Called when a dependency property on this control has changed.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}
