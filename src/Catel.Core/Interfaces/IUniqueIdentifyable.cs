namespace Catel
{
    /// <summary>
    /// Interface to specify that an object is uniquely identifyable.
    /// </summary>
    public interface IUniqueIdentifyable
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        int UniqueIdentifier { get; }
    }
}
