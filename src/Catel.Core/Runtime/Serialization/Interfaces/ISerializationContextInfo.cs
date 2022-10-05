namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Serialization context info.
    /// </summary>
    public interface ISerializationContextInfo
    {
        /// <summary>
        /// Gets a value indicating whether the context should auto generate graph ids for new object instances.
        /// </summary>
        /// <param name="context">The current serialization context.</param>
        /// <returns><c>true</c> if graph ids should automatically be generated, <c>false</c> if they should be registered manually.</returns>
        bool ShouldAutoGenerateGraphIds(ISerializationContext context);
    }
}
