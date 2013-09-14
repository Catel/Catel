// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel.Caching;
    using Catel.Logging;
    using Catel.Reflection;

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
        #endregion

        #region Fields
        /// <summary>
        /// Cache containing all last used constructors for a type.
        /// </summary>
        private readonly Dictionary<ConstructorCacheKey, ConstructorInfo> _constructorCache = new Dictionary<ConstructorCacheKey, ConstructorInfo>();

        /// <summary>
        /// Cache containing all last used constructors for a type without auto-completion.
        /// </summary>
        private readonly Dictionary<ConstructorCacheKey, ConstructorInfo> _specificConstructorCacheWithoutAutoCompletion = new Dictionary<ConstructorCacheKey, ConstructorInfo>();

        /// <summary>
        /// Cache containing all last used constructors for a type with auto-completion.
        /// </summary>
        private readonly Dictionary<ConstructorCacheKey, ConstructorInfo> _specificConstructorCacheWithAutoCompletion = new Dictionary<ConstructorCacheKey, ConstructorInfo>();

        /// <summary>
        /// Cache containing all the constructors of a specific type so this doesn't have to be queried multiple times.
        /// </summary>
        private readonly Dictionary<Type, TypeConstructorsMetadata> _typeConstructorsMetadata = new Dictionary<Type, TypeConstructorsMetadata>();

        /// <summary>
        /// Cache containing whether a type can be created using <c>Activator.CreateInstance</c>.
        /// </summary>
        private readonly Dictionary<Type, bool> _canUseActivatorCache = new Dictionary<Type, bool>();

        /// <summary>
        /// The dependency resolver.
        /// </summary>
        private readonly IDependencyResolver _dependencyResolver;

#if !DISABLE_TYPEPATH
        /// <summary>
        /// The current type request path.
        /// </summary>
        private TypeRequestPath _currentTypeRequestPath;
#endif

        /// <summary>
        /// The lock object.
        /// </summary>
        private readonly object _lockObject = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFactory" /> class.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver"/> is <c>null</c>.</exception>
        public TypeFactory(IDependencyResolver dependencyResolver)
        {
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            _dependencyResolver = dependencyResolver;
            var serviceLocator = _dependencyResolver.Resolve<IServiceLocator>();
            if (serviceLocator != null)
            {
                serviceLocator.TypeRegistered += OnServiceLocatorTypeRegistered;
            }
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
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            lock (_lockObject)
            {
                TypeRequestInfo typeRequestInfo = null;

                try
                {
#if !DISABLE_TYPEPATH
                    typeRequestInfo = new TypeRequestInfo(typeToConstruct);
                    if (_currentTypeRequestPath == null)
                    {
                        _currentTypeRequestPath = new TypeRequestPath(typeRequestInfo);
                    }
                    else
                    {
                        _currentTypeRequestPath.PushType(typeRequestInfo, true);
                    }
#endif

                    var constructorCacheKey = new ConstructorCacheKey(typeToConstruct, new object[] { });
                    if (_constructorCache.ContainsKey(constructorCacheKey))
                    {
                        var cachedConstructor = _constructorCache[constructorCacheKey];
                        var instanceCreatedWithInjection = TryCreateWithConstructorInjection(typeToConstruct, cachedConstructor);
                        if (instanceCreatedWithInjection != null)
                        {
#if !DISABLE_TYPEPATH
                            CompleteTypeRequestPathIfRequired(typeRequestInfo);
#endif

                            return instanceCreatedWithInjection;
                        }

                        Log.Warning("Found constructor for type '{0}' in constructor, but it failed to create an instance. Removing the constructor from the cache", typeToConstruct.FullName);

                        _constructorCache.Remove(constructorCacheKey);
                    }

                    Log.Debug("Creating instance of type '{0}'. No constructor found in the cache, so searching for the right one.", typeToConstruct.FullName);

                    var constructors = (from constructor in typeToConstruct.GetConstructorsEx()
                                        orderby constructor.GetParameters().Count() descending
                                        select constructor).ToList();

                    foreach (var constructor in constructors)
                    {
                        var instanceCreatedWithInjection = TryCreateWithConstructorInjection(typeToConstruct, constructor);
                        if (instanceCreatedWithInjection != null)
                        {
#if !DISABLE_TYPEPATH
                            CompleteTypeRequestPathIfRequired(typeRequestInfo);
#endif

                            // We found a constructor that works, cache it
                            _constructorCache[constructorCacheKey] = constructor;

                            return instanceCreatedWithInjection;
                        }
                    }

                    var createdInstanceUsingActivator = CreateInstanceUsingActivator(typeToConstruct);
                    if (createdInstanceUsingActivator != null)
                    {
#if !DISABLE_TYPEPATH
                        CompleteTypeRequestPathIfRequired(typeRequestInfo);
#endif

                        return createdInstanceUsingActivator;
                    }
                }
                catch (CircularDependencyException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    CloseCurrentTypeTypeIfRequired(typeRequestInfo);

                    Log.Warning(ex, "Failed to construct type '{0}'", typeToConstruct.FullName);

                    throw;
                }

                CloseCurrentTypeTypeIfRequired(typeRequestInfo);

                return null;
            }
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
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            return CreateInstanceWithSpecifiedParameters(typeToConstruct, false, parameters);
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
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            return CreateInstanceWithSpecifiedParameters(typeToConstruct, true, parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type using <c>>Activator.CreateInstance</c>.
        /// <para />
        /// The advantage of using this method is that the results are being cached if the execution fails thus
        /// the next call will be extremely fast.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        public object CreateInstanceUsingActivator(Type typeToConstruct)
        {
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            object instance = null;

            lock (_lockObject)
            {
                if (!_canUseActivatorCache.ContainsKey(typeToConstruct) || _canUseActivatorCache[typeToConstruct])
                {
                    try
                    {
                        Log.Debug("No constructor could be used, using Activator.CreateInstance. Cannot cache this either.");

                        instance = Activator.CreateInstance(typeToConstruct);

                        _canUseActivatorCache[typeToConstruct] = (instance != null);
                    }
#if !NETFX_CORE && !PCL
                    catch (MissingMethodException)
#else
                    catch (Exception)
#endif
                    {
                        _canUseActivatorCache[typeToConstruct] = false;

                        Log.Debug("Failed to use Activator.CreateInstance, cannot instantiate type '{0}'", typeToConstruct.FullName);
                    }
                }
            }

            InitializeAfterConstruction(instance);

            return instance;
        }

#if !DISABLE_TYPEPATH
        /// <summary>
        /// Marks the specified type as not being created. If this was the only type being constructed, the type request
        /// path will be closed.
        /// </summary>
        /// <param name="typeRequestInfoForTypeJustConstructed">The type request info for type just constructed.</param>
        private void CloseCurrentTypeTypeIfRequired(TypeRequestInfo typeRequestInfoForTypeJustConstructed)
        {
            lock (_lockObject)
            {
                if (_currentTypeRequestPath != null)
                {
                    _currentTypeRequestPath.MarkTypeAsNotCreated(typeRequestInfoForTypeJustConstructed);

                    if (_currentTypeRequestPath.TypeCount == 1)
                    {
                        // We failed to create the only type in the request path, exit
                        _currentTypeRequestPath = null;
                    }
                }
            }
        }

        /// <summary>
        /// Completes the type request path by checking if the currently created type is the same as the first
        /// type meaning that the type is successfully created and the current type request path can be set to <c>null</c>.
        /// </summary>
        /// <param name="typeRequestInfoForTypeJustConstructed">The type request info.</param>
        private void CompleteTypeRequestPathIfRequired(TypeRequestInfo typeRequestInfoForTypeJustConstructed)
        {
            lock (_lockObject)
            {
                if (_currentTypeRequestPath != null)
                {
                    if (_currentTypeRequestPath.FirstType == typeRequestInfoForTypeJustConstructed)
                    {
                        _currentTypeRequestPath = null;
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Initializes the created object after its construction.
        /// </summary>
        /// <param name="obj">The object to initialize.</param>
        private void InitializeAfterConstruction(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var dependencyResolverManager = DependencyResolverManager.Default;
            dependencyResolverManager.RegisterDependencyResolverForInstance(obj, _dependencyResolver);

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
        /// <param name="autoCompleteDependencies">if set to <c>true</c>, the additional dependencies will be auto completed.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        private object CreateInstanceWithSpecifiedParameters(Type typeToConstruct, bool autoCompleteDependencies, object[] parameters)
        {
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            lock (_lockObject)
            {
                var constructorCache = GetConstructorCache(autoCompleteDependencies);
                var constructorCacheKey = new ConstructorCacheKey(typeToConstruct, parameters);

                if (constructorCache.ContainsKey(constructorCacheKey))
                {
                    var cachedConstructor = constructorCache[constructorCacheKey];
                    var instanceCreatedWithInjection = TryCreateWithConstructorInjectionWithParameters(typeToConstruct, cachedConstructor, parameters);
                    if (instanceCreatedWithInjection != null)
                    {
                        return instanceCreatedWithInjection;
                    }

                    Log.Warning("Found constructor for type '{0}' in constructor, but it failed to create an instance. Removing the constructor from the cache", typeToConstruct.FullName);

                    constructorCache.Remove(constructorCacheKey);
                }

                Log.Debug("Creating instance of type '{0}' using specific parameters. No constructor found in the cache, so searching for the right one.", typeToConstruct.FullName);

                if (!_typeConstructorsMetadata.ContainsKey(typeToConstruct))
                {
                    _typeConstructorsMetadata.Add(typeToConstruct, new TypeConstructorsMetadata(typeToConstruct));
                }

                var typeConstructorsMetadata = _typeConstructorsMetadata[typeToConstruct];
                var constructors = typeConstructorsMetadata.GetConstructors(parameters.Count(), !autoCompleteDependencies);

                foreach (var constructor in constructors)
                {
                    // Check if this constructor is even possible
                    if (!CanConstructorBeUsed(constructor, autoCompleteDependencies, parameters))
                    {
                        continue;
                    }

                    var instanceCreatedWithInjection = TryCreateWithConstructorInjectionWithParameters(typeToConstruct, constructor, parameters);
                    if (instanceCreatedWithInjection != null)
                    {
                        // We found a constructor that works, cache it
                        constructorCache[constructorCacheKey] = constructor;

                        return instanceCreatedWithInjection;
                    }
                }

                Log.Debug("No constructor could be used, cannot construct type '{0}' with the specified parameters.", typeToConstruct.FullName);

                return null;
            }
        }

        /// <summary>
        /// Gets the constructor cache depending on whether the dependencies should be auto completed.
        /// </summary>
        /// <param name="autoCompleteDependencies">if set to <c>true</c>, the dependencies should be auto completed.</param>
        /// <returns>The correct dictionary.</returns>
        private Dictionary<ConstructorCacheKey, ConstructorInfo> GetConstructorCache(bool autoCompleteDependencies)
        {
            if (autoCompleteDependencies)
            {
                return _specificConstructorCacheWithAutoCompletion;
            }

            return _specificConstructorCacheWithoutAutoCompletion;
        }

        /// <summary>
        /// Determines whether the specified constructor can be used for dependency injection.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="autoCompleteDependencies">if set to <c>true</c>, additional dependencies can be completed from the <see cref="IServiceLocator"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if this instance [can constructor be used] the specified constructor; otherwise, <c>false</c>.</returns>
        private bool CanConstructorBeUsed(ConstructorInfo constructor, bool autoCompleteDependencies, params object[] parameters)
        {
            bool validConstructor = true;
            if (parameters.Length > 0)
            {
                var ctorParameters = constructor.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    var ctorParameter = ctorParameters[i];
                    var ctorParameterType = ctorParameter.ParameterType;

                    if (!IsValidParameterValue(ctorParameterType, parameters[i]))
                    {
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
                            if (!_dependencyResolver.CanResolve(ctorParameters[j].ParameterType))
                            {
                                validConstructor = false;
                                break;
                            }
                        }
                    }
                }
            }

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
            bool isParameterNull = (parameterValue == null);
            bool isCtorParameterValueType = parameterType.IsValueTypeEx();
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

            // 3: check if the types are acceptable
            if (!parameterType.IsInstanceOfTypeEx(parameterValue))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to create the service with the specified constructor by retrieving all values from the
        /// service locator for the arguments.
        /// <para />
        /// This method will not throw an exception when the invocation fails.
        /// </summary>
        /// <param name="typeToConstruct">Type of the service.</param>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns>The instantiated service or <c>null</c> if the instantiation fails.</returns>
        private object TryCreateWithConstructorInjection(Type typeToConstruct, ConstructorInfo constructorInfo)
        {
            var parametersList = new List<Type>();
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                var parameterType = parameterInfo.ParameterType;
                parametersList.Add(parameterType);
            }

            var parametersArray = parametersList.ToArray();
            if (!_dependencyResolver.CanResolveAll(parametersArray))
            {
                return null;
            }

            var parameters = _dependencyResolver.ResolveAll(parametersArray);

            return TryCreateWithConstructorInjectionWithParameters(typeToConstruct, constructorInfo, parameters);
        }

        /// <summary>
        /// Tries to create the service with the specified constructor using the specified parameters.
        /// <para />
        /// This method will not throw an exception when the invocation fails.
        /// </summary>
        /// <remarks>
        /// Note that this method does not require an implementation of <see cref="TypeRequestPath"/> because this already has the parameter values
        /// and thus cannot lead to invalid circular dependencies.
        /// </remarks>
        /// <param name="typeToConstruct">Type of the service.</param>
        /// <param name="constructor">The constructor info.</param>
        /// <param name="parameters">The parameters to pass into the constructor.</param>
        /// <returns>The instantiated service or <c>null</c> if the instantiation fails.</returns>
        private object TryCreateWithConstructorInjectionWithParameters(Type typeToConstruct, ConstructorInfo constructor, object[] parameters)
        {
            if (constructor.IsStatic)
            {
                Log.Debug("Cannot use static constructor to initialize type '{0}'", typeToConstruct.FullName);
                return null;
            }

            try
            {
                var finalParameters = new List<object>(parameters);
                var ctorParameters = constructor.GetParameters();
                for (int i = parameters.Length; i < ctorParameters.Length; i++)
                {
                    var ctorParameterValue = _dependencyResolver.Resolve(ctorParameters[i].ParameterType);
                    finalParameters.Add(ctorParameterValue);
                }

                var instance = constructor.Invoke(finalParameters.ToArray());

                InitializeAfterConstruction(instance);

                return instance;
            }
#if NET
            catch (MissingMethodException)
            {
                // Ignore, we accept this
            }
#endif
            catch (TargetParameterCountException)
            {
                // Ignore, we accept this
            }
            catch (Exception ex)
            {
                // Real exceptions bubble up, otherwise return null
                Log.Error(ex, "Failed to instantiate type '{0}', but this was an unexpected error", typeToConstruct.FullName);
                throw ex.InnerException ?? ex;
            }

            Log.Debug("Failed to create instance using dependency injection for type '{0}' using constructor '{1}'",
                      typeToConstruct.FullName, constructor.GetSignature());

            return null;
        }

        /// <summary>
        /// Clears the cache of all constructors.
        /// <para />
        /// This call is normally not necessary since the type factory should keep an eye on the 
        /// <see cref="IServiceLocator.TypeRegistered"/> event to invalidate the cache.
        /// </summary>
        public void ClearCache()
        {
            lock (_lockObject)
            {
                // Note that we don't clear the constructor metadata cache, constructors on types normally don't change during an
                // application lifetime

                _constructorCache.Clear();
                _specificConstructorCacheWithoutAutoCompletion.Clear();
                _specificConstructorCacheWithAutoCompletion.Clear();
            }

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
        #endregion

        private class ConstructorCacheKey
        {
            private readonly int _hashCode;

            #region Constructors
            public ConstructorCacheKey(Type type, object[] parameters)
            {
                string key = type.GetSafeFullName();
                foreach (var parameter in parameters)
                {
                    key += "_" + ObjectToStringHelper.ToFullTypeString(parameter);
                }

                Key = key;
                _hashCode = Key.GetHashCode();
            }
            #endregion

            #region Properties
            public string Key { get; private set; }
            #endregion

            #region Methods
            public override bool Equals(object obj)
            {
                var cacheKey = obj as ConstructorCacheKey;
                if (cacheKey == null)
                {
                    return false;
                }

                return Equals(cacheKey);
            }

            private bool Equals(ConstructorCacheKey other)
            {
                return string.Equals(Key, other.Key, StringComparison.Ordinal);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
            #endregion
        }

        private class TypeConstructorsMetadata
        {
            private readonly ICacheStorage<string, List<ConstructorInfo>> _callCache = new CacheStorage<string, List<ConstructorInfo>>();

            public TypeConstructorsMetadata(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }

            public List<ConstructorInfo> GetConstructors(int parameterCount, bool mustMatchExactCount)
            {
                string key = string.Format("{0}_{1}", parameterCount, mustMatchExactCount);

                return _callCache.GetFromCacheOrFetch(key, () =>
                {
                    if (mustMatchExactCount)
                    {
                        return (from constructor in Type.GetConstructorsEx()
                                where constructor.GetParameters().Length >= parameterCount
                                select constructor).ToList();
                    }

                    return (from constructor in Type.GetConstructorsEx()
                            where constructor.GetParameters().Length >= parameterCount
                            orderby constructor.GetParameters().Length descending 
                            select constructor).ToList();
                });
            }
        }
    }
}