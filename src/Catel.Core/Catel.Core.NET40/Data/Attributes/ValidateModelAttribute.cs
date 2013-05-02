// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateModelAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;

    /// <summary>
    /// Attribute to define custom validation at class level for all classes that derive from <see cref="ModelBase"/>.
    /// <para />
    /// This attribute follows a naming convention. If 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ValidateModelAttribute : Attribute
    {
    }
}