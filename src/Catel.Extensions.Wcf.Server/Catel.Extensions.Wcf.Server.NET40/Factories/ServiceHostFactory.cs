// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.ServiceModel;
    using IoC;
    using ServiceHost = Hosting.ServiceHost;

    /// <summary>
    /// </summary>
    public class ServiceHostFactory : System.ServiceModel.Activation.ServiceHostFactory
    {
        #region Fields
        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IServiceLocator _serviceLocator;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHostFactory" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public ServiceHostFactory(IServiceLocator serviceLocator = null)
        {
            _serviceLocator = serviceLocator ?? this.GetDependencyResolver().Resolve<IServiceLocator>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost" /> with specific base addresses and initializes it with specified data.
        /// </summary>
        /// <param name="constructorString">The initialization data passed to the <see cref="T:System.ServiceModel.ServiceHostBase" /> instance being constructed by the factory.</param>
        /// <param name="baseAddresses">The <see cref="T:System.Array" /> of type <see cref="T:System.Uri" /> that contains the base addresses for the service hosted.</param>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost" /> with specific base addresses.
        /// </returns>
        /// <exception cref="System.ServiceModel.ServiceActivationException"></exception>
        /// <exception cref="ArgumentException">The <paramref name="constructorString"/> is <c>null</c> or white space.</exception>
        /// <exception cref="ArgumentException">The <paramref name="baseAddresses"/> is <c>null</c> or empty.</exception>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            Argument.IsNotNullOrWhitespace("constructorString", constructorString);
            Argument.IsNotNullOrEmptyArray("baseAddresses", baseAddresses);

            var serviceType = Type.GetType(constructorString, false);

            if (serviceType == null)
            {
                throw new ServiceActivationException();
            }

            var serviceHost = CreateServiceHost(serviceType, baseAddresses);

            return serviceHost;
        }

        /// <summary>
        ///     Creates a <see cref="T:System.ServiceModel.ServiceHost" /> for a specified type of service with a specific base
        ///     address.
        /// </summary>
        /// <param name="serviceType">Specifies the type of service to host.</param>
        /// <param name="baseAddresses">
        ///     The <see cref="T:System.Array" /> of type <see cref="T:System.Uri" /> that contains the base addresses for the
        ///     service hosted.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.ServiceModel.ServiceHost" /> for the type of service specified with a specific base address.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="baseAddresses" /> is <c>null</c> or empty.</exception>
        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            Argument.IsNotNull("serviceType", serviceType);
            Argument.IsNotNullOrEmptyArray("baseAddresses", baseAddresses);

            return new ServiceHost(_serviceLocator, serviceType, baseAddresses);
        }
        #endregion
    }
}