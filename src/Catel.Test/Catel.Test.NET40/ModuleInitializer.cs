// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Test.Data;

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        #region Methods

        /// <summary>
        /// Initializes the module
        /// </summary>
        public static void Initialize()
        {
            LogManager.RegisterDebugListener();

            // For testing purposes, enable features we disabled for CTL-234
            var modelEqualityComparer = ServiceLocator.Default.ResolveType<IModelEqualityComparer>();
            modelEqualityComparer.CompareProperties = true;
            modelEqualityComparer.CompareValues = true;
            modelEqualityComparer.CompareCollections = true;
        }

        #endregion
    }
}