#if NET || NETCORE

namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Services;

    public class DispatcherFastBindingList<T> : FastBindingList<T>
    {
        private static readonly IDispatcherService _dispatcherService;
        /// <summary>
        /// Initializes static members of the <see cref="FastBindingList{T}"/> class.
        /// </summary>
        static DispatcherFastBindingList()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            _dispatcherService = dependencyResolver.Resolve<IDispatcherService>();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherFastBindingList{T}" /> class.
        /// </summary>
        public DispatcherFastBindingList()
        {
        }

        public DispatcherFastBindingList(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; } = true;
      
        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected override void NotifyChanges()
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.BeginInvokeIfRequired(base.NotifyChanges);
            }
            else
            {
                base.NotifyChanges();
            }            
        }

        /// <summary>
        /// Raises the <see cref="BindingList{T}.ListChanged" /> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="ListChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.BeginInvokeIfRequired(() => base.OnListChanged(e));
            }
            else
            {
                base.OnListChanged(e);
            }
        }

    }
}

#endif
