// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPackageRequest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
        /// <param name="assemblyFileRef">The assembly file reference.</param>
        /// <exception cref="System.ArgumentException">The <paramref name="assemblyFileRef" /> is <c>null</c> or whitespace.</exception>
        public InstallPackageRequest(string assemblyFileRef)
        {
            Argument.IsNotNullOrWhitespace(() => assemblyFileRef);

            AssemblyFileRef = assemblyFileRef;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Tge assembly file reference.
        /// </summary>
        /// <value>The assembly file reference.</value>
        public string AssemblyFileRef { get; private set; }
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