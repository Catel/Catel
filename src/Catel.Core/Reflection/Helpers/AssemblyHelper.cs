// ------------------------------------------------------------------------------------------------- -------------------
// <copyright file="AssemblyHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Collections;
    using IoC;
    using Logging;

#if NET
    using System.Runtime.InteropServices;
#endif

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Assembly helper class.
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly object _lockObject = new object();

        private static readonly HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();
        private static readonly Dictionary<string, string> _assemblyMappings = new Dictionary<string, string>();

        /// <summary>
        /// Gets the entry assembly.
        /// </summary>
        /// <returns>Assembly.</returns>
        public static Assembly GetEntryAssembly()
        {
            Assembly assembly = null;

            try
            {
                var serviceLocator = ServiceLocator.Default;
                if (serviceLocator.IsTypeRegistered<IEntryAssemblyResolver>())
                {
                    var entryAssemblyResolver = serviceLocator.ResolveType<IEntryAssemblyResolver>();
                    assembly = entryAssemblyResolver.Resolve();
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }

#if NET
                var httpApplication = HttpContextHelper.GetHttpApplicationInstance();
                if (httpApplication != null)
                {
                    // Special treatment for ASP.NET
                    var type = httpApplication.GetType();
                    while ((type != null) && (type != typeof(object)) && (string.Equals(type.Namespace, "ASP", StringComparison.Ordinal)))
                    {
                        type = type.BaseType;
                    }

                    if (type != null)
                    {
                        assembly = type.Assembly;
                    }
                }

                if (assembly == null)
                {
                    assembly = Assembly.GetEntryAssembly();
                }

                if (assembly == null)
                {
                    var appDomain = AppDomain.CurrentDomain;
                    var setupInfo = appDomain.SetupInformation;
                    var assemblyPath = Path.Combine(setupInfo.ApplicationBase, setupInfo.ApplicationName);

                    assembly = (from x in appDomain.GetLoadedAssemblies(true)
                                where !x.IsDynamicAssembly() && string.Equals(x.Location, assemblyPath)
                                select x).FirstOrDefault();
                }
#elif NETFX_CORE
                assembly = global::Windows.UI.Xaml.Application.Current.GetType().GetAssemblyEx();
#else
                assembly = typeof(AssemblyHelper).GetAssemblyEx();
#endif
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get assembly");
            }

            if (assembly == null)
            {
                Log.Warning("Entry assembly could not be determined, returning Catel.Core as fallback");

                assembly = typeof(AssemblyHelper).GetAssemblyEx();
            }

            return assembly;
        }

        /// <summary>
        /// Gets the assembly name with version which is currently available in the <see cref="AppDomain" />.
        /// </summary>
        /// <param name="assemblyNameWithoutVersion">The assembly name without version.</param>
        /// <returns>The assembly name with version or <c>null</c> if the assembly is not found in the <see cref="AppDomain"/>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="assemblyNameWithoutVersion" /> is <c>null</c> or whitespace.</exception>
        public static string GetAssemblyNameWithVersion(string assemblyNameWithoutVersion)
        {
            Argument.IsNotNullOrWhitespace("assemblyNameWithoutVersion", assemblyNameWithoutVersion);

            lock (_lockObject)
            {
                if (_assemblyMappings.TryGetValue(assemblyNameWithoutVersion, out var assemblyNameWithVersion))
                {
                    return assemblyNameWithVersion;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets all types from the assembly safely. Sometimes, the <see cref="ReflectionTypeLoadException"/> is thrown,
        /// and no types are returned. In that case the user must manually get the successfully loaded types from the
        /// <see cref="ReflectionTypeLoadException.Types"/>.
        /// <para/>
        /// This method automatically loads the types. If the <see cref="ReflectionTypeLoadException"/> occurs, this method
        /// will return the types that were loaded successfully.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="logLoaderExceptions">If set to <c>true</c>, the loader exceptions will be logged.</param>
        /// <returns>The array of successfully loaded types.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        public static Type[] GetAllTypesSafely(this Assembly assembly, bool logLoaderExceptions = true)
        {
            Argument.IsNotNull("assembly", assembly);

            Type[] foundAssemblyTypes;

            RegisterAssemblyWithVersionInfo(assembly);

            try
            {
                foundAssemblyTypes = assembly.GetTypesEx();
            }
            catch (ReflectionTypeLoadException typeLoadException)
            {
                foundAssemblyTypes = (from type in typeLoadException.Types
                                      where type != null
                                      select type).ToArray();

                Log.Warning("A ReflectionTypeLoadException occured, adding all {0} types that were loaded correctly", foundAssemblyTypes.Length);

                if (logLoaderExceptions)
                {
                    Log.Warning("The following loading exceptions occurred:");

                    foreach (var error in typeLoadException.LoaderExceptions)
                    {
                        // Fix mono issue https://github.com/Catel/Catel/issues/1071 
                        if (error != null)
                        {
                            Log.Warning("  " + error.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get types from assembly '{0}'", assembly.FullName);

                foundAssemblyTypes = ArrayShim.Empty<Type>();
            }

            return foundAssemblyTypes;
        }

        /// <summary>
        /// Gets the loaded assemblies by using the right method. For Windows applications, it uses
        /// <c>AppDomain.GetAssemblies()</c>.
        /// </summary>
        /// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
        public static List<Assembly> GetLoadedAssemblies()
        {
            return GetLoadedAssemblies(AppDomain.CurrentDomain);
        }

        /// <summary>
        /// Gets the loaded assemblies by using the right method. For Windows applications, it uses
        /// <c>AppDomain.GetAssemblies()</c>.
        /// </summary>
        /// <param name="appDomain">The app domain to search in.</param>
        /// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
        public static List<Assembly> GetLoadedAssemblies(this AppDomain appDomain)
        {
            return GetLoadedAssemblies(appDomain, false);
        }

        /// <summary>
        /// Gets the loaded assemblies by using the right method. For Windows applications, it uses
        /// <c>AppDomain.GetAssemblies()</c>.
        /// </summary>
        /// <param name="appDomain">The app domain to search in.</param>
        /// <param name="ignoreDynamicAssemblies">if set to <c>true</c>, dynamic assemblies are being ignored.</param>
        /// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
        public static List<Assembly> GetLoadedAssemblies(this AppDomain appDomain, bool ignoreDynamicAssemblies)
        {
            var assemblies = new List<Assembly>();

            assemblies.AddRange(appDomain.GetAssemblies());

            var finalAssemblies = new List<Assembly>();

            // Make sure to prevent duplicates
            foreach (var assembly in assemblies.Distinct())
            {
                if (ShouldIgnoreAssembly(assembly, ignoreDynamicAssemblies))
                {
                    continue;
                }

                finalAssemblies.Add(assembly);

                RegisterAssemblyWithVersionInfo(assembly);
            }

            return finalAssemblies;
        }

        /// <summary>
        /// Determines whether the specified assembly is a dynamic assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns><c>true</c> if the specified assembly is a dynamic assembly; otherwise, <c>false</c>.</returns>
        public static bool IsDynamicAssembly(this Assembly assembly)
        {
            // Simplest & fastest way out
            if (assembly.IsDynamic)
            {
                return true;
            }

            var isDynamicAssembly =
#if NET
                (assembly is System.Reflection.Emit.AssemblyBuilder) &&
#endif
                string.Equals(assembly.GetType().FullName, "System.Reflection.Emit.InternalAssemblyBuilder", StringComparison.Ordinal)
#if NET
                && !assembly.GlobalAssemblyCache
                && ((Assembly.GetExecutingAssembly() != null)
                && !string.Equals(assembly.CodeBase, Assembly.GetExecutingAssembly().CodeBase, StringComparison.OrdinalIgnoreCase))
#endif
                // Note: to make it the same for all platforms
                && true;

            return isDynamicAssembly;
        }

        private static void RegisterAssemblyWithVersionInfo(Assembly assembly)
        {
            lock (_lockObject)
            {
                try
                {
                    if (_registeredAssemblies.Contains(assembly))
                    {
                        return;
                    }

                    _registeredAssemblies.Add(assembly);

                    var assemblyNameWithVersion = assembly.FullName;
                    var assemblyNameWithoutVersion = TypeHelper.GetAssemblyNameWithoutOverhead(assemblyNameWithVersion);
                    _assemblyMappings[assemblyNameWithoutVersion] = assemblyNameWithVersion;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to retrieve the information about assembly '{0}'", assembly);
                }
            }
        }

        private static bool ShouldIgnoreAssembly(Assembly assembly, bool ignoreDynamicAssemblies)
        {
            if (assembly == null)
            {
                return true;
            }

            if (ignoreDynamicAssemblies)
            {
                if (assembly.IsDynamicAssembly())
                {
                    return true;
                }
            }

            return false;
        }

        private struct AssemblyMetaData
        {
            public string NameWithVersion { get; set; }
            public string NameWithoutVersion { get; set; }
        }

#if NET
        /// <summary>
        /// Gets the linker timestamp.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>DateTime.</returns>
        /// <remarks>
        /// For more details, see https://docs.microsoft.com/en-us/windows/desktop/Debug/pe-format#dll-characteristics.
        /// </remarks>
        public static DateTime GetLinkerTimestamp(string fileName)
        {
            Argument.IsNotNullOrWhitespace(() => fileName);

            var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileStream.Position = 0x3C;
                fileStream.Read(buffer, 0, 4);
                fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                fileStream.Read(buffer, 0, 4); // "PE\0\0"
                fileStream.Read(buffer, 0, buffer.Length);
            }

            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));

                var utcStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var offset = TimeSpan.FromSeconds(coffHeader.TimeDateStamp);
                var utcTime = utcStart.Add(offset);

                var localTime = TimeZone.CurrentTimeZone.ToLocalTime(utcTime);
                if (localTime > DateTime.Now.AddDays(1))
                {
                    // Something is off here, are we running a .NET core "deterministic" app?
                    Log.Warning($"Determined '{localTime}' as linker timestamp which is in the future. This app is probably compiled by Roslyn (.NET Core) in deterministic mode. If the linker timestamp is required, set <Deterministic>False</Deterministic> in the project file. For now falling back to DateTime.Now");

                    localTime = DateTime.Now;
                }

                return localTime;
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }

#pragma warning disable 169
#pragma warning disable 649
        private struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        };
#pragma warning restore 169
#pragma warning restore 649
#endif
    }
}
