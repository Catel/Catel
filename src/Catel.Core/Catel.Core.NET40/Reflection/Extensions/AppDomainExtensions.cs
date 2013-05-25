// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Logging;

    /// <summary>
    /// <see cref="AppDomain"/> extensions.
    /// </summary>
    public static class AppDomainExtensions
    {
#if NET
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
#endif

        /// <summary>
        /// Gets a list of all types inside the <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <returns>List of types found in the <see cref="AppDomain"/>.</returns>
        /// <remarks>
        /// This class must only be used by Catel. To make sure that an application performs, make sure to use
        /// <see cref="TypeCache.GetTypes()"/> instead.
        /// </remarks>
        internal static Type[] GetTypes(this AppDomain appDomain)
        {
            Argument.IsNotNull("appDomain", appDomain);

            List<Assembly> assemblies = AssemblyHelper.GetLoadedAssemblies(appDomain);
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(from assemblyType in AssemblyHelper.GetAllTypesSafely(assembly)
                               select assemblyType);
            }

            return types.ToArray();
        }

#if NET
        /// <summary>
        /// Preloads all the assemblies inside the specified directory into the specified <see cref="AppDomain" />.
        /// <para />
        /// This method also preloads all referenced assemblies.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="directory">The directory. If <c>null</c>, only the referenced assemblies are forced to be loaded.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="directory"/> is <c>null</c> or whitespace.</exception>
        public static void PreloadAssemblies(this AppDomain appDomain, string directory = null)
        {
            Argument.IsNotNull("appDomain", appDomain);

            Log.Info("Preloading assemblies from AppDomain");
            Log.Indent();

            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in currentAssemblies)
            {
                LoadAssemblyIntoAppDomain(appDomain, assembly);

                var referencedAssemblies = assembly.GetReferencedAssemblies();
                foreach (var referencedAssembly in referencedAssemblies)
                {
                    LoadAssemblyIntoAppDomain(appDomain, referencedAssembly);
                }
            }

            Log.Unindent();
            Log.Info("Preloaded assemblies from AppDomain");

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Log.Info("Preloading assemblies from directory '{0}'", directory);
                Log.Indent();

                var files = new DirectoryInfo(directory).GetFiles("*.dll");
                foreach (var file in files)
                {
                    LoadAssemblyIntoAppDomain(appDomain, file.FullName);
                }

                Log.Unindent();
                Log.Info("Preloaded assemblies from directory");
            }
        }

        /// <summary>
        /// Loads the assembly into the specified <see cref="AppDomain" />.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="assemblyFilename">The assembly filename.</param>
        /// <param name="includeReferencedAssemblies">if set to <c>true</c>, referenced assemblies will be included as well.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="assemblyFilename" /> is <c>null</c> or whitespace.</exception>
        public static void LoadAssemblyIntoAppDomain(this AppDomain appDomain, string assemblyFilename, bool includeReferencedAssemblies = true)
        {
            Argument.IsNotNull("appDomain", appDomain);
            Argument.IsNotNullOrWhitespace("assemblyFilename", assemblyFilename);

            Log.Debug("Preloading assembly from file '{0}'", assemblyFilename);

            if (!File.Exists(assemblyFilename))
            {
                Log.Warning("Assembly file '{0}' does not exist, cannot preload assembly", assemblyFilename);
                return;
            }

            var assemblyName = AssemblyName.GetAssemblyName(assemblyFilename);
            LoadAssemblyIntoAppDomain(appDomain, assemblyName);
        }

        /// <summary>
        /// Loads the assembly into the specified <see cref="AppDomain" />.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="includeReferencedAssemblies">if set to <c>true</c>, referenced assemblies will be included as well.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        public static void LoadAssemblyIntoAppDomain(this AppDomain appDomain, Assembly assembly, bool includeReferencedAssemblies = true)
        {
            Argument.IsNotNull("appDomain", appDomain);
            Argument.IsNotNull("assembly", assembly);

            Log.Debug("Preloading assembly '{0}'", assembly.FullName);

            LoadAssemblyIntoAppDomain(appDomain, assembly.GetName(), includeReferencedAssemblies);
        }

        /// <summary>
        /// Loads the assembly into the specified <see cref="AppDomain" />.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="includeReferencedAssemblies">if set to <c>true</c>, referenced assemblies will be included as well.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblyName"/> is <c>null</c>.</exception>
        public static void LoadAssemblyIntoAppDomain(this AppDomain appDomain, AssemblyName assemblyName, bool includeReferencedAssemblies = true)
        {
            Argument.IsNotNull("appDomain", appDomain);
            Argument.IsNotNull("assemblyName", assemblyName);

            Log.Debug("Preloading assembly '{0}'", assemblyName);

            try
            {
                if (appDomain.GetAssemblies().Any(assembly => AssemblyName.ReferenceMatchesDefinition(assemblyName, assembly.GetName())))
                {
                    Log.Debug("Assembly already loaded into the AppDomain");
                    return;
                }

                var loadedAssembly = appDomain.Load(assemblyName);
                if (includeReferencedAssemblies)
                {
                    Log.Debug("Loading referenced assemblies of assembly '{0}'", assemblyName);

                    var referencedAssemblies = loadedAssembly.GetReferencedAssemblies();
                    foreach (var referencedAssembly in referencedAssemblies)
                    {
                        LoadAssemblyIntoAppDomain(appDomain, referencedAssembly, false);
                    }
                }

                // Convenience call
                TypeCache.InitializeTypes(false, loadedAssembly);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load assembly '{0}'", assemblyName);
            }
        }

        /// <summary>
        /// Creates the instance in the specified <see cref="AppDomain" /> and unwraps it.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="appDomain">The app domain.</param>
        /// <returns>The created instance of the specified type</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain"/> is <c>null</c>.</exception>
        public static T CreateInstanceAndUnwrap<T>(this AppDomain appDomain)
            where T : new()
        {
            Argument.IsNotNull("appDomain", appDomain);

            Log.Debug("Creating instance of '{0}'", typeof(T).FullName);

            return (T) appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }
#endif
    }
}
