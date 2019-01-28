// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelSerialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    /// <summary>
    /// Defines all serialization members for the models.
    /// </summary>
    public interface IModelSerialization : Runtime.Serialization.ISerializable, System.Xml.Serialization.IXmlSerializable
#if NET || NETCORE || NETSTANDARD
                                        , System.Runtime.Serialization.ISerializable
#endif
    {
    }
}
