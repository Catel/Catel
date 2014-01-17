// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorExtensions.conventions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    /// <summary>
    /// Partial implementation for auto-registration features.
    /// </summary>
    public static partial class ServiceLocatorExtensions
    {
        /// <summary>
        /// Registers the types using all conventions.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler RegisterTypesUsingAllConventions(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            RegisterTypesUsingConvention<NamingRegistrationConvention>(serviceLocator, registrationType);
            var registrationConventionHandler = RegisterTypesUsingConvention<FirstInterfaceRegistrationConvention>(serviceLocator, registrationType);

            return registrationConventionHandler;
        }

        /// <summary>
        /// Registers the types using the default naming convention.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <returns>IRegistrationConventionHandler.</returns>
        public static IRegistrationConventionHandler RegisterTypesUsingDefaultNamingConvention(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            var registrationConventionHandler = RegisterTypesUsingConvention<NamingRegistrationConvention>(serviceLocator, registrationType);

            return registrationConventionHandler;
        }

        /// <summary>
        /// Registers the types using the first interface convention.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <returns>IRegistrationConventionHandler.</returns>
        public static IRegistrationConventionHandler RegisterTypesUsingDefaultFirstInterfaceConvention(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            var registrationConventionHandler = RegisterTypesUsingConvention<FirstInterfaceRegistrationConvention>(serviceLocator, registrationType);

            return registrationConventionHandler;
        }

        /// <summary>
        /// Registers the types using the specified convention.
        /// </summary>
        /// <typeparam name="TRegistrationConvention">The type of the registration convention.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <returns>IRegistrationConventionHandler.</returns>
        public static IRegistrationConventionHandler RegisterTypesUsingConvention<TRegistrationConvention>(this IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton) where TRegistrationConvention : IRegistrationConvention
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            serviceLocator.RegisterTypeIfNotYetRegistered<IRegistrationConventionHandler, RegistrationConventionHandler>();

            var registrationConventionHandler = serviceLocator.ResolveType<IRegistrationConventionHandler>();

            registrationConventionHandler.RegisterConvention<TRegistrationConvention>(registrationType);

            return registrationConventionHandler;
        }
    }
}