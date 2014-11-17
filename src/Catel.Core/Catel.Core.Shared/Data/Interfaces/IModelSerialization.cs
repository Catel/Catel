// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelSerialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Xml.Serialization;

#if NET
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Defines all serialization members for the models.
    /// </summary>
    public interface IModelSerialization : IXmlSerializable
#if NET
                                        , ISerializable
#endif
    {
        /// <summary>
        /// Occurs when the object is deserialized.
        /// </summary>
        event EventHandler<EventArgs> Deserialized;
    }
}