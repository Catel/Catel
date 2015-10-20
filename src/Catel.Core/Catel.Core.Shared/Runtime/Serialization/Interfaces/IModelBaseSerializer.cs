// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelBaseSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using Data;

    /// <summary>
    /// Interface definition to serialize the <see cref="IModel"/>.
    /// </summary>
    [ObsoleteEx(ReplacementTypeOrMember = "ISerializer", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public interface IModelBaseSerializer
    {
    }

    /// <summary>
    /// Interface definition to serialize the <see cref="IModel"/>.
    /// </summary>
    [ObsoleteEx(ReplacementTypeOrMember = "ISerializer<TSerializationContext>", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public interface IModelBaseSerializer<TSerializationContext> : IModelBaseSerializer
        where TSerializationContext : class
    {
    }
}