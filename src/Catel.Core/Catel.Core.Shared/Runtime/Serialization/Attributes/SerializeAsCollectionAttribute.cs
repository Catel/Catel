// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializeAsCollectionAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Attribute to inform the serializers in Catel to serialize the object as collection.
    /// <para />
    /// This can only be used on ModelBase classes that implement <see cref="IList{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeAsCollectionAttribute : Attribute
    {
    }
}