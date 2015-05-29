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
    using Logging;

#if NET
    using System.Runtime.InteropServices;
#endif

#if SILVERLIGHT
    using System.Windows;
#endif

#if SL5
    using System.Windows.Resources;
    using System.Xml.Linq;
    using Threading;
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

        private static readonly Dictionary<string, string> _assemblyMappings = new Dictionary<string, string>();

#if SL5
        private static readonly List<Assembly> _externalAssemblies = new List<Assembly>();

        /// <summary>
        /// Registers the assemblies from a xap file stream. The assemblies are added to a local
        /// cache which will be used by the <see cref="GetLoadedAssemblies()"/> method.
        /// </summary>
        /// <param name="xapStream">The xap stream.</param>
        /// <param name="registerInBackground">If <c>true</c>, the assembly will be loaded in the background.</param>
        /// <returns>List of assemblies in the xap files.</returns>
        /// <remarks>
        /// This method requires that the xap stream contains an <c>AppManifest.xaml</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="xapStream"/> is <c>null</c>.</exception>
        public static void RegisterAssembliesFromXap(Stream xapStream, bool registerInBackground = false)
        {
            Argument.IsNotNull("xapStream", xapStream);

            try
            {
                string appManifest = new StreamReader(Application.GetResourceStream(new StreamResourceInfo(xapStream, null),
                    new Uri("AppManifest.xaml", UriKind.Relative)).Stream).ReadToEnd();

                var deploy = XDocument.Parse(appManifest).Root;

                var parts = (from assemblyParts in deploy.Elements().Elements()
                             select assemblyParts).ToList();

                foreach (var xe in parts)
                {
                    string source = xe.Attribute("Source").Value;
                    var asmPart = new AssemblyPart();
                    var streamInfo = Application.GetResourceStream(new StreamResourceInfo(xapStream, "application/binary"), new Uri(source, UriKind.Relative));

                    var assembly = asmPart.Load(streamInfo.Stream);
                    if ((assembly != null) && !_externalAssemblies.Contains(assembly))
                    {
                        _externalAssemblies.Add(assembly);

                        var action = new Action(() =>
                        {
                            Log.Debug("Initializing types for assembly '{0}'", assembly.FullName);

                            TypeCache.InitializeTypes(false, assembly);

                            RegisterAssemblyWithVersionInfo(assembly);

                            Log.Debug("Initialized types for assembly '{0}'", assembly.FullName);
                        });

                        if (registerInBackground)
                        {
                            TaskHelper.RunAndWait(new [] {action});
                        }
                        else
                        {
                            action();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO: Add logging?
            }
        }
#endif

        /// <summary>
        /// Gets the entry assembly.
        /// </summary>
        /// <returns>Assembly.</returns>
        public static Assembly GetEntryAssembly()
        {
            Assembly assembly = null;

            try
            {
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

                    assembly = (from x in appDomain.GetAssemblies()
                                where string.Equals(x.Location, assemblyPath)
                                select x).FirstOrDefault();
                }
#elif SILVERLIGHT
                assembly = System.Windows.Application.Current.GetType().Assembly;
#elif NETFX_CORE
                assembly = global::Windows.UI.Xaml.Application.Current.GetType().GetAssemblyEx();
#else
                assembly = typeof(AssemblyHelper).GetAssemblyEx();
#endif
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get assembly, returning Catel.Core as fallback");

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

            if (assemblyNameWithoutVersion.Contains(", Version="))
            {
                return assemblyNameWithoutVersion;
            }

            lock (_lockObject)
            {
                if (_assemblyMappings.ContainsKey(assemblyNameWithoutVersion))
                {
                    return _assemblyMappings[assemblyNameWithoutVersion];
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
                        Log.Warning("  " + error.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get types from assembly '{0}'", assembly.FullName);

                foundAssemblyTypes = new Type[] { };
            }

            return foundAssemblyTypes;
        }

        /// <summary>
        /// Gets the loaded assemblies by using the right method. For Windows applications, it uses
        /// <c>AppDomain.GetAssemblies()</c>. For Silverlight, it uses the assemblies
        /// from the current application.
        /// </summary>
        /// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
        public static List<Assembly> GetLoadedAssemblies()
        {
            return GetLoadedAssemblies(AppDomain.CurrentDomain);
        }

        /// <summary>
        /// Gets the loaded assemblies by using the right method. For Windows applications, it uses
        /// <c>AppDomain.GetAssemblies()</c>. For Silverlight, it uses the assemblies
        /// from the current application.
        /// </summary>
        /// <param name="appDomain">The app domain to search in.</param>
        /// <returns><see cref="List{Assembly}" /> of all loaded assemblies.</returns>
        public static List<Assembly> GetLoadedAssemblies(this AppDomain appDomain)
        {
            var assemblies = new List<Assembly>();

            assemblies.AddRange(appDomain.GetAssemblies());

#if SILVERLIGHT
            try
            {
                if (Deployment.Current != null)
                {
                    foreach (AssemblyPart assemblyPart in Deployment.Current.Parts)
                    {
#if WINDOWS_PHONE
                        try
                        {
                            // It's not much, but it's the best we could do for Windows Phone
                            assemblies.Add(Assembly.Load(assemblyPart.Source.Replace(".dll", string.Empty)));
                        }
                        catch (Exception)
                        {
                            // Continue, let's hope this assembly is not required
                        }
#else
                        var sri = Application.GetResourceStream(new Uri(assemblyPart.Source, UriKind.Relative));
                        var assembly = assemblyPart.Load(sri.Stream);
                        if (assembly != null)
                        {
                            assemblies.Add(assembly);
                        }
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load Deployment.Current.Parts");
            }

#if SL5
            // Add the loaded xap cache
            assemblies.AddRange(_externalAssemblies);
#endif
#endif

            // Make sure to prevent duplicates
            assemblies = assemblies.Distinct().ToList();

            // Map all assemblies
            foreach (var assembly in assemblies)
            {
                RegisterAssemblyWithVersionInfo(assembly);
            }

            return assemblies;
        }

        private static void RegisterAssemblyWithVersionInfo(Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            lock (_lockObject)
            {
                try
                {
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

#if NET
        /// <summary>
        /// Gets the linker timestamp.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>DateTime.</returns>
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
                return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond));
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }

#pragma warning disable 169
#pragma warning disable 649
        struct _IMAGE_FILE_HEADER
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
