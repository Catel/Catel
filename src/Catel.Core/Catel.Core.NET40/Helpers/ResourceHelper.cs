namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

#if NETFX_CORE
    using Windows.ApplicationModel.Resources;
#else
using System.Resources;
#endif

    using Reflection;

    /// <summary>
    /// Resource helper class to read resource files.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Dictionary containing a dictionary of resource managers per assembly name.
        /// <para />
        /// The first key is the full assembly name, for example <c>Catel.MVVM</c>. Then, the dictionary it contains is a 
        /// dictionary per item.
        /// </summary>
#if NETFX_CORE
        private static readonly Dictionary<string, Dictionary<string, ResourceLoader>> _dictionaryMappings = new Dictionary<string, Dictionary<string, ResourceLoader>>();
#else
        private static readonly Dictionary<string, Dictionary<string, ResourceManager>> _dictionaryMappings = new Dictionary<string, Dictionary<string, ResourceManager>>();
#endif

        private static readonly Dictionary<string, string> _assemblyMappings = new Dictionary<string, string>();

        // TODO: Add caching etc

        static ResourceHelper()
        {
            AddCustomAssemblyMapping("Catel.Core", "Catel");
            AddCustomAssemblyMapping("Catel.MVVM", "Catel");
        }

        /// <summary>
        /// Adds the custom assembly mapping.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="mappedAssemblyName">Name of the mapped assembly.</param>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="mappedAssemblyName"/> is <c>null</c> or whitespace.</exception>
        public static void AddCustomAssemblyMapping(string assemblyName, string mappedAssemblyName)
        {
            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);
            Argument.IsNotNullOrWhitespace("mappedAssemblyName", mappedAssemblyName);

            _assemblyMappings[assemblyName] = mappedAssemblyName;
        }

        /// <summary>
        /// Gets the string from the specified resource file.
        /// </summary>
        /// <param name="callingType">Type of the calling.</param>
        /// <param name="resourceFile">The resource file.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="callingType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceFile"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceName"/> is <c>null</c> or whitespace.</exception>
        public static string GetString(Type callingType, string resourceFile, string resourceName)
        {
            Argument.IsNotNull("callingType", callingType);
            Argument.IsNotNullOrWhitespace("resourceFile", resourceFile);
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            var callingTypeAssembly = callingType.GetAssemblyEx();
            var resourceManager = GetResourceManager(callingTypeAssembly, resourceFile);

            return resourceManager.GetString(resourceName);
        }

#if NETFX_CORE
        private static ResourceLoader GetResourceManager(Assembly assembly, string resourceFile)
#else
        private static ResourceManager GetResourceManager(Assembly assembly, string resourceFile)
#endif
        {
            var originalAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(assembly.FullName);
            var assemblyName = originalAssemblyName;

            // Allow mapping of assemblies
            if (_assemblyMappings.ContainsKey(originalAssemblyName))
            {
                assemblyName = _assemblyMappings[originalAssemblyName];
            }

            if (!_dictionaryMappings.ContainsKey(originalAssemblyName))
            {
#if NETFX_CORE
                _dictionaryMappings[originalAssemblyName] = new Dictionary<string, ResourceLoader>();
#else
                _dictionaryMappings[originalAssemblyName] = new Dictionary<string, ResourceManager>();
#endif
            }

            var dictionary = _dictionaryMappings[originalAssemblyName];

#if NETFX_CORE
            // TODO: Write
            resourceFile = string.Format("{0}/{1}", originalAssemblyName, resourceFile);
#else
            resourceFile = string.Format("{0}.Properties.{1}", assemblyName, resourceFile);
#endif

            if (!dictionary.ContainsKey(resourceFile))
            {
#if NETFX_CORE
                dictionary[resourceFile] = new ResourceLoader(resourceFile);
#else
                dictionary[resourceFile] = new ResourceManager(resourceFile, assembly);
#endif
            }

            return dictionary[resourceFile];
        }
    }
}
