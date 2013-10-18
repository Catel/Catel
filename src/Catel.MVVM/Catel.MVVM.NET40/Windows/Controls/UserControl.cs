// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Data;
    using IoC;
    using Logging;
    using MVVMProviders.Logic;
    using MVVM;

#if NETFX_CORE
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
    /// This control can resolve a view model in the following ways:<para />
    /// <list type="numbered">
    ///   <item>
    ///     <description>By using the <see cref="GetViewModelType()"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>By using the <see cref="IViewModelLocator"/> which is registered in the <see cref="IServiceLocator"/>.</description>
    ///   </item>
    /// </list>
    /// <para />
    /// This control suffers a lot from the bugs, or features "by design" as Microsoft likes to call it, of WPF. Below are the most 
    /// common issues that this control suffers from:
    /// <list type="number">
    ///   <item>
    ///     <description>WPF sometimes invokes the Loaded multiple times, without invoking Unloaded.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">The view model of the view could not be resolved. Use either the <see cref="GetViewModelType()"/> method or <see cref="IViewModelLocator"/>.</exception>
#if NETFX_CORE
    public class UserControl : global::Windows.UI.Xaml.Controls.UserControl, IUserControl
#else
    public class UserControl : System.Windows.Controls.UserControl, IUserControl
#endif
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IViewModelLocator ViewModelLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();

        private readonly UserControlLogic _logic;
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
            if (Catel.Environment.IsInDesignMode)
            {
                return;
            }

            var viewModelType = (viewModel != null) ? viewModel.GetType() : GetViewModelType();
            if (viewModelType == null)
            {
                Log.Debug("GetViewModelType() returned null, using the ViewModelLocator to resolve the view model");

                viewModelType = ViewModelLocator.ResolveViewModel(GetType());
                if (viewModelType == null)
                {
                    const string error = "The view model of the view could not be resolved. Use either the GetViewModelType() method or IViewModelLocator";
                    Log.Error(error);
                    throw new NotSupportedException(error);
                }
            }

            _logic = new UserControlLogic(this, viewModelType, viewModel);
            _logic.TargetControlPropertyChanged += (sender, e) =>
            {
#if !NET
                // WPF already calls this method automatically
                OnPropertyChanged(e.FxEventArgs);

                PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs(e.PropertyName));
#else
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, "ActualWidth", StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, "ActualHeight", StringComparison.InvariantCulture))
                {
                    PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs(e.PropertyName));
                }
#endif
            };

            _logic.ViewModelChanged += (sender, e) =>
            {
                OnViewModelChanged();

                ViewModelChanged.SafeInvoke(this, e);
                PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs("ViewModel"));
            };

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged.SafeInvoke(this, e);
            };

            _logic.DetermineViewModelInstance += (sender, e) =>
            {
                e.ViewModel = GetViewModelInstance(e.DataContext);
            };

            _logic.DetermineViewModelType += (sender, e) =>
            {
                e.ViewModelType = GetViewModelType(e.DataContext);
            };

            _logic.ViewModelClosed += OnViewModelClosed;

            Loaded += (sender, e) => OnLoaded(e);
            Unloaded += (sender, e) => OnUnloaded(e);
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

#if NET || SL4 || SL5
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
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.SkipSearchingForInfoBarMessageControl, true); }
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
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.CreateWarningAndErrorValidatorForViewModel, false); }
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

#if !NET
        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded
        {
            get { return _logic.GetValue<UserControlLogic, bool>(x => x.IsTargetControlLoaded, true); }
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
        #endregion

        #region Methods
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
        /// Gets the type of the view model at runtime based on the <see cref="FrameworkElement.DataContext"/>. If this method returns 
        /// <c>null</c>, the earlier determined view model type will be used instead.
        /// </summary>
        /// <param name="dataContext">The data context. This value can be <c>null</c>.</param>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks>
        /// Note that this method is only called when the <see cref="FrameworkElement.DataContext"/> changes.
        /// </remarks>
        protected virtual Type GetViewModelType(object dataContext)
        {
            return null;
        }

        /// <summary>
        /// Gets the instance of the view model at runtime based on the <see cref="FrameworkElement.DataContext"/>. If this method returns 
        /// <c>null</c>, the logic will try to construct the view model by itself.
        /// </summary>
        /// <param name="dataContext">The data context. This value can be <c>null</c>.</param>
        /// <returns>The instance of the view model or <c>null</c> in case it should be auto created.</returns>
        /// <remarks>
        /// Note that this method is only called when the <see cref="FrameworkElement.DataContext"/> changes.
        /// </remarks>
        protected virtual IViewModel GetViewModelInstance(object dataContext)
        {
            return null;
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
        protected bool DiscardChanges()
        {
            return _logic.CancelViewModel();
        }

        /// <summary>
        /// Applies all changes made by this window.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected bool ApplyChanges()
        {
            return _logic.SaveViewModel();
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
        protected virtual void OnViewModelClosed(object sender, ViewModelClosedEventArgs e)
        {
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

#if !NET
        /// <summary>
        /// Called when a dependency property on this control has changed.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyValueChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(DependencyPropertyValueChangedEventArgs e)
        {
        }
#endif
        #endregion
    }
}