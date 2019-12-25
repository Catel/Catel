namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Catel.IoC;
    using Catel.Services;

    public class DispatcherObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
    {
        private readonly Lazy<IDispatcherService> _dispatcherService = new Lazy<IDispatcherService>(() => IoCConfiguration.DefaultDependencyResolver.Resolve<IDispatcherService>());

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; } = true;

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// </summary>
        protected override void OnCollectionChanged()
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(base.OnCollectionChanged);
            }
            else
            {
                base.OnCollectionChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="changedItem">The updated item.</param>
        /// <param name="index">The index value of the updated item.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem, int index)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnCollectionChanged(action, changedItem, index));
            }
            else
            {
                base.OnCollectionChanged(action, changedItem, index);
            }

        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="index">The index value of the old item affected.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, int index)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnCollectionChanged(action, newItem, oldItem, index));
            }
            else
            {
                base.OnCollectionChanged(action, newItem, oldItem, index);
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="propertyName">The target property to invoke <see cref="INotifyPropertyChanged.PropertyChanged"/> on.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnPropertyChanged(propertyName));
            }
            else
            {
                base.OnPropertyChanged(propertyName);
            }
        }
    }
}
