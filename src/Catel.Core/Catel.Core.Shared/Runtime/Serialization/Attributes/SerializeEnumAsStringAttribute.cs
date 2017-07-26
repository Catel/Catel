// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializeEnumAsStringAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Attribute to define that a enum member must be serialized as string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SerializeEnumAsStringAttribute : Attribute
    {
    }
}
