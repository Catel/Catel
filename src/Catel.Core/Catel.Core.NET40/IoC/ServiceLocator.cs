// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Scoping;
    using Logging;
    using Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IServiceLocator"/> interface.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        #region Classes

        #region Nested type: RegisteredInstanceInfo
        private class RegisteredInstanceInfo : RegisteredTypeInfo
        {
            #region Constructors
            public RegisteredInstanceInfo(Type declaringType, object implementingInstance, RegistrationType registrationType, object tag, object originalContainer)
                : base(declaringType, implementingInstance.GetType(), tag, registrationType, originalContainer)
            {
                ImplementingInstance = implementingInstance;
            }
            #endregion

            #region Properties
            public object ImplementingInstance { get; private set; }
            #endregion
        }
        #endregion

        #region Nested type: RegisteredTypeInfo
        private class RegisteredTypeInfo
        {
            #region Constructors
            public RegisteredTypeInfo(Type declaringType, Type implementingType, object tag, RegistrationType registrationType, object originalContainer)
            {
                Argument.IsNotNull("declaringType", declaringType);
                Argument.IsNotNull("implementingType", implementingType);
                Argument.IsNotNull("originalContainer", originalContainer);

                DeclaringType = declaringType;
                DeclaringTypeName = declaringType.AssemblyQualifiedName;

                ImplementingType = implementingType;
                ImplementingTypeName = implementingType.AssemblyQualifiedName;

                Tag = tag;
                RegistrationType = registrationType;
                OriginalContainer = originalContainer;
            }
            #endregion

            #region Properties
            public Type DeclaringType { get; private set; }

            public string DeclaringTypeName { get; private set; }

            public Type ImplementingType { get; private set; }

            public string ImplementingTypeName { get; private set; }

            public RegistrationType RegistrationType { get; private set; }

            public object Tag { get; private set; }

            public object OriginalContainer { get; private set; }
            #endregion
        }
        #endregion

        #region Nested type: ServiceInfo
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
        /// The _external containers.
        /// </summary>
        private readonly Dictionary<object, IExternalContainerHelper> _externalContainers = new Dictionary<object, IExternalContainerHelper>();

        /// <summary>
        /// A list of registered instances of objects.
        /// </summary>
        private readonly Dictionary<ServiceInfo, RegisteredInstanceInfo> _registeredInstances = new Dictionary<ServiceInfo, RegisteredInstanceInfo>();

        /// <summary>
        /// A list of registered types including the types to instantiate.
        /// </summary>
        private readonly Dictionary<ServiceInfo, RegisteredTypeInfo> _registeredTypes = new Dictionary<ServiceInfo, RegisteredTypeInfo>();

        /// <summary>
        /// The _supported external containers.
        /// </summary>
        private readonly List<IExternalContainerHelper> _supportedExternalContainers = new List<IExternalContainerHelper>();

        /// <summary>
        /// The types currently being exported.
        /// </summary>
        private readonly List<ServiceInfo> _typesCurrentlyBeingExported = new List<ServiceInfo>();

        /// <summary>
        /// The synchronization object.
        /// </summary>
        private readonly object _syncObject = new object();

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

            var dependencyResolver = new CatelDependencyResolver(this);
            _typeFactory = new TypeFactory(dependencyResolver);
            _autoRegistrationManager = new ServiceLocatorAutoRegistrationManager(this);

            IgnoreRuntimeIncorrectUsageOfRegisterAttribute = true;
            AutomaticallyKeepContainersSynchronized = true;
            SupportDependencyInjection = true;
            CanResolveNonAbstractTypesWithoutRegistration = true;

#if !NETFX_CORE && !PCL
#if !WINDOWS_PHONE
            RegisterExternalContainerHelper(new MefHelper());
            RegisterExternalContainerHelper(new UnityHelper());
            RegisterExternalContainerHelper(new WindsorHelper());
#endif
            RegisterExternalContainerHelper(new NinjectHelper());
#endif

            RegisterInstance(typeof(ITypeFactory), _typeFactory);
            RegisterInstance(typeof(IDependencyResolver), dependencyResolver);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance of the default service locator. This property serves as as singleton.
        /// </summary>
        /// <value>The instance.</value>
        [ObsoleteEx(Replacement = "ServiceLocator.Default", TreatAsErrorFromVersion = "3.5", RemoveInVersion = "4.0")]
        public static IServiceLocator Instance
        {
            get { return Default; }
        }

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
        /// Gets or sets a value indicating whether the service locator should keep the external containers
        /// in sync with the current <see cref="ServiceLocator"/>.
        /// <para />
        /// This means that after every change inside the container, this class will automatically invoke the <see cref="ExportToExternalContainers"/> method.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the service locator should keep all containers synchronized; otherwise, <c>false</c>.
        /// </value>
        public bool AutomaticallyKeepContainersSynchronized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service locator can resolve non abstract types without registration.
        /// </summary>
        public bool CanResolveNonAbstractTypesWithoutRegistration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service locator supports dependency inject.
        /// <para />
        /// If this property is <c>true</c>, the service locator will try to instantiate the object with 
        /// all constructors, starting with the one with the most parameters.
        /// <para />
        /// If a constructor fails, the service locator will try the constructor with less parameters until the
        /// type is either constructed successfully or all constructors are tried.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the service locator should support dependency injection; otherwise, <c>false</c>.
        /// </value>
        public bool SupportDependencyInjection { get; set; }

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

            lock (_syncObject)
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

            lock (_syncObject)
            {
                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    return true;
                }

                if (_registeredTypes.ContainsKey(serviceInfo))
                {
                    return true;
                }

                foreach (var externalContainerKeyValuePair in _externalContainers)
                {
                    var registrationInfo = externalContainerKeyValuePair.Value.GetRegistrationInfo(externalContainerKeyValuePair.Key, serviceType);
                    if (registrationInfo != null)
                    {
                        // Now we know the container, register it as typeof(object), we will re-register as soon as the actual type is known
                        _registeredTypes[serviceInfo] = new RegisteredTypeInfo(serviceType, typeof(object), tag, registrationInfo.RegistrationType, externalContainerKeyValuePair.Value);
                        return true;
                    }
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
                            Log.Debug("An open generic type '{0}' is registered, registering new closed generic type '{1}' based on the open registration", genericType.GetSafeFullName(), serviceType.GetSafeFullName());

                            var registrationInfo = GetRegistrationInfo(genericType, tag);
                            var finalType = registrationInfo.ImplementingType.MakeGenericType(genericArguments.ToArray());

                            RegisterType(serviceType, finalType, tag, registrationInfo.RegistrationType);

                            return true;
                        }
                    }
                }

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

                    RegisterType(serviceType, eventArgs.ImplementingType, eventArgs.Tag, eventArgs.RegistrationType, true, this);
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
            // Required to support the MissingTypeEventArgs
            if (!IsTypeRegistered(serviceType, tag))
            {
                return false;
            }

            var serviceInfo = new ServiceInfo(serviceType, tag);

            lock (_syncObject)
            {
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
        /// Registers an implementation of an service, but only if the type is not yet registered.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceImplementationType">The type of the implementation.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton"/>.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <remarks>
        /// Note that the actual implementation lays in the hands of the IoC technique being used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        public void RegisterType(Type serviceType, Type serviceImplementationType, object tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
        {
            RegisterType(serviceType, serviceImplementationType, tag, registrationType, registerIfAlreadyRegistered, this);
        }

        /// <summary>
        /// Resolves an instance of the type registered on the service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns>An instance of the type registered on the service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The type is not found in any container.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public object ResolveType(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            bool isTypeRegistered = false;

            lock (_syncObject)
            {
                isTypeRegistered = IsTypeRegistered(serviceType, tag);
            }

            if (!isTypeRegistered)
            {
                if (CanResolveNonAbstractTypesWithoutRegistration && serviceType.IsClassEx() && !serviceType.IsAbstractEx())
                {
                    return _typeFactory.CreateInstance(serviceType);
                }

                var error = string.Format("The type '{0}' is not registered, not even in an external container", serviceType.FullName);
                Log.Error(error);
                throw new NotSupportedException(error);
            }

            lock (_syncObject)
            {
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

            lock (_syncObject)
            {
                for (int i = 0; i < _registeredTypes.Keys.Count; i++)
                {
                    ServiceInfo serviceInfo = _registeredTypes.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        yield return ResolveType(serviceInfo.Type, serviceInfo.Tag);
                    }
                }
            }
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

            lock (_syncObject)
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

            lock (_syncObject)
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
        /// Remove the registered instance of a service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag of the registered the service.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        public void RemoveInstance(Type serviceType, object tag = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (_syncObject)
            {
                var serviceInfo = new ServiceInfo(serviceType, tag);
                if (_registeredInstances.ContainsKey(serviceInfo))
                {
                    _registeredInstances.Remove(serviceInfo);
                }
            }
        }

        /// <summary>
        /// Remove all registered instances of a service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public void RemoveAllInstances(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            lock (_syncObject)
            {
                for (int i = _registeredInstances.Count - 1; i >= 0; i--)
                {
                    ServiceInfo serviceInfo = _registeredInstances.Keys.ElementAt(i);
                    if (serviceInfo.Type == serviceType)
                    {
                        _registeredInstances.Remove(serviceInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Remove all registered instances.
        /// </summary>
        /// <param name="tag">The tag of the registered the service. The default value is <c>null</c>.</param>
        public void RemoveAllInstances(object tag = null)
        {
            lock (_syncObject)
            {
                if (tag == null)
                {
                    _registeredInstances.Clear();
                }
                else
                {
                    for (int i = _registeredInstances.Count - 1; i >= 0; i--)
                    {
                        var serviceInfo = _registeredInstances.Keys.ElementAt(i);
                        if (TagHelper.AreTagsEqual(serviceInfo.Tag, tag))
                        {
                            _registeredInstances.Remove(serviceInfo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified <paramref name="externalContainer">external container</paramref> is supported
        /// by this <see cref="IServiceLocator" />.
        /// </summary>
        /// <param name="externalContainer">The external container.</param>
        /// <returns><c>true</c> if the external container type is supported; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="externalContainer" /> is <c>null</c>.</exception>
        /// <exception cref="ExternalContainerNotSupportedException">If the <paramref name="externalContainer" /> is not supported.</exception>
        public bool IsExternalContainerSupported(object externalContainer)
        {
            Argument.IsNotNull("externalContainer", externalContainer);

            return GetContainerHelperForContainer(externalContainer) != null;
        }

        /// <summary>
        /// Registers an external container. This can be an external IoC container such
        /// as a Unity container.
        /// <para>
        /// </para>
        /// Registering an external container in the service locator is very useful in case types are
        /// already registered in another container (in case of Prism, for example).
        /// <para>
        /// </para>
        /// The <see cref="IServiceLocator" /> will use the external container to resolve unregistered types.
        /// <para>
        /// </para>
        /// Use the <see cref="IsExternalContainerSupported" /> to check whether an external container is registered
        /// before registering it (otherwise this method will thrown an exception).
        /// </summary>
        /// <param name="externalContainer">The external container.</param>
        /// <exception cref="ExternalContainerNotSupportedException">If the <paramref name="externalContainer" /> is not supported.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="externalContainer" /> is <c>null</c>.</exception>
        public void RegisterExternalContainer(object externalContainer)
        {
            Argument.IsNotNull("externalContainer", externalContainer);

            lock (_syncObject)
            {
                var externalContainerHelper = GetContainerHelperForContainer(externalContainer);
                if (externalContainerHelper == null)
                {
                    var supportedExternalContainers = new List<string>();
                    foreach (var supportedExternalContainer in _supportedExternalContainers)
                    {
                        supportedExternalContainers.Add(supportedExternalContainer.Name);
                    }

                    throw new ExternalContainerNotSupportedException(supportedExternalContainers.ToArray());
                }

                if (!_externalContainers.ContainsKey(externalContainer))
                {
                    _externalContainers.Add(externalContainer, externalContainerHelper);
                }

                if (AutomaticallyKeepContainersSynchronized)
                {
                    ExportToExternalContainers();
                }
            }
        }

        /// <summary>
        /// Registers an implementation of the <see cref="IExternalContainerHelper" /> class.
        /// <para>
        /// </para>
        /// This method can be used to add support for new external IoC containers.
        /// </summary>
        /// <param name="externalContainerHelper">The external container helper.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="externalContainerHelper" /> is <c>null</c>.</exception>
        public void RegisterExternalContainerHelper(IExternalContainerHelper externalContainerHelper)
        {
            Argument.IsNotNull("externalContainerHelper", externalContainerHelper);

            lock (_syncObject)
            {
                if (!_supportedExternalContainers.Contains(externalContainerHelper))
                {
                    _supportedExternalContainers.Add(externalContainerHelper);

                    Log.Debug("Registered external container helper '{0}'", externalContainerHelper.Name);
                }
            }
        }

        /// <summary>
        /// Exports all the current instances of the services to the external containers. This means that
        /// non-instantiated services will not be exported.
        /// <para>
        /// </para>
        /// This method will only export services if the services are not already registered with the
        /// external container.
        /// </summary>
        public void ExportInstancesToExternalContainers()
        {
            lock (_syncObject)
            {
                if (_externalContainers.Count == 0)
                {
                    return;
                }

                Log.Debug("Exporting instances to all external containers");

                foreach (var externalContainer in _externalContainers)
                {
                    CreateInstanceOfAllRegisteredTypes();

                    foreach (var instance in _registeredInstances)
                    {
                        var registrationInfo = externalContainer.Value.GetRegistrationInfo(externalContainer.Key, instance.Value.DeclaringType);
                        if ((registrationInfo == null) && (instance.Value.RegistrationType == RegistrationType.Singleton))
                        {
                            Log.Debug("Registering type '{0}' in external container '{1}'", instance.Key, externalContainer.Value.Name);

                            externalContainer.Value.RegisterInstance(externalContainer.Key, instance.Value.DeclaringType, instance.Value.ImplementingInstance);

                            Log.Debug("Registered type '{0}' in external container '{1}'", instance.Key, externalContainer.Value.Name);
                        }
                    }
                }
            }

            Log.Debug("Exported instances to all external containers");
        }

        /// <summary>
        /// Exports all services to external containers. If a service is not yet instantiated, the instance
        /// will be registered with the external container. Otherwise, the type will be registered.
        /// <para>
        /// </para>
        /// This method will only export services if the services are not already registered with the
        /// external container.
        /// </summary>
        public void ExportToExternalContainers()
        {
            lock (_syncObject)
            {
                if (_externalContainers.Count == 0)
                {
                    return;
                }

                Log.Debug("Exporting all services to all external containers");

                ExportInstancesToExternalContainers();

                foreach (var externalContainer in _externalContainers)
                {
                    if (externalContainer.Value.CanRegisterTypesWithoutInstantiating)
                    {
                        Log.Debug("External container '{0}' can register types without instantiating, registering not yet instantiated services", externalContainer.Value.Name);

                        foreach (var type in _registeredTypes)
                        {
                            var registrationInfo = externalContainer.Value.GetRegistrationInfo(externalContainer.Key, type.Value.DeclaringType);
                            if (registrationInfo == null)
                            {
                                externalContainer.Value.RegisterType(externalContainer.Key, type.Value.DeclaringType, type.Value.ImplementingType, type.Value.RegistrationType);
                            }
                        }

                        Log.Debug("Registered not yet instantiated services", externalContainer.Value.Name);
                    }
                }
            }

            Log.Debug("Exported all services to all external containers");
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
        internal void RegisterInstance(Type serviceType, object instance, object tag, object originalContainer)
        {
            Argument.IsNotNull("serviceType", serviceType);
            Argument.IsNotNull("instance", instance);

            Log.Debug("Registering type '{0}' to instance of type '{1}'", serviceType.FullName, instance.GetType().FullName);

            if (originalContainer == null)
            {
                originalContainer = this;
            }

            var registeredTypeInfo = new RegisteredTypeInfo(serviceType, instance.GetType(), tag, RegistrationType.Singleton, originalContainer);

            lock (_syncObject)
            {
                var serviceInfo = new ServiceInfo(serviceType, tag);
                _registeredInstances[serviceInfo] = new RegisteredInstanceInfo(serviceType, instance, RegistrationType.Singleton, tag, originalContainer);
                _registeredTypes[serviceInfo] = registeredTypeInfo;

                if (AutomaticallyKeepContainersSynchronized)
                {
                    ExportToExternalContainers();
                }
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
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        internal void RegisterType(Type serviceType, Type serviceImplementationType, object tag, RegistrationType registrationType, bool registerIfAlreadyRegistered, object originalContainer)
        {
            Argument.IsNotNull("serviceType", serviceType);
            Argument.IsNotNull("implementingType", serviceImplementationType);

            if (serviceImplementationType.IsInterfaceEx())
            {
                string error = string.Format("Cannot register interface type '{0}' as implementation, make sure to specify an actual class", serviceImplementationType.GetSafeFullName());
                Log.Error(error);
                throw new InvalidOperationException(error);
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
            RegisteredTypeInfo registeredTypeInfo = null;

            lock (_syncObject)
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

                registeredTypeInfo = new RegisteredTypeInfo(serviceType, serviceImplementationType, tag, registrationType, originalContainer);
                _registeredTypes[serviceInfo] = registeredTypeInfo;

                if (AutomaticallyKeepContainersSynchronized)
                {
                    ExportToExternalContainers();
                }
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

            lock (_syncObject)
            {
                var typeRequestInfo = new TypeRequestInfo(serviceType);
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

                // First check if we are the container
                var serviceInfo = new ServiceInfo(serviceType, tag);
                var registeredTypeInfo = _registeredTypes[serviceInfo];
                if (ObjectHelper.AreEqual(registeredTypeInfo.OriginalContainer, this))
                {
                    object instance = CreateServiceInstance(registeredTypeInfo.ImplementingType);
                    if (instance != null)
                    {
                        if (IsTypeRegisteredAsSingleton(serviceType, tag))
                        {
                            RegisterInstance(serviceType, instance, tag, this);
                        }
                    }

                    CompleteTypeRequestPathIfRequired(typeRequestInfo);

                    return instance;
                }

                // Get the type from the original container, it might be a specific instance
                foreach (var externalContainerKeyValuePair in _externalContainers)
                {
                    if (ObjectHelper.AreEqual(externalContainerKeyValuePair.Value, registeredTypeInfo.OriginalContainer))
                    {
                        var registrationInfo = externalContainerKeyValuePair.Value.GetRegistrationInfo(externalContainerKeyValuePair.Key, serviceType);
                        object instance = externalContainerKeyValuePair.Value.ResolveType(externalContainerKeyValuePair.Key, serviceType);

                        if (registrationInfo.RegistrationType == RegistrationType.Singleton)
                        {
                            RegisterInstance(serviceType, instance, tag, externalContainerKeyValuePair.Value);
                        }
                        else
                        {
                            // Note: we cannot register a transient because we don't know the implementing type
                        }

                        CompleteTypeRequestPathIfRequired(typeRequestInfo);

                        return instance;
                    }
                }

                CompleteTypeRequestPathIfRequired(typeRequestInfo);
            }

            var error = string.Format("The type '{0}' is registered, so why weren't we able to retrieve it?", serviceType.FullName);
            Log.Error(error);
            throw new ArgumentOutOfRangeException(error);
        }

        /// <summary>
        /// Creates an instance of all registered types. This method is required in case an <see cref="IExternalContainerHelper"/>
        /// does not support the registration of types without instantiating them.
        /// <para>
        /// </para>
        /// This method only creates instances of types that are not yet instantiated.
        /// </summary>
        private void CreateInstanceOfAllRegisteredTypes()
        {
            // Store in varaible because ResolveTypeFromKnownContainer might change the collection
            var keyValuePairs = _registeredTypes.ToList();

            lock (_syncObject)
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    var key = keyValuePair.Key;
                    var value = keyValuePair.Value;

                    var serviceInfo = new ServiceInfo(value.DeclaringType, key.Tag);
                    if (_typesCurrentlyBeingExported.Contains(serviceInfo))
                    {
                        continue;
                    }

                    if (!_registeredInstances.ContainsKey(serviceInfo) && IsTypeRegisteredAsSingleton(value.DeclaringType))
                    {
                        _typesCurrentlyBeingExported.Add(serviceInfo);

                        // Resolving automatically creates an instance
                        ResolveTypeFromKnownContainer(value.DeclaringType, key.Tag);

                        _typesCurrentlyBeingExported.Remove(serviceInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the container helper for the container.
        /// </summary>
        /// <param name="externalContainer">The external container.</param>
        /// <returns>The container helper or <c>null</c> if the container is not supported.</returns>
        private IExternalContainerHelper GetContainerHelperForContainer(object externalContainer)
        {
            if (externalContainer == null)
            {
                return null;
            }

            lock (_syncObject)
            {
                foreach (var supportedExternalContainer in _supportedExternalContainers)
                {
                    if (supportedExternalContainer.IsValidContainer(externalContainer))
                    {
                        return supportedExternalContainer;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the service instance.
        /// </summary>
        /// <param name="serviceType">Type of the service to instantiate.</param>
        /// <returns>The service instance.</returns>
        private object CreateServiceInstance(Type serviceType)
        {
            if (SupportDependencyInjection)
            {
                return _typeFactory.CreateInstance(serviceType);
            }

            return _typeFactory.CreateInstanceUsingActivator(serviceType);
        }

        /// <summary>
        /// Completes the type request path by checking if the currently created type is the same as the first
        /// type meaning that the type is successfully created and the current type request path can be set to <c>null</c>.
        /// </summary>
        /// <param name="typeRequestInfoForTypeJustConstructed">The type request info.</param>
        private void CompleteTypeRequestPathIfRequired(TypeRequestInfo typeRequestInfoForTypeJustConstructed)
        {
            lock (_syncObject)
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
    }
}