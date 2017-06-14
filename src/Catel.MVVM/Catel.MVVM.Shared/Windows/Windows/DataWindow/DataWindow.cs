// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWindow.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using MVVM.Views;
    using Controls;
    using Logging;
    using MVVM;
    using Exceptions = MVVM.Properties.Exceptions;
    using MVVM.Providers;
    using Catel.Services;
    using Catel.Threading;
    using Threading;
    using IoC;

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
    public class DataWindow : System.Windows.Window, IDataWindow
    {
        #region Constants
        /// <summary>
        /// Offset of the window to the sides of the primary monitor.
        /// </summary>
        private const int Offset = 50;
        #endregion

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IWrapControlService WrapControlService = ServiceLocator.Default.ResolveType<IWrapControlService>();

        private bool _isWrapped;
        private bool _forceClose;

        private ICommand _defaultOkCommand;
        private ButtonBase _defaultOkElement;
        private ICommand _defaultCancelCommand;

        private readonly Collection<DataWindowButton> _buttons = new Collection<DataWindowButton>();
        private readonly Collection<ICommand> _commands = new Collection<ICommand>();
        private readonly InfoBarMessageControlGenerationMode _infoBarMessageControlGenerationMode;

        private readonly WindowLogic _logic;

        private event EventHandler<EventArgs> _viewLoaded;
        private event EventHandler<EventArgs> _viewUnloaded;
        private event EventHandler<DataContextChangedEventArgs> _viewDataContextChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.FrameworkElement"/> class.
        /// </summary>
        /// <remarks>
        /// This method is required for design time support.
        /// </remarks>
        public DataWindow()
            : this(DataWindowMode.OkCancel)
        { }

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
            : this(null, mode, additionalButtons, defaultButton, setOwnerAndFocus, infoBarMessageControlGenerationMode)
        { }

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
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            // Set window style (WPF doesn't allow styling on root elements of XAML files, too bad)
            // For more info, see http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/3059c0e4-c372-4da2-b384-28f271feef05/
            SetResourceReference(StyleProperty, typeof(DataWindow));

            Mode = mode;
            DefaultButton = defaultButton;
            _infoBarMessageControlGenerationMode = infoBarMessageControlGenerationMode;

            this.FixBlurriness();

            SizeToContent = SizeToContent.WidthAndHeight;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.ApplyIconFromApplication();

            ThemeHelper.EnsureCatelMvvmThemeIsLoaded();

            _logic = new WindowLogic(this, null, viewModel);
            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, "ActualWidth", StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, "ActualHeight", StringComparison.InvariantCulture))
                {
                    PropertyChanged.SafeInvoke(this, e);
                }
            };

            _logic.ViewModelClosedAsync += OnViewModelClosedAsync;
            _logic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(sender, e);

                ViewModelPropertyChanged.SafeInvoke(this, e);
            };

            Loaded += (sender, e) =>
            {
                _viewLoaded.SafeInvoke(this);

                OnLoaded(e);
            };

            Unloaded += (sender, e) =>
            {
                _viewUnloaded.SafeInvoke(this);

                OnUnloaded(e);
            };

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
            DataContextChanged += (sender, e) => _viewDataContextChanged.SafeInvoke(this, () => new DataContextChangedEventArgs(e.OldValue, e.NewValue));

            if (setOwnerAndFocus)
            {
                this.SetOwnerWindowAndFocus();
            }
            else
            {
                this.FocusFirstControl();
            }
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
        /// <c>true</c> if this instance is close button available; otherwise, <c>false</c>.
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
        #endregion

        #region Commands
        /// <summary>
        /// Executes the OK command.
        /// </summary>
        protected Task ExecuteOkAsync()
        {
            if (OnOkCanExecute())
            {
                return OnOkExecuteAsync();
            }

            return TaskHelper.Completed;
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
        protected async Task OnOkExecuteAsync()
        {
            if (!await ApplyChangesAsync())
            {
                return;
            }

            ClosedByButton = true;
            SetDialogResultAndMakeSureWindowGetsClosed(true);
        }

        /// <summary>
        /// Executes the Cancel command.
        /// </summary>
        protected Task ExecuteCancelAsync()
        {
            if (OnCancelCanExecute())
            {
                return OnCancelExecuteAsync();
            }

            return TaskHelper.Completed;
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
        protected async Task OnCancelExecuteAsync()
        {
            if (!await DiscardChangesAsync())
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
        protected Task ExecuteApplyAsync()
        {
            if (OnApplyCanExecute())
            {
                return OnApplyExecuteAsync();
            }

            return TaskHelper.Completed;
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
        protected async Task OnApplyExecuteAsync()
        {
            await ApplyChangesAsync();
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
        event EventHandler<DataContextChangedEventArgs> IView.DataContextChanged
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
            PropertyChanged.SafeInvoke(this, () => new PropertyChangedEventArgs("ViewModel"));
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="Keyboard.KeyDownEvent"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

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

            var newContentAsFrameworkElement = newContent as FrameworkElement;
            if (_isWrapped || !WrapControlService.CanBeWrapped(newContentAsFrameworkElement))
            {
                return;
            }

            var languageService = ServiceLocator.Default.ResolveType<ILanguageService>();

            if (IsOKButtonAvailable)
            {
                var button = DataWindowButton.FromAsync(languageService.GetString("OK"), OnOkExecuteAsync, OnOkCanExecute);
                button.IsDefault = DefaultButton == DataWindowDefaultButton.OK;
                _buttons.Add(button);
            }
            if (IsCancelButtonAvailable)
            {
                var button = DataWindowButton.FromAsync(languageService.GetString("Cancel"), OnCancelExecuteAsync, OnCancelCanExecute);
                button.IsCancel = true;
                _buttons.Add(button);
            }
            if (IsApplyButtonAvailable)
            {
                var button = DataWindowButton.FromAsync(languageService.GetString("Apply"), OnApplyExecuteAsync, OnApplyCanExecute);
                button.IsDefault = DefaultButton == DataWindowDefaultButton.Apply;
                _buttons.Add(button);
            }
            if (IsCloseButtonAvailable)
            {
                var button = DataWindowButton.FromSync(languageService.GetString("Close"), OnCloseExecute, OnCloseCanExecute);
                button.IsDefault = DefaultButton == DataWindowDefaultButton.Close;
                _buttons.Add(button);
            }

            foreach (var button in _buttons)
            {
                _commands.Add(button.Command);
            }

            var wrapOptions = WrapControlServiceWrapOptions.GenerateWarningAndErrorValidatorForDataContext;
            switch (_infoBarMessageControlGenerationMode)
            {
                case InfoBarMessageControlGenerationMode.None:
                    break;

                case InfoBarMessageControlGenerationMode.Inline:
                    wrapOptions |= WrapControlServiceWrapOptions.GenerateInlineInfoBarMessageControl;
                    break;

                case InfoBarMessageControlGenerationMode.Overlay:
                    wrapOptions |= WrapControlServiceWrapOptions.GenerateOverlayInfoBarMessageControl;
                    break;
            }

            _isWrapped = true;

            var contentGrid = WrapControlService.Wrap(newContentAsFrameworkElement, wrapOptions, _buttons.ToArray(), this);

            var internalGrid = contentGrid.FindVisualDescendant(obj => (obj is FrameworkElement) && string.Equals(((FrameworkElement)obj).Name, WrapControlServiceControlNames.InternalGridName)) as Grid;
            if (internalGrid != null)
            {
                internalGrid.SetResourceReference(StyleProperty, "WindowGridStyle");

                newContentAsFrameworkElement.FocusFirstControl();

                _defaultOkCommand = (from button in _buttons
                                     where button.IsDefault
                                     select button.Command).FirstOrDefault();
                _defaultOkElement = WrapControlService.GetWrappedElement<ButtonBase>(contentGrid, WrapControlServiceControlNames.DefaultOkButtonName);

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
#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnDataWindowClosing(object sender, CancelEventArgs args)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            if (!_forceClose && !ClosedByButton)
            {
                var vm = ViewModel;
                if (vm != null && vm.IsClosed)
                {
                    // Being closed from the vm
                    return;
                }

                // CTL-735 always cancel, we will close later once we handled our async result
                args.Cancel = true;

                if (await DiscardChangesAsync())
                {
                    // Now we can close for sure
                    _forceClose = true;

                    // Dispatcher to make sure we are not inside the same loop
#pragma warning disable 4014
                    Dispatcher.BeginInvoke(() => SetDialogResultAndMakeSureWindowGetsClosed(false));
#pragma warning restore 4014
                }
            }
        }

        /// <summary>
        /// Initializes the window.
        /// </summary>
        protected virtual void Initialize()
        {
            Dispatcher.BeginInvoke(new Action(RaiseCanExecuteChangedForAllCommands));
        }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected virtual bool ValidateData()
        {
            _logic.ValidateViewModel();

            var vm = _logic.ViewModel;
            if (vm == null)
            {
                return true;
            }

            var validationContext = vm.ValidationContext;
            return !validationContext.HasErrors;
        }

        /// <summary>
        /// Applies all changes made by this window.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected async virtual Task<bool> ApplyChangesAsync()
        {
            var result = await _logic.SaveViewModelAsync();
            return result;
        }

        /// <summary>
        /// Discards all changes made by this window.
        /// </summary>
        protected async virtual Task<bool> DiscardChangesAsync()
        {
            // CTL-735 We might be handling the ViewModel.Closed event
            var vm = _logic.ViewModel;
            if (vm != null)
            {
                if (vm.IsClosed)
                {
                    return true;
                }
            }

            var result = await _logic.CancelViewModelAsync();
            return result;
        }

        /// <summary>
        /// Raises the can <see cref="ICommand.CanExecuteChanged"/> for all commands.
        /// </summary>
        protected void RaiseCanExecuteChangedForAllCommands()
        {
            foreach (var command in Commands)
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

        protected virtual Task OnViewModelClosedAsync(object sender, ViewModelClosedEventArgs e)
        {
            return TaskHelper.Completed;
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