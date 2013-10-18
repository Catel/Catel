// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using Interception.Handlers;
    using IoC;

    /// <summary>
    /// The <see cref="IServiceLocator"/> extensions.
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        #region Methods
        /// <summary>
        /// Configures the type of the interception for.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TServiceImplementation">The type of the service implementation.</typeparam>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="targetInstanceToUse">The target instance you want use in proxy instanciation.</param>
        /// <returns></returns>
        public static IInterceptorHandler<TService, TServiceImplementation> ConfigureInterceptionForType
            <TService, TServiceImplementation>(this IServiceLocator serviceLocator,
                                               object tag = null, object targetInstanceToUse = null)
            where TServiceImplementation : TService
        {
            return
                new InterceptorHandler<TService, TServiceImplementation>(serviceLocator, tag, targetInstanceToUse);
        }
        #endregion
    }
}