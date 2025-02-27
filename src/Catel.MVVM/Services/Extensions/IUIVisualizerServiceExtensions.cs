namespace Catel.Services
{
    using System;
    using System.Windows;
    using Windows.Threading;
    using Logging;
    using MVVM;
    using Reflection;

    /// <summary>
    /// Extension methods for the <see cref="IUIVisualizerService" />.
    /// </summary>
    public static partial class IUIVisualizerServiceExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the specified view model type is registered.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <returns><c>true</c> if the specified view model type is registered; otherwise, <c>false</c>.</returns>
        public static bool IsRegistered<TViewModel>(this IUIVisualizerService uiVisualizerService)
        {
            return IsRegistered(uiVisualizerService, typeof(TViewModel));
        }

        /// <summary>
        /// Determines whether the specified view model type is registered.
        /// </summary>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns><c>true</c> if the specified view model type is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        public static bool IsRegistered(this IUIVisualizerService uiVisualizerService, Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return uiVisualizerService.IsRegistered(viewModelType.GetSafeFullName());
        }

        /// <summary>
        /// Registers the specified view model and the window type. This way, Catel knowns what
        /// window to show when a specific view model window is requested.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c>, this method will throw an exception when already registered.</param>
        /// <exception cref="System.ArgumentException">viewModelType</exception>
        /// <exception cref="ArgumentException">viewModelType</exception>
        public static void Register<TViewModel, TView>(this IUIVisualizerService uiVisualizerService, bool throwExceptionIfExists = true)
        {
            Register(uiVisualizerService, typeof(TViewModel), typeof(TView), throwExceptionIfExists);
        }

        /// <summary>
        /// Registers the specified view model and the window type. This way, Catel knowns what
        /// window to show when a specific view model window is requested.
        /// </summary>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c>, this method will throw an exception when already registered.</param>
        /// <exception cref="System.ArgumentException">viewModelType</exception>
        /// <exception cref="ArgumentException">viewModelType</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType" /> does not implement <see cref="IViewModel" />.</exception>
        public static void Register(this IUIVisualizerService uiVisualizerService, Type viewModelType, Type windowType, bool throwExceptionIfExists = true)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);
            Argument.ImplementsInterface<IViewModel>(nameof(viewModelType), viewModelType);

            uiVisualizerService.Register(viewModelType.GetSafeFullName(), windowType, throwExceptionIfExists);
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <returns><c>true</c> if the view model is unregistered; otherwise <c>false</c>.</returns>
        public static bool Unregister<TViewModel>(this IUIVisualizerService uiVisualizerService)
        {
            return Unregister(uiVisualizerService, typeof(TViewModel));
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="viewModelType">Type of the view model to unregister.</param>
        /// <returns><c>true</c> if the view model is unregistered; otherwise <c>false</c>.</returns>
        public static bool Unregister(this IUIVisualizerService uiVisualizerService, Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return uiVisualizerService.Unregister(viewModelType.GetSafeFullName());
        }

        /// <summary>
        /// Activates the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns><c>true</c> if the window is activated with success; otherwise <c>false</c> or <c>null</c>.</returns>
        public static bool? ActivateWindow(Window window)
        {
            ArgumentNullException.ThrowIfNull(window);

            var activateMethodInfo = window.GetType().GetMethodEx("Activate");
            if (activateMethodInfo is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>($"Method 'Activate' not found on '{window.GetType().Name}', cannot activate the window");
            }

            bool? result = false;
            window.Dispatcher.InvokeIfRequired(() => result = (bool?)activateMethodInfo.Invoke(window, null));
            return result;
        }
    }
}
