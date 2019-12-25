namespace Catel.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Catel.IoC;
    using Catel.Services;

    public class DispatcherFastObservableCollection<T> : FastObservableCollection<T>
    {
        private static readonly Lazy<IDispatcherService> _dispatcherService = new Lazy<IDispatcherService>(() =>
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            return dependencyResolver.Resolve<IDispatcherService>();
        });

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherFastObservableCollection{T}" /> class.
        /// </summary>
        public DispatcherFastObservableCollection()
        {
            AutomaticallyDispatchChangeNotifications = true;
        }

        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected override void NotifyChanges()
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(base.NotifyChanges);
            }
            else
            {   
                base.NotifyChanges();
            }

        }
        /// <summary>
        /// Raises the <c>ObservableCollection{T}.PropertyChanged</c> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnPropertyChanged(e));
            }
            else
            {
                base.OnPropertyChanged(e);
            }

        }

        /// <summary>
        /// Raises the <see cref="ObservableCollection{T}.CollectionChanged" /> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnCollectionChanged(e));
            }
            else
            {
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; }
    }
}
