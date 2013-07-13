// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactoryExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="ITypeFactory"/>.
    /// </summary>
    public static class TypeFactoryExtensions
    {
        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public static T CreateInstance<T>(this ITypeFactory typeFactory)
        {
            Argument.IsNotNull("typeFactory", typeFactory);

            return (T)typeFactory.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Creates an instance of the specified type using <c>Activator.CreateInstance</c>.
        /// <para />
        /// The advantage of using this method is that the results are being cached if the execution fails thus
        /// the next call will be extremely fast.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public static T CreateInstanceUsingActivator<T>(this ITypeFactory typeFactory)
        {
            Argument.IsNotNull("typeFactory", typeFactory);

            return (T)typeFactory.CreateInstanceUsingActivator(typeof(T));
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory"/> is <c>null</c>.</exception>
        public static T CreateInstanceWithParameters<T>(this ITypeFactory typeFactory, params object[] parameters)
        {
            Argument.IsNotNull("typeFactory", typeFactory);

            return (T)typeFactory.CreateInstanceWithParameters(typeof(T), parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory"/> is <c>null</c>.</exception>
        public static T CreateInstanceWithParametersAndAutoCompletion<T>(this ITypeFactory typeFactory, params object[] parameters)
        {
            Argument.IsNotNull("typeFactory", typeFactory);

            return (T)typeFactory.CreateInstanceWithParametersAndAutoCompletion(typeof(T), parameters);
        }
    }
}