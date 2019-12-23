// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageResourceSource.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System.Diagnostics;

    /// <summary>
    /// <see cref="ILanguageSource" /> implementation for resource files.
    /// </summary>
    [DebuggerDisplay("{GetSource()}")]
    public class LanguageResourceSource : ILanguageSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageResourceSource" /> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="resourceFileName">Name of the resource file.</param>
        public LanguageResourceSource(string assemblyName, string namespaceName, string resourceFileName)
        {
            Argument.IsNotNullOrWhitespace(nameof(assemblyName), assemblyName);
            Argument.IsNotNullOrWhitespace(nameof(namespaceName), namespaceName);
            Argument.IsNotNullOrWhitespace(nameof(resourceFileName), resourceFileName);

            AssemblyName = assemblyName;
            NamespaceName = namespaceName;
            ResourceFileName = resourceFileName;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Gets the name of the namespace.
        /// </summary>
        /// <value>The name of the namespace.</value>
        public string NamespaceName { get; private set; }

        /// <summary>
        /// Gets the name of the resource file.
        /// </summary>
        /// <value>The name of the resource file.</value>
        public string ResourceFileName { get; private set; }

        /// <summary>
        /// Gets the source for the current language source.
        /// </summary>
        /// <returns>The source string.</returns>
        public string GetSource()
        {
#if UWP
            return string.Format("{0}|{1}", AssemblyName, ResourceFileName);
#else
            return string.Format("{0}.{1}, {2}", NamespaceName, ResourceFileName, AssemblyName);
#endif
        }
    }
}
