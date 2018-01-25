// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericBasedObjectIdGenerators.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    /// <summary>
    /// Integer object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class IntegerObjectIdGenerator<TObjectType> : NumericBasedObjectIdGenerator<TObjectType, int>
    {
        /// <inheritdoc />
        protected override int GenerateUniqueIdentifier()
        {
            return Value++;
        }
    }

    /// <summary>
    /// Long object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class LongObjectIdGenerator<TObjectType> : NumericBasedObjectIdGenerator<TObjectType, long>
    {
        /// <inheritdoc />
        protected override long GenerateUniqueIdentifier()
        {
            return Value++;
        }
    }

    /// <summary>
    /// ULong object id generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The object type.
    /// </typeparam>
    public sealed class ULongObjectIdGenerator<TObjectType> : NumericBasedObjectIdGenerator<TObjectType, ulong>
    {
        /// <inheritdoc />
        protected override ulong GenerateUniqueIdentifier()
        {
            return Value++;
        }
    }
}