// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;
    using Reflection;

    /// <summary>
    /// Helper class to instantiate objects with dependency injection.
    /// </summary>
    [ObsoleteEx(Replacement = "TypeFactory class (or ITypeFactory)", TreatAsErrorFromVersion = "3.5", RemoveInVersion = "4.0")]
    public static class DependencyInjectionHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, <see cref="ServiceLocator.Default"/> will be used.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <remarks></remarks>
        public static T CreateInstance<T>(IServiceLocator serviceLocator = null)
        {
            return (T)CreateInstance(typeof(T), serviceLocator);
        }

        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, <see cref="ServiceLocator.Default"/> will be used.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public static object CreateInstance(Type serviceType, IServiceLocator serviceLocator = null)
        {
            Argument.IsNotNull("serviceType", serviceType);

            //Log.Debug("Creating instance of type '{0}'", serviceType.FullName);

            //Log.Debug("Trying all constructors");

            if (serviceLocator == null)
            {
                serviceLocator = ServiceLocator.Default;
            }

            var constructors = (from constructor in serviceType.GetConstructorsEx()
                                orderby constructor.GetParameters().Count() descending
                                select constructor).ToList();

            foreach (var constructor in constructors)
            {
                var instanceCreatedWithInjection = TryCreateWithConstructorInjection(serviceType, constructor, serviceLocator);
                if (instanceCreatedWithInjection != null)
                {
                    return instanceCreatedWithInjection;
                }
            }

            var instance = Activator.CreateInstance(serviceType);

            return instance;
        }

        /// <summary>
        /// Tries to create the service with the specified constructor by retrieving all values from the
        /// service locator for the arguments.
        /// <para/>
        /// This method will not throw an exception when the invocation fails.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <param name="serviceLocator">The service locator.</param>
        /// <returns>The instantiated service or <c>null</c> if the instantiation fails.</returns>
        private static object TryCreateWithConstructorInjection(Type serviceType, ConstructorInfo constructorInfo, IServiceLocator serviceLocator)
        {
            //Log.Debug("Trying to instantiate type '{0}' using constructor '{1}'", serviceType.FullName, constructorInfo.GetSignature());

            var parameters = new List<object>();
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                var parameterType = parameterInfo.ParameterType;

                try
                {
                    parameters.Add(serviceLocator.ResolveType(parameterType));
                }
                catch (Exception)
                {
                    //Log.Debug("Failed to retrieve type '{0}' from the service locator, cannot use constructor to instantiate type", parameterType.FullName);
                }
            }

            //Log.Debug("All parameters for the constructor were found in the service locator, trying to construct type now");

            try
            {
                var instance = constructorInfo.Invoke(parameters.ToArray());
                //instance = Activator.CreateInstance(serviceType, parameters.ToArray());

                //Log.Debug("Successfully created instance using dependency injection for type '{0}' using constructor '{1}'",
                //    serviceType.FullName, constructorInfo.GetSignature());

                return instance;
            }
            catch (Exception)
            {
                Log.Debug("Failed to create instance using dependency injection for type '{0}' using constructor '{1}'",
                    serviceType.FullName, constructorInfo.GetSignature());

                return null;
            }
        }
    }
}