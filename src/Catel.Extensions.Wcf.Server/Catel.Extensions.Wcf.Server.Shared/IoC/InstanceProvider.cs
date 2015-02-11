﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel.Dispatching
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using IoC;
    using Logging;

    /// <summary>
    /// 
    /// </summary>
    public class InstanceProvider : IInstanceProvider
    {
        #region Fields
        /// <summary>
        /// The contract type
        /// </summary>
        private readonly Type _contractType;

        /// <summary>
        /// The log
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IServiceLocator _serviceLocator;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceProvider" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="contractType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public InstanceProvider(IServiceLocator serviceLocator, Type contractType, Type serviceType, object tag = null, RegistrationType registrationType = RegistrationType.Singleton)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);
            Argument.IsNotNull("contractType", contractType);
            Argument.IsNotNull("serviceType", serviceType);

            _log = LogManager.GetCurrentClassLogger();

            _serviceLocator = serviceLocator;
            _contractType = contractType;

            _log.Debug("Register the contract type '{0}' for service type '{1}'", _contractType.Name, serviceType.Name);
            _serviceLocator.RegisterTypeIfNotYetRegisteredWithTag(_contractType, serviceType, tag, registrationType);
        }
        #endregion

        #region IInstanceProvider Members
        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceContext"/> is <c>null</c>.</exception>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            Argument.IsNotNull("instanceContext", instanceContext);

            var instanceContextExtension = instanceContext.Extensions.Find<InstanceContextExtension>();

            if (instanceContextExtension == null)
            {
                return null;
            }

            var childContainer = instanceContextExtension.GetChildContainer(_serviceLocator);

            _log.Debug("Resolving the contract type '{0}'", _contractType.Name);
            return childContainer.ResolveType(_contractType);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext" /> object.</param>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceContext"/> is <c>null</c>.</exception>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext" /> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceContext"/> is <c>null</c>.</exception>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            Argument.IsNotNull("instanceContext", instanceContext);

            var instanceContextExtension = instanceContext.Extensions.Find<InstanceContextExtension>();

            if (instanceContextExtension == null)
            {
                return;
            }

            _log.Debug("Releasing the child container");
            instanceContextExtension.DisposeChildContainer();
        }
        #endregion
    }
}