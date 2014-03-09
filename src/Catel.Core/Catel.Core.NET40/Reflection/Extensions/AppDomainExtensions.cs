// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
        /// <see cref="TypeCache.GetTypes"/> instead.
        /// </remarks>
        internal static Type[] GetTypes(this AppDomain appDomain)
        {
            Argument.IsNotNull("appDomain", appDomain);

            return TypeCache.GetTypes();

            //List<Assembly> assemblies = AssemblyHelper.GetLoadedAssemblies(appDomain);
            //var types = new List<Type>();

            //foreach (var assembly in assemblies)
            //{
            //    types.AddRange(from assemblyType in AssemblyHelper.GetAllTypesSafely(assembly)
            //                   select assemblyType);
            //}

            //return types.ToArray();
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
        public static void PreloadAssemblies(this AppDomain appDomain, string directory = null)
        {
            Argument.IsNotNull("appDomain", appDomain);

            Log.Info("Preloading assemblies from AppDomain");
            Log.Indent();

            var loadedAssemblies = new List<string>();

            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in currentAssemblies)
            {
                var referencedAssemblies = assembly.GetReferencedAssemblies();
                foreach (var referencedAssembly in referencedAssemblies)
                {
                    LoadAssemblyIntoAppDomain(appDomain, referencedAssembly, true, loadedAssemblies);
                }
            }

            Log.Unindent();

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Log.Info("Preloading assemblies from directory '{0}'", directory);
                Log.Indent();

                var files = new DirectoryInfo(directory).GetFiles("*.dll");
                foreach (var file in files)
                {
                    LoadAssemblyIntoAppDomain(appDomain, file.FullName, true, loadedAssemblies);
                }

                Log.Unindent();
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
            LoadAssemblyIntoAppDomain(appDomain, assemblyFilename, includeReferencedAssemblies, new List<string>());
        }

        /// <summary>
        /// Loads the assembly into the specified <see cref="AppDomain" />.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="assemblyFilename">The assembly filename.</param>
        /// <param name="includeReferencedAssemblies">if set to <c>true</c>, referenced assemblies will be included as well.</param>
        /// <param name="alreadyLoadedAssemblies">The already loaded assemblies.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="assemblyFilename" /> is <c>null</c> or whitespace.</exception>
        private static void LoadAssemblyIntoAppDomain(this AppDomain appDomain, string assemblyFilename, bool includeReferencedAssemblies,
            List<string> alreadyLoadedAssemblies)
        {
            Argument.IsNotNull("appDomain", appDomain);
            Argument.IsNotNullOrWhitespace("assemblyFilename", assemblyFilename);

            if (!File.Exists(assemblyFilename))
            {
                Log.Warning("Assembly file '{0}' does not exist, cannot preload assembly", assemblyFilename);
                return;
            }

            var assemblyName = AssemblyName.GetAssemblyName(assemblyFilename);
            LoadAssemblyIntoAppDomain(appDomain, assemblyName, includeReferencedAssemblies, alreadyLoadedAssemblies);
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
            LoadAssemblyIntoAppDomain(appDomain, assembly.GetName(), includeReferencedAssemblies, new List<string>());
        }

        /// <summary>
        /// Loads the assembly into the specified <see cref="AppDomain" />.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="includeReferencedAssemblies">if set to <c>true</c>, referenced assemblies will be included as well.</param>
        /// <param name="alreadyLoadedAssemblies">The already loaded assemblies.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly" /> is <c>null</c>.</exception>
        public static void LoadAssemblyIntoAppDomain(this AppDomain appDomain, Assembly assembly, bool includeReferencedAssemblies,
            List<string> alreadyLoadedAssemblies)
        {
            LoadAssemblyIntoAppDomain(appDomain, assembly.GetName(), includeReferencedAssemblies, alreadyLoadedAssemblies);
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
            LoadAssemblyIntoAppDomain(appDomain, assemblyName, includeReferencedAssemblies, new List<string>());
        }

        /// <summary>
        /// Loads the assembly into the specified <see cref="AppDomain" />.
        /// </summary>
        /// <param name="appDomain">The app domain.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="includeReferencedAssemblies">if set to <c>true</c>, referenced assemblies will be included as well.</param>
        /// <param name="alreadyLoadedAssemblies">The already loaded assemblies.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="appDomain" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblyName" /> is <c>null</c>.</exception>
        private static void LoadAssemblyIntoAppDomain(this AppDomain appDomain, AssemblyName assemblyName, bool includeReferencedAssemblies,
            List<string> alreadyLoadedAssemblies)
        {
            Argument.IsNotNull("appDomain", appDomain);
            Argument.IsNotNull("assemblyName", assemblyName);

            try
            {
                if (alreadyLoadedAssemblies.Contains(assemblyName.FullName))
                {
                    return;
                }

                if (appDomain.GetAssemblies().Any(assembly => AssemblyName.ReferenceMatchesDefinition(assemblyName, assembly.GetName())))
                {
                    return;
                }

                alreadyLoadedAssemblies.Add(assemblyName.FullName);

                Log.Debug("Preloading assembly '{0}'", assemblyName);

                var loadedAssembly = appDomain.Load(assemblyName);

                // Note: actually load a type so the assembly is loaded
                var type = loadedAssembly.GetTypesEx().FirstOrDefault(x => x.IsClassEx() && !x.IsInterfaceEx());
                Log.Debug("Loaded assembly, found '{0}' as first class type", type.GetSafeFullName());

                if (includeReferencedAssemblies)
                {
                    Log.Debug("Loading referenced assemblies of assembly '{0}'", assemblyName);

                    var referencedAssemblies = loadedAssembly.GetReferencedAssemblies();
                    foreach (var referencedAssembly in referencedAssemblies)
                    {
                        LoadAssemblyIntoAppDomain(appDomain, referencedAssembly, false, alreadyLoadedAssemblies);
                    }
                }

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

            return (T)appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }
#endif
    }
}
