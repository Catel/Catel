namespace Catel.Services
{
    /// <summary>
    /// Integer object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class IntegerObjectIdGenerator<TObjectType> : NumericBasedObjectIdGenerator<TObjectType, int>
        where TObjectType : class
    {
        /// <inheritdoc />
        protected override int GenerateUniqueIdentifier()
        {
            lock (_lock)
            {
                return Value++;
            }
        }
    }

    /// <summary>
    /// Long object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class LongObjectIdGenerator<TObjectType> : NumericBasedObjectIdGenerator<TObjectType, long>
        where TObjectType : class
    {
        /// <inheritdoc />
        protected override long GenerateUniqueIdentifier()
        {
            lock (_lock)
            {
                return Value++;
            }
        }
    }

    /// <summary>
    /// ULong object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class ULongObjectIdGenerator<TObjectType> : NumericBasedObjectIdGenerator<TObjectType, ulong>
        where TObjectType : class
    {
        /// <inheritdoc />
        protected override ulong GenerateUniqueIdentifier()
        {
            lock (_lock)
            {
                return Value++;
            }
        }
    }
}
