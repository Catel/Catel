// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.Threading;
    using MVVM.Providers;
    using MVVM.Views;
    using MVVM;

#if UWP
    using global::Windows.UI.Xaml;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using UIEventArgs = System.EventArgs;
#endif

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
#if UWP
    public class UserControl : global::Windows.UI.Xaml.Controls.UserControl, IUserControl
#else
    public class UserControl : System.Windows.Controls.UserControl, IUserControl
#endif
    {
        #region Fields
        private readonly UserControlLogic _logic;

        private event EventHandler<EventArgs> _viewLoaded;
        private event EventHandler<EventArgs> _viewUnloaded;
        private event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> _viewDataContextChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UserControl"/> class.
        /// </summary>
        /// <remarks>
        /// This method is required for design time support.
        /// </remarks>
        public UserControl()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControl"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public UserControl(IViewModel viewModel)
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            this.FixBlurriness();

            _logic = new UserControlLogic(this, null, viewModel);

            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
#if !NET && !NETCORE
                // WPF already calls this method automatically
                OnPropertyChanged(e);

                PropertyChanged?.Invoke(this, e);
#else
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, "ActualWidth", StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, "ActualHeight", StringComparison.InvariantCulture))
                {
                    PropertyChanged?.Invoke(this, e);
                }
#endif
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

            this.AddDataContextChangedHandler((sender, e) => _viewDataContextChanged?.Invoke(this, new Catel.MVVM.Views.DataContextChangedEventArgs(e.OldValue, e.NewValue)));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<UserControlLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model container should prevent the 
        /// creation of a view model.
        /// <para />
        /// This property is very useful when using views in transitions where the view model is no longer required.
        /// </summary>
        /// <value><c>true</c> if the view model container should prevent view model creation; otherwise, <c>false</c>.</value>
        public bool PreventViewModelCreation
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.PreventViewModelCreation); }
            set { _logic.SetValue<UserControlLogic>(x => x.PreventViewModelCreation = value); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel
        {
            get { return _logic.GetValue<UserControlLogic, IViewModel>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user control should close any existing
        /// view model when the control is unloaded from the visual tree.
        /// <para />
        /// Set this property to <c>false</c> if a view model should be kept alive and re-used
        /// for unloading/loading instead of creating a new one.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the view model should be closed when the control is unloaded; otherwise, <c>false</c>.
        /// </value>
        public bool CloseViewModelOnUnloaded
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.CloseViewModelOnUnloaded, true); }
            set { _logic.SetValue<UserControlLogic>(x => x.CloseViewModelOnUnloaded = value); }
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

#if NET || NETCORE
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
#endif

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

#if !NET && !NETCORE && !UWP
        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.IsTargetViewLoaded, true); }
        }
#endif

        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property on the container has changed.
        /// </summary>
        /// <remarks>
        /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
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
        /// Occurs when the view is loaded.
        /// </summary>
        event EventHandler<EventArgs> IView.Loaded
        {
            add { _viewLoaded += value; }
            remove { _viewLoaded -= value; }
        }

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        event EventHandler<EventArgs> IView.Unloaded
        {
            add { _viewUnloaded += value; }
            remove { _viewUnloaded -= value; }
        }

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> IView.DataContextChanged
        {
            add { _viewDataContextChanged += value; }
            remove { _viewDataContextChanged -= value; }
        }
        #endregion

        #region Methods
        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();

            ViewModelChanged?.Invoke(this, EventArgs.Empty);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
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
            return TaskHelper.Completed;
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
        #endregion
    }
}

#endif
