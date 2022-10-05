namespace Catel.IoC
{
    /// <summary>
    /// Available registration types.
    /// </summary>
    public enum RegistrationType
    {
        /// <summary>
        /// Singleton mode which means that the same instance will be returned every time a type is resolved.
        /// </summary>
        Singleton,

        /// <summary>
        /// Transient mode which means that a new instance will be returned every time a type is resolved.
        /// </summary>
        Transient
    }
}
