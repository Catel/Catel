// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationJsonModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using IoC;
    using Runtime.Serialization.Json;

    /// <summary>
    /// Core module which allows the registration of default services in the service locator.
    /// </summary>
    public class SerializationJsonModule : IServiceLocatorInitializer
    {
        #region Methods
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            serviceLocator.RegisterType<IJsonSerializer, JsonSerializer>();
        }
        #endregion
    }
}