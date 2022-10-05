namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Catel.Reflection;
    using Logging;

    /// <summary>
    /// Extension methods for the <see cref="IServiceLocator"/> interface.
    /// </summary>
    public static partial class ServiceLocatorExtensions
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Resolves the type using parameters. This method combines the <see cref="IServiceLocator.GetRegistrationInfo" /> and
        /// the <see cref="ITypeFactory.CreateInstanceWithParameters" /> to provide the functionality.
        /// </summary>
        /// <typeparam name="T">The type of the interface to resolve.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The instantiated type constructed with the specified parameters.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameters" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The type is not registered in the container as transient type.</exception>
        public static T? ResolveTypeUsingParameters<T>(this IServiceLocator serviceLocator, object[] parameters, object? tag = null)
            where T : notnull
        {
            return (T?)ResolveTypeUsingParameters(serviceLocator, typeof(T), parameters, tag);
        }

        /// <summary>
        /// Resolves the type using parameters. This method combines the <see cref="IServiceLocator.GetRegistrationInfo" /> and
        /// the <see cref="ITypeFactory.CreateInstanceWithParameters" /> to provide the functionality.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The instantiated type constructed with the specified parameters.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameters" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The type is not registered in the container as transient type.</exception>
        public static object? ResolveTypeUsingParameters(this IServiceLocator serviceLocator, Type serviceType, object[] parameters, object? tag = null)
        {
            var registrationInfo = serviceLocator.GetRegistrationInfo(serviceType, tag);
            if (registrationInfo is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("The service locator could not return the registration info for type '{0}' with tag '{1}', cannot resolve type",
                    serviceType.GetSafeFullName(), ObjectToStringHelper.ToString(tag));
            }

            var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();

            if (registrationInfo.RegistrationType == RegistrationType.Singleton)
            {
                if (registrationInfo.IsTypeInstantiatedForSingleton)
                {
                    return serviceLocator.ResolveType(serviceType);
                }

                Log.Debug("Type '{0}' is registered as singleton but has not yet been instantiated. Instantiated it with the specified parameters now and registering it in the ServiceLocator", serviceType.GetSafeFullName());

                var instance = typeFactory.CreateInstanceWithParameters(registrationInfo.ImplementingType, parameters);
                if (instance is not null)
                {
                    serviceLocator.RegisterInstance(serviceType, instance);
                }

                return instance;
            }

            return typeFactory.CreateInstanceWithParameters(registrationInfo.ImplementingType, parameters);
        }

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified service type is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static bool IsTypeRegistered<TService>(this IServiceLocator serviceLocator, object? tag = null)
            where TService : notnull
        {
            return serviceLocator.IsTypeRegistered(typeof(TService), tag);
        }

        /// <summary>
        /// Determines whether the specified service type is registered as singleton.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the <typeparamref name="TService" /> type is registered as singleton, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        public static bool IsTypeRegisteredAsSingleton<TService>(this IServiceLocator serviceLocator, object? tag = null)
            where TService : notnull
        {
            return serviceLocator.IsTypeRegisteredAsSingleton(typeof(TService), tag);
        }

        /// <summary>
        /// Registers a specific instance of a service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="instance">The specific instance to register.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        public static void RegisterInstance<TService>(this IServiceLocator serviceLocator, TService instance, object? tag = null)
            where TService : notnull
        {
            serviceLocator.RegisterInstance(typeof(TService), instance, tag);
        }

        /// <summary>
        /// Registers an implementation of an service, but only if the type is not yet registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeIfNotYetRegistered<TService, TServiceImplementation>(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton)
            where TService : notnull
            where TServiceImplementation : TService
        {
            RegisterTypeIfNotYetRegisteredWithTag<TService, TServiceImplementation>(serviceLocator, null, registrationType);
        }

        /// <summary>
        /// Registers an implementation of an service, but only if the type is not yet registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeIfNotYetRegisteredWithTag<TService, TServiceImplementation>(this IServiceLocator serviceLocator, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton)
            where TService : notnull
            where TServiceImplementation : TService
        {
            serviceLocator.RegisterTypeWithTag<TService, TServiceImplementation>(tag, registrationType, false);
        }

        /// <summary>
        /// Registers an implementation of an service, but only if the type is not yet registered.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceImplementationType">The type of the implementation.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceImplementationType" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeIfNotYetRegistered(this IServiceLocator serviceLocator, Type serviceType, Type serviceImplementationType, RegistrationType registrationType = RegistrationType.Singleton)
        {
            RegisterTypeIfNotYetRegisteredWithTag(serviceLocator, serviceType, serviceImplementationType, null, registrationType);
        }

        /// <summary>
        /// Registers an implementation of an service, but only if the type is not yet registered.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceImplementationType">The type of the implementation.</param>
        /// <param name="tag">The tag to register the service with. The default value is <c>null</c>.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceImplementationType" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeIfNotYetRegisteredWithTag(this IServiceLocator serviceLocator, Type serviceType, Type serviceImplementationType, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton)
        {
            serviceLocator.RegisterType(serviceType, serviceImplementationType, tag, registrationType, false);
        }

        /// <summary>
        /// Registers a service where the implementation type is the same as the registered type.
        /// </summary>
        /// <typeparam name="TServiceImplementation">The type of the service definition and implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterType<TServiceImplementation>(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton)
        {
            RegisterTypeWithTag<TServiceImplementation>(serviceLocator, null, registrationType);
        }

        /// <summary>
        /// Registers a service where the implementation type is the same as the registered type.
        /// </summary>
        /// <typeparam name="TServiceImplementation">The type of the service definition and implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeWithTag<TServiceImplementation>(this IServiceLocator serviceLocator, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton)
        {
            serviceLocator.RegisterType(typeof(TServiceImplementation), typeof(TServiceImplementation), tag, registrationType);
        }

        /// <summary>
        /// Registers an implementation of a service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterType<TService, TServiceImplementation>(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            where TService : notnull
            where TServiceImplementation : TService
        {
            RegisterTypeWithTag<TService, TServiceImplementation>(serviceLocator, null, registrationType, registerIfAlreadyRegistered);
        }

        /// <summary>
        /// Registers an implementation of a service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeWithTag<TService, TServiceImplementation>(this IServiceLocator serviceLocator, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            where TService : notnull
            where TServiceImplementation : TService
        {
            serviceLocator.RegisterType(typeof(TService), typeof(TServiceImplementation), tag, registrationType, registerIfAlreadyRegistered);
        }

        /// <summary>
        /// Registers an implementation of ea service using a create type callback
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="createServiceFunc">The create service function.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="createServiceFunc" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterType<TService>(this IServiceLocator serviceLocator, Func<ITypeFactory, ServiceLocatorRegistration, TService> createServiceFunc, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            where TService : notnull
        {
            RegisterTypeWithTag(serviceLocator, createServiceFunc, null, registrationType, registerIfAlreadyRegistered);
        }

        /// <summary>
        /// Registers an implementation of ea service using a create type callback
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="createServiceFunc">The create service function.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">The registration type. The default value is <see cref="RegistrationType.Singleton" />.</param>
        /// <param name="registerIfAlreadyRegistered">If set to <c>true</c>, an older type registration is overwritten by this new one.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="createServiceFunc" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static void RegisterTypeWithTag<TService>(this IServiceLocator serviceLocator, Func<ITypeFactory, ServiceLocatorRegistration, TService> createServiceFunc, object? tag = null, RegistrationType registrationType = RegistrationType.Singleton, bool registerIfAlreadyRegistered = true)
            where TService : notnull
        {
            serviceLocator.RegisterType(typeof(TService), (tf, reg) => createServiceFunc(tf, reg), tag, registrationType, registerIfAlreadyRegistered);
        }

        /// <summary>
        /// Resolves an instance of the type registered on the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>An instance of the type registered on the service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="TypeNotRegisteredException">The type is not found in any container.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static TService? ResolveType<TService>(this IServiceLocator serviceLocator, object? tag = null)
            where TService : notnull
        {
            return (TService?)serviceLocator.ResolveType(typeof(TService), tag);
        }

        /// <summary>
        /// Resolves an instance of the type registered on the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="typeFactory">The type factory to use when the object needs to be created.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>An instance of the type registered on the service.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <exception cref="TypeNotRegisteredException">The type is not found in any container.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static TService? ResolveTypeUsingFactory<TService>(this IServiceLocator serviceLocator, ITypeFactory typeFactory, object? tag = null)
            where TService : notnull
        {
            return (TService?)serviceLocator.ResolveTypeUsingFactory(typeFactory, typeof(TService), tag);
        }

        /// <summary>
        /// Resolve the required type or throw an exception when it fails to do so.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>An instance of the type registered on the service or <c>null</c> if missing.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static TService ResolveRequiredType<TService>(this IServiceLocator serviceLocator, object? tag = null)
            where TService : notnull
        {
            return (TService)ResolveRequiredType(serviceLocator, typeof(TService), tag);
        }

        /// <summary>
        /// Resolve the required type or throw an exception when it fails to do so.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>An instance of the type registered on the service or <c>null</c> if missing.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static object ResolveRequiredType(this IServiceLocator serviceLocator, Type serviceType, object? tag = null)
        {
            lock (serviceLocator)
            {
                var instance = serviceLocator.ResolveType(serviceType, tag);
                if (instance is null)
                {
                    throw Log.ErrorAndCreateException(msg => new TypeNotRegisteredException(serviceType, msg), $"Type '{serviceType.GetSafeFullName(true)}' cannot be instantiated");
                }

                return instance;
            }
        }

        /// <summary>
        /// Resolves all instances of the type registered on the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <returns>All instance of the type registered on the service.</returns>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        public static IEnumerable<TService> ResolveTypes<TService>(this IServiceLocator serviceLocator)
            where TService : notnull
        {
            return serviceLocator.ResolveTypes(typeof(TService)).Cast<TService>();
        }

        /// <summary>
        /// Removes the type from the service locator.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        public static void RemoveType<TService>(this IServiceLocator serviceLocator, object? tag = null)
            where TService : notnull
        {
            serviceLocator.RemoveType(typeof(TService), tag);
        }

        /// <summary>
        /// Registers a service where the implementation type is the same as the registered type and immediately instantiates the type using the type factory.
        /// </summary>
        /// <typeparam name="TServiceImplementation">The type of the service definition and implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static TServiceImplementation? RegisterTypeAndInstantiate<TServiceImplementation>(this IServiceLocator serviceLocator)
            where TServiceImplementation : notnull
        {
            return RegisterTypeAndInstantiate<TServiceImplementation, TServiceImplementation>(serviceLocator);
        }

        /// <summary>
        /// Registers a service where the implementation type is the same as the registered type and immediately instantiates the type using the type factory.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">The type of the service definition and implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <returns>TService.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        /// <remarks>Note that the actual implementation lays in the hands of the IoC technique being used.</remarks>
        public static TService? RegisterTypeAndInstantiate<TService, TServiceImplementation>(this IServiceLocator serviceLocator)
            where TService : notnull
            where TServiceImplementation : TService
        {
            object? tag = null;

            RegisterTypeWithTag<TService, TServiceImplementation>(serviceLocator, tag, RegistrationType.Singleton);

            return ResolveType<TService>(serviceLocator, tag);
        }
    }
}
