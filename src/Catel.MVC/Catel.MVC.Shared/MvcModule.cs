// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MvcModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.IoC;

    /// <summary>
    /// Mvc module which allows the registration of default services in the service locator.
    /// </summary>
    public partial class MvcModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            // Register services here
        }

        partial void InitializePlatform();
    }
}