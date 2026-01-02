namespace Catel.Services
{
    /// <summary>
    /// Integer object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class IntegerObjectIdGenerator<TObjectType> : IntegerObjectIdGenerator<TObjectType, int>
        where TObjectType : class
    {
    }

    /// <summary>
    /// Integer object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    /// <typeparam name="TInt">
    /// The type, must be 'int', but will be ignored.
    /// </typeparam>
    /// <remarks>
    /// Explicitly adding the TInt type parameter to make sure the type can be registered
    /// as open generic:
    /// 
    /// <code>
    /// serviceCollection.TryAddSingleton(typeof(IObjectIdGenerator&lt;,&gt;), typeof(IntegerObjectIdGenerator&lt;,&gt;));
    /// </code>
    /// </remarks>
    public class IntegerObjectIdGenerator<TObjectType, TInt> : NumericBasedObjectIdGenerator<TObjectType, int>
        where TObjectType : class
    {
        public IntegerObjectIdGenerator()
        {
            Value = 1;
        }

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
    public sealed class LongObjectIdGenerator<TObjectType> : LongObjectIdGenerator<TObjectType, long>
        where TObjectType : class
    {
    }

    /// <summary>
    /// Long object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    /// <typeparam name="TLong">
    /// The type, must be 'long', but will be ignored.
    /// </typeparam>
    /// <remarks>
    /// Explicitly adding the TLong type parameter to make sure the type can be registered
    /// as open generic:
    /// 
    /// <code>
    /// serviceCollection.TryAddSingleton(typeof(IObjectIdGenerator&lt;,&gt;), typeof(LongObjectIdGenerator&lt;,&gt;));
    /// </code>
    /// </remarks>
    public class LongObjectIdGenerator<TObjectType, TLong> : NumericBasedObjectIdGenerator<TObjectType, long>
        where TObjectType : class
    {
        public LongObjectIdGenerator()
        {
            Value = 1;
        }

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
    public sealed class ULongObjectIdGenerator<TObjectType> : ULongObjectIdGenerator<TObjectType, ulong>
        where TObjectType : class
    {
    }

    /// <summary>
    /// ULong object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    /// <typeparam name="TLong">
    /// The type, must be 'ulong', but will be ignored.
    /// </typeparam>
    /// <remarks>
    /// Explicitly adding the TLong type parameter to make sure the type can be registered
    /// as open generic:
    /// 
    /// <code>
    /// serviceCollection.TryAddSingleton(typeof(IObjectIdGenerator&lt;,&gt;), typeof(ULongObjectIdGenerator&lt;,&gt;));
    /// </code>
    /// </remarks>
    public class ULongObjectIdGenerator<TObjectType, TLong> : NumericBasedObjectIdGenerator<TObjectType, ulong>
        where TObjectType : class
    {
        public ULongObjectIdGenerator()
        {
            Value = 1;
        }

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
