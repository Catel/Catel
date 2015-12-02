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
            #region Constructors
            public RegisteredInstanceInfo(ServiceLocatorRegistration registration, object instance)
                : base(registration.DeclaringType, registration.ImplementingType, registration.Tag, registration.RegistrationType, registration.CreateServiceFunc)
            {
                ImplementingInstance = instance;
            }
            #endregion

            #region Properties
            public object ImplementingInstance { get; private set; }
            #endregion
        }
        #endregion

        #region Nested type: ServiceInfo
        [DebuggerDisplay("{Type} ({Tag})")]
        private class ServiceInfo
        {
            private int _hash;

            #region Constructors
            public ServiceInfo(Type type, object tag)
            {
                Type = type;
                Tag = tag;
            }
            #endregion

            #region Properties
            public Type Type { get; private set; }

            public object Tag { get; private set; }

            public int Hash
            {
                get
                {
                    if (_hash == 0)
                    {
                        _hash = HashHelper.CombineHash(Type.GetHashCode(), Tag != null ? Tag.GetHashCode() : 0);
                    }

                    return _hash;
                }
            }
            #endregion

            #region Methods
            public override int GetHashCode()
            {
                return Hash;
            }

            public override bool Equals(object obj)
            {
                var objAsServiceInfo = obj as ServiceInfo;
                if (objAsServiceInfo == null)
                {
                    return false;
                }

                return objAsServiceInfo.Hash == Hash;
            }
            #endregion
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
        private TypeRequestPath _currentTypeRequestPath;

        /// <summary>
        /// The type factory.
        /// </summary>
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        public ServiceLocator()
        {
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
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the instance of the default service locator. This property serves as as singleton.
        /// </summary>
        /// <value>The default instance.</value>
        public static IServiceLocator Default
        {
            get
            {
                return IoCConfiguration.DefaultServiceLocator;
            }
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

            lock (this)
            {
                // Always check via IsTypeRegistered, allow late-time registration
                if (!IsTypeRegistered(serviceType, tag))
                {
                    return null;
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);
                var registeredTypeInfo = _registeredTypes[serviceInfo];
                bool hasSingletonInstance = (registeredTypeInfo.RegistrationType == RegistrationType.Singleton) && _registeredInstances.ContainsKey(serviceInfo);

                return new RegistrationInfo(registeredTypeInfo.DeclaringType, registeredTypeInfo.ImplementingType, registeredTypeInfo.RegistrationType, hasSingletonInstance);
            }
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

            var serviceInfo = new ServiceInfo(serviceType, tag);

            lock (this)
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
                if (serviceType.IsGenericTypeEx())
                {
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
                }

                // CTL-271 Support generic lists (array specific type)
                // TODO: Can register, 

                // Last resort
                var eventArgs = new MissingTypeEventArgs(serviceType);
                MissingType.SafeInvoke(this, eventArgs);

                if (eventArgs.ImplementingInstance != null)
                {
                    Log.Debug("Late registering type '{0}' to instance of type '{1}' via MissingTypeEventArgs.ImplementingInstance", serviceType.FullName, eventArgs.ImplementingInstance.GetType().FullName);

                    RegisterInstance(serviceType, eventArgs.ImplementingInstance, eventArgs.Tag, this);
                    return true;
                }

                if (eventArgs.ImplementingType != null)
                {
                    Log.Debug("Late registering type '{0}' to type '{1}' via MissingTypeEventArgs.ImplementingType", serviceType.FullName, eventArgs.ImplementingType.FullName);

                    RegisterType(serviceType, eventArgs.ImplementingType, eventArgs.Tag, eventArgs.RegistrationType, true, this, null);
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
            lock (this)
            {
                // Required to support the MissingTypeEventArgs
                if (!IsTypeRegistered(serviceType, tag))
                {
                    return false;
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);


                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    return _registeredInstances[serviceInfo].RegistrationType == RegistrationType.Singleton;
                }

                if (_registeredTypes.ContainsKey(serviceInfo))
                {
                    return _registeredTypes[serviceInfo].RegistrationType == RegistrationType.Singleton;
                }
            }

            return false;
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

        /// <summary>
        /// Registers an implementation of a service using a create type callback, but only if the type is not yet registered.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="createServiceFunc">The create service function.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="createServiceFunc" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public void RegisterType(Type serviceType, Func<ServiceLocatorRegistration, object> createServiceFunc, object tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
        {
            Argument.IsNotNull("createServiceFunc", createServiceFunc);

            RegisterType(serviceType, null, tag, registrationType, registerIfAlreadyRegistered, this, createServiceFunc);
        }

        /// <summary>
        /// Resolves an instance of the type registered on the service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns>An instance of the type registered on the service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="TypeNotRegisteredException">The type is not found in any container.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public object ResolveType(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (this)
            {
                var isTypeRegistered = IsTypeRegistered(serviceType, tag);

                if (!isTypeRegistered)
                {
                    if (CanResolveNonAbstractTypesWithoutRegistration && serviceType.IsClassEx() && !serviceType.IsAbstractEx())
                    {
                        return _typeFactory.CreateInstance(serviceType);
                    }

                    ThrowTypeNotRegisteredException(serviceType);
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);
                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    return _registeredInstances[serviceInfo].ImplementingInstance;
                }

                // If a type is registered, the original container is always known
                return ResolveTypeFromKnownContainer(serviceType, tag);
            }
        }

        /// <summary>
        /// Resolves all instances of the type registered on the service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>All instance of the type registered on the service.</returns>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        public IEnumerable<object> ResolveTypes(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            var resolvedInstances = new List<object>();

            lock (this)
            {
                for (int i = 0; i < _registeredTypes.Keys.Count; i++)
                {
                    var serviceInfo = _registeredTypes.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        try
                        {
                            resolvedInstances.Add(ResolveType(serviceInfo.Type, serviceInfo.Tag));
                        }
                        catch (TypeNotRegisteredException ex)
                        {
                            Log.Debug(ex, "Failed to resolve type '{0}', returning null", ex.RequestedType.GetSafeFullName(false));
                        }
                    }
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
        public bool AreAllTypesRegistered(params Type[] types)
        {
            Argument.IsNotNull("types", types);

            lock (this)
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
        public object[] ResolveAllTypes(params Type[] types)
        {
            Argument.IsNotNull("types", types);

            lock (this)
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

        /// <summary>
        /// Removes the registered type with the specific tag.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag of the registered the service. The default value is <c>null</c>.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public void RemoveType(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (this)
            {
                var serviceInfo = new ServiceInfo(serviceType, tag);
                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    _registeredInstances.Remove(serviceInfo);
                }

                if (_registeredTypes.ContainsKey(serviceInfo))
                {
                    _registeredTypes.Remove(serviceInfo);
                }
            }
        }

        /// <summary>
        /// Removes all registered types of a certain service type.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public void RemoveAllTypes(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (this)
            {
                // Instances
                for (int i = _registeredInstances.Count - 1; i >= 0; i--)
                {
                    var serviceInfo = _registeredInstances.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        _registeredInstances.Remove(serviceInfo);
                    }
                }

                // Registration
                for (int i = _registeredTypes.Count - 1; i >= 0; i--)
                {
                    var serviceInfo = _registeredTypes.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        _registeredTypes.Remove(serviceInfo);
                    }
                }
            }
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
        /// Occurs when a type is instantiated in the service locator.
        /// </summary>
        public event EventHandler<TypeInstantiatedEventArgs> TypeInstantiated;
        #endregion

        #region Methods
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

            var registeredTypeInfo = new ServiceLocatorRegistration(serviceType, instance.GetType(), tag, RegistrationType.Singleton, x => instance);

            lock (this)
            {
                var serviceInfo = new ServiceInfo(serviceType, tag);

                if (_registeredTypes.ContainsKey(serviceInfo))
                {
                    // Re-use previous subscription
                    registeredTypeInfo = _registeredTypes[serviceInfo];
                }
                else
                {
                    _registeredTypes[serviceInfo] = registeredTypeInfo;
                }

                _registeredInstances[serviceInfo] = new RegisteredInstanceInfo(registeredTypeInfo, instance);
            }

            TypeRegistered.SafeInvoke(this, new TypeRegisteredEventArgs(registeredTypeInfo.DeclaringType, registeredTypeInfo.ImplementingType, tag, RegistrationType.Singleton));

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
        private void RegisterType(Type serviceType, Type serviceImplementationType, object tag, RegistrationType registrationType, bool registerIfAlreadyRegistered, object originalContainer, Func<ServiceLocatorRegistration, object> createServiceFunc)
        {
            Argument.IsNotNull("serviceType", serviceType);

            if (serviceImplementationType == null)
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

            // Outside lock scope for event
            ServiceLocatorRegistration registeredTypeInfo = null;

            lock (this)
            {
                if (!registerIfAlreadyRegistered && IsTypeRegistered(serviceType, tag))
                {
                    //Log.Debug("Type '{0}' already registered, will not overwrite registration", serviceType.FullName);
                    return;
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);
                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    _registeredInstances.Remove(serviceInfo);
                }

                Log.Debug("Registering type '{0}' to type '{1}'", serviceType.FullName, serviceImplementationType.FullName);

                registeredTypeInfo = new ServiceLocatorRegistration(serviceType, serviceImplementationType, tag, registrationType,
                    x => (createServiceFunc != null) ? createServiceFunc(x) : CreateServiceInstance(x));
                _registeredTypes[serviceInfo] = registeredTypeInfo;
            }

            TypeRegistered.SafeInvoke(this, new TypeRegisteredEventArgs(registeredTypeInfo.DeclaringType, registeredTypeInfo.ImplementingType, tag, registeredTypeInfo.RegistrationType));

            Log.Debug("Registered type '{0}' to type '{1}'", serviceType.FullName, serviceImplementationType.FullName);
        }

        /// <summary>
        /// Resolves the type from a known container.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns>An instance of the type registered on the service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The type is not found in any container.</exception>
        private object ResolveTypeFromKnownContainer(Type serviceType, object tag)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (this)
            {
                var typeRequestInfo = new TypeRequestInfo(serviceType, tag);
                if (_currentTypeRequestPath == null)
                {
                    _currentTypeRequestPath = new TypeRequestPath(typeRequestInfo, name: "ServiceLocator");
                    _currentTypeRequestPath.IgnoreDuplicateRequestsDirectlyAfterEachother = false;
                }
                else
                {
                    _currentTypeRequestPath.PushType(typeRequestInfo, false);

                    if (!_currentTypeRequestPath.IsValid)
                    {
                        // Reset path for next types that are being resolved
                        var typeRequestPath = _currentTypeRequestPath;
                        _currentTypeRequestPath = null;

                        typeRequestPath.ThrowsExceptionIfInvalid();
                    }
                }

                var serviceInfo = new ServiceInfo(serviceType, tag);
                var registeredTypeInfo = _registeredTypes[serviceInfo];

                object instance = registeredTypeInfo.CreateServiceFunc(registeredTypeInfo);
                if (instance == null)
                {
                    ThrowTypeNotRegisteredException(serviceType);
                }

                if (IsTypeRegisteredAsSingleton(serviceType, tag))
                {
                    RegisterInstance(serviceType, instance, tag, this);
                }

                CompleteTypeRequestPathIfRequired(typeRequestInfo);

                return instance;
            }
        }

        /// <summary>
        /// Creates the service instance.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>The service instance.</returns>
        private object CreateServiceInstance(ServiceLocatorRegistration registration)
        {
            object instance = _typeFactory.CreateInstance(registration.ImplementingType);
            if (instance == null)
            {
                ThrowTypeNotRegisteredException(registration.DeclaringType, "Failed to instantiate the type using the TypeFactory. Check if the required dependencies are registered as well or that the type has a valid constructor that can be used.");
            }

            var handler = TypeInstantiated;
            if (handler != null)
            {
                handler(this, new TypeInstantiatedEventArgs(registration.DeclaringType, registration.ImplementingType,
                    registration.Tag, registration.RegistrationType));
            }

            return instance;
        }

        /// <summary>
        /// Completes the type request path by checking if the currently created type is the same as the first
        /// type meaning that the type is successfully created and the current type request path can be set to <c>null</c>.
        /// </summary>
        /// <param name="typeRequestInfoForTypeJustConstructed">The type request info.</param>
        private void CompleteTypeRequestPathIfRequired(TypeRequestInfo typeRequestInfoForTypeJustConstructed)
        {
            lock (this)
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
        /// Throws the <see cref="TypeNotRegisteredException" /> but will also reset the current type request path.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        private void ThrowTypeNotRegisteredException(Type type, string message = null)
        {
            _currentTypeRequestPath = null;
            // or _currentTypeRequestPath.PopType();

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
            lock (this)
            {
                foreach (var registeredInstance in _registeredInstances)
                {
                    var instance = registeredInstance.Value.ImplementingInstance;
                    if (ReferenceEquals(this, instance))
                    {
                        continue;
                    }

                    var disposable = instance as IDisposable;
                    if (disposable != null)
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
