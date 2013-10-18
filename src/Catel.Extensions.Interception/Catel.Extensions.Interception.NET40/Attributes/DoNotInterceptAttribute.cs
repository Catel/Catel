// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotInterceptAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using System;

    /// <summary>
    ///   Represents an attribute which indicates that a property or method should not be intercepted.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false,
        Inherited = false)]
    public sealed class DoNotInterceptAttribute : Attribute
    {
    }
}