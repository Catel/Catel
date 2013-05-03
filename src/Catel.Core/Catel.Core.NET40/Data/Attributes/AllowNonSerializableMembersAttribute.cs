// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllowNonSerializableMembersAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;

    /// <summary>
    /// Attribute to define that a <see cref="ModelBase"/> class can contain non-serializable members. This attribute
    /// must be used with care, because it will disable the serialization functionality of the <see cref="ModelBase"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AllowNonSerializableMembersAttribute : Attribute
    {
    }
}
