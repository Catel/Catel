namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;
    using Logging;
    using Microsoft.Extensions.Logging;
    using MVVM;
    using Reflection;

    /// <summary>
    /// Hides the view until the view model is loaded.
    /// </summary>
    public partial class HideUntilViewModelLoaded : BehaviorBase<FrameworkElement>
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(HideUntilViewModelLoaded));

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var viewModelContainer = AssociatedObject as IViewModelContainer;
            if (viewModelContainer is null)
            {
                var error = string.Format("This behavior can only be used on IViewModelContainer classes, '{0}' does not implement; IViewModelContainer", AssociatedObject.GetType().GetSafeFullName(false));
                throw Logger.LogErrorAndCreateException<InvalidOperationException>(error);
            }

            viewModelContainer.ViewModelChanged += OnViewModelChanged;

            UpdateVisibility();
        }

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        protected override void Uninitialize()
        {
            var viewModelContainer = AssociatedObject as IViewModelContainer;
            if (viewModelContainer is not null)
            {
                viewModelContainer.ViewModelChanged -= OnViewModelChanged;
            }

            base.Uninitialize();
        }

        private void OnViewModelChanged(object? sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (!IsEnabled)
            {
                return;
            }

            var viewModelContainer = AssociatedObject as IViewModelContainer;
            if (viewModelContainer is not null)
            {
                AssociatedObject.Visibility = (viewModelContainer.ViewModel is null) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
