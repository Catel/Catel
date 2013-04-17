// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Available registration types.
    /// </summary>
    public enum RegistrationType
    {
        /// <summary>
        /// Singleton mode which means that the same instance will be returned every time a type is resolved.
        /// </summary>
        Singleton,

        /// <summary>
        /// Transient mode which means that a new instance will be returned every time a type is resolved.
        /// </summary>
        Transient
    }

    /// <summary>
    /// The service locator which is used to retrieve the right instances of services.
    /// <para />
    /// The cool thing about this service locator is that it can use external containers (from example from Unity)
    /// to resolve types if the types are not registered in the container itself. To do this, use the following code:
    /// <para />
    /// <code>
    ///   var serviceLocator = ServiceLocator.Default;
    ///   serviceLocator.RegisterExternalContainer(myUnityContainer);
    /// </code>
    /// <para />
    /// The service locator will use the external containers in case the current container does not contain the
    /// type. If the external containers also don't contain the type, there is one last way to resolve the type
    /// using the <see cref="MissingType"/> event. The event passes <see cref="MissingTypeEventArgs"/> that contains
    /// the type the service locator is looking for. By setting the <see cref="MissingTypeEventArgs.ImplementingInstance"/> or 
    /// <see cref="MissingTypeEventArgs.ImplementingType"/> in the handler, the service locator will resolve the type.
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        #region Properties
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
        bool AutomaticallyKeepContainersSynchronized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service locator can resolve non abstract types without registration.
        /// </summary>
        bool CanResolveNonAbstractTypesWithoutRegistration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service locator supports dependency inject.
        /// <para />
        /// If this property is <c>true</c>, the service locator will try to instantiate the object with 
        /// all constructors, starting with the one with the most parameters.
        /// <para />
        /// If a constructor fails, the service locator will try the constructor with less parameters until the
        /// type is either constructed successfully or all constructors are tried.
        /// <para />
        /// If this property is <c>false</c>, it will only use <see cref="Activator.CreateInstance(System.Type)"/> to
        /// instantiate the service.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the service locator should support dependency injection; otherwise, <c>false</c>.
        /// </value>
        bool SupportDependencyInjection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service locators will automatically register types via attributes.
        /// </summary>
        /// <remarks>
        /// By default, this value is <c>true</c>.
        /// </remarks>
        bool AutoRegisterTypesViaAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service locators will ignore incorrect usage of <see cref="ServiceLocatorRegistrationAttribute"/> and do not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>
        /// By default, this value is <c>true</c>.
        /// </remarks>
        bool IgnoreRuntimeIncorrectUsageOfRegisterAttribute { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a type cannot be resolved the by service locator. It first tries to raise this event.
        /// <para />
        /// If there are no handlers or no handler can fill up the missing type, an exception will be thrown by
        /// the service locator.
        /// </summary>
        event EventHandler<MissingTypeEventArgs> MissingType;

        /// <summary>
        /// Occurs when a type is registered in the service locator.
        /// </summary>
        event EventHandler<TypeRegisteredEventArgs> TypeRegistered;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the registration info about the specified type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="tag">The tag the service is registered with. The default value is <c>null</c>.</param>
        /// <returns>The <see cref="RegistrationInfo" /> or <c>null</c> if the type is not registered.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        RegistrationInfo GetRegistrationInfo(Type serviceType, object tag = null);

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns><c>true</c> if the specified service type is registered; otherwise, <c>false</c>.</returns>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        bool IsTypeRegistered(Type serviceType, object tag = null);

        /// <summary>
        /// Determines whether the specified service type is registered as singleton.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns><c>true</c> if the <paramref name="serviceType" /> type is registered as singleton, otherwise <c>false</c>.</returns>
        bool IsTypeRegisteredAsSingleton(Type serviceType, object tag = null);

        /// <summary>
        /// Registers a specific instance of a service. 
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="instance"/> is not of the right type.</exception>
        void RegisterInstance(Type serviceType, object instance, object tag = null);

        /// <summary>
        /// Registers an implementation of an service, but only if the type is not yet registered.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceImplementationType">The type of the implementation.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton"/>.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <remarks>
        /// Note that the actual implementation lays in the hands of the IoC technique being used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        void RegisterType(Type serviceType, Type serviceImplementationType, object tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true);

        /// <summary>
        /// Resolves an instance of the type registered on the service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <returns>An instance of the type registered on the service.</returns>
        /// <remarks>
        /// Note that the actual implementation lays in the hands of the IoC technique being used.
        /// </remarks>
        object ResolveType(Type serviceType, object tag = null);

        /// <summary>
        /// Resolves all instances of the type registered on the service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>All instance of the type registered on the service.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        IEnumerable<object> ResolveTypes(Type serviceType);

        /// <summary>
        /// Remove the registered instance of a service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="tag">
        /// The tag of the registered the service.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="serviceType"/> is <c>null</c>.
        /// </exception>
        void RemoveInstance(Type serviceType, object tag = null);

        /// <summary>
        /// Remove all registered instances of a service.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        void RemoveAllInstances(Type serviceType);

        /// <summary>
        /// Remove all registered instances.
        /// </summary>
        /// <param name="tag">The tag of the registered the service. The default value is <c>null</c>.</param>
        void RemoveAllInstances(object tag = null);

        /// <summary>
        /// Determines whether the specified <paramref name="externalContainer">external container</paramref> is supported
        /// by this <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="externalContainer">The external container.</param>
        /// <returns>
        /// <c>true</c> if the external container type is supported; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="externalContainer"/> is <c>null</c>.</exception>
        /// <exception cref="ExternalContainerNotSupportedException">If the <paramref name="externalContainer"/> is not supported.</exception>
        bool IsExternalContainerSupported(object externalContainer);

        /// <summary>
        /// Registers an external container. This can be an external IoC container such
        /// as a Unity container.
        /// <para />
        /// Registering an external container in the service locator is very useful in case types are 
        /// already registered in another container (in case of Prism, for example).
        /// <para />
        /// The <see cref="IServiceLocator"/> will use the external container to resolve unregistered types.
        /// <para />
        /// Use the <see cref="IsExternalContainerSupported"/> to check whether an external container is registered
        /// before registering it (otherwise this method will thrown an exception).
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="externalContainer"/> is <c>null</c>.</exception>
        /// <exception cref="ExternalContainerNotSupportedException">If the <paramref name="externalContainer"/> is not supported.</exception>
        void RegisterExternalContainer(object externalContainer);

        /// <summary>
        /// Registers an implementation of the <see cref="IExternalContainerHelper"/> class.
        /// <para />
        /// This method can be used to add support for new external IoC containers.
        /// </summary>
        /// <param name="externalContainerHelper">The external container helper.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="externalContainerHelper"/> is <c>null</c>.</exception>
        void RegisterExternalContainerHelper(IExternalContainerHelper externalContainerHelper);

        /// <summary>
        /// Exports all the current instances of the services to the external containers. This means that
        /// non-instantiated services will not be exported.
        /// <para />
        /// This method will only export services if the services are not already registered with the
        /// external container.
        /// </summary>
        ///// <exception cref="InvalidOperationException">The external containers contain a different instance than the service locator.</exception>
        void ExportInstancesToExternalContainers();

        /// <summary>
        /// Exports all services to external containers. If a service is not yet instantiated, the instance
        /// will be registered with the external container. Otherwise, the type will be registered.
        /// <para />
        /// This method will only export services if the services are not already registered with the
        /// external container.
        /// </summary>
        ///// <exception cref="InvalidOperationException">The external containers contain a different instance than the service locator.</exception>
        void ExportToExternalContainers();
        #endregion

        /// <summary>
        /// Determines whether all the specified types are registered with the service locator.
        /// </summary>
        /// <remarks>
        /// Note that this method is written for optimalization by the <see cref="TypeFactory"/>. This means that the 
        /// <see cref="TypeFactory"/> does not need to call the <see cref="ServiceLocator"/> several times to construct
        /// a single type using dependency injection.
        /// <para />
        /// Only use this method if you know what you are doing, otherwise use the <see cref="ServiceLocator.IsTypeRegistered"/> instead.
        /// </remarks>
        /// <param name="types">The types that should be registered.</param>
        /// <returns><c>true</c> if all the specified types are registered with this instance of the <see cref="IServiceLocator" />; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="types"/> is <c>null</c> or an empty array.</exception>
        bool AreAllTypesRegistered(params Type[] types);

        /// <summary>
        /// Resolves all the specified types.
        /// </summary>
        /// <remarks>
        /// Note that this method is written for optimalization by the <see cref="TypeFactory"/>. This means that the 
        /// <see cref="TypeFactory"/> does not need to call the <see cref="ServiceLocator"/> several times to construct
        /// a single type using dependency injection.
        /// <para />
        /// Only use this method if you know what you are doing, otherwise use the <see cref="ServiceLocator.IsTypeRegistered"/> instead.
        /// </remarks>
        /// <param name="types">The collection of types that should be resolved.</param>
        /// <returns>The resolved types in the same order as the types.</returns>
        /// <exception cref="ArgumentException">The <paramref name="types"/> is <c>null</c> or an empty array.</exception>
        object[] ResolveAllTypes(params Type[] types);
    }
}
