// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCache.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;

    /// <summary>
    /// Cache containing the types of an appdomain.
    /// </summary>
    public static class TypeCache
    {
        /// <summary>
        ///   The <see cref = "ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Cache containing all the types based on a string. This way, it is easy to retrieve a type based on a 
        /// string containing the type name and assembly without the overhead, such as <c>Catel.TypeHelper, Catel.Core</c>.
        /// </summary>
        private static Dictionary<string, Type> _typesWithAssembly;

        /// <summary>
        /// Cache containing all the types based on a string. This way, it is easy to retrieve a type based on a 
        /// string containing the type name and assembly without the overhead, such as <c>Catel.TypeHelper, Catel.Core</c>.
        /// </summary>
        private static Dictionary<string, Type> _typesWithAssemblyLowerCase;

        /// <summary>
        /// Cache containing all the types based without an assembly. This means that a type with this format:
        /// <c>Catel.TypeHelper, Catel.Core</c> will be located as <c>Catel.TypeHelper</c>.
        /// <para />
        /// The values resolved from this dictionary can be used as key in the <see cref="_typesWithAssembly"/> dictionary.
        /// </summary>
        private static Dictionary<string, string> _typesWithoutAssembly;

        /// <summary>
        /// Cache containing all the types based without an assembly. This means that a type with this format:
        /// <c>Catel.TypeHelper, Catel.Core</c> will be located as <c>Catel.TypeHelper</c>.
        /// <para />
        /// The values resolved from this dictionary can be used as key in the <see cref="_typesWithAssembly"/> dictionary.
        /// </summary>
        private static Dictionary<string, string> _typesWithoutAssemblyLowerCase;

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object _lockObject = new object();

        static TypeCache()
        {
#if NET
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

            // Initialize the types of early loaded assemblies.
            lock (_lockObject)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    InitializeTypes(false, assembly);
                }
            }
#endif
        }

#if NET
        /// <summary>
        /// Called when an assembly is loaded in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AssemblyLoadEventArgs" /> instance containing the event data.</param>
        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;

            if (assembly.ReflectionOnly)
            {
                Log.Debug("Reflection '{0}' is loaded for reflection-only, cannot use this assembly", assembly.FullName);
                return;
            }

            InitializeTypes(false, assembly);

            var handler = AssemblyLoaded;
            if (handler != null)
            {
                var types = GetTypesOfAssembly(assembly);
                var eventArgs = new AssemblyLoadedEventArgs(assembly, types);

                handler(null, eventArgs);
            }
        }
#endif

        #region Events
        /// <summary>
        /// Occurs when an assembly is loaded into the currently <see cref="AppDomain"/>.
        /// </summary>
#pragma warning disable 67
        public static event EventHandler<AssemblyLoadedEventArgs> AssemblyLoaded;
#pragma warning restore 67
        #endregion

        /// <summary>
        /// Gets the specified type from the loaded assemblies. This is a great way to load types without having
        /// to know the exact version in Silverlight.
        /// </summary>
        /// <param name="typeName">The name of the type including namespace.</param>
        /// <param name="assemblyName">The name of the type including namespace.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        public static Type GetTypeWithAssembly(string typeName, string assemblyName, bool ignoreCase = false)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);
            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);

            return GetType(typeName, assemblyName, ignoreCase);
        }

        /// <summary>
        /// Gets the type without assembly. For example, when the value <c>Catel.TypeHelper</c> is used as parameter, the type for
        /// <c>Catel.TypeHelper, Catel.Core</c> will be returned.
        /// </summary>
        /// <param name="typeNameWithoutAssembly">The type name without assembly.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <remarks>
        /// Note that this method can only support one type of "simple type name" resolving. For example, if "Catel.TypeHelper" is located in
        /// multiple assemblies, it will always use the latest known type for resolving the type.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="typeNameWithoutAssembly"/> is <c>null</c> or whitespace.</exception>
        public static Type GetTypeWithoutAssembly(string typeNameWithoutAssembly, bool ignoreCase = false)
        {
            Argument.IsNotNullOrWhitespace("typeNameWithoutAssembly", typeNameWithoutAssembly);

            return GetType(typeNameWithoutAssembly, null, ignoreCase);
        }

        /// <summary>
        /// Gets the specified type from the loaded assemblies. This is a great way to load types without having
        /// to know the exact version in Silverlight.
        /// </summary>
        /// <param name="typeNameWithAssembly">The name of the type including namespace and assembly, formatted with the <see cref="TypeHelper.FormatType"/> method.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeNameWithAssembly"/> is <c>null</c> or whitespace.</exception>
        public static Type GetType(string typeNameWithAssembly, bool ignoreCase = false)
        {
            Argument.IsNotNullOrWhitespace("typeNameWithAssembly", typeNameWithAssembly);

            var typeName = TypeHelper.GetTypeName(typeNameWithAssembly);
            var assemblyName = TypeHelper.GetAssemblyName(typeNameWithAssembly);

            return GetType(typeName, assemblyName, ignoreCase);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly. Can be <c>null</c> if no assembly is known.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        private static Type GetType(string typeName, string assemblyName, bool ignoreCase)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);

            InitializeTypes(false);

            lock (_lockObject)
            {
                var typesWithoutAssembly = ignoreCase ? _typesWithoutAssemblyLowerCase : _typesWithoutAssembly;
                var typesWithAssembly = ignoreCase ? _typesWithAssemblyLowerCase : _typesWithAssembly;

                if (ignoreCase)
                {
                    typeName = typeName.ToLowerInvariant();
                }

                var typeNameWithAssembly = string.IsNullOrEmpty(assemblyName) ? null : TypeHelper.FormatType(assemblyName, typeName);
                if (typeNameWithAssembly == null)
                {
                    if (typesWithoutAssembly.ContainsKey(typeName))
                    {
                        return typesWithAssembly[typesWithoutAssembly[typeName]];
                    }

                    // Note that lazy-loaded types (a few lines below) are added to the types *with* assemblies so we have
                    // a direct access cache
                    if (typesWithAssembly.ContainsKey(typeName))
                    {
                        return typesWithAssembly[typeName];
                    }

                    var fallbackType = Type.GetType(typeName);
                    if (fallbackType != null)
                    {
                        // Though it was not initially found, we still have found a new type, register it
                        typesWithAssembly[typeName] = fallbackType;
                    }

                    return fallbackType;
                }

                if (typesWithAssembly.ContainsKey(typeNameWithAssembly))
                {
                    return typesWithAssembly[typeNameWithAssembly];
                }

                // Try to remove version info from assembly info
                var assemblyNameWithoutOverhead = TypeHelper.GetAssemblyNameWithoutOverhead(assemblyName);
                var typeNameWithoutAssemblyOverhead = TypeHelper.FormatType(assemblyNameWithoutOverhead, typeName);
                if (typesWithAssembly.ContainsKey(typeNameWithoutAssemblyOverhead))
                {
                    return typesWithAssembly[typeNameWithoutAssemblyOverhead];
                }

                // Fallback to GetType
                try
                {
#if NETFX_CORE || PCL
                    var type = Type.GetType(typeNameWithAssembly, false);
#elif SILVERLIGHT
                    // Due to a FileLoadException when loading types without a specific version, we need to map the assembly version here
                    var assemblyNameWithVersion = AssemblyHelper.GetAssemblyNameWithVersion(assemblyName);
                    var typeNameWithAssemblyNameWithVersion = TypeHelper.FormatType(assemblyNameWithVersion, typeName);
                    var type = Type.GetType(typeNameWithAssemblyNameWithVersion, false, ignoreCase);
#else
                    var type = Type.GetType(typeNameWithAssembly, false, ignoreCase);
#endif
                    if (type != null)
                    {
                        typesWithAssembly.Add(typeNameWithAssembly, type);
                        return type;
                    }
                }
#if !NETFX_CORE && !PCL
                catch (System.IO.FileLoadException fle)
                {
                    Log.Debug(fle, "Failed to load type '{0}' using Type.GetType(), failed to load file", typeNameWithAssembly);
                }
#endif
                catch (Exception ex)
                {
                    Log.Debug(ex, "Failed to load type '{0}' using Type.GetType()", typeNameWithAssembly);
                }

                // Fallback for this assembly only
                InitializeTypes(false, assemblyName);

                if (typesWithAssembly.ContainsKey(typeNameWithAssembly))
                {
                    return typesWithAssembly[typeNameWithAssembly];
                }

                if (typesWithAssembly.ContainsKey(typeNameWithoutAssemblyOverhead))
                {
                    return typesWithAssembly[typeNameWithoutAssemblyOverhead];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the types of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>All types of the specified assembly.</returns>
        public static Type[] GetTypesOfAssembly(Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            return GetTypes(x => x.GetAssemblyEx().Equals(assembly));
        }

        /// <summary>
        /// Gets all the types from the current <see cref="AppDomain"/>.
        /// </summary>
        /// <returns>An array containing all the <see cref="Type"/>.</returns>
        public static Type[] GetTypes()
        {
            return GetTypes(t => true);
        }

        /// <summary>
        /// Gets all the types from the current <see cref="AppDomain"/> where the <paramref name="predicate"/> returns true.
        /// </summary>
        /// <param name="predicate">The predicate where the type should apply to.</param>
        /// <returns>An array containing all the <see cref="Type"/> that match the predicate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public static Type[] GetTypes(Func<Type, bool> predicate)
        {
            Argument.IsNotNull("predicate", predicate);

            InitializeTypes(false);

            lock (_lockObject)
            {
                int retryCount = 3;
                while (retryCount > 0)
                {
                    retryCount--;

                    try
                    {
                        var values = _typesWithAssembly.Values.ToList();
                        return values.Where(predicate).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to get types, '{0}' retries left", retryCount);
                    }
                }

                return new Type[] { };
            }
        }

        /// <summary>
        /// Initializes the types in Silverlight. It does this by looping through all loaded assemblies and
        /// registering the type by type name and assembly name.
        /// <para/>
        /// The types initialized by this method are used by <see cref="object.GetType"/>.
        /// </summary>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        /// <param name="assemblyName">Name of the assembly. If <c>null</c>, all assemblies will be checked.</param>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        public static void InitializeTypes(bool forceFullInitialization, string assemblyName)
        {
            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);

            lock (_lockObject)
            {
                foreach (var assembly in AssemblyHelper.GetLoadedAssemblies())
                {
                    try
                    {
                        if (assembly.FullName.Contains(assemblyName))
                        {
                            InitializeTypes(forceFullInitialization, assembly);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to get all types in assembly '{0}'", assembly.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the types in the specified assembly. It does this by looping through all loaded assemblies and
        /// registering the type by type name and assembly name.
        /// <para/>
        /// The types initialized by this method are used by <see cref="object.GetType"/>.
        /// </summary>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        /// <param name="assembly">The assembly to initialize the types from. If <c>null</c>, all assemblies will be checked.</param>
        public static void InitializeTypes(bool forceFullInitialization, Assembly assembly = null)
        {
            bool checkSingleAssemblyOnly = assembly != null;

            if (!forceFullInitialization && !checkSingleAssemblyOnly && (_typesWithAssembly != null))
            {
                return;
            }

            lock (_lockObject)
            {
                if (forceFullInitialization)
                {
                    _typesWithAssembly.Clear();
                    _typesWithAssembly = null;

                    _typesWithAssemblyLowerCase.Clear();
                    _typesWithAssemblyLowerCase = null;

                    _typesWithoutAssembly.Clear();
                    _typesWithoutAssembly = null;

                    _typesWithoutAssemblyLowerCase.Clear();
                    _typesWithoutAssemblyLowerCase = null;
                }

                if (_typesWithAssembly == null)
                {
                    _typesWithAssembly = new Dictionary<string, Type>();
                }

                if (_typesWithAssemblyLowerCase == null)
                {
                    _typesWithAssemblyLowerCase = new Dictionary<string, Type>();
                }

                if (_typesWithoutAssembly == null)
                {
                    _typesWithoutAssembly = new Dictionary<string, string>();
                }

                if (_typesWithoutAssemblyLowerCase == null)
                {
                    _typesWithoutAssemblyLowerCase = new Dictionary<string, string>();
                }

                var typesToAdd = new HashSet<Type>();

                var assembliesToLoad = new List<Assembly>();
                if (assembly != null)
                {
                    assembliesToLoad.Add(assembly);
                }
                else
                {
                    assembliesToLoad.AddRange(AssemblyHelper.GetLoadedAssemblies());
                }

                foreach (var loadedAssembly in assembliesToLoad)
                {
                    try
                    {
                        foreach (var type in AssemblyHelper.GetAllTypesSafely(loadedAssembly))
                        {
                            typesToAdd.Add(type);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to get all types in assembly '{0}'", loadedAssembly.FullName);
                    }
                }

                foreach (var type in typesToAdd)
                {
                    if (ShouldIgnoreType(type))
                    {
                        continue;
                    }

                    var newAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(type.GetAssemblyFullNameEx());
                    string newFullType = TypeHelper.FormatType(newAssemblyName, type.FullName);
                    if (!_typesWithAssembly.ContainsKey(newFullType))
                    {
                        _typesWithAssembly[newFullType] = type;
                        _typesWithAssemblyLowerCase[newFullType.ToLowerInvariant()] = type;

                        var typeNameWithoutAssembly = TypeHelper.GetTypeName(newFullType);
                        _typesWithoutAssembly[typeNameWithoutAssembly] = newFullType;
                        _typesWithoutAssemblyLowerCase[typeNameWithoutAssembly.ToLowerInvariant()] = newFullType.ToLowerInvariant();
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified type must be ignored by the type cache.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type should be ignored, <c>false</c> otherwise.</returns>
        private static bool ShouldIgnoreType(Type type)
        {
            if (type == null)
            {
                return true;
            }

            var typeName = type.FullName;

            // Ignore useless types
            if (typeName.Contains("<PrivateImplementationDetails>") ||
                typeName.Contains("<>f__AnonymousType") ||
                typeName.Contains("__DynamicallyInvokableAttribute") ||
                typeName.Contains("ProcessedByFody") ||
                typeName.Contains("FXAssembly") ||
                typeName.Contains("ThisAssembly") ||
                typeName.Contains("AssemblyRef"))
            {
                return true;
            }

            // Ignore types that might cause a security exception
            if (typeName.Contains("System.Data.Metadata.Edm.") ||
                typeName.Contains("System.Data.EntityModel.SchemaObjectModel."))
            {
                return true;
            }

            return false;
        }
    }
}