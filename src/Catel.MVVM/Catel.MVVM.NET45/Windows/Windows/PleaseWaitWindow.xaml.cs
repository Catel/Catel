// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitWindow.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// The action that the <see cref="PleaseWaitWindow"/> should take when it becomes visible.
    /// </summary>
    public enum PleaseWaitMode
    {
        /// <summary>
        /// Dimm the parent window.
        /// </summary>
        Dimm,

        /// <summary>
        /// Blur the parent window.
        /// </summary>
        Blur,

        /// <summary>
        /// Don't do anything.
        /// </summary>
        Nothing
    }

    /// <summary>
    /// Please wait window to show a please wait window with the option to customize the text.
    /// </summary>
    /// <remarks>
    /// Parts of this code comes from this blog: http://blogs.msdn.com/b/dwayneneed/archive/2007/04/26/multithreaded-ui-hostvisual.aspx.
    /// </remarks>
    public partial class PleaseWaitWindow
    {
        #region Fields
        private static readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        private Thread _thread;

        private readonly List<FrameworkElement> _dimmedElements = new List<FrameworkElement>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="PleaseWaitWindow"/> class.
        /// </summary>
        static PleaseWaitWindow()
        {
            OnlyDimmOwnerWindow = false;
            DoNotDimmPopups = false;
            Mode = PleaseWaitMode.Blur;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitWindow"/> class.
        /// </summary>
        public PleaseWaitWindow()
            : this(MVVM.Properties.Resources.PleaseWait) { }

        /// <summary>
        /// Initializes a please wait window with default text.
        /// </summary>
        /// <param name="text">Text to display in the window.</param>
        public PleaseWaitWindow(string text)
        {
            InitializeComponent();

            this.SetOwnerWindow();

            Topmost = true;

            Loaded += OnLoaded;
            LayoutUpdated += OnLayoutUpdated;

            Text = text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PleaseWaitWindow"/> should only dimm the owner window.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if if only the owner window should be dimmed; otherwise, <c>false</c>.
        /// </value>
        public static bool OnlyDimmOwnerWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PleaseWaitWindow"/> should not dimm popups. This
        /// value is only used when <see cref="OnlyDimmOwnerWindow"/> is <c>false</c>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if if only the owner window should be dimmed; otherwise, <c>false</c>.
        /// </value>
        public static bool DoNotDimmPopups { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// <para />
        /// The default value is <see cref="PleaseWaitMode.Blur"/>.
        /// </summary>
        /// <value>The mode.</value>
        public static PleaseWaitMode Mode { get; set; }

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        /// <remarks>
        /// Wrapper for the Text dependency property.
        /// </remarks>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PleaseWaitWindow));

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        /// <value>The percentage.</value>
        public int Percentage
        {
            get { return (int)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(int), typeof(PleaseWaitWindow), new UIPropertyMetadata(0));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is indeterminate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is indeterminate; otherwise, <c>false</c>.
        /// </value>
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(PleaseWaitWindow), new UIPropertyMetadata(true));
        #endregion

        #region Methods
        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == VisibilityProperty.Name)
            {
                // Update the layout to prevent resizing while showing
                UpdateLayout();

                switch (Visibility)
                {
                    case Visibility.Visible:
                        if (Owner != null)
                        {
                            Left = (Owner.Left + (Owner.ActualWidth / 2)) - (ActualWidth / 2);
                            Top = (Owner.Top + (Owner.ActualHeight / 2)) - (ActualHeight / 2);
                        }

                        this.BringWindowToTop();

                        ChangeMode(true);
                        break;

                    case Visibility.Hidden:
                    case Visibility.Collapsed:
                        ChangeMode(false);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dimmed.
        /// </summary>
        /// <value><c>true</c> if this instance is dimmed; otherwise, <c>false</c>.</value>
        public bool IsOwnerDimmed { get { return _dimmedElements.Count > 0; } }

        /// <summary>
        /// Changes the mode of all windows of the application.
        /// </summary>
        /// <param name="dimm">if set to <c>true</c>, all windows should be dimmed.</param>
        private void ChangeMode(bool dimm)
        {
            if (!dimm)
            {
                ChangeMode(_dimmedElements, false);
            }
            else
            {
                var elements = new List<FrameworkElement>();

                var currentApplication = Application.Current;
                if (OnlyDimmOwnerWindow || currentApplication == null)
                {
                    if (Owner != null)
                    {
                        elements.Add(Owner);
                    }
                }
                else
                {
                    elements.AddRange(currentApplication.Windows.Cast<FrameworkElement>().Where(window => !(window is PleaseWaitWindow)));

                    if (!DoNotDimmPopups)
                    {
                        elements.AddRange((from popup in PopupHelper.GetAllPopups()
                                           where popup.Child is FrameworkElement
                                           select popup.Child).Cast<FrameworkElement>());
                    }
                }

                if (elements.Count > 0)
                {
                    ChangeMode(elements, true);
                }
            }
        }

        /// <summary>
        /// Changes the dimming.
        /// </summary>
        /// <param name="elements">The framework elements.</param>
        /// <param name="dimm">if set to <c>true</c>, all windows should be dimmed.</param>
        private void ChangeMode(IEnumerable<FrameworkElement> elements, bool dimm)
        {
            if (!dimm)
            {
                Action action = () =>
                {
                    UpdateLayout(); 
                    Close();
                };

                foreach (var element in _dimmedElements)
                {
                    switch (Mode)
                    {
                        case PleaseWaitMode.Dimm:
                            element.Undimm(action);
                            break;

                        case PleaseWaitMode.Blur:
                            element.Unblur(action);
                            break;

                        case PleaseWaitMode.Nothing:
                            // Execute when all windows are done...
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    element.IsHitTestVisible = true;
                }

                _dimmedElements.Clear();

                if (Mode == PleaseWaitMode.Nothing)
                {
                    action();
                }
            }
            else
            {
                Action action = UpdateLayout;

                foreach (var element in elements)
                {
                    switch (Mode)
                    {
                        case PleaseWaitMode.Dimm:
                            element.Dimm(action);
                            break;

                        case PleaseWaitMode.Blur:
                            element.Blur(action);
                            break;

                        case PleaseWaitMode.Nothing:
                            // Execute when all windows are done...
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    element.IsHitTestVisible = false;

                    if (!_dimmedElements.Contains(element))
                    {
                        _dimmedElements.Add(element);
                    }
                }

                if (Mode == PleaseWaitMode.Nothing)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Called when the window is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hostVisuals = CreateMediaElementsOnWorkerThread();

            indeterminateVisualWrapper.Child = hostVisuals[0];
            //determinateVisualWrapper.Child = hostVisuals[1];
        }

        /// <summary>
        /// Called when the layout of the window is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Owner.UpdateLayout();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            Loaded -= OnLoaded;
            LayoutUpdated -= OnLayoutUpdated;

            if (_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }

            // Make sure we are not leaving the owner window dimmed in case a direct call to Close is invoked
            // instead of Hide
            if (_dimmedElements.Count > 0)
            {
                foreach (var window in _dimmedElements)
                {
                    window.IsHitTestVisible = true;
                    window.Opacity = 1d;
                }
            }

            if (Owner != null)
            {
                Owner.Focus();
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// Creates the media element on worker thread.
        /// </summary>
        /// <returns></returns>
        private HostVisual[] CreateMediaElementsOnWorkerThread()
        {
            var visuals = new[] { new HostVisual(), new HostVisual() };

            _thread = new Thread(WorkerThread);

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.IsBackground = true;
            _thread.Start(visuals);

            _autoResetEvent.WaitOne();

            return visuals;
        }

        private static void WorkerThread(object arg)
        {
            try
            {
                var hostVisuals = (HostVisual[])arg;

                var indeterminateSource = new VisualTargetPresentationSource(hostVisuals[0]);
                var determinateSource = new VisualTargetPresentationSource(hostVisuals[1]);

                _autoResetEvent.Set();

                indeterminateSource.RootVisual = CreateIndeterminateElement();
                determinateSource.RootVisual = CreateDeterminateElement();

                Dispatcher.Run();
            }
            catch
            {
            }
        }

        private static FrameworkElement CreateIndeterminateElement()
        {
            return new LoaderAnimation { Width = 32, Height = 32 };
        }

        private static FrameworkElement CreateDeterminateElement()
        {
            return null;

            //var progressBar = new ProgressBar { Minimum = 0, Maximum = 100 };
            //progressBar.Value = 50;

            //return progressBar;
        }
        #endregion
    }
}