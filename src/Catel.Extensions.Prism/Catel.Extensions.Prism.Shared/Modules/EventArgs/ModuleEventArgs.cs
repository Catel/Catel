// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    
#if PRISM6
    using Prism.Modularity;
#else
    using Microsoft.Practices.Prism.Modularity;
#endif

    /// <summary>
    /// Event args for moldue events.
    /// </summary>
    public class ModuleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleEventArgs" /> class.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        public ModuleEventArgs(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull("moduleInfo", moduleInfo);

            ModuleName = moduleInfo.ModuleName;
        }

        /// <summary>
        /// Gets the module info.
        /// </summary>
        /// <value>The module info.</value>
        public string ModuleName { get; private set; }
    }
}