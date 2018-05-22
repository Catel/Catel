// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionConstructorAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Attribute to specify the constructor to use for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class InjectionConstructorAttribute : Attribute
    {
    }
}