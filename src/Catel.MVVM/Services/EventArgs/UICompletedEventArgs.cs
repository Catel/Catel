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
        /// <param name="result">The result.</param>
        public UICompletedEventArgs(UIVisualizerResult result)
        {
            Argument.IsNotNull(nameof(result), result);

            Context = result.Context;
            Result = result;
        }

        /// <summary>
        /// The ui visualizer context.
        /// </summary>
        public UIVisualizerContext Context { get; private set; }

        /// <summary>
        /// The result.
        /// </summary>
        public UIVisualizerResult Result { get; private set; }
    }
}
