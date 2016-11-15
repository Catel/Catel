// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using ApiCop;
    using ApiCop.Rules;
    using Caching;
    using Logging;
    using Reflection;

#if !XAMARIN
    using System.Dynamic;
#endif

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
        /// Cache containing all last used constructors for a type without auto-completion.
        /// </summary>
        private readonly Dictionary<ConstructorCacheKey, ConstructorInfo> _specificConstructorCacheWithoutAutoCompletion = new Dictionary<ConstructorCacheKey, ConstructorInfo>();

        /// <summary>
        /// Cache containing all last used constructors for a type with auto-completion.
        /// </summary>
        private readonly Dictionary<ConstructorCacheKey, ConstructorInfo> _specificConstructorCacheWithAutoCompletion = new Dictionary<ConstructorCacheKey, ConstructorInfo>();

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
        private TypeRequestPath _currentTypeRequestPath;
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
        /// Marks the specified type as not being created. If this was the only type being constructed, the type request
        /// path will be closed.
        /// </summary>
        /// <param name="typeRequestInfoForTypeJustConstructed">The type request info for type just constructed.</param>
        private void CloseCurrentTypeIfRequired(TypeRequestInfo typeRequestInfoForTypeJustConstructed)
        {
            lock (_serviceLocator.LockObject)
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
            lock (_serviceLocator.LockObject)
            {
                if (_currentTypeRequestPath != null)
                {
                    if (_currentTypeRequestPath.LastType == typeRequestInfoForTypeJustConstructed)
                    {
                        _currentTypeRequestPath.MarkTypeAsCreated(typeRequestInfoForTypeJustConstructed);
                    }

                    if (_currentTypeRequestPath.TypeCount == 0)
                    {
                        _currentTypeRequestPath = null;
                    }
                }
            }
        }

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

            string objectType = ObjectToStringHelper.ToTypeString(obj);

            Log.Debug("Initializing type '{0}' after construction", objectType);

            // TODO: Consider to cache for performance
            var dependencyResolverManager = DependencyResolverManager.Default;
            var dependencyResolver = _serviceLocator.ResolveType<IDependencyResolver>();
            dependencyResolverManager.RegisterDependencyResolverForInstance(obj, dependencyResolver);

            Log.Debug("Injecting properties into type '{0}' after construction", objectType);

            var type = obj.GetType();
            var typeMetaData = GetTypeMetaData(type);
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
        /// <param name="preventCircularDependencies">if set to <c>true</c>, prevent circular dependencies using the <see cref="TypeRequestPath" />.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        private object CreateInstanceWithSpecifiedParameters(Type typeToConstruct, object tag, object[] parameters,
            bool autoCompleteDependencies, bool preventCircularDependencies = true)
        {
            Argument.IsNotNull("typeToConstruct", typeToConstruct);

            if (parameters == null)
            {
                parameters = new object[] { };
            }

            lock (_serviceLocator.LockObject)
            {
                TypeRequestInfo typeRequestInfo = null;

                try
                {
                    if (preventCircularDependencies)
                    {
                        typeRequestInfo = new TypeRequestInfo(typeToConstruct);
                        if (_currentTypeRequestPath == null)
                        {
                            _currentTypeRequestPath = new TypeRequestPath(typeRequestInfo, name: TypeRequestPathName);
                        }
                        else
                        {
                            _currentTypeRequestPath.PushType(typeRequestInfo, true);
                        }
                    }

                    var constructorCache = GetConstructorCache(autoCompleteDependencies);
                    var constructorCacheKey = new ConstructorCacheKey(typeToConstruct, parameters);

                    if (constructorCache.ContainsKey(constructorCacheKey))
                    {
                        var cachedConstructor = constructorCache[constructorCacheKey];
                        var instanceCreatedWithInjection = TryCreateToConstruct(typeToConstruct, cachedConstructor, tag, parameters, false, false);
                        if (instanceCreatedWithInjection != null)
                        {
                            if (preventCircularDependencies)
                            {
                                CompleteTypeRequestPathIfRequired(typeRequestInfo);
                            }

                            return instanceCreatedWithInjection;
                        }

                        Log.Warning("Found constructor for type '{0}' in constructor, but it failed to create an instance. Removing the constructor from the cache", typeToConstruct.FullName);

                        constructorCache.Remove(constructorCacheKey);
                    }

                    Log.Debug("Creating instance of type '{0}' using specific parameters. No constructor found in the cache, so searching for the right one", typeToConstruct.FullName);

                    var typeConstructorsMetadata = GetTypeMetaData(typeToConstruct);
                    var constructors = typeConstructorsMetadata.GetConstructors(parameters.Count(), !autoCompleteDependencies);

                    for (int i = 0; i < constructors.Count; i++)
                    {
                        var constructor = constructors[i];

                        var instanceCreatedWithInjection = TryCreateToConstruct(typeToConstruct, constructor, tag, parameters, true, i < constructors.Count - 1);
                        if (instanceCreatedWithInjection != null)
                        {
                            if (preventCircularDependencies)
                            {
                                CompleteTypeRequestPathIfRequired(typeRequestInfo);
                            }

                            // We found a constructor that works, cache it
                            constructorCache[constructorCacheKey] = constructor;

                            // Only update the rule when using a constructor for the first time, not when using it from the cache
                            ApiCop.UpdateRule<TooManyDependenciesApiCopRule>("TypeFactory.LimitDependencyInjection",
                                x => x.SetNumberOfDependenciesInjected(typeToConstruct, constructor.GetParameters().Count()));

                            return instanceCreatedWithInjection;
                        }
                    }

                    Log.Debug("No constructor could be used, cannot construct type '{0}' with the specified parameters", typeToConstruct.FullName);
                }
                catch (CircularDependencyException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (preventCircularDependencies)
                    {
                        CloseCurrentTypeIfRequired(typeRequestInfo);
                    }

                    Log.Warning(ex, "Failed to construct type '{0}'", typeToConstruct.FullName);

                    throw;
                }

                if (preventCircularDependencies)
                {
                    CloseCurrentTypeIfRequired(typeRequestInfo);
                }

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
        /// Gets the constructors metadata.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="TypeMetaData"/>.</returns>
        private TypeMetaData GetTypeMetaData(Type type)
        {
            lock (_serviceLocator.LockObject)
            {
                if (!_typeConstructorsMetadata.ContainsKey(type))
                {
                    _typeConstructorsMetadata.Add(type, new TypeMetaData(type));
                }

                var typeConstructorsMetadata = _typeConstructorsMetadata[type];
                return typeConstructorsMetadata;
            }
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
            Log.Debug("Checking if constructor '{0}' can be used", constructor.GetSignature());

            if (constructor.IsStatic)
            {
                Log.Debug("Constructor is not valid because it is static");
                return false;
            }

            bool validConstructor = true;
            var ctorParameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var ctorParameter = ctorParameters[i];
                var ctorParameterType = ctorParameter.ParameterType;

                if (!IsValidParameterValue(ctorParameterType, parameters[i]))
                {
                    Log.Debug("Constructor is not valid because value '{0}' cannot be used for parameter '{0}'",
                        ObjectToStringHelper.ToString(parameters[i]), ctorParameter.Name);

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

                        if (!_serviceLocator.IsTypeRegistered(parameterTypeToResolve))
                        {
                            Log.Debug("Constructor is not valid because parameter '{0}' cannot be resolved from the dependency resolver", parameterToResolve.Name);

                            validConstructor = false;
                            break;
                        }
                    }
                }
            }

            Log.Debug("The constructor is valid and can be used");

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
        /// <returns>The instantiated service or <c>null</c> if the instantiation fails.</returns>
        /// <remarks>Note that this method does not require an implementation of
        /// <see cref="TypeRequestPath" /> because this already has the parameter values
        /// and thus cannot lead to invalid circular dependencies.</remarks>
        private object TryCreateToConstruct(Type typeToConstruct, ConstructorInfo constructor, object tag, object[] parameters,
            bool checkConstructor, bool hasMoreConstructorsLeft)
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
                for (int i = parameters.Length; i < ctorParameters.Length; i++)
                {
                    object ctorParameterValue = null;

                    if (tag != null && _serviceLocator.IsTypeRegistered(ctorParameters[i].ParameterType, tag))
                    {
                        // Use preferred tag
                        ctorParameterValue = _serviceLocator.ResolveType(ctorParameters[i].ParameterType, tag);
                    }
                    else
                    {
                        // No tag or fallback to default without tag
                        ctorParameterValue = _serviceLocator.ResolveType(ctorParameters[i].ParameterType);
                    }

                    finalParameters.Add(ctorParameterValue);
                }

                var finalParametersArray = finalParameters.ToArray();

                Log.Debug("Calling constructor.Invoke with the right parameters");

                var instance = constructor.Invoke(finalParametersArray);

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
                Log.Error(ex, "Failed to instantiate type '{0}', but this was an unexpected error", typeToConstruct.FullName);
                throw;
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
            // Note that we don't clear the constructor metadata cache, constructors on types normally don't change during an
            // application lifetime

            // Clear cache isn't really that important, it's better to prevent deadlocks. How can a deadlock occur? If thread x is creating 
            // a type and loads an assembly, but thread y is also loading an assembly. Thread x will lock because it's creating the type, 
            // thread y will lock because it wants to clear the cache because new types were added. In that case ignore clearing the cache

            if (Monitor.TryEnter(_serviceLocator))
            {
                try
                {
                    _specificConstructorCacheWithoutAutoCompletion.Clear();
                    _specificConstructorCacheWithAutoCompletion.Clear();
                }
                finally
                {
                    Monitor.Exit(_serviceLocator);
                }
            }

            //Log.Debug("Cleared type constructor cache");
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
            public ConstructorCacheKey(Type type, object[] parameters)
            {
                string key = type.GetSafeFullName(true);
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

        private class TypeMetaData
        {
            private readonly ICacheStorage<string, List<ConstructorInfo>> _callCache = new CacheStorage<string, List<ConstructorInfo>>();
            private Dictionary<PropertyInfo, InjectAttribute> _injectedProperties;
            private readonly object _lockObject = new object();

            public TypeMetaData(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }

            public Dictionary<PropertyInfo, InjectAttribute> GetInjectedProperties()
            {
                lock (_lockObject)
                {
                    if (_injectedProperties == null)
                    {
                        _injectedProperties = new Dictionary<PropertyInfo, InjectAttribute>();

                        var properties = Type.GetPropertiesEx();
                        foreach (var property in properties)
                        {
                            var injectAttribute = property.GetCustomAttributeEx(typeof(InjectAttribute), false) as InjectAttribute;
                            if (injectAttribute != null)
                            {
                                if (injectAttribute.Type == null)
                                {
                                    injectAttribute.Type = property.PropertyType;
                                }

                                _injectedProperties.Add(property, injectAttribute);
                            }
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
                                    where AttributeHelper.IsDecoratedWithAttribute<InjectionConstructorAttribute>(ctor)
                                    select ctor).ToList();
                }
                else
                {
                    constructors = (from ctor in constructors
                                    where !AttributeHelper.IsDecoratedWithAttribute<InjectionConstructorAttribute>(ctor)
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
                if (parameterType == typeof(DynamicObject))
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