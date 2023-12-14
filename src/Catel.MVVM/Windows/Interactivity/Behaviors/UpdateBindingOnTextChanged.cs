namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using System.Windows.Threading;
    using TimerTickEventArgs = System.EventArgs;

    /// <summary>
    /// This behavior automatically updates the binding of a <see cref="TextBox"/> when the
    /// <c>TextChanged</c> event occurs.
    /// </summary>
    public class UpdateBindingOnTextChanged : UpdateBindingBehaviorBase<TextBox>
    {
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateBindingOnTextChanged"/> class.
        /// </summary>
        public UpdateBindingOnTextChanged()
            : base("Text")
        {
            UpdateDelay = 250;

            _timer = new DispatcherTimer();
        }

        /// <summary>
        /// Gets or sets the update delay.
        /// <para/>
        /// This is the value that is used between updates in milliseconds. The binding will be updated
        /// when no new text change event is detected within the delay.
        /// <para/>
        /// The default value is <c>250</c>. If the value is smaller than <c>50</c>, the value
        /// will be ignored and there will be no delay between the key down and the binding update. If the
        /// value is higher than <c>5000</c>, it will be set to <c>5000</c>.
        /// </summary>
        /// <value>The update delay.</value>
        public int UpdateDelay { get; set; }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.TextChanged += OnAssociatedObjectTextChanged;

            _timer.Tick += OnTimerTick;
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;

            AssociatedObject.TextChanged -= OnAssociatedObjectTextChanged;
        }

        /// <summary>
        /// Called when the <c>TextChanged</c> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The text change event args instance containing the event data.</param>
        private void OnAssociatedObjectTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (UpdateDelay < 50)
            {
                UpdateBinding();
                return;
            }

            if (UpdateDelay > 5000)
            {
                UpdateDelay = 5000;
            }

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }

            _timer.Interval = new TimeSpan(0, 0, 0, 0, UpdateDelay);
            _timer.Start();
        }

        /// <summary>
        /// Called when timer ticks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object? sender, TimerTickEventArgs e)
        {
            _timer.Stop();

            UpdateBinding();
        }
    }
}
