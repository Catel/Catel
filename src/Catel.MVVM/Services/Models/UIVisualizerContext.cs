namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

    public class UIVisualizerContext
    {
        public UIVisualizerContext()
        {
            Name = string.Empty;
            IsModal = true;
            SetParentWindow = true;
        }

        /// <summary>
        /// If set the <c>true</c>, the UI will be shown in a modal state.
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// The name that the window is registered with.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The data to inject into the view model.
        /// <para />
        /// The inject a view model, set this property to a view model instance.
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Gets the callback that will be called when the ui is closed.
        /// </summary>
        public EventHandler<UICompletedEventArgs>? CompletedCallback { get; set; }

        /// <summary>
        /// If set to <c>true</c>, the visualizer service should set the parent window.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        public bool SetParentWindow { get; set; }

        /// <summary>
        /// Customized callback to allow setting a custom parent window to override default behavior.
        /// </summary>
        public Func<UIVisualizerContext, object, Task>? SetParentWindowCallback { get; set; }
    }
}
