namespace Catel.Services
{
    /// <summary>
    /// The numeric based object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">The object type.</typeparam>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type.</typeparam>
    public abstract class NumericBasedObjectIdGenerator<TObjectType, TUniqueIdentifier> : ObjectIdGenerator<TObjectType, TUniqueIdentifier>
        where TObjectType : class
    {
        static NumericBasedObjectIdGenerator()
        {
            Argument.IsValid("TUniqueIdentifier", typeof(TUniqueIdentifier), type => typeof(int).IsAssignableFrom(type) || typeof(long).IsAssignableFrom(type) || typeof(ulong).IsAssignableFrom(type));
        }

        /// <summary>
        /// Gets and sets the value.
        /// </summary>
        protected static TUniqueIdentifier Value { get; set; } = default(TUniqueIdentifier);
    }
}