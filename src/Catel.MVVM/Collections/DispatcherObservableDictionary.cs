﻿namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Catel.IoC;
    using Catel.Services;

    public class DispatcherObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
    {
        private readonly Lazy<IDispatcherService> _dispatcherService = new Lazy<IDispatcherService>(() => IoCConfiguration.DefaultDependencyResolver.Resolve<IDispatcherService>());

        public DispatcherObservableDictionary()
        {
        }

        public DispatcherObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }

        public DispatcherObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        public DispatcherObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
        }

        protected DispatcherObservableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; } = true;
        
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
        /// Raises the <see cref="ObservableDictionary{TKey, TValue}.CollectionChanged" /> event, but also makes sure the event is dispatched to the UI thread.
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
    }
}
