// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromSerializationAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Attribute to define that a specific member must be excluded from the serialization by the serialization engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ExcludeFromSerializationAttribute : Attribute
    {
    }
}