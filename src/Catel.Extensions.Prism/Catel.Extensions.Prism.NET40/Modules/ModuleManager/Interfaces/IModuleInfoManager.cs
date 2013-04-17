// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModuleManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager
{
    using System.Collections.Generic;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// 
    /// </summary>
    public interface IModuleInfoManager
    {
        #region Properties
        /// <summary>
        /// Gets the known modules.
        /// </summary>
        IEnumerable<ModuleInfo> KnownModules { get; }
        #endregion
    }
}