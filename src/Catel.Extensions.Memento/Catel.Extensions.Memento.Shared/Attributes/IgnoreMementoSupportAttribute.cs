// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IgnoreMementoSupportAttri​bute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;

    /// <summary>
    /// This attribute prevents the specified property or method to be monitored by the <see cref="IMementoService"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class IgnoreMementoSupportAttribute : Attribute
    {
    }
}