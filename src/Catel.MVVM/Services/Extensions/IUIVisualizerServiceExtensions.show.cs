namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel.MVVM;

    public static partial class IUIVisualizerServiceExtensions
    {
        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="uiVisualizerService">The ui visualizer service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// 	<c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public static async Task<UIVisualizerResult> ShowAsync(this IUIVisualizerService uiVisualizerService, IViewModel viewModel, EventHandler<UICompletedEventArgs>? completedProc = null)
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);
            ArgumentNullException.ThrowIfNull(viewModel);

            var result = await uiVisualizerService.ShowContextAsync(new UIVisualizerContext
            {
                Data = viewModel,
                CompletedCallback = completedProc,
                IsModal = false,
            });

            return result;
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="uiVisualizerService">The ui visualizer service.</param>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// 	<c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public static async Task<UIVisualizerResult> ShowAsync(this IUIVisualizerService uiVisualizerService, string name, object data, EventHandler<UICompletedEventArgs>? completedProc = null)
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);
            Argument.IsNotNullOrWhitespace("name", name);

            var result = await uiVisualizerService.ShowContextAsync(new UIVisualizerContext
            {
                Name = name,
                Data = data,
                CompletedCallback = completedProc,
                IsModal = false,
            });

            return result;
        }
    }
}
