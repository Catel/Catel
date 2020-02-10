// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConventionHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Extensions of <see cref="IRegistrationConventionHandler"/>.
    /// </summary>
    public static class RegistrationConventionHandlerExtensions
    {
        #region Methods
        /// <summary>
        /// Only exclude types that match the specified predicate when scanning.
        /// </summary>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="exclude">The exclude.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="exclude" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ExcludeTypesWhere(this IRegistrationConventionHandler registrationConventionHandler, Predicate<Type> exclude)
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);
            Argument.IsNotNull("exclude", exclude);

            registrationConventionHandler.TypeFilter.Excludes += exclude;
            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }

        /// <summary>
        /// Only exclude assemblies that match the specified predicate when scanning.
        /// </summary>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="exclude">The exclude.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="exclude" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ExcludeAssembliesWhere(this IRegistrationConventionHandler registrationConventionHandler, Predicate<Assembly> exclude)
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);
            Argument.IsNotNull("exclude", exclude);

            registrationConventionHandler.AssemblyFilter.Excludes += exclude;
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
        public static IRegistrationConventionHandler ExcludeAllTypesOfNamespace(this IRegistrationConventionHandler registrationConventionHandler, string @namespace)
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);
            Argument.IsNotNullOrWhitespace("@namespace", @namespace);

            registrationConventionHandler.ExcludeTypesWhere(type => type.Namespace != null && type.Namespace.StartsWith(@namespace));

            return registrationConventionHandler;
        }

        /// <summary>
        /// Excludes the namespace containing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ExcludeAllTypesOfNamespaceContaining<T>(this IRegistrationConventionHandler registrationConventionHandler)
            where T : class
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.ExcludeAllTypesOfNamespace(typeof (T).Namespace);

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
        public static IRegistrationConventionHandler ShouldAlsoUseConvention<TRegistrationConvention>(this IRegistrationConventionHandler registrationConventionHandler, RegistrationType registrationType = RegistrationType.Singleton)
            where TRegistrationConvention : class, IRegistrationConvention
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.RegisterConvention<TRegistrationConvention>(registrationType);

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
            where T : class
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.ExcludeTypesWhere(type => ObjectHelper.AreEqual(type, typeof (T)));

            return registrationConventionHandler;
        }

        /// <typeparam name="T"></typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler"/> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler IncludeType<T>(this IRegistrationConventionHandler registrationConventionHandler)
            where T : class
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.IncludeTypesWhere(type => ObjectHelper.AreEqual(type, typeof(T)));

            return registrationConventionHandler;
        }

        /// <summary>
        /// Excludes the assembly.
        /// </summary>
        /// <typeparam name="TAssembly">The type of the assembly.</typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler"/> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler ExcludeAssembly<TAssembly>(this IRegistrationConventionHandler registrationConventionHandler)
            where TAssembly : Assembly
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.ExcludeAssembliesWhere(assembly => ObjectHelper.AreEqual(assembly.GetType(), typeof(TAssembly)));

            return registrationConventionHandler;
        }

        /// <summary>
        /// Only include types that match the specified predicate when scanning.
        /// </summary>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="include">The predicate to use for matching.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler IncludeTypesWhere(this IRegistrationConventionHandler registrationConventionHandler, Predicate<Type> include)
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.TypeFilter.Includes += include;
            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }

        /// <summary>
        /// Adds the assembly to scan.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler"/> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler AddAssemblyToScan<TAssembly>(this IRegistrationConventionHandler registrationConventionHandler)
            where TAssembly : Assembly
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            var assembly = TypeFactory.Default.CreateInstance<TAssembly>();

            registrationConventionHandler.AddAssemblyToScan(assembly);

            return registrationConventionHandler;
        }

        /// <summary>
        /// Only include types in the same namespace as the specified type or its sub namespaces
        /// when scanning.
        /// </summary>
        /// <typeparam name="T">A type in the namespace to include.</typeparam>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler IncludeAllTypesOfNamespaceContaining<T>(this IRegistrationConventionHandler registrationConventionHandler)
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);

            registrationConventionHandler.IncludeAllTypesOfNamespace(typeof (T).Namespace);

            return registrationConventionHandler;
        }

        /// <summary>
        /// Only include types in the specified namespace or its sub namespaces when scanning.
        /// </summary>
        /// <param name="registrationConventionHandler">The registration convention handler.</param>
        /// <param name="namespace">The namespace to include.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="registrationConventionHandler" /> is <c>null</c>.</exception>
        public static IRegistrationConventionHandler IncludeAllTypesOfNamespace(this IRegistrationConventionHandler registrationConventionHandler, string @namespace)
        {
            Argument.IsNotNull("registrationConventionHandler", registrationConventionHandler);
            Argument.IsNotNullOrWhitespace("@namespace", @namespace);

            registrationConventionHandler.TypeFilter.Includes += type => type.Namespace != null && type.Namespace.StartsWith(@namespace);
            registrationConventionHandler.ApplyConventions();

            return registrationConventionHandler;
        }
        #endregion
    }
}
