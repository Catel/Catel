// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Logging;
    using Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IServiceLocator"/> interface.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        #region Classes
        #region Nested type: RegisteredInstanceInfo
        private class RegisteredInstanceInfo : ServiceLocatorRegistration
        {
            public RegisteredInstanceInfo(ServiceLocatorRegistration registration, object instance)
                : base(registration.DeclaringType, registration.ImplementingType, registration.Tag, registration.RegistrationType, registration.CreateServiceFunc)
            {
                ImplementingInstance = instance;
            }

            public object ImplementingInstance { get; private set; }
        }
        #endregion

        #region Nested type: ServiceInfo
        [DebuggerDisplay("{Type} ({Tag})")]
        private class ServiceInfo
        {
            private readonly int _hash;

            public ServiceInfo(Type type, object tag)
            {
                Type = type;
                Tag = tag;
#pragma warning disable HAA0101 // Array allocation for params parameter
                _hash = HashHelper.CombineHash(Type.GetHashCode(), Tag is not null ? Tag.GetHashCode() : 0);
#pragma warning restore HAA0101 // Array allocation for params parameter
            }
            #endregion

            #region Properties
            public Type Type { get; private set; }

            public object Tag { get; private set; }

            public override int GetHashCode()
            {
                return _hash;
            }

            public override bool Equals(object obj)
            {
                var objAsServiceInfo = obj as ServiceInfo;
                if (objAsServiceInfo is null)
                {
                    return false;
                }

                if (objAsServiceInfo._hash != _hash)
                {
                    return false;
                }
                if (objAsServiceInfo.Type != Type)
                {
                    return false;
                }

                return Equals(objAsServiceInfo.Tag, Tag);
            }
        }
        #endregion
        #endregion

        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        /// <summary>
        /// The auto registration manager which handles the registration via attributes.
        /// </summary>
        private readonly ServiceLocatorAutoRegistrationManager _autoRegistrationManager;

        /// <summary>
        /// A list of registered instances of objects.
        /// </summary>
        private readonly Dictionary<ServiceInfo, RegisteredInstanceInfo> _registeredInstances = new Dictionary<ServiceInfo, RegisteredInstanceInfo>();

        /// <summary>
        /// A list of registered types including the types to instantiate.
        /// </summary>
        private readonly Dictionary<ServiceInfo, ServiceLocatorRegistration> _registeredTypes = new Dictionary<ServiceInfo, ServiceLocatorRegistration>();

        /// <summary>
        /// The current type request path.
        /// </summary>
        private readonly ThreadLocal<TypeRequestPath> _currentTypeRequestPath;

        /// <summary>
        /// The type factory.
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        /// The parent service locator.
        /// </summary>
        private readonly IServiceLocator _parentServiceLocator;

        /// <summary>
        /// The lock object.
        /// </summary>
        private readonly object _lockObject = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        public ServiceLocator()
        {
            _currentTypeRequestPath = new ThreadLocal<TypeRequestPath>(() => TypeRequestPath.Root("ServiceLocator"));

            // Must be registered first, already resolved by TypeFactory
            RegisterInstance(typeof(IServiceLocator), this);
            RegisterInstance(typeof(IDependencyResolver), IoCFactory.CreateDependencyResolverFunc(this));

            _typeFactory = IoCFactory.CreateTypeFactoryFunc(this);
            RegisterInstance(typeof(ITypeFactory), _typeFactory);

            _autoRegistrationManager = new ServiceLocatorAutoRegistrationManager(this);

            IgnoreRuntimeIncorrectUsageOfRegisterAttribute = true;
            CanResolveNonAbstractTypesWithoutRegistration = true;

            // Register default implementations
            // TODO: Enable for CTL-272 
            //RegisterType(typeof(ICollection<>), typeof(Collection<>));
            //RegisterType(typeof(IEnumerable<>), typeof(List<>));
            //RegisterType(typeof(IList<>), typeof(List<>));
        }

        /// <summary>
        /// Creates a cloned instance of this service locator with registered (non-instance) types.
        /// </summary>
        /// <param name="serviceLocator">The service locator to clone.</param>
        public ServiceLocator(IServiceLocator serviceLocator)
            : this()
        {
            Argument.IsNotNull(nameof(serviceLocator), serviceLocator);

            _parentServiceLocator = serviceLocator;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the instance of the default service locator. This property serves as as singleton.
        /// </summary>
        /// <value>The default instance.</value>
        public static IServiceLocator Default
        {
            get { return IoCConfiguration.DefaultServiceLocator; }
        }
        #endregion

        #region IServiceLocator Members
        /// <summary>
        /// Gets or sets a value indicating whether the service locator can resolve non abstract types without registration.
        /// </summary>
        public bool CanResolveNonAbstractTypesWithoutRegistration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service locators will ignore incorrect usage of <see cref="ServiceLocatorRegistrationAttribute"/> 
        /// and do not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>
        /// By default, this value is <c>true</c>.
        /// </remarks>
        public bool IgnoreRuntimeIncorrectUsageOfRegisterAttribute
        {
            get { return _autoRegistrationManager.IgnoreRuntimeIncorrectUsageOfRegisterAttribute; }
            set { _autoRegistrationManager.IgnoreRuntimeIncorrectUsageOfRegisterAttribute = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this service locators will automatically register types via attributes.
        /// </summary>
        /// <remarks>
        /// By default, this value is <c>false</c>
        /// </remarks>
        public bool AutoRegisterTypesViaAttributes
        {
            get { return _autoRegistrationManager.AutoRegisterTypesViaAttributes; }
            set { _autoRegistrationManager.AutoRegisterTypesViaAttributes = value; }
        }

        /// <summary>
        /// Gets the registration info about the specified type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="tag">The tag the service is registered with. The default value is <c>null</c>.</param>
        /// <returns>The <see cref="RegistrationInfo" /> or <c>null</c> if the type is not registered.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        public RegistrationInfo GetRegistrationInfo(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (_lockObject)
            {
                // Always check via IsTypeRegistered, allow late-time registration
                if (!IsTypeRegisteredInCurrentLocator(serviceType, tag))
                {
                    return _parentServiceLocator?.GetRegistrationInfo(serviceType, tag);
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);
                var registeredTypeInfo = _registeredTypes[serviceInfo];
                bool hasSingletonInstance = (registeredTypeInfo.RegistrationType == RegistrationType.Singleton) && _registeredInstances.ContainsKey(serviceInfo);

                return new RegistrationInfo(registeredTypeInfo.DeclaringType, registeredTypeInfo.ImplementingType, registeredTypeInfo.RegistrationType, hasSingletonInstance);
            }
        }

        /// <summary>
        /// Determines whether the specified service type is registered with or without tag.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns><c>true</c> if the specified service type is registered; otherwise, <c>false</c>.</returns>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public bool IsTypeRegisteredWithOrWithoutTag(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (_lockObject)
            {
                if (_registeredInstances.Keys.Any(info => info.Type == serviceType))
                {
                    return true;
                }

                if (_registeredTypes.Keys.Any(info => info.Type == serviceType))
                {
                    return true;
                }

                // CTL-161, support generic types
                if (IsTypeRegisteredAsOpenGeneric(serviceType))
                {
                    return true;
                }

                // Last resort
                if (IsTypeRegisteredByMissingTypeHandler(serviceType, null))
                {
                    return true;
                }
            }

            return _parentServiceLocator?.IsTypeRegisteredWithOrWithoutTag(serviceType) ?? false;
        }

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns><c>true</c> if the specified service type is registered; otherwise, <c>false</c>.</returns>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public bool IsTypeRegistered(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            var isRegistered = IsTypeRegisteredInCurrentLocator(serviceType, tag);
            if (!isRegistered && _parentServiceLocator is not null)
            {
                isRegistered = _parentServiceLocator.IsTypeRegistered(serviceType, tag);
            }

            return isRegistered;
        }

        private bool IsTypeRegisteredInCurrentLocator(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            var serviceInfo = new ServiceInfo(serviceType, tag);

            lock (_lockObject)
            {
                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    return true;
                }

                if (_registeredTypes.ContainsKey(serviceInfo))
                {
                    return true;
                }

                // CTL-161, support generic types
                if (serviceType.IsGenericTypeEx() && IsTypeRegisteredAsOpenGeneric(serviceType, tag))
                {
                    return true;
                }

                // CTL-271 Support generic lists (array specific type)
                // TODO: Can register, 

                // Last resort
                if (IsTypeRegisteredByMissingTypeHandler(serviceType, tag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified service type is registered as singleton.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns><c>true</c> if the <paramref name="serviceType" /> type is registered as singleton, otherwise <c>false</c>.</returns>
        public bool IsTypeRegisteredAsSingleton(Type serviceType, object tag = null)
        {
            lock (_lockObject)
            {
                // Required to support the MissingTypeEventArgs
                if (!IsTypeRegisteredInCurrentLocator(serviceType, tag))
                {
                    return _parentServiceLocator?.IsTypeRegisteredAsSingleton(serviceType, tag) ?? false;
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);

                if (_registeredInstances.TryGetValue(serviceInfo, out var instanceInfo))
                {
                    return instanceInfo.RegistrationType == RegistrationType.Singleton;
                }

                if (_registeredTypes.TryGetValue(serviceInfo, out var registration))
                {
                    return registration.RegistrationType == RegistrationType.Singleton;
                }
            }

            return _parentServiceLocator?.IsTypeRegisteredAsSingleton(serviceType, tag) ?? false;
        }

        /// <summary>
        /// Registers a specific instance of a service. 
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="instance"/> is not of the right type.</exception>
        public void RegisterInstance(Type serviceType, object instance, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);
            Argument.IsNotNull("instance", instance);
            Argument.IsOfType("instance", instance, serviceType);

            RegisterInstance(serviceType, instance, tag, this);
        }

        /// <summary>
        /// Registers an implementation of a service, but only if the type is not yet registered.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceImplementationType">The type of the implementation.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceImplementationType" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public void RegisterType(Type serviceType, Type serviceImplementationType, object tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
        {
            Argument.IsNotNull("serviceImplementationType", serviceImplementationType);

            RegisterType(serviceType, serviceImplementationType, tag, registrationType, registerIfAlreadyRegistered,
                this, null);
        }

        public void RegisterType(Type serviceType, Func<ITypeFactory, ServiceLocatorRegistration, object> createServiceFunc, object tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
        {
            Argument.IsNotNull("createServiceFunc", createServiceFunc);

            RegisterType(serviceType, null, tag, registrationType, registerIfAlreadyRegistered, this, createServiceFunc);
        }

        public virtual object ResolveType(Type serviceType, object tag = null)
        {
            return ResolveTypeUsingFactory(_typeFactory, serviceType, tag);
        }

        public virtual object ResolveTypeUsingFactory(ITypeFactory typeFactory, Type serviceType, object tag = null)
        {
            Argument.IsNotNull("typeFactory", typeFactory);
            Argument.IsNotNull("serviceType", serviceType);

            lock (_lockObject)
            {
                var isTypeRegistered = IsTypeRegisteredInCurrentLocator(serviceType, tag);
                if (!isTypeRegistered)
                {
                    if (CanResolveNonAbstractTypesWithoutRegistration && serviceType.IsClassEx() && !serviceType.IsAbstractEx())
                    {
                        return typeFactory.CreateInstanceWithTag(serviceType, tag);
                    }

                    if (_parentServiceLocator is not null)
                    {
                        return _parentServiceLocator.ResolveTypeUsingFactory(typeFactory, serviceType, tag);
                    }

                    ThrowTypeNotRegisteredException(serviceType);
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);

                if (_registeredInstances.TryGetValue(serviceInfo, out var registeredInstanceInfo))
                {
                    return registeredInstanceInfo.ImplementingInstance;
                }

                // If a type is registered, the original container is always known
                return ResolveTypeFromKnownContainer(typeFactory, serviceInfo);
            }
        }

        public IEnumerable<object> ResolveTypes(Type serviceType)
        {
            return ResolveTypesUsingFactory(_typeFactory, serviceType);
        }

        public IEnumerable<object> ResolveTypesUsingFactory(ITypeFactory typeFactory, Type serviceType)
        {
            Argument.IsNotNull("typeFactory", typeFactory);
            Argument.IsNotNull("serviceType", serviceType);

            var resolvedInstances = new List<object>();

            lock (_lockObject)
            {
                for (var i = 0; i < _registeredTypes.Keys.Count; i++)
                {
                    var serviceInfo = _registeredTypes.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        try
                        {
                            resolvedInstances.Add(ResolveTypeUsingFactory(typeFactory, serviceInfo.Type, serviceInfo.Tag));
                        }
                        catch (TypeNotRegisteredException ex)
                        {
                            Log.Debug(ex, "Failed to resolve type '{0}', returning null", ex.RequestedType.GetSafeFullName(false));
                        }
                    }
                }

                if (_parentServiceLocator is not null)
                {
                    resolvedInstances.AddRange(_parentServiceLocator.ResolveTypesUsingFactory(typeFactory, serviceType));
                }
            }

            return resolvedInstances;
        }

        /// <summary>
        /// Determines whether all the specified types are registered with the service locator.
        /// </summary>
        /// <remarks>
        /// Note that this method is written for optimalization by the <see cref="TypeFactory"/>. This means that the 
        /// <see cref="TypeFactory"/> does not need to call the <see cref="ServiceLocator"/> several times to construct
        /// a single type using dependency injection.
        /// <para />
        /// Only use this method if you know what you are doing, otherwise use the <see cref="IsTypeRegistered"/> instead.
        /// </remarks>
        /// <param name="types">The types that should be registered.</param>
        /// <returns><c>true</c> if all the specified types are registered with this instance of the <see cref="IServiceLocator" />; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="types"/> is <c>null</c>.</exception>
        public bool AreMultipleTypesRegistered(params Type[] types)
        {
            Argument.IsNotNull("types", types);

            lock (_lockObject)
            {
                // Note: do NOT rewrite as linq because that is much slower
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (var type in types)
                // ReSharper restore LoopCanBeConvertedToQuery
                {
                    if (!IsTypeRegistered(type))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Resolves all the specified types.
        /// </summary>
        /// <remarks>
        /// Note that this method is written for optimalization by the <see cref="TypeFactory"/>. This means that the 
        /// <see cref="TypeFactory"/> does not need to call the <see cref="ServiceLocator"/> several times to construct
        /// a single type using dependency injection.
        /// <para />
        /// Only use this method if you know what you are doing, otherwise use the <see cref="ResolveType"/> instead.
        /// </remarks>
        /// <param name="types">The collection of types that should be resolved.</param>
        /// <returns>The resolved types in the same order as the types.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="types"/> is <c>null</c>.</exception>
        public object[] ResolveMultipleTypes(params Type[] types)
        {
            Argument.IsNotNull("types", types);

            lock (_lockObject)
            {
                // Note: do NOT rewrite as linq because that is much slower
                var values = new List<object>();
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (var type in types)
                // ReSharper restore LoopCanBeConvertedToQuery
                {
                    var resolvedType = ResolveType(type);
                    values.Add(resolvedType);
                }

                return values.ToArray();
            }
        }

        public bool RemoveType(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            var wasRemoved = false;

            lock (_lockObject)
            {
                var serviceInfo = new ServiceInfo(serviceType, tag);

                if (_registeredInstances.TryGetValue(serviceInfo, out var existingInstance))
                {
                    _registeredInstances.Remove(serviceInfo);
                    wasRemoved = true;
                }

                if (_registeredTypes.TryGetValue(serviceInfo, out var existingRegistration))
                {
                    _registeredTypes.Remove(serviceInfo);
                    wasRemoved = true;
                }

                if (wasRemoved)
                {
                    TypeUnregistered?.Invoke(this, new TypeUnregisteredEventArgs(serviceType, existingRegistration.ImplementingType,
                        tag, existingRegistration.RegistrationType, existingInstance?.ImplementingInstance));
                }
            }

            if (_parentServiceLocator is not null)
            {
                // Important: || wasRemoved must be at the end
                wasRemoved = _parentServiceLocator.RemoveType(serviceType, tag) || wasRemoved;
            }

            return wasRemoved;
        }

        public bool RemoveAllTypes(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            var hasRemoved = false;

            lock (_lockObject)
            {
                for (int i = _registeredTypes.Count - 1; i >= 0; i--)
                {
                    var serviceInfo = _registeredTypes.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        // Important: || hasRemoved must be at the end
                        hasRemoved = RemoveType(serviceType, serviceInfo.Tag) || hasRemoved;
                    }
                }
            }

            if (_parentServiceLocator is not null)
            {
                // Important: || hasRemoved must be at the end
                hasRemoved = _parentServiceLocator.RemoveAllTypes(serviceType) || hasRemoved;
            }

            return hasRemoved;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a type cannot be resolved the by service locator. It first tries to raise this event.
        /// <para/>
        /// If there are no handlers or no handler can fill up the missing type, an exception will be thrown by
        /// the service locator.
        /// </summary>
        public event EventHandler<MissingTypeEventArgs> MissingType;

        /// <summary>
        /// Occurs when a type is registered in the service locator.
        /// </summary>
        public event EventHandler<TypeRegisteredEventArgs> TypeRegistered;

        /// <summary>
        /// Occurs when a type is unregistered in the service locator.
        /// </summary>
        public event EventHandler<TypeUnregisteredEventArgs> TypeUnregistered;

        /// <summary>
        /// Occurs when a type is instantiated in the service locator.
        /// </summary>
        public event EventHandler<TypeInstantiatedEventArgs> TypeInstantiated;
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified service type is registered as open generic.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns><c>true</c> if the specified service type is registered; otherwise, <c>false</c>.</returns>
        private bool IsTypeRegisteredAsOpenGeneric(Type serviceType, object tag = null)
        {
            if (!serviceType.IsGenericTypeEx())
            {
                return false;
            }

            var genericArguments = serviceType.GetGenericArgumentsEx().ToList();
            var hasRealGenericArguments = (from genericArgument in genericArguments
                                           where !string.IsNullOrEmpty(genericArgument.FullName)
                                           select genericArgument).Any();
            if (hasRealGenericArguments)
            {
                var genericType = serviceType.GetGenericTypeDefinitionEx();
                var isOpenGenericTypeRegistered = IsTypeRegistered(genericType, tag);
                if (isOpenGenericTypeRegistered)
                {
                    Log.Debug("An open generic type '{0}' is registered, registering new closed generic type '{1}' based on the open registration", genericType.GetSafeFullName(false), serviceType.GetSafeFullName(false));

                    var registrationInfo = GetRegistrationInfo(genericType, tag);
                    var finalType = registrationInfo.ImplementingType.MakeGenericType(genericArguments.ToArray());

                    RegisterType(serviceType, finalType, tag, registrationInfo.RegistrationType);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified service type is registered by missing type handler.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">Tag to resolve or null</param>
        /// <returns><c>true</c> if the specified service type is registered; otherwise, <c>false</c>.</returns>
        private bool IsTypeRegisteredByMissingTypeHandler(Type serviceType, object tag)
        {
            var missingTypeHandler = MissingType;
            if (missingTypeHandler is not null)
            {
                var eventArgs = new MissingTypeEventArgs(serviceType, tag);
                missingTypeHandler(this, eventArgs);

                if (eventArgs.ImplementingInstance is not null)
                {
                    Log.Debug("Late registering type '{0}' to instance of type '{1}' via MissingTypeEventArgs.ImplementingInstance", serviceType.FullName, eventArgs.ImplementingInstance.GetType().FullName);

                    RegisterInstance(serviceType, eventArgs.ImplementingInstance, eventArgs.Tag, this);
                    return true;
                }

                if (eventArgs.ImplementingType is not null)
                {
                    Log.Debug("Late registering type '{0}' to type '{1}' via MissingTypeEventArgs.ImplementingType", serviceType.FullName, eventArgs.ImplementingType.FullName);

                    RegisterType(serviceType, eventArgs.ImplementingType, eventArgs.Tag, eventArgs.RegistrationType, true, this, null);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Registers a specific instance of an service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The specific instance to register.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <param name="originalContainer">The original container where the instance was found in.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        private void RegisterInstance(Type serviceType, object instance, object tag, object originalContainer)
        {
            Argument.IsNotNull("serviceType", serviceType);
            Argument.IsNotNull("instance", instance);

            Log.Debug("Registering type '{0}' to instance of type '{1}'", serviceType.FullName, instance.GetType().FullName);

            var registeredTypeInfo = new ServiceLocatorRegistration(serviceType, instance.GetType(), tag, RegistrationType.Singleton, (tf, r) => instance);

            lock (_lockObject)
            {
                var serviceInfo = new ServiceInfo(serviceType, tag);

                if (!_registeredTypes.TryGetValue(serviceInfo, out var existingRegisteredTypeInfo))
                {
                    _registeredTypes[serviceInfo] = registeredTypeInfo;
                }
                else
                {
                    // Re-use previous subscription
                    registeredTypeInfo = existingRegisteredTypeInfo;
                }

                _registeredInstances[serviceInfo] = new RegisteredInstanceInfo(registeredTypeInfo, instance);
            }

            TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(registeredTypeInfo.DeclaringType, registeredTypeInfo.ImplementingType, tag, RegistrationType.Singleton));

            Log.Debug("Registered type '{0}' to instance of type '{1}'", serviceType.FullName, instance.GetType().FullName);
        }

        /// <summary>
        /// Registers the specific implementing type for the service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">Type of the implementing.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <param name="registrationType">The registration type.</param>
        /// <param name="registerIfAlreadyRegistered">if set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <param name="originalContainer">The original container where the type was found in.</param>
        /// <param name="createServiceFunc">The create service function.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        private void RegisterType(Type serviceType, Type serviceImplementationType, object tag, RegistrationType registrationType, bool registerIfAlreadyRegistered, object originalContainer, Func<ITypeFactory, ServiceLocatorRegistration, object> createServiceFunc)
        {
            Argument.IsNotNull("serviceType", serviceType);

            // Outside lock scope for event
            ServiceLocatorRegistration registeredTypeInfo = null;

            lock (_lockObject)
            {
                if (!registerIfAlreadyRegistered && IsTypeRegistered(serviceType, tag))
                {
                    //Log.Debug("Type '{0}' already registered, will not overwrite registration", serviceType.FullName);
                    return;
                }

                if (serviceImplementationType is null)
                {
                    // Dynamic late-bound type
                    serviceImplementationType = typeof(LateBoundImplementation);
                }

                if (serviceImplementationType.IsInterfaceEx())
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot register interface type '{0}' as implementation, make sure to specify an actual class", serviceImplementationType.GetSafeFullName(false));
                }

                /* TODO: This code have to be here to ensure the right usage of non-generic overloads of register methods.
                 * TODO: If it is finally accepted then remove it from ServiceLocatorAutoRegistrationManager
                if (serviceImplementationType.IsAbstractEx() || !serviceType.IsAssignableFromEx(serviceImplementationType))
                {
                    string message = string.Format("The type '{0}' is abstract or can't be registered as '{1}'", serviceImplementationType, serviceType);
                    Log.Error(message);
                    throw new InvalidOperationException(message);
                }
                */

                var serviceInfo = new ServiceInfo(serviceType, tag);
                _registeredInstances.Remove(serviceInfo);

                Log.Debug("Registering type '{0}' to type '{1}'", serviceType.FullName, serviceImplementationType.FullName);

                registeredTypeInfo = new ServiceLocatorRegistration(serviceType, serviceImplementationType, tag, registrationType,
                    (tf, reg) => CreateServiceInstanceWrapper(tf, createServiceFunc ?? DefaultCreateServiceFunc, reg));

                _registeredTypes[serviceInfo] = registeredTypeInfo;
            }

            TypeRegistered?.Invoke(this, new TypeRegisteredEventArgs(registeredTypeInfo.DeclaringType, registeredTypeInfo.ImplementingType, tag, registeredTypeInfo.RegistrationType));

            Log.Debug("Registered type '{0}' to type '{1}'", serviceType.FullName, serviceImplementationType.FullName);
        }

        private object ResolveTypeFromKnownContainer(ITypeFactory typeFactory, ServiceInfo serviceInfo)
        {
            Argument.IsNotNull("serviceInfo", serviceInfo);

            lock (_lockObject)
            {
                var previousTypeRequestPath = _currentTypeRequestPath.Value;
                try
                {
                    var typeRequestInfo = new TypeRequestInfo(serviceInfo.Type, serviceInfo.Tag);
                    _currentTypeRequestPath.Value = TypeRequestPath.Branch(previousTypeRequestPath, typeRequestInfo);

                    var registeredTypeInfo = _registeredTypes[serviceInfo];

                    var serviceType = serviceInfo.Type;
                    var tag = serviceInfo.Tag;

                    var instance = registeredTypeInfo.CreateServiceFunc(typeFactory, registeredTypeInfo);
                    if (instance is not null && instance is Type)
                    {
                        instance = _typeFactory.CreateInstanceWithTag((Type)instance, serviceInfo.Tag);
                    }

                    if (instance is null)
                    {
                        ThrowTypeNotRegisteredException(serviceType);
                    }

                    if (IsTypeRegisteredAsSingleton(serviceType, tag))
                    {
                        RegisterInstance(serviceType, instance, tag, this);
                    }

                    return instance;
                }
                finally
                {
                    _currentTypeRequestPath.Value = previousTypeRequestPath;
                }
            }
        }

        private object DefaultCreateServiceFunc(ITypeFactory typeFactory, ServiceLocatorRegistration registration)
        {
            var instance = typeFactory.CreateInstanceWithTag(registration.ImplementingType, registration.Tag);
            return instance;
        }

        private object CreateServiceInstanceWrapper(ITypeFactory typeFactory, Func<ITypeFactory, ServiceLocatorRegistration, object> createServiceFunc, ServiceLocatorRegistration registration)
        {
            var instance = createServiceFunc(typeFactory, registration);
            if (instance is null)
            {
                ThrowTypeNotRegisteredException(registration.DeclaringType, "Failed to instantiate the type using the TypeFactory. Check if the required dependencies are registered as well or that the type has a valid constructor that can be used.");
            }

            var handler = TypeInstantiated;
            if (handler is not null)
            {
                handler(this, new TypeInstantiatedEventArgs(registration.DeclaringType, registration.ImplementingType,
                    registration.Tag, registration.RegistrationType, instance));
            }

            return instance;
        }

        /// <summary>
        /// Throws the <see cref="TypeNotRegisteredException" /> but will also reset the current type request path.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        private void ThrowTypeNotRegisteredException(Type type, string message = null)
        {
            throw Log.ErrorAndCreateException(msg => new TypeNotRegisteredException(type, msg),
                "The type '{0}' is not registered", type.GetSafeFullName(true));
        }
        #endregion

        #region IServiceProvider interface
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.</returns>
        object IServiceProvider.GetService(Type serviceType)
        {
            return ResolveType(serviceType);
        }
        #endregion

        #region IDisposable interface
        /// <summary>
        /// Disposes this instance and all registered instances.
        /// </summary>
        public void Dispose()
        {
            lock (_lockObject)
            {
                foreach (var registeredInstance in _registeredInstances)
                {
                    var instance = registeredInstance.Value.ImplementingInstance;
                    if (ReferenceEquals(this, instance))
                    {
                        continue;
                    }

                    var disposable = instance as IDisposable;
                    if (disposable is not null)
                    {
                        disposable.Dispose();
                    }
                }

                _registeredInstances.Clear();
                _registeredTypes.Clear();
            }
        }
        #endregion
    }
}
