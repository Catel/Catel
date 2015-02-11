﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCache.cs" company="Catel development team">
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
    using System.Threading;
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

#if NET
        private static readonly Queue<Assembly> _threadSafeAssemblyQueue = new Queue<Assembly>();

        /// <summary>
        /// Assemblies, loaded while Catel was processing AssemblyLoaded event.
        /// </summary>
        private static readonly Queue<Assembly> _onAssemblyLoadedDelayQueue = new Queue<Assembly>();

        /// <summary>
        /// The boolean specifying whether the type cache is already loading assemblies via the loaded event.
        /// </summary>
        private static bool _isAlreadyInLoadingEvent = false;
#endif

        /// <summary>
        /// Cache containing all the types implementing a specific interface.
        /// </summary>
        private static readonly Dictionary<Type, HashSet<Type>> _typesByInterface = new Dictionary<Type, HashSet<Type>>();

        /// <summary>
        /// Cache containing all the types by assembly. This means that the first dictionary contains the assembly name
        /// and all types contained by that assembly.
        /// </summary>
        private static Dictionary<string, Dictionary<string, Type>> _typesByAssembly;

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
        /// The list of loaded assemblies which do not required additional initialization again.
        /// <para />
        /// This is required because the AppDomain.AssemblyLoad might be called several times for the same AppDomain
        /// </summary>
        private static readonly HashSet<string> _loadedAssemblies = new HashSet<string>();

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object _lockObject = new object();

        static TypeCache()
        {
            ShouldIgnoreAssemblyEvaluators = new List<Func<Assembly, bool>>();
            ShouldIgnoreTypeEvaluators = new List<Func<Assembly, Type, bool>>();

#if NET
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

            // Initialize the types of early loaded assemblies
            lock (_lockObject)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    InitializeTypes(assembly);
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
            if (ShouldIgnoreAssembly(assembly))
            {
                Log.Debug("Reflection '{0}' is on the list to be ignored (for example, ReflectionOnly is true), cannot use this assembly", assembly.FullName);
                return;
            }

            // Prevent deadlocks by checking whether this assembly might be loaded from a different thread:
            // 1) 
            if (Monitor.TryEnter(_lockObject))
            {
                var assemblyName = assembly.FullName;
                if (!_loadedAssemblies.Contains(assemblyName))
                {
                    // Fix for CTL-543
                    // General idea of fix - prevent to call GetTypes() method recursively.
                    // When type load will fail CLR will try to localize message, and on
                    // some OS's (i suspect on non english Windows and .NET) will try to load
                    // satellite assembly with localization, Catel will get event before CLR
                    // finishes handling process. Catel will try to initialize types. When another
                    // type won't load CLR will detect that it still trying to handle previous 
                    // type load problem and will crash whole process.

                    if (_isAlreadyInLoadingEvent)
                    {
                        // Will be proceed in finally block
                        _onAssemblyLoadedDelayQueue.Enqueue(assembly);
                    }
                    else
                    {
                        try
                        {
                            _isAlreadyInLoadingEvent = true;

                            InitializeTypes(assembly);
                        }
                        finally
                        {
                            while (_onAssemblyLoadedDelayQueue.Count > 0)
                            {
                                var delayedAssembly = _onAssemblyLoadedDelayQueue.Dequeue();

                                // Copy/pasted assembly processing behaviour, like types were processed without any delay
                                InitializeTypes(delayedAssembly);
                            }

                            _isAlreadyInLoadingEvent = false;
                        }
                    }
                }

                Monitor.Exit(_lockObject);
            }
            else
            {
                lock (_threadSafeAssemblyQueue)
                {
                    _threadSafeAssemblyQueue.Enqueue(assembly);
                }
            }

            // Important to do outside of the lock
            var handler = AssemblyLoaded;
            if (handler != null)
            {
                var types = GetTypesOfAssembly(assembly);
                var eventArgs = new AssemblyLoadedEventArgs(assembly, types);

                handler(null, eventArgs);
            }
        }
#endif

        /// <summary>
        /// Gets the evaluators used to determine whether a specific assembly should be ignored.
        /// </summary>
        /// <value>The should ignore assembly function.</value>
        public static List<Func<Assembly, bool>> ShouldIgnoreAssemblyEvaluators { get; private set; }

        /// <summary>
        /// Gets the evaluators used to determine whether a specific type should be ignored.
        /// </summary>
        /// <value>The should ignore assembly function.</value>
        public static List<Func<Assembly, Type, bool>> ShouldIgnoreTypeEvaluators { get; private set; }

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

            InitializeTypes();

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

                    var fallbackType = GetTypeBySplittingInternals(typeName);
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
        /// Gets the type by splitting internal types. This means that System.Collections.List`1[[MyCustomType.Item]] will be splitted
        /// and resolved separately.
        /// </summary>
        /// <param name="typeWithInnerTypes">The type with inner types.</param>
        /// <returns></returns>
        private static Type GetTypeBySplittingInternals(string typeWithInnerTypes)
        {
            // Try fast method first
            var fastType = Type.GetType(typeWithInnerTypes);
            if (fastType != null)
            {
                return fastType;
            }

            if (typeWithInnerTypes.EndsWith("[]"))
            {
                // Array type
                var arrayTypeElementString = typeWithInnerTypes.Replace("[]", string.Empty);
                var arrayTypeElement = TypeCache.GetType(arrayTypeElementString);
                if (arrayTypeElement != null)
                {
                    return arrayTypeElement.MakeArrayType();
                }

                return null;
            }

            var innerTypes = new List<Type>();
            var innerTypesShortNames = TypeHelper.GetInnerTypes(typeWithInnerTypes);
            if (innerTypesShortNames.Length > 0)
            {
                foreach (var innerTypesShortName in innerTypesShortNames)
                {
                    var innerType = TypeCache.GetType(innerTypesShortName);
                    if (innerType == null)
                    {
                        return null;
                    }

                    innerTypes.Add(innerType);
                }

                var innerTypesNames = new List<string>();
                foreach (var innerType in innerTypes)
                {
                    innerTypesNames.Add(innerType.AssemblyQualifiedName);
                }

                var firstBracketIndex = typeWithInnerTypes.IndexOf('[');
                var typeWithImprovedInnerTypes = string.Format("{0}[{1}]", typeWithInnerTypes.Substring(0, firstBracketIndex), TypeHelper.FormatInnerTypes(innerTypesNames.ToArray()));

                var fallbackType = Type.GetType(typeWithImprovedInnerTypes);
                return fallbackType;
            }

            // This is not yet supported or type is really not available
            return null;
        }

        /// <summary>
        /// Gets the types implementing the specified interface.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetTypesImplementingInterface(Type interfaceType)
        {
            Argument.IsNotNull("interfaceType", interfaceType);

            lock (_lockObject)
            {
                if (!_typesByInterface.ContainsKey(interfaceType))
                {
                    _typesByInterface[interfaceType] = new HashSet<Type>(GetTypes(interfaceType.ImplementsInterfaceEx));
                }

                return _typesByInterface[interfaceType].ToArray();
            }
        }

        /// <summary>
        /// Gets the types of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="predicate">The predicate to use on the types.</param>
        /// <returns>All types of the specified assembly.</returns>
        public static Type[] GetTypesOfAssembly(Assembly assembly, Func<Type, bool> predicate = null)
        {
            Argument.IsNotNull("assembly", assembly);

            var assemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(assembly.FullName);
            return GetTypesPrefilteredByAssembly(assemblyName, predicate);
        }

        /// <summary>
        /// Gets all the types from the current <see cref="AppDomain"/> where the <paramref name="predicate"/> returns true.
        /// </summary>
        /// <param name="predicate">The predicate where the type should apply to.</param>
        /// <returns>An array containing all the <see cref="Type"/> that match the predicate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public static Type[] GetTypes(Func<Type, bool> predicate = null)
        {
            return GetTypesPrefilteredByAssembly(null, predicate);
        }

        /// <summary>
        /// Gets the types prefiltered by assembly. If types must be retrieved from a single assembly only, this method is very fast.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Type[].</returns>
        private static Type[] GetTypesPrefilteredByAssembly(string assemblyName, Func<Type, bool> predicate)
        {
            InitializeTypes();

            lock (_lockObject)
            {
                Dictionary<string, Type> typeSource = null;

                if (!string.IsNullOrWhiteSpace(assemblyName))
                {
                    if (_typesByAssembly.ContainsKey(assemblyName))
                    {
                        typeSource = _typesByAssembly[assemblyName];
                    }
                }
                else
                {
                    typeSource = _typesWithAssembly;
                }

                if (typeSource == null)
                {
                    return new Type[] { };
                }

                int retryCount = 3;
                while (retryCount > 0)
                {
                    retryCount--;

                    try
                    {
                        if (predicate != null)
                        {
                            return typeSource.Values.Where(predicate).ToArray();
                        }

                        return typeSource.Values.ToArray();
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
                            InitializeTypes(assembly, forceFullInitialization);
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
        [ObsoleteEx(Replacement = "InitializeTypes(Assembly, bool)", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        public static void InitializeTypes(bool forceFullInitialization, Assembly assembly = null)
        {
            InitializeTypes(assembly, forceFullInitialization);
        }

        /// <summary>
        /// Initializes the types in the specified assembly. It does this by looping through all loaded assemblies and
        /// registering the type by type name and assembly name.
        /// <para/>
        /// The types initialized by this method are used by <see cref="object.GetType"/>.
        /// </summary>
        /// <param name="assembly">The assembly to initialize the types from. If <c>null</c>, all assemblies will be checked.</param>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        public static void InitializeTypes(Assembly assembly = null, bool forceFullInitialization = false)
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
                    _loadedAssemblies.Clear();

                    if (_typesByAssembly != null)
                    {
                        _typesByAssembly.Clear();
                        _typesByAssembly = null;
                    }

                    if (_typesWithAssembly != null)
                    {
                        _typesWithAssembly.Clear();
                        _typesWithAssembly = null;
                    }

                    if (_typesWithAssemblyLowerCase != null)
                    {
                        _typesWithAssemblyLowerCase.Clear();
                        _typesWithAssemblyLowerCase = null;
                    }

                    if (_typesWithoutAssembly != null)
                    {
                        _typesWithoutAssembly.Clear();
                        _typesWithoutAssembly = null;
                    }

                    if (_typesWithoutAssemblyLowerCase != null)
                    {
                        _typesWithoutAssemblyLowerCase.Clear();
                        _typesWithoutAssemblyLowerCase = null;
                    }
                }

                if (_typesByAssembly == null)
                {
                    _typesByAssembly = new Dictionary<string, Dictionary<string, Type>>();
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

                var assembliesToInitialize = checkSingleAssemblyOnly ? new List<Assembly>(new[] {assembly}) : AssemblyHelper.GetLoadedAssemblies();
                InitializeAssemblies(assembliesToInitialize);
            }
        }

        private static void InitializeAssemblies(IEnumerable<Assembly> assemblies)
        {
            lock (_lockObject)
            {
                var typesToAdd = new Dictionary<Assembly, HashSet<Type>>();

                foreach (var assembly in assemblies)
                {
                    var loadedAssemblyFullName = assembly.FullName;

                    try
                    {
                        if (_loadedAssemblies.Contains(loadedAssemblyFullName))
                        {
                            continue;
                        }

                        _loadedAssemblies.Add(loadedAssemblyFullName);

                        if (ShouldIgnoreAssembly(assembly))
                        {
                            continue;
                        }

                        typesToAdd[assembly] = new HashSet<Type>();

                        foreach (var type in AssemblyHelper.GetAllTypesSafely(assembly))
                        {
                            typesToAdd[assembly].Add(type);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to get all types in assembly '{0}'", loadedAssemblyFullName);
                    }
                }

                foreach (var assemblyWithTypes in typesToAdd)
                {
                    foreach (var type in assemblyWithTypes.Value)
                    {
                        InitializeType(assemblyWithTypes.Key, type);
                    }
                }

#if NET
                var lateLoadedAssemblies = new List<Assembly>();

                lock (_threadSafeAssemblyQueue)
                {
                    while (_threadSafeAssemblyQueue.Count > 0)
                    {
                        var assembly = _threadSafeAssemblyQueue.Dequeue();

                        lateLoadedAssemblies.Add(assembly);
                    }
                }

                if (lateLoadedAssemblies.Count > 0)
                {
                    InitializeAssemblies(lateLoadedAssemblies);
                }
#endif
            }
        }

        private static void InitializeType(Assembly assembly, Type type)
        {
            if (ShouldIgnoreType(assembly, type))
            {
                return;
            }

            var newAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(assembly.FullName);
            string newFullType = TypeHelper.FormatType(newAssemblyName, type.FullName);

            if (!_typesByAssembly.ContainsKey(newAssemblyName))
            {
                _typesByAssembly[newAssemblyName] = new Dictionary<string, Type>();
            }

            var typesByAssembly = _typesByAssembly[newAssemblyName];
            if (!typesByAssembly.ContainsKey(newFullType))
            {
                typesByAssembly[newFullType] = type;

                _typesWithAssembly[newFullType] = type;
                _typesWithAssemblyLowerCase[newFullType.ToLowerInvariant()] = type;

                var typeNameWithoutAssembly = TypeHelper.GetTypeName(newFullType);
                _typesWithoutAssembly[typeNameWithoutAssembly] = newFullType;
                _typesWithoutAssemblyLowerCase[typeNameWithoutAssembly.ToLowerInvariant()] = newFullType.ToLowerInvariant();

                //var interfaces = type.GetInterfacesEx();
                //foreach (var iface in interfaces)
                //{
                //    if (!_typesByInterface.ContainsKey(iface))
                //    {
                //        _typesByInterface.Add(iface, new HashSet<Type>());
                //    }

                //    _typesByInterface[iface].Add(type);
                //}
            }
        }

        /// <summary>
        /// Determines whether the specified assembly must be ignored by the type cache.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the assembly should be ignored, <c>false</c> otherwise.</returns>
        private static bool ShouldIgnoreAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return true;
            }

#if NET
            if (assembly.ReflectionOnly)
            {
                return true;
            }
#endif

            var assemblyFullName = assembly.FullName;
            if (assemblyFullName.Contains("Anonymously Hosted DynamicMethods Assembly"))
            {
                return true;
            }

            foreach (var evaluator in ShouldIgnoreAssemblyEvaluators)
            {
                if (evaluator.Invoke(assembly))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type must be ignored by the type cache.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type should be ignored, <c>false</c> otherwise.</returns>
        private static bool ShouldIgnoreType(Assembly assembly, Type type)
        {
            if (type == null)
            {
                return true;
            }

            var typeName = type.FullName;

            // Ignore useless types
            if (typeName.Contains("<PrivateImplementationDetails>") ||
                typeName.Contains("c__DisplayClass") ||
                typeName.Contains("d__") ||
                typeName.Contains("f__AnonymousType") ||
                typeName.Contains("o__") ||
                typeName.Contains("__DynamicallyInvokableAttribute") ||
                typeName.Contains("ProcessedByFody") ||
                typeName.Contains("FXAssembly") ||
                typeName.Contains("ThisAssembly") ||
                typeName.Contains("AssemblyRef") ||
                typeName.Contains("MS.Internal") ||
                typeName.Contains("::") ||
                typeName.Contains("\\*") ||
                typeName.Contains("_extraBytes_") ||
                typeName.Contains("CppImplementationDetails"))
            {
                return true;
            }

            // Ignore types that might cause a security exception
            if (typeName.Contains("System.Data.Metadata.Edm.") ||
                typeName.Contains("System.Data.EntityModel.SchemaObjectModel."))
            {
                return true;
            }

            foreach (var evaluator in ShouldIgnoreTypeEvaluators)
            {
                if (evaluator.Invoke(assembly, type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}