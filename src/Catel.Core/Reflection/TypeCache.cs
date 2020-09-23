// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCache.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Logging;
    using MethodTimer;
    using Threading;

    /// <summary>
    /// Cache containing the types of an appdomain.
    /// </summary>
    public static class TypeCache
    {
        private const int DefaultCollectionSizeForTypes = 10 * 1000;
        private const int DefaultCollectionSizeForAssemblies = 50;

        /// <summary>
        ///   The <see cref = "ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NET || NETCORE || NETSTANDARD
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
        private static readonly Dictionary<Type, Type[]> _typesByInterface = new Dictionary<Type, Type[]>(500);

        /// <summary>
        /// Cache containing all the types by assembly. This means that the first dictionary contains the assembly name
        /// and all types contained by that assembly.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, Type>> _typesByAssembly = new Dictionary<string, Dictionary<string, Type>>(DefaultCollectionSizeForAssemblies);

        /// <summary>
        /// Cache containing all the types based on a string. This way, it is easy to retrieve a type based on a 
        /// string containing the type name and assembly without the overhead, such as <c>Catel.TypeHelper, Catel.Core</c>.
        /// </summary>
        private static readonly Dictionary<string, Type> _typesWithAssembly = new Dictionary<string, Type>(DefaultCollectionSizeForTypes, StringComparer.Ordinal);

        /// <summary>
        /// Cache containing all the types based on a string. This way, it is easy to retrieve a type based on a 
        /// string containing the type name and assembly without the overhead, such as <c>Catel.TypeHelper, Catel.Core</c>.
        /// </summary>
        private static readonly Dictionary<string, Type> _typesWithAssemblyIgnoreCase = new Dictionary<string, Type>(DefaultCollectionSizeForTypes, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Cache containing all the types based without an assembly. This means that a type with this format:
        /// <c>Catel.TypeHelper, Catel.Core</c> will be located as <c>Catel.TypeHelper</c>.
        /// <para />
        /// The values resolved from this dictionary can be used as key in the <see cref="_typesWithAssembly"/> dictionary.
        /// </summary>
        private static readonly Dictionary<string, string> _typesWithoutAssembly = new Dictionary<string, string>(DefaultCollectionSizeForTypes, StringComparer.Ordinal);

        /// <summary>
        /// Cache containing all the types based without an assembly. This means that a type with this format:
        /// <c>Catel.TypeHelper, Catel.Core</c> will be located as <c>Catel.TypeHelper</c>.
        /// <para />
        /// The values resolved from this dictionary can be used as key in the <see cref="_typesWithAssembly"/> dictionary.
        /// </summary>
        private static readonly Dictionary<string, string> _typesWithoutAssemblyIgnoreCase = new Dictionary<string, string>(DefaultCollectionSizeForTypes, StringComparer.OrdinalIgnoreCase);

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

        private static bool _hasInitializedOnce;

        static TypeCache()
        {
#if NET || NETCORE || NETSTANDARD
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
#endif
        }

        #region Properties
        /// <summary>
        /// Gets the names of the assemblies initialized by the TypeCache.
        /// </summary>
        /// <value>
        /// The initialized assemblies.
        /// </value>
        public static List<string> InitializedAssemblies
        {
            get
            {
                lock (_loadedAssemblies)
                {
                    return _loadedAssemblies.ToList();
                }
            }
        }
        #endregion

#if NET || NETCORE || NETSTANDARD
        /// <summary>
        /// Called when an assembly is loaded in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AssemblyLoadEventArgs" /> instance containing the event data.</param>
#if DEBUG
        [Time("{args}")]
#endif
        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            if (ShouldIgnoreAssembly(assembly))
            {
                Log.Debug($"Reflection '{assembly.FullName}' is on the list to be ignored (for example, ReflectionOnly is true), cannot use this assembly");
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

                // Important to do outside of the lock
                var handler = AssemblyLoaded;
                if (handler != null)
                {
                    var eventArgs = new AssemblyLoadedEventArgs(assembly, new Lazy<IEnumerable<Type>>(() => GetTypesOfAssembly(assembly)));

                    handler(null, eventArgs);
                }
            }
            else
            {
                lock (_threadSafeAssemblyQueue)
                {
                    _threadSafeAssemblyQueue.Enqueue(assembly);
                }

                // Note: AssemblyLoaded will be invoked later when the assembly
                // is actually loaded, fixed for https://github.com/Catel/Catel/issues/1120
            }
        }
#endif

        /// <summary>
        /// Gets the evaluators used to determine whether a specific assembly should be ignored.
        /// </summary>
        /// <value>The should ignore assembly function.</value>
        public static List<Func<Assembly, bool>> ShouldIgnoreAssemblyEvaluators
        {
            get
            {
                return TypeCacheEvaluator.AssemblyEvaluators;
            }
        }

        /// <summary>
        /// Gets the evaluators used to determine whether a specific type should be ignored.
        /// </summary>
        /// <value>The should ignore assembly function.</value>
        public static List<Func<Assembly, Type, bool>> ShouldIgnoreTypeEvaluators
        {
            get
            {
                return TypeCacheEvaluator.TypeEvaluators;
            }
        }

        #region Events
        /// <summary>
        /// Occurs when an assembly is loaded into the currently <see cref="AppDomain"/>.
        /// </summary>
#pragma warning disable 67
        public static event EventHandler<AssemblyLoadedEventArgs> AssemblyLoaded;
#pragma warning restore 67
        #endregion

        /// <summary>
        /// Gets the specified type from the loaded assemblies.
        /// </summary>
        /// <param name="typeName">The name of the type including namespace.</param>
        /// <param name="assemblyName">The name of the type including namespace.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <param name="allowInitialization">If set to <c>true</c>, allow initialization of the AppDomain if it hasn't happened yet. If <c>false</c>, deal with the types currently in the cache.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        public static Type GetTypeWithAssembly(string typeName, string assemblyName, bool ignoreCase = false, bool allowInitialization = true)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);
            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);

            return GetType(typeName, assemblyName, ignoreCase, allowInitialization);
        }

        /// <summary>
        /// Gets the type without assembly. For example, when the value <c>Catel.TypeHelper</c> is used as parameter, the type for
        /// <c>Catel.TypeHelper, Catel.Core</c> will be returned.
        /// </summary>
        /// <param name="typeNameWithoutAssembly">The type name without assembly.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <param name="allowInitialization">If set to <c>true</c>, allow initialization of the AppDomain if it hasn't happened yet. If <c>false</c>, deal with the types currently in the cache.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <remarks>
        /// Note that this method can only support one type of "simple type name" resolving. For example, if "Catel.TypeHelper" is located in
        /// multiple assemblies, it will always use the latest known type for resolving the type.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="typeNameWithoutAssembly"/> is <c>null</c> or whitespace.</exception>
        public static Type GetTypeWithoutAssembly(string typeNameWithoutAssembly, bool ignoreCase = false, bool allowInitialization = true)
        {
            Argument.IsNotNullOrWhitespace("typeNameWithoutAssembly", typeNameWithoutAssembly);

            return GetType(typeNameWithoutAssembly, null, ignoreCase, allowInitialization);
        }

        /// <summary>
        /// Gets the specified type from the loaded assemblies.
        /// </summary>
        /// <param name="typeNameWithAssembly">The name of the type including namespace and assembly, formatted with the <see cref="TypeHelper.FormatType"/> method.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <param name="allowInitialization">If set to <c>true</c>, allow initialization of the AppDomain if it hasn't happened yet. If <c>false</c>, deal with the types currently in the cache.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeNameWithAssembly"/> is <c>null</c> or whitespace.</exception>
        public static Type GetType(string typeNameWithAssembly, bool ignoreCase = false, bool allowInitialization = true)
        {
            Argument.IsNotNullOrWhitespace("typeNameWithAssembly", typeNameWithAssembly);

            var typeName = TypeHelper.GetTypeName(typeNameWithAssembly);
            var assemblyName = TypeHelper.GetAssemblyName(typeNameWithAssembly);

            return GetType(typeName, assemblyName, ignoreCase, allowInitialization);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly. Can be <c>null</c> if no assembly is known.</param>
        /// <param name="ignoreCase">A value indicating whether the case should be ignored.</param>
        /// <param name="allowInitialization">If set to <c>true</c>, allow initialization of the AppDomain if it hasn't happened yet. If <c>false</c>, deal with the types currently in the cache.</param>
        /// <returns>The <see cref="Type"/> or <c>null</c> if the type cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        private static Type GetType(string typeName, string assemblyName, bool ignoreCase, bool allowInitialization = true)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);

            if (allowInitialization)
            {
                InitializeTypes();
            }

            lock (_lockObject)
            {
                var typesWithoutAssembly = ignoreCase ? _typesWithoutAssemblyIgnoreCase : _typesWithoutAssembly;
                var typesWithAssembly = ignoreCase ? _typesWithAssemblyIgnoreCase : _typesWithAssembly;

                var typeNameWithAssembly = string.IsNullOrEmpty(assemblyName) ? null : TypeHelper.FormatType(assemblyName, typeName);
                if (typeNameWithAssembly is null)
                {
                    // If we have a mapping, use that instead
                    if (typesWithoutAssembly.TryGetValue(typeName, out var typeNameMapping))
                    {
                        typeName = typeNameMapping;
                    }

                    // Note that lazy-loaded types (a few lines below) are added to the types *with* assemblies so we have
                    // a direct access cache
                    if (typesWithAssembly.TryGetValue(typeName, out var cachedType))
                    {
                        return cachedType;
                    }

                    var fallbackType = GetTypeBySplittingInternals(typeName);
                    if (fallbackType != null)
                    {
                        // Though it was not initially found, we still have found a new type, register it
                        typesWithAssembly[typeName] = fallbackType;
                    }

                    return fallbackType;
                }

                if (typesWithAssembly.TryGetValue(typeNameWithAssembly, out var typeWithAssembly))
                {
                    return typeWithAssembly;
                }

                // Try to remove version info from assembly info
                var assemblyNameWithoutOverhead = TypeHelper.GetAssemblyNameWithoutOverhead(assemblyName);
                var typeNameWithoutAssemblyOverhead = TypeHelper.FormatType(assemblyNameWithoutOverhead, typeName);

                if (typesWithAssembly.TryGetValue(typeNameWithoutAssemblyOverhead, out var typeWithoutAssembly))
                {
                    return typeWithoutAssembly;
                }

                // Fallback to GetType
                try
                {
#if NETFX_CORE
                    var type = Type.GetType(typeNameWithAssembly, false);
#else
                    var type = Type.GetType(typeNameWithAssembly, false, ignoreCase);
#endif
                    if (type != null)
                    {
                        typesWithAssembly[typeNameWithAssembly] = type;
                        return type;
                    }
                }
#if !NETFX_CORE
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
                InitializeTypes(assemblyName, false);

                if (typesWithAssembly.TryGetValue(typeNameWithAssembly, out var exisingTypeWithAssembly))
                {
                    return exisingTypeWithAssembly;
                }

                if (typesWithAssembly.TryGetValue(typeNameWithoutAssemblyOverhead, out var existingTypeWithoutAssembly))
                {
                    return existingTypeWithoutAssembly;
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
                var arrayTypeElement = GetType(arrayTypeElementString, allowInitialization: false);
                if (arrayTypeElement != null)
                {
                    return arrayTypeElement.MakeArrayType();
                }

                return null;
            }

            var firstBracketIndex = typeWithInnerTypes.IndexOf('[');
            if (firstBracketIndex < 0)
            {
                // Not a generic type, and we failed to retrieve it the first time
                return null;
            }

            var genericTypeName = typeWithInnerTypes.Substring(0, firstBracketIndex);
            var genericType = TypeCache.GetType(genericTypeName);
            if (genericType is null)
            {
                // We couldn't resolve List`1
                return null;
            }

            var innerTypesShortNames = TypeHelper.GetInnerTypes(typeWithInnerTypes);
            var innerTypeCount = innerTypesShortNames.Length;
            if (innerTypeCount > 0)
            {
                var innerTypes = new Type[innerTypeCount];

                for (var i = 0; i < innerTypeCount; i++)
                {
                    var innerType = GetType(innerTypesShortNames[i], allowInitialization: false);
                    if (innerType is null)
                    {
                        return null;
                    }

                    innerTypes[i] = innerType;
                }

                var finalType = genericType.MakeGenericTypeEx(innerTypes);
                return finalType;
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
                if (!_typesByInterface.TryGetValue(interfaceType, out var typesByInterface))
                {
                    typesByInterface = GetTypes(x =>
                    {
                        if (x == interfaceType)
                        {
                            return false;
                        }

                        return x.ImplementsInterfaceEx(interfaceType);
                    }).ToArray();

                    _typesByInterface[interfaceType] = typesByInterface;
                }

                return typesByInterface;
            }
        }

        /// <summary>
        /// Gets the types of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="predicate">The predicate to use on the types.</param>
        /// <returns>All types of the specified assembly.</returns>
#if DEBUG
        [Time]
#endif
        public static Type[] GetTypesOfAssembly(Assembly assembly, Func<Type, bool> predicate = null)
        {
            Argument.IsNotNull("assembly", assembly);

            return GetTypesPrefilteredByAssembly(assembly, predicate, true);
        }

        /// <summary>
        /// Gets all the types from the current <see cref="AppDomain"/> where the <paramref name="predicate"/> returns true.
        /// </summary>
        /// <param name="predicate">The predicate where the type should apply to.</param>
        /// <param name="allowInitialization">If set to <c>true</c>, allow initialization of the AppDomain if it hasn't happened yet. If <c>false</c>, deal with the types currently in the cache.</param>
        /// <returns>An array containing all the <see cref="Type"/> that match the predicate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public static Type[] GetTypes(Func<Type, bool> predicate = null, bool allowInitialization = true)
        {
            return GetTypesPrefilteredByAssembly(null, predicate, allowInitialization);
        }

        /// <summary>
        /// Gets the types prefiltered by assembly. If types must be retrieved from a single assembly only, this method is very fast.
        /// </summary>
        /// <param name="assembly">Name of the assembly.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="allowInitialization">If set to <c>true</c>, allow initialization of the AppDomain if it hasn't happened yet. If <c>false</c>, deal with the types currently in the cache.</param>
        /// <returns>System.Type[].</returns>
        private static Type[] GetTypesPrefilteredByAssembly(Assembly assembly, Func<Type, bool> predicate, bool allowInitialization)
        {
            if (allowInitialization)
            {
                InitializeTypes(assembly);
            }

            var assemblyName = (assembly != null) ? TypeHelper.GetAssemblyNameWithoutOverhead(assembly.FullName) : string.Empty;

            // IMPORTANT NOTE!!!! DON'T USE LOGGING IN THE CODE BELOW BECAUSE IT MIGHT CAUSE DEADLOCK (BatchLogListener will load
            // async stuff which can deadlock). Keep it simple without calls to other code. Do any type initialization *outside* 
            // the lock and make sure not to make calls to other methods

            Dictionary<string, Type> typeSource = null;

            lock (_lockObject)
            {
                if (!string.IsNullOrWhiteSpace(assemblyName))
                {
                    _typesByAssembly.TryGetValue(assemblyName, out typeSource);
                }
                else
                {
                    typeSource = _typesWithAssembly;
                }
            }

            if (typeSource is null)
            {
                return ArrayShim.Empty<Type>();
            }

            var retryCount = 3;
            while (retryCount > 0)
            {
                retryCount--;

                try
                {
                    // Important, accessing the type source should happen inside the lock. We lock
                    // every time so the collection can be modified in between retries so we always run
                    // against the latest possible state
                    lock (_lockObject)
                    {
                        if (predicate != null)
                        {
                            return typeSource.Values.Where(predicate).ToArray();
                        }

                        return typeSource.Values.ToArray();
                    }
                }
                catch (Exception)
                {
                    // Ignore
                }
            }

            return ArrayShim.Empty<Type>();

            // IMPORTANT NOTE: READ NOTE ABOVE BEFORE EDITING THIS METHOD!!!!
        }

        /// <summary>
        /// Initializes the types. It does this by looping through all loaded assemblies and
        /// registering the type by type name and assembly name.
        /// <para/>
        /// The types initialized by this method are used by <see cref="object.GetType"/>.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly. If <c>null</c>, all assemblies will be checked.</param>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        /// <param name="allowMultithreadedInitialization">If <c>true</c>, allow multithreaded initialization.</param>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is <c>null</c> or whitespace.</exception>
        public static void InitializeTypes(string assemblyName, bool forceFullInitialization, bool allowMultithreadedInitialization = false)
        {
            // Important note: only allow explicit multithreaded initialization

            Argument.IsNotNullOrWhitespace("assemblyName", assemblyName);

            lock (_lockObject)
            {
                if (_loadedAssemblies.Any(x =>
                {
                    var assemblyNameWithoutVersion = TypeHelper.GetAssemblyNameWithoutOverhead(x);
                    if (string.Equals(assemblyNameWithoutVersion, assemblyName))
                    {
                        return true;
                    }

                    return false;
                }))
                {
                    // No need to initialize (or get the loaded assemblies)
                    return;
                }

                var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies();

                foreach (var assembly in loadedAssemblies)
                {
                    try
                    {
                        if (assembly.FullName.Contains(assemblyName))
                        {
                            InitializeTypes(assembly, forceFullInitialization, allowMultithreadedInitialization);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, $"Failed to get all types in assembly '{assembly.FullName}'");
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
        /// <param name="assembly">The assembly to initialize the types from. If <c>null</c>, all assemblies will be checked.</param>
        /// <param name="forceFullInitialization">If <c>true</c>, the types are initialized, even when the types are already initialized.</param>
        /// <param name="allowMultithreadedInitialization">If <c>true</c>, allow multithreaded initialization. The default value is <c>false</c>.</param>
#if DEBUG
        [Time("{assembly}")]
#endif
        public static void InitializeTypes(Assembly assembly = null, bool forceFullInitialization = false, bool allowMultithreadedInitialization = false)
        {
            // Important note: only allow explicit multithreaded initialization

            var checkSingleAssemblyOnly = assembly != null;

            lock (_lockObject)
            {
                if (_hasInitializedOnce && !forceFullInitialization && !checkSingleAssemblyOnly)
                {
                    return;
                }

                if (!checkSingleAssemblyOnly)
                {
                    _hasInitializedOnce = true;
                }

                // CTL-877 Only clear when assembly != null
                if (forceFullInitialization && assembly is null)
                {
                    _loadedAssemblies.Clear();
                    _typesByAssembly?.Clear();
                    _typesWithAssembly?.Clear();
                    _typesWithAssemblyIgnoreCase?.Clear();
                    _typesWithoutAssembly?.Clear();
                    _typesWithoutAssemblyIgnoreCase?.Clear();
                }

                var assembliesToInitialize = checkSingleAssemblyOnly ? (IEnumerable<Assembly>)new [] { assembly } : AssemblyHelper.GetLoadedAssemblies();
                InitializeAssemblies(assembliesToInitialize, forceFullInitialization, allowMultithreadedInitialization);
            }
        }

#if DEBUG
        [Time]
#endif
        private static void InitializeAssemblies(IEnumerable<Assembly> assemblies, bool force, bool allowMultithreadedInitialization = false)
        {
            // Important note: only allow explicit multithreaded initialization

            var listForLoadedEvent = new List<Tuple<Assembly, Type[]>>();

            lock (_lockObject)
            {
                var assembliesToRetrieve = new List<Assembly>();

                foreach (var assembly in assemblies)
                {
                    var loadedAssemblyFullName = assembly.FullName;
                    var containsLoadedAssembly = _loadedAssemblies.Contains(loadedAssemblyFullName);
                    if (!force && containsLoadedAssembly)
                    {
                        continue;
                    }

                    if (!containsLoadedAssembly)
                    {
                        _loadedAssemblies.Add(loadedAssemblyFullName);
                    }

                    if (ShouldIgnoreAssembly(assembly))
                    {
                        continue;
                    }

                    assembliesToRetrieve.Add(assembly);
                }

                var typesToAdd = GetAssemblyTypes(assembliesToRetrieve, allowMultithreadedInitialization);
                foreach (var assemblyWithTypes in typesToAdd)
                {
                    foreach (var type in assemblyWithTypes.Value)
                    {
                        InitializeType(assemblyWithTypes.Key, type);
                    }
                }

#if NET || NETCORE || NETSTANDARD
                var lateLoadedAssemblies = new List<Assembly>();

                lock (_threadSafeAssemblyQueue)
                {
                    while (_threadSafeAssemblyQueue.Count > 0)
                    {
                        var assembly = _threadSafeAssemblyQueue.Dequeue();

                        lateLoadedAssemblies.Add(assembly);
                    }
                }

                foreach (var lateLoadedAssembly in lateLoadedAssemblies)
                {
                    var tuple = Tuple.Create(lateLoadedAssembly, GetTypesOfAssembly(lateLoadedAssembly));
                    listForLoadedEvent.Add(tuple);
                }
#endif
            }

            // Calling out of lock statement, but still may happens that would be called inside of it 
            var handler = AssemblyLoaded;
            if (handler != null)
            {
                foreach (var tuple in listForLoadedEvent)
                {
                    var assembly = tuple.Item1;
                    var types = tuple.Item2;
                    var eventArgs = new AssemblyLoadedEventArgs(assembly, types);

                    handler(null, eventArgs);
                }
            }
        }

#if DEBUG
        [Time]
#endif
        private static Dictionary<Assembly, IEnumerable<Type>> GetAssemblyTypes(List<Assembly> assemblies, bool allowMultithreadedInitialization = false)
        {
            // Important note: only allow explicit multithreaded initialization

            // We have 2 fast paths out, in case assembly count is either 0 or 1
            if (assemblies.Count == 0)
            {
                return new Dictionary<Assembly, IEnumerable<Type>>();
            }
            else if (assemblies.Count == 1)
            {
                var dictionary = new Dictionary<Assembly, IEnumerable<Type>>();
                var assembly = assemblies[0];

                dictionary[assembly] = assembly.GetAllTypesSafely();

                return dictionary;
            }
            else if (allowMultithreadedInitialization)
            {
                // We try to use multiple threads since GetAllTypesSafely() is an expensive operation, try to multithread
                // without causing to much expansive context switching going on. Using .AsParallel wasn't doing a lot.
                // 
                // After some manual performance benchmarking, the optimum for UWP apps (the most important for performance)
                // is between 15 and 25 threads
                const int PreferredNumberOfThreads = 20;

                var tasks = new List<Task<List<KeyValuePair<Assembly, HashSet<Type>>>>>();
                var taskLists = new List<List<Assembly>>();

                var assemblyCount = assemblies.Count;
                var assembliesPerBatch = (int)Math.Ceiling(assemblyCount / (double)PreferredNumberOfThreads);
                var batchCount = (int)Math.Ceiling(assemblyCount / (double)assembliesPerBatch);

                for (var i = 0; i < batchCount; i++)
                {
                    var taskList = new List<Assembly>();

                    var startIndex = (assembliesPerBatch * i);
                    var endIndex = Math.Min(assembliesPerBatch * (i + 1), assemblyCount);

                    for (var j = startIndex; j < endIndex; j++)
                    {
                        taskList.Add(assemblies[j]);
                    }

                    taskLists.Add(taskList);
                }

                for (var i = 0; i < taskLists.Count; i++)
                {
                    var taskList = taskLists[i];

                    var task = TaskHelper.Run(() =>
                    {
                        var taskResults = new List<KeyValuePair<Assembly, HashSet<Type>>>();

                        foreach (var assembly in taskList)
                        {
                            var assemblyTypes = assembly.GetAllTypesSafely();
                            taskResults.Add(new KeyValuePair<Assembly, HashSet<Type>>(assembly, new HashSet<Type>(assemblyTypes)));
                        }

                        return taskResults;
                    });

                    tasks.Add(task);
                }

                var waitTask = TaskShim.WhenAll(tasks);
                waitTask.Wait();

                var dictionary = new Dictionary<Assembly, IEnumerable<Type>>();

                foreach (var task in tasks)
                {
                    var results = task.Result;

                    foreach (var result in results)
                    {
                        dictionary[result.Key] = result.Value;
                    }
                }

                return dictionary;
            }
            else
            {
                var types = (from assembly in assemblies
                             select new KeyValuePair<Assembly, IEnumerable<Type>>(assembly, assembly.GetAllTypesSafely()));

                var results = types.AsParallel();

                return results.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        private static void InitializeType(Assembly assembly, Type type)
        {
            if (ShouldIgnoreType(assembly, type))
            {
                return;
            }

            var newAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(assembly.FullName);
            var newFullType = TypeHelper.FormatType(newAssemblyName, type.FullName);

            if (!_typesByAssembly.TryGetValue(newAssemblyName, out var typesByAssembly))
            {
                typesByAssembly = new Dictionary<string, Type>();
                _typesByAssembly[newAssemblyName] = typesByAssembly;
            }

            if (!typesByAssembly.ContainsKey(newFullType))
            {
                typesByAssembly[newFullType] = type;

                _typesWithAssembly[newFullType] = type;
                _typesWithAssemblyIgnoreCase[newFullType] = type;

                var typeNameWithoutAssembly = TypeHelper.GetTypeName(newFullType);
                _typesWithoutAssembly[typeNameWithoutAssembly] = newFullType;
                _typesWithoutAssemblyIgnoreCase[typeNameWithoutAssembly] = newFullType;
            }
        }

        /// <summary>
        /// Determines whether the specified assembly must be ignored by the type cache.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns><c>true</c> if the assembly should be ignored, <c>false</c> otherwise.</returns>
        private static bool ShouldIgnoreAssembly(Assembly assembly)
        {
            if (assembly is null)
            {
                return true;
            }

            // Note: don't check with .NET Standard / .NET Core, it's "not implemented by design"
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

            //// Ignore windows assemblies
            //if (assemblyFullName.Contains("ContentType=WindowsRuntime"))
            //{
            //    return true;
            //}

            //// Ignore System.Runtime (.net standard)
            //if (assemblyFullName.StartsWith("System.Runtime,"))
            //{
            //    return true;
            //}

            foreach (var evaluator in TypeCacheEvaluator.AssemblyEvaluators)
            {
                if (evaluator(assembly))
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
            if (type is null)
            {
                return true;
            }

            var typeName = type.FullName;

            // CTL-653
            if (string.IsNullOrEmpty(typeName))
            {
                // Some types don't have a name (for example, after obfuscation), let's ignore these
                return true;
            }

            // Ignore useless types
            if (typeName.Contains("<PrivateImplementationDetails>") ||
                typeName.Contains("+<") || // C# compiler generated classes
                typeName.Contains("+_Closure") || // VB.NET compiler generated classes
                typeName.Contains(".__") || // System.Runtime.CompilerServices.*
                typeName.Contains("Interop+") || // System.IO.FileSystem, System.Net.Sockets, etc
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

            foreach (var evaluator in TypeCacheEvaluator.TypeEvaluators)
            {
                if (evaluator(assembly, type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
