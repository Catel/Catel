// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleAssemblyRef.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The Module Assembly Ref
    /// </summary>
    public class ModuleAssemblyRef
    {
        #region Constants
        /// <summary>
        /// The relative Url Pattern
        /// </summary>
        private const string RelativeUrlPattern = "{0}/{1}/lib/{2}/{3}";
        #endregion

        #region Fields
        
        /// <summary>
        /// The ref
        /// </summary>
        private string _ref;
        
        #endregion

        #region Constructors
        /// <summary>
        /// </summary>
        /// <param name="ref"></param>
        private ModuleAssemblyRef(string @ref)
        {
            Argument.IsNotNullOrWhitespace(() => @ref);

            _ref = @ref;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="directoryName"></param>
        /// <param name="assemblyName"></param>
        public ModuleAssemblyRef(string baseUri, string directoryName, string assemblyName)
            : this(string.Format(CultureInfo.InvariantCulture, RelativeUrlPattern, baseUri, directoryName, Platforms.CurrentPlatform, assemblyName))
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the assembly ref 
        /// </summary>
        public string Ref
        {
            get
            {
                string assemblyFilePath = AssemblyFilePath;
                if (!string.IsNullOrWhiteSpace(assemblyFilePath))
                {
                    _ref = new Uri(assemblyFilePath).AbsoluteUri;
                }

                return _ref;
            }
        }

        /// <summary>
        /// Gets the assembly file path.
        /// </summary>
        private string AssemblyFilePath
        {
            get
            {
                string assemblyFilePath = new Uri(_ref).LocalPath.ToLower();
                if (!File.Exists(assemblyFilePath))
                {
                    string assemblyFileName = Path.GetFileName(assemblyFilePath);
                    int idx = assemblyFilePath.IndexOf(string.Format(CultureInfo.InvariantCulture, "\\{0}\\", Platforms.CurrentPlatform).ToLower(), StringComparison.InvariantCultureIgnoreCase);
                    if (idx != -1)
                    {
                        string directoryPath = assemblyFilePath.Substring(0, idx);
                        if (Directory.Exists(directoryPath))
                        {
                            assemblyFilePath = Directory.EnumerateFiles(directoryPath, assemblyFileName, SearchOption.AllDirectories).FirstOrDefault() ?? assemblyFilePath;
                        }
                    }
                }

                return assemblyFilePath;
            }
        }

        /// <summary>
        /// Gets whether the module is already installed.
        /// </summary>
        public bool IsInstalled
        {
            get
            {
                return File.Exists(AssemblyFilePath);
            }
        }
        #endregion

    }
}