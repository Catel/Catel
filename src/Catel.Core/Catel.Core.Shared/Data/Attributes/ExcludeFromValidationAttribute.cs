// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromValidationAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;

    /// <summary>
    /// Attribute that can be used to exclude properties from validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcludeFromValidationAttribute : Attribute
    {
    }
}