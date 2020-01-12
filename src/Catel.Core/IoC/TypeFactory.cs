// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

//#define EXTREME_LOGGING

namespace Catel.IoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using ApiCop;
    using ApiCop.Rules;
    using Caching;
    using Logging;
    using Reflection;
    using Threading;
    using Catel.Collections;
    using Catel.Linq;

    /// <summary>
    /// Type factory which will cache constructors to ensure the best performance available.
    /// <para />
    /// This class will automatically watch the <see cref="IServiceLocator.TypeRegistered"/> event and clear
    /// the cache automatically when the event occurs.
    /// </summary>
    public class TypeFactory : ITypeFactory
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The API cop.
        /// </summary>
        private static readonly IApiCop ApiCop = ApiCopManager.GetCurrentClassApiCop();

        /// <summary>
        /// The type request path name.
        /// </summary>
        private const string TypeRequestPathName = "TypeFactory";
        #endregion

        #region Fields
        /// <summary>
        /// Provides thread safe access to constructors cache.
        /// </summary>
        private readonly ReaderWriterLockSlim _constructorCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Cache containing all last used constructors.
        /// </summary>
        private readonly Dictionary<ConstructorCacheKey, ConstructorCacheValue> _constructorCache = new Dictionary<ConstructorCacheKey, ConstructorCacheValue>();

        /// <summary>
        /// Provides thread safe access to type constructors.
        /// </summary>
        private readonly ReaderWriterLockSlim _typeConstructorsMetadataLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Cache containing all the metadata of a specific type so this doesn't have to be queried multiple times.
        /// </summary>
        private readonly Dictionary<Type, TypeMetaData> _typeConstructorsMetadata = new Dictionary<Type, TypeMetaData>();

        /// <summary>
        /// The service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// The current type request path.
        /// </summary>
        private readonly ThreadLocal<TypeRequestPath> _currentTypeRequestPath;
        #endregion

        #region Constructors
        static TypeFactory()
        {
            ApiCop.RegisterRule(new TooManyDependenciesApiCopRule("TypeFactory.LimitDependencyInjection", "It is recommended not to inject too many types using dependency injection as this might be a code-smell. Is the class too big? Try splitting it up into smaller classes with less dependencies.", ApiCopRuleLevel.Hint));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFactory" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public TypeFactory(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            _currentTypeRequestPath = new ThreadLocal<TypeRequestPath>(() => TypeRequestPath.Root(TypeRequestPathName));

            _serviceLocator = serviceLocator;
            _serviceLocator.TypeRegistered += OnServiceLocatorTypeRegistered;

            // Note: this will cause memory leaks (TypeCache will keep this class alive), but it's an acceptable "loss"
            TypeCache.AssemblyLoaded += OnAssemblyLoaded;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default instance.
        /// </summary>
        /// <value>The instance.</value>
        /// <remarks>
        /// Do not move initialization to a field, it will cause a deadlock with the initialization of the default ServiceLocator.
        /// </remarks>
        public static ITypeFactory Default
        {
            get
            {
                return IoCConfiguration.DefaultTypeFactory;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        public object CreateInstance(Type typeToConstruct)
        {
            return CreateInstanceWithTag(typeToConstruct, null);
        }

        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        public object CreateInstanceWithTag(Type typeToConstruct, object tag)
        {
            return CreateInstanceWithSpecifiedParameters(typeToConstruct, tag, null, true);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        public object CreateInstanceWithParameters(Type typeToConstruct, params object[] parameters)
        {
            return CreateInstanceWithParametersWithTag(typeToConstruct, null, parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        public object CreateInstanceWithParametersWithTag(Type typeToConstruct, object tag, params object[] parameters)
        {
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            return CreateInstanceWithSpecifiedParameters(typeToConstruct, tag, parameters, false);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        public object CreateInstanceWithParametersAndAutoCompletion(Type typeToConstruct, params object[] parameters)
        {
            return CreateInstanceWithParametersAndAutoCompletionWithTag(typeToConstruct, null, parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator" />.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        public object CreateInstanceWithParametersAndAutoCompletionWithTag(Type typeToConstruct, object tag, params object[] parameters)
        {
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            return CreateInstanceWithSpecifiedParameters(typeToConstruct, tag, parameters, true);
        }

        /// <summary>
        /// Initializes the created object after its construction.
        /// </summary>
        /// <param name="obj">The object to initialize.</param>
        /// <param name="typeMetaData">Metadata about object to initialize</param>
        private void InitializeAfterConstruction(object obj, TypeMetaData typeMetaData)
        {
            if (obj is null)
            {
                return;
            }

            var objectType = ObjectToStringHelper.ToTypeString(obj);

#if EXTREME_LOGGING
            Log.Debug($"Initializing type '{objectType}' after construction");
#endif

            // TODO: Consider to cache for performance
            var dependencyResolverManager = DependencyResolverManager.Default;
            var dependencyResolver = _serviceLocator.ResolveType<IDependencyResolver>();
            dependencyResolverManager.RegisterDependencyResolverForInstance(obj, dependencyResolver);

#if EXTREME_LOGGING
            Log.Debug($"Injecting properties into type '{objectType}' after construction");
#endif

            var type = obj.GetType();
            foreach (var injectedProperty in typeMetaData.GetInjectedProperties())
            {
                var propertyInfo = injectedProperty.Key;
                var injectAttribute = injectedProperty.Value;

                try
                {
                    var dependency = _serviceLocator.ResolveType(injectAttribute.Type, injectAttribute.Tag);
                    propertyInfo.SetValue(obj, dependency, null);
                }
                catch (Exception ex)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>(ex, "Failed to set property '{0}.{1}' during property dependency injection", type.Name, propertyInfo.Name);
                }
            }

            var objAsINeedCustomInitialization = obj as INeedCustomInitialization;
            if (objAsINeedCustomInitialization != null)
            {
                objAsINeedCustomInitialization.Initialize();
            }
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <param name="autoCompleteDependencies">if set to <c>true</c>, the additional dependencies will be auto completed.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        private object CreateInstanceWithSpecifiedParameters(Type typeToConstruct, object tag, object[] parameters, bool autoCompleteDependencies)
        {
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            if (parameters is null)
            {
                parameters = ArrayShim.Empty<object>();
            }

            var previousRequestPath = _currentTypeRequestPath.Value;
            try
            {
                var typeRequestInfo = new TypeRequestInfo(typeToConstruct);
                _currentTypeRequestPath.Value = TypeRequestPath.Branch(previousRequestPath, typeRequestInfo);

                var constructorCacheKey = new ConstructorCacheKey(typeToConstruct, autoCompleteDependencies, parameters);

                var typeConstructorsMetadata = GetTypeMetaData(typeToConstruct);

                var constructorCacheValue = GetConstructor(constructorCacheKey);

                if (constructorCacheValue.ConstructorInfo != null)
                {
                    var cachedConstructor = constructorCacheValue.ConstructorInfo;
                    var instanceCreatedWithInjection = TryCreateToConstruct(typeToConstruct, cachedConstructor, tag, parameters, false, false, typeConstructorsMetadata);
                    if (instanceCreatedWithInjection != null)
                    {
                        return instanceCreatedWithInjection;
                    }

                    Log.Warning($"Found constructor for type '{typeToConstruct.FullName}' in constructor, but it failed to create an instance. Removing the constructor from the cache");

                    SetConstructor(constructorCacheKey, constructorCacheValue, null);
                }

#if EXTREME_LOGGING
                Log.Debug($"Creating instance of type '{typeToConstruct.FullName}' using specific parameters. No constructor found in the cache, so searching for the right one");
#endif

                var constructors = typeConstructorsMetadata.GetConstructors(parameters.Length, !autoCompleteDependencies).SortByParametersMatchDistance(parameters).ToList();

                for (var i = 0; i < constructors.Count; i++)
                {
                    var constructor = constructors[i];

                    var instanceCreatedWithInjection = TryCreateToConstruct(typeToConstruct, constructor, tag, parameters, true, i < constructors.Count - 1, typeConstructorsMetadata);
                    if (instanceCreatedWithInjection != null)
                    {
                        // We found a constructor that works, cache it
                        SetConstructor(constructorCacheKey, constructorCacheValue, constructor);

                        // Only update the rule when using a constructor for the first time, not when using it from the cache
                        ApiCop.UpdateRule<TooManyDependenciesApiCopRule>("TypeFactory.LimitDependencyInjection", x => x.SetNumberOfDependenciesInjected(typeToConstruct, constructor.GetParameters().Count()));

                        return instanceCreatedWithInjection;
                    }
                }

#if EXTREME_LOGGING
                Log.Debug($"No constructor could be used, cannot construct type '{typeToConstruct.FullName}' with the specified parameters");
#endif
            }
            catch (CircularDependencyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to construct type '{typeToConstruct.FullName}'");

                throw;
            }
            finally
            {
                _currentTypeRequestPath.Value = previousRequestPath;
            }

            return null;
        }

        /// <summary>
        /// Gets the constructors metadata.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="TypeMetaData"/>.</returns>
        private TypeMetaData GetTypeMetaData(Type type)
        {
            return _typeConstructorsMetadataLock.PerformUpgradableRead(() =>
            {
                if (_typeConstructorsMetadata.TryGetValue(type, out var result))
                {
                    return result;
                }

                result = new TypeMetaData(type);

                _typeConstructorsMetadataLock.PerformWrite(() =>
                {
                    _typeConstructorsMetadata.Add(type, result);
                });

                return result;
            });
        }

        /// <summary>
        /// Determines whether the specified constructor can be used for dependency injection.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="autoCompleteDependencies">if set to <c>true</c>, additional dependencies can be completed from the <see cref="IServiceLocator" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if this instance [can constructor be used] the specified constructor; otherwise, <c>false</c>.</returns>
        private bool CanConstructorBeUsed(ConstructorInfo constructor, object tag, bool autoCompleteDependencies, params object[] parameters)
        {
            // Some loging like .GetSignature are expensive
#if EXTREME_LOGGING
            var logDebug = LogManager.LogInfo.IsDebugEnabled && !LogManager.LogInfo.IgnoreCatelLogging;
#else
            var logDebug = false;
#endif
            if (logDebug)
            {
                Log.Debug($"Checking if constructor '{constructor.GetSignature()}' can be used");
            }

            if (constructor.IsStatic)
            {
                Log.Debug("Constructor is not valid because it is static");
                return false;
            }

            var validConstructor = true;
            var ctorParameters = constructor.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var ctorParameter = ctorParameters[i];
                var ctorParameterType = ctorParameter.ParameterType;
                var parameter = parameters[i];

                if (!IsValidParameterValue(ctorParameterType, parameter))
                {
                    if (logDebug)
                    {
                        Log.Debug($"Constructor is not valid because value '{ObjectToStringHelper.ToString(parameter)}' cannot be used for parameter '{ctorParameter.Name}'");
                    }

                    validConstructor = false;
                    break;
                }
            }

            if (validConstructor && autoCompleteDependencies)
            {
                if (ctorParameters.Length > parameters.Length)
                {
                    // check if all the additional parameters are registered in the service locator
                    for (int j = parameters.Length; j < ctorParameters.Length; j++)
                    {
                        var parameterToResolve = ctorParameters[j];
                        var parameterTypeToResolve = parameterToResolve.ParameterType;

                        if (!typeof(string).IsAssignableFromEx(parameterTypeToResolve) &&
                            typeof(IEnumerable).IsAssignableFromEx(parameterTypeToResolve))
                        {
                            var collectionElementType = parameterTypeToResolve.GetCollectionElementType();
                            if (collectionElementType != null && _serviceLocator.IsTypeRegisteredWithOrWithoutTag(collectionElementType))
                            {
                                continue;
                            }
                        }

                        if (!_serviceLocator.IsTypeRegistered(parameterTypeToResolve, tag) &&
                            !_serviceLocator.IsTypeRegistered(parameterTypeToResolve))
                        {
                            if (logDebug)
                            {
                                Log.Debug($"Constructor is not valid because parameter '{parameterToResolve.Name}' cannot be resolved from the dependency resolver");
                            }

                            validConstructor = false;
                            break;
                        }
                    }
                }
            }

#if EXTREME_LOGGING
            Log.Debug("The constructor is valid and can be used");
#endif

            return validConstructor;
        }

        /// <summary>
        /// Determines whether the specified parameter value can be used for the specified parameter type.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        private bool IsValidParameterValue(Type parameterType, object parameterValue)
        {
            // 1: check if value is null and if the ctor accepts that
            var isParameterNull = (parameterValue is null);
            var isCtorParameterValueType = parameterType.IsValueTypeEx();
            if (isParameterNull && isCtorParameterValueType)
            {
                return false;
            }

            if (isParameterNull)
            {
                // valid
                return true;
            }

            // 2: check if the values are both value or reference types 
            var parameterValueType = parameterValue.GetType();
            bool isParameterValueType = parameterValueType.IsValueTypeEx();
            if (isParameterValueType != isCtorParameterValueType)
            {
                return false;
            }

            // 3.a: check if this is a COM object
            if (parameterValueType.IsCOMObjectEx())
            {
                return true;
            }

            // 3.b: check if the types are acceptable
            if (!parameterType.IsInstanceOfTypeEx(parameterValue))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to create the service with the specified constructor using the specified parameters.
        /// <para />
        /// This method will not throw an exception when the invocation fails.
        /// </summary>
        /// <param name="typeToConstruct">Type of the service.</param>
        /// <param name="constructor">The constructor info.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to pass into the constructor.</param>
        /// <param name="checkConstructor">if set to <c>true</c>, check whether the constructor can be used before using it.</param>
        /// <param name="hasMoreConstructorsLeft">if set to <c>true</c>, more constructors are left so don't throw exceptions.</param>
        /// <param name="typeMetaData">Metadata about type beiing constructed.</param>
        /// <returns>The instantiated service or <c>null</c> if the instantiation fails.</returns>
        /// <remarks>Note that this method does not require an implementation of
        /// <see cref="TypeRequestPath" /> because this already has the parameter values
        /// and thus cannot lead to invalid circular dependencies.</remarks>
        private object TryCreateToConstruct(Type typeToConstruct, ConstructorInfo constructor, object tag, object[] parameters,
            bool checkConstructor, bool hasMoreConstructorsLeft, TypeMetaData typeMetaData)
        {
            // Check if this constructor is even possible
            if (checkConstructor)
            {
                if (!CanConstructorBeUsed(constructor, tag, true, parameters))
                {
                    return null;
                }
            }

            try
            {
                var finalParameters = new List<object>(parameters);
                var ctorParameters = constructor.GetParameters();
                for (var i = parameters.Length; i < ctorParameters.Length; i++)
                {
                    object ctorParameterValue = null;

                    var parameterTypeToResolve = ctorParameters[i].ParameterType;
                    if (!typeof(string).IsAssignableFromEx(parameterTypeToResolve) && typeof(IEnumerable).IsAssignableFromEx(parameterTypeToResolve))
                    {
                        var collectionElementType = parameterTypeToResolve.GetCollectionElementType();
                        if (collectionElementType != null && _serviceLocator.IsTypeRegisteredWithOrWithoutTag(collectionElementType))
                        {
                            var ctorParameterValueLocal = _serviceLocator.ResolveTypes(collectionElementType).Cast(collectionElementType);

                            if (parameterTypeToResolve.IsArray)
                            {
                                ctorParameterValueLocal = ctorParameterValueLocal.ToSystemArray(collectionElementType);
                            }
                            else if (typeof(IReadOnlyList<>).MakeGenericType(collectionElementType).IsAssignableFromEx(parameterTypeToResolve) || typeof(IReadOnlyCollection<>).MakeGenericType(collectionElementType).IsAssignableFromEx(parameterTypeToResolve))
                            {
                                ctorParameterValueLocal = ctorParameterValueLocal.AsReadOnly(collectionElementType);
                            }
                            else if (typeof(IList<>).MakeGenericType(collectionElementType).IsAssignableFromEx(parameterTypeToResolve) || typeof(ICollection<>).MakeGenericType(collectionElementType).IsAssignableFromEx(parameterTypeToResolve))
                            {
                                ctorParameterValueLocal = ctorParameterValueLocal.ToList(collectionElementType);
                            }

                            ctorParameterValue = ctorParameterValueLocal;
                        }
                    }

                    if (ctorParameterValue is null)
                    {
                        if (!(tag is null) && _serviceLocator.IsTypeRegistered(parameterTypeToResolve, tag))
                        {
                            // Use preferred tag
                            ctorParameterValue = _serviceLocator.ResolveType(parameterTypeToResolve, tag);
                        }
                        else
                        {
                            // No tag or fallback to default without tag
                            ctorParameterValue = _serviceLocator.ResolveType(parameterTypeToResolve);
                        }
                    }


                    finalParameters.Add(ctorParameterValue);
                }

                var finalParametersArray = finalParameters.ToArray();

                //Log.Debug("Calling constructor.Invoke with the right parameters");

                var instance = constructor.Invoke(finalParametersArray);

                InitializeAfterConstruction(instance, typeMetaData);

                return instance;
            }
#if NET || NETCORE || NETSTANDARD
            catch (MissingMethodException)
            {
                // Ignore, we accept this
            }
#endif
            catch (TargetParameterCountException)
            {
                // Ignore, we accept this
            }
            catch (CircularDependencyException)
            {
                // Only handle CircularDependencyExceptions we throw ourselves because we support generic types such as 
                // Dictionary<TKey, TValue> which has a constructor with IDictionary<TKey, TValue>
                if (!hasMoreConstructorsLeft)
                //if (string.Equals(TypeRequestPathName, ex.TypePath.Name, StringComparison.Ordinal))
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Real exceptions bubble up, otherwise return null
                Log.Error(ex, $"Failed to instantiate type '{typeToConstruct.FullName}', but this was an unexpected error");
                throw;
            }

            // Some loging like .GetSignature are expensive
            var logDebug = LogManager.LogInfo.IsDebugEnabled && !LogManager.LogInfo.IgnoreCatelLogging;
            if (logDebug)
            {
                Log.Debug($"Failed to create instance using dependency injection for type '{typeToConstruct.FullName}' using constructor '{constructor.GetSignature()}'");
            }

            return null;
        }

        private ConstructorCacheValue GetConstructor(ConstructorCacheKey cacheKey)
        {
            return _constructorCacheLock.PerformRead(() =>
            {
                if (_constructorCache.TryGetValue(cacheKey, out var result))
                {
                    return result;
                }

                return ConstructorCacheValue.First();
            });
        }

        private void SetConstructor(ConstructorCacheKey cacheKey, ConstructorCacheValue previousCacheValue, ConstructorInfo constructorInfo)
        {
            // Currently I choose last-win strategy but maybe other should be used
            _constructorCacheLock.PerformWrite(() =>
            {
                if (!_constructorCache.TryGetValue(cacheKey, out var storedValue))
                {
                    storedValue = ConstructorCacheValue.First();
                }

                if (storedValue.Version == previousCacheValue.Version)
                {
                    // Log.Debug("Everything is fine");
                }
                else if (storedValue.Version > previousCacheValue.Version)
                {
                    Log.Debug("Data in cache have been changed between read & write");
                }
                else
                {
                    // TODO: Maybe exception should be thrown
                    Log.Debug("Something strange have happend. Deep log analyze required");
                }

                // I'm not sure storedValue or previousCacheValue should be passed in here
                var cacheValue = ConstructorCacheValue.Next(storedValue, constructorInfo);
                _constructorCache[cacheKey] = cacheValue;
            });
        }

        /// <summary>
        /// Clears the cache of all constructors.
        /// <para />
        /// This call is normally not necessary since the type factory should keep an eye on the 
        /// <see cref="IServiceLocator.TypeRegistered"/> event to invalidate the cache.
        /// </summary>
        public void ClearCache()
        {
            // Note that we don't clear the constructor metadata cache, constructors on types normally don't change during an
            // application lifetime

            // Clear cache isn't really that important, it's better to prevent deadlocks. How can a deadlock occur? If thread x is creating 
            // a type and loads an assembly, but thread y is also loading an assembly. Thread x will lock because it's creating the type, 
            // thread y will lock because it wants to clear the cache because new types were added. In that case ignore clearing the cache

            // Edit: Probability of loading assembly inside of lock have been drastically reduced so comment above probably isn't relevant
            _constructorCacheLock.PerformWrite(() =>
            {
                var array = _constructorCache.ToArray();

                foreach (var keyValuePair in array)
                {
                    _constructorCache[keyValuePair.Key] = ConstructorCacheValue.Next(keyValuePair.Value, null);
                }
            });

            // This log entry makes sence in optimistic lock scenarios
            Log.Debug("Cleared type constructor cache");
        }

        /// <summary>
        /// Called when the <see cref="IServiceLocator.TypeRegistered"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="TypeRegisteredEventArgs" /> instance containing the event data.</param>
        private void OnServiceLocatorTypeRegistered(object sender, TypeRegisteredEventArgs eventArgs)
        {
            ClearCache();
        }

        /// <summary>
        /// Called when the <see cref="TypeCache.AssemblyLoaded"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssemblyLoadedEventArgs"/> instance containing the event data.</param>
        private void OnAssemblyLoaded(object sender, AssemblyLoadedEventArgs e)
        {
            ClearCache();
        }
#endregion

        private class ConstructorCacheKey
        {
            private readonly int _hashCode;

#region Constructors
            public ConstructorCacheKey(Type type, bool autoCompleteDependecies, object[] parameters)
            {
                string key = type.GetSafeFullName(true);
                foreach (var parameter in parameters)
                {
                    key += "_" + ObjectToStringHelper.ToFullTypeString(parameter);
                }

                Key = key;
                AutoCompleteDependecies = autoCompleteDependecies;
                _hashCode = Key.GetHashCode();
            }
#endregion

#region Properties
            public string Key { get; private set; }

            public bool AutoCompleteDependecies { get; private set; }

#endregion

#region Methods
            public override bool Equals(object obj)
            {
                var cacheKey = obj as ConstructorCacheKey;
                if (cacheKey is null)
                {
                    return false;
                }

                return Equals(cacheKey);
            }

            private bool Equals(ConstructorCacheKey other)
            {
                if (AutoCompleteDependecies != other.AutoCompleteDependecies)
                {
                    return false;
                }

                return string.Equals(Key, other.Key, StringComparison.Ordinal);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
#endregion
        }

        private class ConstructorCacheValue
        {
            private static readonly ConstructorCacheValue FirstValue = new ConstructorCacheValue(null, 0);

            /// <summary>
            /// Creates first entry in cache for key.
            /// </summary>
            /// <returns></returns>
            public static ConstructorCacheValue First()
            {
                return FirstValue;
            }

            /// <summary>
            /// Creates entry that replaces previous entry in cache for key.
            /// </summary>
            /// <param name="previousValue">Previously used constructor.</param>
            /// <param name="constructorInfo">Creates first entry in cache for key.</param>
            /// <returns></returns>
            public static ConstructorCacheValue Next(ConstructorCacheValue previousValue, ConstructorInfo constructorInfo)
            {
                return new ConstructorCacheValue(constructorInfo, previousValue.Version + 1);
            }

            /// <summary>
            /// Creates new instance of <see cref="ConstructorCacheValue"/>
            /// </summary>
            /// <param name="constructorInfo">Constructor info used to creating new instances</param>
            /// <param name="version">Flag used for detecting race-conditions</param>
            private ConstructorCacheValue(ConstructorInfo constructorInfo, uint version)
            {
                // TODO: Think about using Func<object> it should work faster than calling reflection
                ConstructorInfo = constructorInfo;
                Version = version;
            }

#region Properties

            public ConstructorInfo ConstructorInfo { get; private set; }

            public uint Version { get; private set; }
#endregion

            // TODO: Equals & GetHashCode currently are redundant
        }

        private class TypeMetaData
        {
            private readonly ICacheStorage<string, List<ConstructorInfo>> _callCache = new CacheStorage<string, List<ConstructorInfo>>();
            private volatile Dictionary<PropertyInfo, InjectAttribute> _injectedProperties;
            private readonly object _lockObject = new object();

            public TypeMetaData(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }

            public Dictionary<PropertyInfo, InjectAttribute> GetInjectedProperties()
            {
                if (_injectedProperties != null)
                {
                    return _injectedProperties;
                }

                lock (_lockObject)
                {
                    if (_injectedProperties != null)
                    {
                        return _injectedProperties;
                    }

                    _injectedProperties = new Dictionary<PropertyInfo, InjectAttribute>();

                    var properties = Type.GetPropertiesEx();
                    foreach (var property in properties)
                    {
                        var injectAttribute = property.GetCustomAttributeEx(typeof(InjectAttribute), false) as InjectAttribute;
                        if (injectAttribute != null)
                        {
                            if (injectAttribute.Type is null)
                            {
                                injectAttribute.Type = property.PropertyType;
                            }

                            _injectedProperties.Add(property, injectAttribute);
                        }
                    }
                }

                return _injectedProperties;
            }

            public List<ConstructorInfo> GetConstructors()
            {
                return GetConstructors(-1, false);
            }

            public List<ConstructorInfo> GetConstructors(int parameterCount, bool mustMatchExactCount)
            {
                string key = string.Format("{0}_{1}", parameterCount, mustMatchExactCount);

                return _callCache.GetFromCacheOrFetch(key, () =>
                {
                    var constructors = new List<ConstructorInfo>();

                    constructors.AddRange(GetConstructors(parameterCount, mustMatchExactCount, true));
                    constructors.AddRange(GetConstructors(parameterCount, mustMatchExactCount, false));

                    return constructors;
                });
            }

            private List<ConstructorInfo> GetConstructors(int parameterCount, bool mustMatchExactCount, bool decoratedWithInjectionConstructorAttribute)
            {
                List<ConstructorInfo> constructors;

                if (mustMatchExactCount)
                {
                    constructors = (from ctor in Type.GetConstructorsEx()
                                    where ctor.GetParameters().Length == parameterCount
                                    orderby ctor.GetParameters().Length descending, CountSpecialObjects(ctor)
                                    select ctor).ToList();
                }
                else
                {
                    constructors = (from ctor in Type.GetConstructorsEx()
                                    where ctor.GetParameters().Length >= parameterCount
                                    orderby ctor.GetParameters().Length descending, CountSpecialObjects(ctor)
                                    select ctor).ToList();
                }

                if (decoratedWithInjectionConstructorAttribute)
                {
                    constructors = (from ctor in constructors
                                    where ctor.IsDecoratedWithAttribute<InjectionConstructorAttribute>()
                                    select ctor).ToList();
                }
                else
                {
                    constructors = (from ctor in constructors
                                    where !ctor.IsDecoratedWithAttribute<InjectionConstructorAttribute>()
                                    select ctor).ToList();
                }

                return constructors;
            }
        }

        /// <summary>
        /// Gets the special objects count for the specific constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The number of special objects.</returns>
        private static int CountSpecialObjects(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            int counter = 0;

            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;
                if (parameterType == typeof(Object))
                {
                    counter++;
                    continue;
                }

#if !XAMARIN
                // Because we check on string, we don't need a dependency
                //if (parameterType.FullName == typeof(DynamicObject))
                if (parameterType.FullName == "System.Dynamic.DynamicObject")
                {
                    counter++;
                    continue;
                }
#endif
            }

            return counter;
        }
    }
}
