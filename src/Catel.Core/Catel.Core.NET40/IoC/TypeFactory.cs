// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

//#define DISABLE_TYPEPATH
//#define DISABLE_RESOLVEALLTYPESATONCE

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;
    using Reflection;

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
        /// The default type factory for the default service locator.
        /// </summary>
        private static TypeFactory _default;

        /// <summary>
        /// Cache containing all last used constructors for a type.
        /// </summary>
        private readonly Dictionary<Type, ConstructorInfo> _constructorCache = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// Cache containing all last used constructors for a type.
        /// </summary>
        private readonly Dictionary<Type, ConstructorInfo> _specificConstructorCache = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// Cache containing whether a type can be created using <c>Activator.CreateInstance</c>.
        /// </summary>
        private readonly Dictionary<Type, bool> _canUseActivatorCache = new Dictionary<Type, bool>();

        /// <summary>
        /// The service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

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
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public TypeFactory(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            _serviceLocator = serviceLocator;
            _serviceLocator.TypeRegistered += OnServiceLocatorTypeRegistered;
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
                if (_default == null)
                {
                    _default = new TypeFactory(ServiceLocator.Default);
                }

                return _default;
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

                    if (_constructorCache.ContainsKey(typeToConstruct))
                    {
                        var instanceCreatedWithInjection = TryCreateWithConstructorInjection(typeToConstruct, _constructorCache[typeToConstruct]);
                        if (instanceCreatedWithInjection != null)
                        {
#if !DISABLE_TYPEPATH
                            CompleteTypeRequestPathIfRequired(typeRequestInfo);
#endif

                            return instanceCreatedWithInjection;
                        }

                        Log.Warning("Found constructor for type '{0}' in constructor, but it failed to create an instance. Removing the constructor from the cache", typeToConstruct.FullName);

                        _constructorCache.Remove(typeToConstruct);
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
                            _constructorCache[typeToConstruct] = constructor;

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

            lock (_lockObject)
            {
                if (_specificConstructorCache.ContainsKey(typeToConstruct))
                {
                    var instanceCreatedWithInjection = TryCreateWithConstructorInjectionWithParameters(typeToConstruct, _specificConstructorCache[typeToConstruct], parameters);
                    if (instanceCreatedWithInjection != null)
                    {
                        return instanceCreatedWithInjection;
                    }

                    Log.Warning("Found constructor for type '{0}' in constructor, but it failed to create an instance. Removing the constructor from the cache", typeToConstruct.FullName);

                    _specificConstructorCache.Remove(typeToConstruct);
                }

                Log.Debug("Creating instance of type '{0}' using specific parameters. No constructor found in the cache, so searching for the right one.", typeToConstruct.FullName);

                var constructors = (from constructor in typeToConstruct.GetConstructorsEx()
                                    where constructor.GetParameters().Count() == parameters.Length
                                    select constructor).ToList();

                foreach (var constructor in constructors)
                {
                    // Check if this constructor is even possible
                    bool validConstructor = true;
                    if (parameters.Length > 0)
                    {
                        var ctorParameters = constructor.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var ctorParameter = ctorParameters[i];
                            var ctorParameterType = ctorParameter.ParameterType;

                            // 1: check if value is null and if the ctor accepts that
                            bool isParameterNull = (parameters[i] == null);
                            bool isCtorParameterValueType = ctorParameterType.IsValueTypeEx();
                            if (isParameterNull && isCtorParameterValueType)
                            {
                                validConstructor = false;
                                break;
                            }

                            if (isParameterNull)
                            {
                                // valid
                                continue;
                            }

                            // 2: check if the values are both value or reference types 
                            var parameterType = parameters[i].GetType();
                            bool isParameterValueType = parameterType.IsValueTypeEx();
                            if (isParameterValueType != isCtorParameterValueType)
                            {
                                validConstructor = false;
                                break;
                            }

                            // 3: check if the types are acceptable
                            if (!ctorParameterType.IsInstanceOfTypeEx(parameters[i]))
                            {
                                validConstructor = false;
                                break;
                            }
                        }
                    }

                    if (!validConstructor)
                    {
                        continue;
                    }

                    var instanceCreatedWithInjection = TryCreateWithConstructorInjectionWithParameters(typeToConstruct, constructor, parameters);
                    if (instanceCreatedWithInjection != null)
                    {
                        // We found a constructor that works, cache it
                        _constructorCache[typeToConstruct] = constructor;

                        return instanceCreatedWithInjection;
                    }
                }

                Log.Debug("No constructor could be used, cannot construct type '{0}' with the specified parameters.", typeToConstruct.FullName);

                return null;
            }
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
            var objAsINeedCustomInitialization = obj as INeedCustomInitialization;
            if (objAsINeedCustomInitialization != null)
            {
                objAsINeedCustomInitialization.Initialize();
            }
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
#if DISABLE_RESOLVEALLTYPESATONCE
            var parameterValues = new List<object>();
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                var parameterType = parameterInfo.ParameterType;

                if (!_serviceLocator.IsTypeRegistered(parameterType))
                {
                    return null;
                }

                // In a try/catch because it might, in theory, be possible that the service is unregistered between the is registered
                // check and the resolve check. Because the service locator will throw an exception for non-registered types, we can
                // mimick a lock using a try/catch
                try
                {
                    var parameter = _serviceLocator.ResolveType(parameterType);
                    parameterValues.Add(parameter);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "The service type '{0}' was registered, but is not anymore. This happening is highly unlikely, but is possible in theory. Please contact the development team.", parameterType.FullName);
                    return null;
                }
            }

            var parameters = parameterValues.ToArray();
#else
            var parametersList = new List<Type>();
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                var parameterType = parameterInfo.ParameterType;
                parametersList.Add(parameterType);
            }

            var parametersArray = parametersList.ToArray();
            if (!_serviceLocator.AreAllTypesRegistered(parametersArray))
            {
                return null;
            }

            var parameters = _serviceLocator.ResolveAllTypes(parametersArray);
#endif

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
        /// <param name="constructorInfo">The constructor info.</param>
        /// <param name="parameters">The parameters to pass into the constructor.</param>
        /// <returns>The instantiated service or <c>null</c> if the instantiation fails.</returns>
        private object TryCreateWithConstructorInjectionWithParameters(Type typeToConstruct, ConstructorInfo constructorInfo, object[] parameters)
        {
            if (constructorInfo.IsStatic)
            {
                Log.Debug("Cannot use static constructor to initialize type '{0}'", typeToConstruct.FullName);
                return null;
            }

            try
            {
                var instance = constructorInfo.Invoke(parameters);

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
                typeToConstruct.FullName, constructorInfo.GetSignature());

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
                _constructorCache.Clear();
                _specificConstructorCache.Clear();
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
    }
}