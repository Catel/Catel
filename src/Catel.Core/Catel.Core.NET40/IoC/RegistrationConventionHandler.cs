// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConventionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Collections;
    using Logging;
    using Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class RegistrationConventionHandler : IRegistrationConventionHandler
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        /// <summary>
        /// The registered conventions
        /// </summary>
        private readonly IList<IRegistrationConvention> _registeredConventions = new List<IRegistrationConvention>();

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationConventionHandler" /> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeFactory"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public RegistrationConventionHandler(IServiceLocator serviceLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);
            Argument.IsNotNull("typeFactory", typeFactory);

            _serviceLocator = serviceLocator;
            _typeFactory = typeFactory;
            Filter = new CompositeFilter<Type>();
        }
        #endregion

        #region IRegistrationConventionHandler Members
        /// <summary>
        /// Gets the registration conventions.
        /// </summary>
        /// <value>
        /// The registration conventions.
        /// </value>
        public IEnumerable<IRegistrationConvention> RegistrationConventions
        {
            get
            {
                lock (_registeredConventions)
                {
                    return _registeredConventions;
                }
            }
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public CompositeFilter<Type> Filter { get; private set; }

        /// <summary>
        /// Registers the convention.
        /// </summary>
        public void RegisterConvention<TRegistrationConvention>(RegistrationType registrationType = RegistrationType.Singleton) where TRegistrationConvention : IRegistrationConvention
        {
            var registrationConvention = _typeFactory.CreateInstanceWithParameters<TRegistrationConvention>(_serviceLocator, registrationType);

            _registeredConventions.Add(registrationConvention);
        }

        /// <summary>
        /// Applies the registered conventions.
        /// </summary>
        public void ApplyConventions()
        {
            lock (_registeredConventions)
            {
                if (!_registeredConventions.Any())
                {
                    return;
                }

                var types = TypeCache.GetTypes().Where(type => Filter.Matches(type));

                _registeredConventions.ForEach(convention => convention.Process(types));
            }
        }
        #endregion
    }
}