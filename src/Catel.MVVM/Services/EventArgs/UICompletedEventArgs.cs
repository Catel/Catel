namespace Catel.Services
{
    using System;

    /// <summary>
    /// This is the EventArgs return value for the IUIVisualizer.Show completed event.
    /// </summary>
    public class UICompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UICompletedEventArgs"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public UICompletedEventArgs(UIVisualizerContext context, bool? result)
        {
            Argument.IsNotNull(nameof(context), context);

            Context = context;
            DataContext = context.Data;
            Result = result;
        }

        /// <summary>
        /// The ui visualizer context.
        /// </summary>
        public UIVisualizerContext Context { get; private set; }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public object DataContext { get; private set; }

        /// <summary>
        /// Gets the result of the window.
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; private set; }
    }
}
