// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Catel.Logging;
    using Catel.MVVM.Properties;
    using Catel.Services;
    using Catel.Windows.Threading;

    /// <summary>
    /// PleaseWait window Helper class.
    /// </summary>
    public class PleaseWaitHelper
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static PleaseWaitHelper _instance;

        private static readonly object _padlock = new object();

        private static readonly object _visibleStopwatchLock = new object();

        private static Stopwatch _visibleStopwatch;
        #endregion

        #region Fields
        private string _currentStatusText = LanguageHelper.GetString("PleaseWait");

        private double _currentWindowWidth = 0.0d;
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="PleaseWaitHelper"/> class from being created. 
        /// Initializes a new instance of the <see cref="PleaseWaitHelper"/> class.
        /// </summary>
        private PleaseWaitHelper()
        {
            MinimumDurationBeforeShow = 500;
            MinimumShowTime = 1000;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance of this singleton class.
        /// </summary>
        protected static PleaseWaitHelper Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new PleaseWaitHelper();
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PleaseWaitWindow"/> instance.
        /// </summary>
        private PleaseWaitWindow PleaseWaitWindow { get; set; }

        /// <summary>
        /// Gets or sets the minimum duration in milliseconds that an operation must take before the window is actually shown.
        /// </summary>
        /// <value>
        /// The minimum duration in milliseconds that an operation must take before the window is actually shown.
        /// </value>
        public static int MinimumDurationBeforeShow { get; set; }

        /// <summary>
        /// Gets or sets the minimum show time in milliseconds.
        /// </summary>
        /// <value>
        /// The minimum show time in milliseconds.
        /// </value>
        public static int MinimumShowTime { get; set; }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        /// <value>The dispatcher.</value>
        private static Dispatcher Dispatcher { get { return DispatcherHelper.CurrentDispatcher; } }
        #endregion

        #region Methods
        /// <summary>
        /// Shows the please wait window with the specified status text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <remarks>When this method is used, the <see cref="Hide" /> method must be called to hide the window again.</remarks>
        public static void Show(string status = "")
        {
            UpdateStatus(status);

            Dispatcher.Invoke(() => Instance.ShowWindow(-1));
        }

        /// <summary>
        /// Shows the specified status.
        /// </summary>
        /// <param name="statusFormat">The status format.</param>
        /// <param name="currentItem">The current item.</param>
        /// <param name="totalItems">The total items.</param>
        public static void Show(string statusFormat, int currentItem, int totalItems)
        {
            Dispatcher.Invoke(() => UpdateStatus(statusFormat, currentItem, totalItems));
        }

        /// <summary>
        /// Shows the please wait window with the default status text and executes the work delegate (in a background thread). When the work
        /// is finished, the please wait window will be automatically closed. This method will also subscribe to the
        /// <see cref="BackgroundWorker.RunWorkerCompleted" /> event.
        /// </summary>
        /// <param name="workDelegate">The work delegate.</param>
        /// <param name="runWorkerCompletedDelegate">The run worker completed delegate.</param>
        /// <param name="status">The status.</param>
        /// <param name="windowWidth">Width of the window.</param>
        public static void Show(PleaseWaitWorkDelegate workDelegate, RunWorkerCompletedEventHandler runWorkerCompletedDelegate = null, string status = "", double windowWidth = double.NaN)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateStatus(status, windowWidth);

                Instance.ShowWindow(-1);
            });

            if (workDelegate != null)
            {
                workDelegate();

                lock (_visibleStopwatchLock)
                {
                    // Make sure the window is shown for a minimum duration
                    int milliSecondsLeftToShow = 0;
                    if (_visibleStopwatch != null)
                    {
                        _visibleStopwatch.Stop();
                        milliSecondsLeftToShow = MinimumShowTime - (int)_visibleStopwatch.ElapsedMilliseconds;
                        _visibleStopwatch = null;
                    }

                    if (milliSecondsLeftToShow > 0)
                    {
                        ThreadHelper.Sleep(milliSecondsLeftToShow);
                    }
                }
            }

            if (runWorkerCompletedDelegate != null)
            {
                runWorkerCompletedDelegate(null, null);
            }

            Dispatcher.Invoke(() => Hide());
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="status">The status.</param>
        public static void UpdateStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                status = LanguageHelper.GetString("PleaseWait");
            }

            Dispatcher.Invoke(() => Instance.UpdateStatusText(status, double.NaN));
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="width">The width.</param>
        public static void UpdateStatus(string status, double width)
        {
            Dispatcher.Invoke(() => Instance.UpdateStatusText(status, width));
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="statusFormat">The status format.</param>
        /// <param name="currentItem">The current item.</param>
        /// <param name="totalItems">The total items.</param>
        public static void UpdateStatus(string statusFormat, int currentItem, int totalItems)
        {
            Dispatcher.Invoke(() =>
            {
                if (currentItem > totalItems)
                {
                    Instance.HideWindow();
                    return;
                }

                UpdateStatus(string.Format(statusFormat, currentItem, totalItems));

                int percentage = (100/totalItems)*currentItem;

                Instance.ShowWindow(percentage);
            });
        }

        /// <summary>
        /// Hides the Please Wait window.
        /// </summary>
        public static void Hide()
        {
            Dispatcher.Invoke(() => Instance.HideWindow());
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="windowWidth">Width of the window.</param>
        private void UpdateStatusText(string text, double windowWidth)
        {
            _currentStatusText = text;
            _currentWindowWidth = windowWidth;

            if (PleaseWaitWindow == null)
            {
                return;
            }

            Dispatcher.Invoke(() =>
            {
                PleaseWaitWindow.Text = _currentStatusText;
                PleaseWaitWindow.MinWidth = double.IsNaN(_currentWindowWidth) ? 0d : _currentWindowWidth;
                PleaseWaitWindow.UpdateLayout();                
            });
        }

        /// <summary>
        /// Shows the window delayed by using the <see cref="MinimumDurationBeforeShow"/>.
        /// </summary>
        /// <param name="percentage">
        /// The percentage. If <c>-1</c>, the window is assumed to be indeterminate.
        /// </param>
        private void ShowWindow(int percentage)
        {
            bool isIndeterminate = (percentage == -1);

            var pleaseWaitWindow = PleaseWaitWindow;
            if (pleaseWaitWindow != null)
            {
                pleaseWaitWindow.IsIndeterminate = isIndeterminate;
                pleaseWaitWindow.Percentage = percentage;
                return;
            }

            pleaseWaitWindow = new PleaseWaitWindow();
            var activeWindow = Application.Current.GetActiveWindow();

            // When the application has no a WPF window then the active window could be the pleaseWaitWindow, so
            if (!pleaseWaitWindow.Equals(activeWindow))
            {
                pleaseWaitWindow.Owner = activeWindow;
            }

            pleaseWaitWindow.IsIndeterminate = isIndeterminate;
            pleaseWaitWindow.Percentage = percentage;
            pleaseWaitWindow.Text = _currentStatusText;
            pleaseWaitWindow.MinWidth = double.IsNaN(_currentWindowWidth) ? 0d : _currentWindowWidth;

            Log.Debug("Showing please wait window");

            pleaseWaitWindow.Show();

            // Skip the turn dark wait loop if there are nothing to turn dark. 
            if (pleaseWaitWindow.Owner != null)
            {
                // Yes, check for PleaseWaitWindow (property). Sometimes the show immediately hides
                while (!pleaseWaitWindow.IsOwnerDimmed && PleaseWaitWindow != null)
                {
                    // It's a bad practice to use this "equivalent" of DoEvents in WPF, but I don't see another choice
                    // to wait until the animation of the ShowWindow has finished without blocking the UI
                    pleaseWaitWindow.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate { });
                    pleaseWaitWindow.UpdateLayout();
                }
            }

            // The property is set when the window is displayed
            PleaseWaitWindow = pleaseWaitWindow;

            lock (_visibleStopwatchLock)
            {
                if (_visibleStopwatch == null)
                {
                    _visibleStopwatch = new Stopwatch();
                    _visibleStopwatch.Start();
                }
                else
                {
                    _visibleStopwatch.Reset();
                    _visibleStopwatch.Start();
                }
            }
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        private void HideWindow()
        {
            var pleaseWaitWindow = PleaseWaitWindow;
            if (pleaseWaitWindow == null)
            {
                return;
            }

            Dispatcher.Invoke(() =>
            {
                Log.Debug("Hiding please wait window");

                // Hide the window, this will start the animation to undimm the parent and then the please wait window
                // will close itself
                pleaseWaitWindow.Hide();
                PleaseWaitWindow = null;

                _currentStatusText = LanguageHelper.GetString("PleaseWait");
                _currentWindowWidth = 0d;
            });
        }
        #endregion

        #region Nested type: HideWindowDelegate
        /// <summary>
        /// Delegate that allows this class to re-invoke the HideWindow method.
        /// </summary>
        protected delegate void HideWindowDelegate();
        #endregion

        #region Nested type: UpdateStatusTextDelegate
        /// <summary>
        /// Delegate to update the status text of the <see cref="PleaseWaitWindow"/>.
        /// </summary>
        private delegate void UpdateStatusTextDelegate(string text, double windowWidth);
        #endregion
    }
}