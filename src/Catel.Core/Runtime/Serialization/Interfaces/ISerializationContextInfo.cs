// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
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
