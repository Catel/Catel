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
    [ObsoleteEx(Message = "No longer required because of new serialization engine in Catel 3.7", TreatAsErrorFromVersion = "3.9", RemoveInVersion = "4.0")]
    public class AllowNonSerializableMembersAttribute : Attribute
    {
    }
}
