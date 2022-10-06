namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Service that determines how to subscribe to data context.
    /// </summary>
    public class DataContextSubscriptionService : IDataContextSubscriptionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextSubscriptionService"/> class.
        /// </summary>
        public DataContextSubscriptionService()
        {
            DefaultDataContextSubscriptionMode = DataContextSubscriptionMode.InheritedDataContext;
        }

        /// <summary>
        /// Gets or sets the default data context subscription mode.
        /// </summary>
        /// <value>The default data context subscription mode.</value>
        public DataContextSubscriptionMode DefaultDataContextSubscriptionMode { get; set; }

        /// <summary>
        /// Gets the data context subscription mode for the specific view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>The data context subscription mode.</returns>
        public virtual DataContextSubscriptionMode GetDataContextSubscriptionMode(Type viewType)
        {
            ArgumentNullException.ThrowIfNull(viewType);

            return DefaultDataContextSubscriptionMode;
        }
    }
}
