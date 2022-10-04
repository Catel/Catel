namespace Catel.IoC
{
    /// <summary>
    /// Available modes for the <see cref="ServiceLocatorRegistrationAttribute" />.
    /// </summary>
    public enum ServiceLocatorRegistrationMode
    {
        /// <summary>
        /// The type will be registered as transient.
        /// </summary>
        Transient,

        /// <summary>
        /// The singleton instance will be created immediately and then registered.
        /// </summary>
        SingletonInstantiateImmediately,

        /// <summary>
        /// The singleton instance will be created when it is first queried.
        /// </summary>
        SingletonInstantiateWhenRequired
    }
}
