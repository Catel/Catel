﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Extensions.Prism
{
    using IoC;

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initializes the module
        /// </summary>
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Default;
            var module = new ExtensionsPrismModule();
            module.Initialize(serviceLocator);
        }
    }
}