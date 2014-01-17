// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using Hosting;
    using IoC;

    /// <summary>
    /// </summary>
    public class ServiceHostFactory : System.ServiceModel.Activation.ServiceHostFactory
    {
        #region Methods
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

            return new ServiceHost(ServiceLocator.Default, serviceType, baseAddresses);
        }
        #endregion
    }
}