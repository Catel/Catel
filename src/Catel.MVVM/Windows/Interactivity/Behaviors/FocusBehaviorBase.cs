namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using System.Windows.Threading;
    using UIEventArgs = System.EventArgs;
    using TimerTickEventArgs = System.EventArgs;
    using System;
    using System.ComponentModel;
    using Logging;
    using Reflection;

    /// <summary>
    /// Base class for focus behaviors.
    /// </summary>
    public class FocusBehaviorBase : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusBehaviorBase"/> class.
        /// </summary>
        public FocusBehaviorBase()
        {
            FocusDelay = 0;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is focus already set.
        /// </summary>
        /// <value><c>true</c> if this instance is focus already set; otherwise, <c>false</c>.</value>
        protected bool IsFocusAlreadySet { get; private set; }

        /// <summary>
        /// Gets or sets the focus delay. If smaller than 25, no delay will be used. If larger than 5000, it will be set to 5000.
        /// <para />
        /// The default value in WPF is <c>0</c>.
        /// </summary>
        /// <value>The focus delay.</value>
        /// <example>
        /// </example>
        public int FocusDelay
        {
            get { return (int)GetValue(FocusDelayProperty); }
            set { SetValue(FocusDelayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FocusDelay.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FocusDelayProperty =
            DependencyProperty.Register(nameof(FocusDelay), typeof(int), typeof(FocusBehaviorBase), new PropertyMetadata(0));

        /// <summary>
        /// Starts the focus.
        /// </summary>
        protected void StartFocus()
        {
            var focusDelay = FocusDelay;
            if (focusDelay > 5000)
            {
                focusDelay = 5000;
            }

            Log.Debug("Starting focus on element '{0}' with a delay of '{1}' ms", AssociatedObject.GetType().GetSafeFullName(false), focusDelay);

            if (focusDelay > 25)
            {
                _timer.Stop();
                _timer.Tick -= OnTimerTick;

                _timer.Interval = new TimeSpan(0, 0, 0, 0, focusDelay);
                _timer.Tick += OnTimerTick;
                _timer.Start();
            }
            else
            {
                if (SetFocus())
                {
                    IsFocusAlreadySet = true;
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="DispatcherTimer.Tick" /> event occurs on the timer.
        /// </summary>
        private void OnTimerTick(object? sender, TimerTickEventArgs e)
        {
            IsFocusAlreadySet = true;

            _timer.Stop();
            _timer.Tick -= OnTimerTick;

            SetFocus();
        }

        /// <summary>
        /// Sets the focus to the assoicated object.
        /// </summary>
        private bool SetFocus()
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (AssociatedObject.Focus())
            {
                Log.Debug("Focused '{0}'", AssociatedObject.GetType().GetSafeFullName(false));

                var textBox = AssociatedObject as TextBox;
                if (textBox is not null)
                {
                    textBox.SelectionStart = textBox.Text.Length;
                }

                return true;
            }

            Log.Debug("Failed to focus '{0}'", AssociatedObject.GetType().GetSafeFullName(false));

            return false;
        }
    }
}
