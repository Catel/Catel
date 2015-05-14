// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPackageRequest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    /// <summary>
    /// The install package request.
    /// </summary>
    public class InstallPackageRequest
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallPackageRequest" /> class.
        /// </summary>
        /// <param name="moduleAssemblyRef">The assembly file reference.</param>
        /// <exception cref="System.ArgumentException">The <paramref name="moduleAssemblyRef" /> is <c>null</c> or whitespace.</exception>
        public InstallPackageRequest(ModuleAssemblyRef moduleAssemblyRef)
        {
            Argument.IsNotNull(() => moduleAssemblyRef);

            this.ModuleAssemblyRef = moduleAssemblyRef;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the module assembly reference.
        /// </summary>
        /// <value>The assembly file reference.</value>
        public ModuleAssemblyRef ModuleAssemblyRef { get; private set; }

        #endregion

        /// <summary>
        /// Execute the package.
        /// </summary>
        public virtual void Execute()
        {
            // Do nothing
        }
    }
}

#endif