// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionConfig.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Mvc
{
    using IoC;
    using DependencyResolver = System.Web.Mvc.DependencyResolver;

    /// <summary>
    /// Configures the <see cref="Catel.IoC.ServiceLocator"/> as dependency resolver for ASP.NET MVC.
    /// </summary>
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Registers the <see cref="Catel.IoC.ServiceLocator" /> as dependency resolver for ASP.NET MVC.
        /// </summary>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, the <see cref="ServiceLocator.Default"/> will be used.</param>
        public static void RegisterServiceLocatorAsDependencyResolver(IServiceLocator serviceLocator = null)
        { 
            DependencyResolver.SetResolver(new IoC.DependencyResolver(serviceLocator));
        }
    }
}