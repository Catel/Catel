// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IXmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System.Xml.Linq;

    /// <summary>
    /// Interface for the xml serializer.
    /// </summary>
    public interface IXmlSerializer : IModelBaseSerializer<XElement>
    {
    }
}