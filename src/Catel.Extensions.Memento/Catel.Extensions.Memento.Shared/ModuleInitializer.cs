// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Extensions.Memento
{
    using Catel.IoC;
    using Catel.Memento;

    /// <summary>
    /// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
    /// </summary>
    public static class ModuleInitializer
    {
        #region Methods
        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Default;
            var module = new ExtensionsMementoModule();
            module.Initialize(serviceLocator);
        }
        #endregion
    }
}