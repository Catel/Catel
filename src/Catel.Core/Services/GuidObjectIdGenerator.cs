// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidObjectIdGenerator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// Guid object generator.
    /// </summary>
    /// <typeparam name="TObjectType">
    /// The entity type.
    /// </typeparam>
    public class GuidObjectIdGenerator<TObjectType> : ObjectIdGenerator<TObjectType, Guid>
        where TObjectType : class
    {
        /// <inheritdoc />
        protected override Guid GenerateUniqueIdentifier()
        {
            return Guid.NewGuid();
        }
    }
}