namespace Catel.Data
{
    using System.ComponentModel;
    using Services;

    /// <summary>
    /// Implementation of the <see cref="ObservableObject"/> class that will dispatch all change notifications
    /// to the UI thread using the <see cref="IDispatcherService"/>.
    /// </summary>
    public class DispatcherObservableObject : ObservableObject
    {
        private readonly IDispatcherService _dispatcherService;

        public DispatcherObservableObject(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        /// <summary>
        /// Raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// <para/>
        /// This is the one and only method that actually raises the <see cref="ObservableObject.PropertyChanged"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void RaisePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _dispatcherService.BeginInvokeIfRequired(() => base.RaisePropertyChanged(sender, e));
        }
    }
}
