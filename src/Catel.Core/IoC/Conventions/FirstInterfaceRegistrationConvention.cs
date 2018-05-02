// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstInterfaceRegistrationConvention.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    /// The first interface convention based on <see cref="RegistrationConventionBase"/>.
    /// </summary>
    public class FirstInterfaceRegistrationConvention : RegistrationConventionBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstInterfaceRegistrationConvention"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        public FirstInterfaceRegistrationConvention(IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton) 
            : base(serviceLocator, registrationType)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Processes the specified types to register.
        /// </summary>
        /// <param name="typesToRegister">The types to register.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typesToRegister" /> is <c>null</c>.</exception>
        public override void Process(IEnumerable<Type> typesToRegister)
        {
            Argument.IsNotNull("typesToRegister", typesToRegister);

            var typesToHandle = typesToRegister as Type[] ?? typesToRegister.ToArray();

            typesToHandle.ForEach(type =>
            {
                if (type.IsInterfaceEx())
                {
                    var interfaceType = type;
                    var implementationTypes = typesToHandle.Where(t => t.IsAssignableFromEx(interfaceType) && !t.IsInterfaceEx());

                    var enumerable = implementationTypes as Type[] ?? implementationTypes.ToArray();
                    if (!enumerable.Any())
                    {
                        return;
                    }

                    foreach (var implementingType in enumerable)
                    {
                        var firstInterface = implementingType.GetInterfacesEx().FirstOrDefault();
                        if (firstInterface != null)
                        {
                            Container.RegisterTypeIfNotYetRegistered(firstInterface, implementingType, RegistrationType);
                        }
                    }
                }
                else
                {
                    var implementationType = type;
                    var firstInterface = implementationType.GetInterfacesEx().FirstOrDefault();

                    if (firstInterface == null)
                    {
                        return;
                    }

                    Container.RegisterTypeIfNotYetRegistered(firstInterface, implementationType, RegistrationType);
                }
            });
        }
        #endregion
    }
}