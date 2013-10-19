// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObserverBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using IoC;

    /// <summary>
    /// Base class for all observer classes.
    /// </summary>
    public abstract class ObserverBase
    {
        /// <summary>
        /// The memento service.
        /// </summary>
        private readonly IMementoService _mementoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObserverBase"/> class.
        /// </summary>
        /// <param name="tag">The tag, can be <c>null</c>.</param>
        /// <param name="mementoService">The memento service. If <c>null</c>, the service will be retrieved from the <see cref="IServiceLocator"/>.</param>
        protected ObserverBase(object tag, IMementoService mementoService = null)
        {
            Tag = tag;

            if (mementoService == null)
            {
                var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
                mementoService = dependencyResolver.Resolve<IMementoService>();
            }

            _mementoService = mementoService;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the memento service.
        /// </summary>
        protected IMementoService MementoService
        {
            get { return _mementoService; }
        }

        /// <summary>
        /// Clears all the values and unsubscribes any existing change notifications.
        /// </summary>
        public abstract void CancelSubscription();
    }
}