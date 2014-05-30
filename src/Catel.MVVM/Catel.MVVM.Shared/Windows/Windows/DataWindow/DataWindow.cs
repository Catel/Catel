// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWindow.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using Catel.MVVM.Providers;
    using Catel.MVVM.Views;
    using Controls;
    using IoC;
    using Logging;
    using MVVM;
    using Exceptions = MVVM.Properties.Exceptions;

#if SILVERLIGHT
    using System.Windows.Media;
#endif

    /// <summary>
    /// Mode of the <see cref="DataWindow"/>.
    /// </summary>
    public enum DataWindowMode
    {
        /// <summary>
        /// Window contains OK and Cancel buttons.
        /// </summary>
        OkCancel,

        /// <summary>
        /// Window contains OK, Cancel and Apply buttons.
        /// </summary>
        OkCancelApply,

        /// <summary>
        /// Window contains Close button.
        /// </summary>
        Close,

        /// <summary>
        /// Window contains custom buttons.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Available default buttons on the data window mode.
    /// </summary>
    public enum DataWindowDefaultButton
    {
        /// <summary>
        /// OK button.
        /// </summary>
        OK,

        /// <summary>
        /// Apply button.
        /// </summary>
        Apply,

        /// <summary>
        /// Close button.
        /// </summary>
        Close,

        /// <summary>
        /// No button.
        /// </summary>
        None
    }

    /// <summary>
    /// Defines the way the <see cref="InfoBarMessageControl"/> is included in the <see cref="DataWindow"/>.
    /// </summary>
    public enum InfoBarMessageControlGenerationMode
    {
        /// <summary>
        /// No <see cref="InfoBarMessageControl"/> is generated.
        /// </summary>
        None,

        /// <summary>
        /// Generate the <see cref="InfoBarMessageControl"/> as inline.
        /// </summary>
        Inline,

        /// <summary>
        /// Generate the <see cref="InfoBarMessageControl"/> as overlay.
        /// </summary>
        Overlay
    }

    /// <summary>
    /// <see cref="Window"/> class that implements the <see cref="InfoBarMessageControl"/> and
    /// the default buttons, according to the <see cref="DataWindowMode"/>.
    /// </summary>
    public class DataWindow
#if SILVERLIGHT
        : ChildWindow, IDataWindow
#else
        : Window, IDataWindow
#endif
    {
        #region Constants
#if NET
        /// <summary>
        /// Offset of the window to the sides of the primary monitor.
        /// </summary>
        private const int Offset = 50;
#endif
        #endregion

        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IViewModelLocator _viewModelLocator;

        private bool _isWrapped;

        private ICommand _defaultOkCommand;
        private ButtonBase _defaultOkElement;
        private ICommand _defaultCancelCommand;

        private readonly Collection<DataWindowButton> _buttons = new Collection<DataWindowButton>();
        private readonly Collection<ICommand> _commands = new Collection<ICommand>();
        private readonly InfoBarMessageControlGenerationMode _infoBarMessageControlGenerationMode;

        private readonly WindowLogic _logic;

        private event EventHandler<EventArgs> _viewLoaded;
        private event EventHandler<EventArgs> _viewUnloaded;
        private event EventHandler<EventArgs> _viewDataContextChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindow"/> class.
        /// <para />
        /// Registers the <see cref="IViewModelLocator"/> in the <see cref="IServiceLocator"/> if it is not yet registered.
        /// </summary>
        static DataWindow()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            _viewModelLocator = dependencyResolver.Resolve<IViewModelLocator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.FrameworkElement"/> class.
        /// </summary>
        /// <remarks>
        /// This method is required for design time support.
        /// </remarks>
        public DataWindow()
            : this(DataWindowMode.OkCancel) { }

        /// <summary>
        /// Initializes a new instance of this class with custom commands.
        /// </summary>
        /// <param name="mode"><see cref="DataWindowMode"/>.</param>
        /// <param name="additionalButtons">The additional buttons.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <param name="setOwnerAndFocus">if set to <c>true</c>, set the main window as owner window and focus the window.</param>
        /// <param name="infoBarMessageControlGenerationMode">The info bar message control generation mode.</param>
        public DataWindow(DataWindowMode mode, IEnumerable<DataWindowButton> additionalButtons = null, 
            DataWindowDefaultButton defaultButton = DataWindowDefaultButton.OK, bool setOwnerAndFocus = true,
            InfoBarMessageControlGenerationMode infoBarMessageControlGenerationMode = InfoBarMessageControlGenerationMode.Inline)
            : this(null, mode, additionalButtons, defaultButton, setOwnerAndFocus, infoBarMessageControlGenerationMode) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <remarks>
        /// Explicit constructor with view model injection, required for <see cref="Activator.CreateInstance(System.Type)"/> which
        /// does not seem to support default parameter values.
        /// </remarks>
        public DataWindow(IViewModel viewModel)
            : this(viewModel, DataWindowMode.OkCancel)
        {
            // Do not remove this constructor, see remarks
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="mode"><see cref="DataWindowMode"/>.</param>
        /// <param name="additionalButtons">The additional buttons.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <param name="setOwnerAndFocus">if set to <c>true</c>, set the main window as owner window and focus the window.</param>
        /// <param name="infoBarMessageControlGenerationMode">The info bar message control generation mode.</param>
        public DataWindow(IViewModel viewModel, DataWindowMode mode, IEnumerable<DataWindowButton> additionalButtons = null, 
            DataWindowDefaultButton defaultButton = DataWindowDefaultButton.OK, bool setOwnerAndFocus = true,
            InfoBarMessageControlGenerationMode infoBarMessageControlGenerationMode = InfoBarMessageControlGenerationMode.Inline)
        {
            // Set window style (WPF doesn't allow styling on root elements of XAML files, too bad)
            // For more info, see http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/3059c0e4-c372-4da2-b384-28f271feef05/
#if SILVERLIGHT
            Style dataWindowStyle = null;
            if (this.TryFindResource(typeof(DataWindow), out dataWindowStyle))
            {
                DefaultStyleKey = typeof (DataWindow);
                //Style = dataWindowStyle;
            }
#else
            SetResourceReference(StyleProperty, typeof(DataWindow));
#endif

            Mode = mode;
            DefaultButton = defaultButton;
            _infoBarMessageControlGenerationMode = infoBarMessageControlGenerationMode;

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

#if NET
            SizeToContent = SizeToContent.WidthAndHeight;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            SnapsToDevicePixels = true;

            this.ApplyIconFromApplication();
#endif

            ThemeHelper.EnsureCatelMvvmThemeIsLoaded();

            var viewModelType = (viewModel != null) ? viewModel.GetType() : GetViewModelType();
            if (viewModelType == null)
            {
                Log.Debug("GetViewModelType() returned null, using the ViewModelLocator to resolve the view model");

                viewModelType = _viewModelLocator.ResolveViewModel(GetType());
                if (viewModelType == null)
                {
                    const string error = "The view model of the view could not be resolved. Use either the GetViewModelType() method or IViewModelLocator";
                    Log.Error(error);
                    throw new NotSupportedException(error);
                }
            }

            _logic = new WindowLogic(this, viewModelType, viewModel);
            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
#if !NET
                // WPF already calls this method automatically
                OnPropertyChanged(e);

                PropertyChanged.SafeInvoke(this, e);
#else
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, "ActualWidth", StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, "ActualHeight", StringComparison.InvariantCulture))
                {
                    PropertyChanged.SafeInvoke(this, e);
                }
#endif
            };

            _logic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(sender, e);

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
            _logic.ViewLoading += (sender, e) => ViewLoading.SafeInvoke(this);
            _logic.ViewLoaded += (sender, e) => ViewLoaded.SafeInvoke(this);
            _logic.ViewUnloading += (sender, e) => ViewUnloading.SafeInvoke(this);
            _logic.ViewUnloaded += (sender, e) => ViewUnloaded.SafeInvoke(this);

            SetBinding(TitleProperty, new Binding("Title"));

            if (additionalButtons != null)
            {
                foreach (var button in additionalButtons)
                {
                    _buttons.Add(button);
                }
            }

            CanClose = true;
            CanCloseUsingEscape = true;

            Loaded += (sender, e) => Initialize();
            Closing += OnDataWindowClosing;

            Loaded += (sender, e) =>
            {
#if SL5
                Dispatcher.BeginInvoke(RaiseCanExecuteChangedForAllCommands);
#endif

                OnLoaded(e);

                _viewLoaded.SafeInvoke(this);
            };

            Unloaded += (sender, e) =>
            {
                OnUnloaded(e);

                _viewUnloaded.SafeInvoke(this);
            };

            DataContextChanged += (sender, e) =>
            {
                _viewDataContextChanged.SafeInvoke(this);
            };

#if NET
            if (setOwnerAndFocus)
            {
                this.SetOwnerWindowAndFocus();
            }
            else
            {
                this.FocusFirstControl();
            }
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<WindowLogic, Type>(x => x.ViewModelType); }
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
            get { return _logic.GetValue<WindowLogic, bool>(x => x.PreventViewModelCreation); }
            set { _logic.SetValue<WindowLogic>(x => x.PreventViewModelCreation = value); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel
        {
            get { return _logic.GetValue<WindowLogic, IViewModel>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets the <see cref="DataWindowMode"/> that this window uses.
        /// </summary>
        protected DataWindowMode Mode { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can close.
        /// </summary>
        /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
        protected bool CanClose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can close using escape.
        /// </summary>
        /// <value><c>true</c> if this instance can close using escape; otherwise, <c>false</c>.</value>
        public bool CanCloseUsingEscape { get; set; }

        /// <summary>
        /// Gets the commands that are currently available on the data window.
        /// </summary>
        /// <value>The commands.</value>
        protected ReadOnlyCollection<ICommand> Commands { get { return new ReadOnlyCollection<ICommand>(_commands); } }

        /// <summary>
        /// Gets the default button.
        /// </summary>
        /// <value>The default button.</value>
        protected DataWindowDefaultButton DefaultButton { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is OK button available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is OK button available; otherwise, <c>false</c>.
        /// </value>
        private bool IsOKButtonAvailable
        {
            get
            {
                if (Mode == DataWindowMode.OkCancel || Mode == DataWindowMode.OkCancelApply)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is cancel button available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is cancel button available; otherwise, <c>false</c>.
        /// </value>
        private bool IsCancelButtonAvailable
        {
            get
            {
                if (Mode == DataWindowMode.OkCancel || Mode == DataWindowMode.OkCancelApply)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is apply button available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is apply button available; otherwise, <c>false</c>.
        /// </value>
        private bool IsApplyButtonAvailable
        {
            get
            {
                if (Mode == DataWindowMode.OkCancelApply)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is close button available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is close button available; otherwise, <c>false</c>.
        /// </value>
        private bool IsCloseButtonAvailable
        {
            get
            {
                if (Mode == DataWindowMode.Close)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window was closed by a 'user'-button.
        /// </summary>
        /// <value><c>true</c> if closed by button; otherwise, <c>false</c>.</value>
        private bool ClosedByButton { get; set; }

        /// <summary>
        /// Gets the internal grid. This control gives internal classes a change to add additional controls to
        /// the dynamically created grid.
        /// </summary>
        /// <value>The internal grid.</value>
        internal Grid InternalGrid { get; private set; }

        /// <summary>
        /// Gets the parent of the view.
        /// </summary>
        /// <value>The parent.</value>
        object IView.Parent
        {
            get { return this.GetParent(); }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Executes the OK command.
        /// </summary>
        protected void ExecuteOk()
        {
            if (OnOkCanExecute())
            {
                OnOkExecute();
            }
        }

        /// <summary>
        /// Determines whether the user can execute the OK command.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c>.</returns>
        protected bool OnOkCanExecute()
        {
            return ValidateData();
        }

        /// <summary>
        /// Handled when the user invokes the OK command.
        /// </summary>
        protected void OnOkExecute()
        {
            if (!ApplyChanges())
            {
                return;
            }

            ClosedByButton = true;
            SetDialogResultAndMakeSureWindowGetsClosed(true);
        }

        /// <summary>
        /// Executes the Cancel command.
        /// </summary>
        protected void ExecuteCancel()
        {
            if (OnCancelCanExecute())
            {
                OnCancelExecute();
            }
        }

        /// <summary>
        /// Determines whether the user can execute the Cancel command.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c>.</returns>
        protected bool OnCancelCanExecute()
        {
            return true;
        }

        /// <summary>
        /// Handled when the user invokes the Cancel command.
        /// </summary>
        protected void OnCancelExecute()
        {
            if (!DiscardChanges())
            {
                return;
            }

            ClosedByButton = true;
            if (!SetDialogResultAndMakeSureWindowGetsClosed(false))
            {
                Close();
            }
        }

        /// <summary>
        /// Executes the Apply command.
        /// </summary>
        protected void ExecuteApply()
        {
            if (OnApplyCanExecute())
            {
                OnApplyExcute();
            }
        }

        /// <summary>
        /// Determines whether the user can execute the Apply command.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c>.</returns>
        protected bool OnApplyCanExecute()
        {
            return ValidateData();
        }

        /// <summary>
        /// Handled when the user invokes the Apply command.
        /// </summary>
        protected void OnApplyExcute()
        {
            ApplyChanges();
        }

        /// <summary>
        /// Executes the Close command.
        /// </summary>
        protected void ExecuteClose()
        {
            if (OnCloseCanExecute())
            {
                OnCloseExecute();
            }
        }

        /// <summary>
        /// Determines whether the user can execute the Close command.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c>.</returns>
        protected bool OnCloseCanExecute()
        {
            return CanClose;
        }

        /// <summary>
        /// Handled when the user invokes the Close command.
        /// </summary>
        protected void OnCloseExecute()
        {
            Close();
        }
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
        event EventHandler<EventArgs> IView.DataContextChanged
        {
            add { _viewDataContextChanged += value; }
            remove { _viewDataContextChanged -= value; }
        }
        #endregion

        #region Methods
        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();

            ViewModelChanged.SafeInvoke(this);
            PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs("ViewModel"));
        }

#if SILVERLIGHT
        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.ChildWindow"/> control when a new template is applied.
        /// </summary>
        /// <remarks></remarks>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Solves issue where ChildWindow does not center when browser is not active
            var contentRoot = GetTemplateChild("ContentRoot") as FrameworkElement;
            if (contentRoot != null)
            {
                bool centerChildWindow = (HorizontalAlignment == HorizontalAlignment.Center) && (VerticalAlignment == VerticalAlignment.Center);
                if (centerChildWindow)
                {
                    Dispatcher.BeginInvoke(CenterInScreen);
                }
            }
            else
            {
                Log.Debug("Cannot center childwindow because 'ContentRoot' cannot be found");
            }
        }

        /// <summary>
        /// Centers the Silverlight ChildWindow in screen.
        /// </summary>
        protected void CenterInScreen()
        {
            var contentRoot = GetTemplateChild("ContentRoot") as FrameworkElement;
            if (contentRoot == null)
            {
                return;
            }

            var group = contentRoot.RenderTransform as TransformGroup;
            if (group == null)
            {
                return;
            }

            foreach (var transform in group.Children.OfType<TranslateTransform>())
            {
                // reset transform
                transform.X = 0.0;
                transform.Y = 0.0;
            }
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Invoked when an unhandled <c>KeyUp</c> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
#else
        /// <summary>
        /// Invoked when an unhandled <see cref="Keyboard.KeyDownEvent"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
#endif

            if (e.Handled)
            {
                return;
            }

            if (Keyboard.Modifiers != ModifierKeys.None)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (_defaultOkElement != null)
                {
                    _defaultOkElement.GotFocus += OnButtonReceivedFocus;
                    if (!_defaultOkElement.Focus())
                    {
                        HandleDefaultButton();
                    }

                    e.Handled = true;
                }
                else if (_defaultOkCommand != null)
                {
                    HandleDefaultButton();
                    e.Handled = true;
                }

                // Else let it go, it's a custom button
            }

            if (e.Key == Key.Escape && CanCloseUsingEscape)
            {
                if (_defaultCancelCommand != null)
                {
                    Log.Info("User pressed 'Escape', executing cancel command");

                    // Not everyone is using the ICatelCommand, make sure to check if execution is allowed
                    if (_defaultCancelCommand.CanExecute(null))
                    {
                        _defaultCancelCommand.Execute(null);
                        e.Handled = true;
                    }
                }
                else
                {
                    Log.Info("User pressed 'Escape' but no cancel command is found, setting DialogResult to false");

                    if (!SetDialogResultAndMakeSureWindowGetsClosed(false))
                    {
                        Close();
                    }

                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Sets the DialogResult, but keeps track of whether the DialogResult can actually be set. For example, 
        /// dialogs which are not called with <c>ShowDialog</c> can not set the DialogResult.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if the DialogResult is set correctly. Otherwise <c>false</c>.</returns>
        private bool SetDialogResultAndMakeSureWindowGetsClosed(bool? result)
        {
            try
            {
                DialogResult = result;
                return true;
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning(ex, "Failed to set DialogResult, probably the main window, closing window manually");
                Close();
                return true;
            }
        }

        /// <summary>
        /// Called when a button has received the focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnButtonReceivedFocus(object sender, EventArgs e)
        {
            var buttonBase = sender as ButtonBase;
            if (buttonBase == null)
            {
                return;
            }

            buttonBase.GotFocus -= OnButtonReceivedFocus;

            HandleDefaultButton();
        }

        /// <summary>
        /// Handles the default button, which can be done via a key event (enter).
        /// </summary>
        private void HandleDefaultButton()
        {
            if (_defaultOkCommand != null)
            {
                Log.Info("User pressed 'Enter', executing default command");

                // Not everyone is using the ICatelCommand, make sure to check if execution is allowed
                if (_defaultOkCommand.CanExecute(null))
                {
                    _defaultOkCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Adds a custom button to the list of buttons.
        /// </summary>
        /// <param name="dataWindowButton">The data window button.</param>
        /// <exception cref="InvalidOperationException">The <paramref name="dataWindowButton"/> is added when the window is already loaded.</exception>
        protected void AddCustomButton(DataWindowButton dataWindowButton)
        {
            if (InternalGrid != null)
            {
                throw new InvalidOperationException(Exceptions.DataWindowButtonCanOnlyBeAddedWhenWindowIsNotLoaded);
            }

            _buttons.Add(dataWindowButton);
        }

        /// <summary>
        /// Invoked when the content of this control has been changed. This method will add the dynamic controls automatically.
        /// </summary>
        /// <param name="oldContent">Old content.</param>
        /// <param name="newContent">New content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            if (!WrapControlHelper.CanBeWrapped(newContent as FrameworkElement) || _isWrapped)
            {
                return;
            }

            if (IsOKButtonAvailable)
            {
                var button = new DataWindowButton(MVVM.Properties.Resources.OK, OnOkExecute, OnOkCanExecute) { IsDefault = true };
                button.IsDefault = (DefaultButton == DataWindowDefaultButton.OK);
                _buttons.Add(button);
            }
            if (IsCancelButtonAvailable)
            {
                var button = new DataWindowButton(MVVM.Properties.Resources.Cancel, OnCancelExecute, OnCancelCanExecute);
                button.IsCancel = true;
                _buttons.Add(button);
            }
            if (IsApplyButtonAvailable)
            {
                var button = new DataWindowButton(MVVM.Properties.Resources.Apply, OnApplyExcute, OnApplyCanExecute);
                button.IsDefault = (DefaultButton == DataWindowDefaultButton.Apply);
                _buttons.Add(button);
            }
            if (IsCloseButtonAvailable)
            {
                var button = new DataWindowButton(MVVM.Properties.Resources.Close, OnCloseExecute, OnCloseCanExecute);
                button.IsDefault = (DefaultButton == DataWindowDefaultButton.Close);
                _buttons.Add(button);
            }

            foreach (DataWindowButton button in _buttons)
            {
                _commands.Add(button.Command);
            }

            var wrapOptions = WrapOptions.GenerateWarningAndErrorValidatorForDataContext;
            switch (_infoBarMessageControlGenerationMode)
            {
                case InfoBarMessageControlGenerationMode.None:
                    break;

                case InfoBarMessageControlGenerationMode.Inline:
                    wrapOptions |= WrapOptions.GenerateInlineInfoBarMessageControl;
                    break;

                case InfoBarMessageControlGenerationMode.Overlay:
                    wrapOptions |= WrapOptions.GenerateOverlayInfoBarMessageControl;
                    break;
            }

            _isWrapped = true;

            var contentGrid = WrapControlHelper.Wrap((FrameworkElement)newContent, wrapOptions, _buttons.ToArray(), this);

            var internalGrid = contentGrid.FindVisualDescendant(obj => (obj is FrameworkElement) && string.Equals(((FrameworkElement)obj).Name,  WrapControlHelper.InternalGridName)) as Grid;
            if (internalGrid != null)
            {
#if SILVERLIGHT
                internalGrid.Style = Application.Current.Resources["WindowGridStyle"] as Style;
#else
                internalGrid.SetResourceReference(StyleProperty, "WindowGridStyle");

                ((UIElement)newContent).FocusFirstControl();
#endif

                _defaultOkCommand = (from button in _buttons
                                     where button.IsDefault
                                     select button.Command).FirstOrDefault();
                _defaultOkElement = WrapControlHelper.GetWrappedElement<ButtonBase>(contentGrid, WrapControlHelper.DefaultOkButtonName);

                _defaultCancelCommand = (from button in _buttons
                                         where button.IsCancel
                                         select button.Command).FirstOrDefault();

                InternalGrid = internalGrid;

                OnInternalGridChanged();
            }
        }

        /// <summary>
        /// Called when the internal grid has changed.
        /// </summary>
        /// <remarks>
        /// This method is only invoked when the grid is set, not when the grid is cleared (which is something that should never happen).
        /// </remarks>
        protected virtual void OnInternalGridChanged() { }

        /// <summary>
        /// Handles the Closing event of the DataWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void OnDataWindowClosing(object sender, CancelEventArgs args)
        {
            if (!ClosedByButton)
            {
                DiscardChanges();
            }
        }

        /// <summary>
        /// Initializes the window.
        /// </summary>
        protected virtual void Initialize()
        { }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected virtual bool ValidateData()
        {
            return _logic.ValidateViewModel();
        }

        /// <summary>
        /// Applies all changes made by this window.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected virtual bool ApplyChanges()
        {
            return _logic.SaveViewModel();
        }

        /// <summary>
        /// Discards all changes made by this window.
        /// </summary>
        protected virtual bool DiscardChanges()
        {
            return _logic.CancelViewModel();
        }

        /// <summary>
        /// Raises the can <see cref="ICommand.CanExecuteChanged"/> for all commands.
        /// </summary>
        protected void RaiseCanExecuteChangedForAllCommands()
        {
            foreach (ICommand command in Commands)
            {
                var commandAsICatelCommand = command as ICatelCommand;
                if (commandAsICatelCommand != null)
                {
                    commandAsICatelCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Called when a property on the current view model has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChangedForAllCommands();

            OnViewModelPropertyChanged(e);
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
        /// Called when the <see cref="DataWindow"/> is loaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoaded(EventArgs e) { }

        /// <summary>
        /// Called when the <see cref="DataWindow"/> is unloaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnloaded(EventArgs e) { }

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