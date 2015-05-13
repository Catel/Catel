// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPackageRequest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

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

        /// <summary>
        /// 
        /// </summary>
        public void EnsureAssemblyFileRef()
        {
            var path = AssemblyFileRef.Substring(7).Replace('/', '\\').ToLower();
            if (!File.Exists(path))
            {
                var assemblyFileName = Path.GetFileName(path);
                var idx = path.IndexOf(string.Format(CultureInfo.InvariantCulture, "\\{0}\\", Platforms.CurrentPlatform).ToLower(), StringComparison.InvariantCultureIgnoreCase);
                if (idx != -1)
                {
                    var directory = path.Substring(0, idx);
                    var assemblyFilePath = Directory.EnumerateFiles(directory, assemblyFileName, SearchOption.AllDirectories).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(assemblyFilePath))
                    {
                        AssemblyFileRef = string.Format("file://{0}", assemblyFilePath.Replace('\\', '/'));
                    }
                }
            }
        }
    }
}

#endif