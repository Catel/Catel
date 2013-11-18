// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConventionHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class RegistrationConventionHandlerExtensions
    {
        #region Methods
        /// <summary>
        /// Excludes the specified registration convention handler.
        /// </summary>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="exclude">The exclude.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="exclude"/> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler Exclude(this IRegistrationConventionHandler registrationConventionHandler, Predicate<Type> exclude)
        {
            Argument.IsNotNull("exclude", exclude);
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.Filter.Excludes += exclude;

            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }

        /// <summary>
        /// Excludes the namespace.
        /// </summary>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="namespace">The namespace.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="namespace"/> is <c>null</c> or whitespace.</exception>
        public static IRegistrationConventionHandler ExcludeNamespace(this IRegistrationConventionHandler registrationConventionHandler, string @namespace)
        {
            Argument.IsNotNullOrWhitespace(() => @namespace);
            Argument.IsNotNull(() => registrationConventionHandler);

            registrationConventionHandler.Exclude(type => type.Namespace != null && type.Namespace.StartsWith(@namespace));

            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }

        /// <summary>
        /// Excludes the namespace containing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ExcludeNamespaceContaining<T>(this IRegistrationConventionHandler registrationConventionHandler)
        {
            Argument.IsNotNull(() => registrationConventionHandler);

            registrationConventionHandler.ExcludeNamespace(typeof (T).Namespace);

            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }

        /// <summary>
        /// Registers the convention.
        /// </summary>
        /// <typeparam name="TRegistrationConvention">The type of the registration convention.</typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ShouldAlsoUseConvention<TRegistrationConvention>(this IRegistrationConventionHandler registrationConventionHandler, RegistrationType registrationType = RegistrationType.Singleton) where TRegistrationConvention : IRegistrationConvention
        {
            Argument.IsNotNull(() => registrationConventionHandler);

            registrationConventionHandler.RegisterConvention<TRegistrationConvention>(registrationType);

            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }

        /// <summary>
        /// Excludes the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler"/> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ExcludeType<T>(this IRegistrationConventionHandler registrationConventionHandler)
        {
            Argument.IsNotNull(() => registrationConventionHandler);
            registrationConventionHandler.Exclude(type => type == typeof (T));

            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }
        #endregion
    }
}