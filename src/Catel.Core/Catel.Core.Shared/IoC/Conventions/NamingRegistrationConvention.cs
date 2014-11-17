// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamingRegistrationConvention.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
    /// The naming convention based on <see cref="RegistrationConventionBase"/>.
    /// </summary>
    public class NamingRegistrationConvention : RegistrationConventionBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NamingRegistrationConvention" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        public NamingRegistrationConvention(IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton) 
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
            var interfaceTypes = typesToHandle.Select(type =>
            {
                if (type.IsInterfaceEx() && type.Name.StartsWith("I"))
                {
                    var implementationType = typesToHandle.FirstOrDefault(row => TagHelper.AreTagsEqual(row.Name, type.Name.Replace("I", string.Empty).Trim()) && row.IsClassEx() && type.IsAssignableFromEx(row));

                    if (implementationType != null)
                    {
                        return new {InterfaceType = type, ImplementationType = implementationType};
                    }
                }

                return null;
            }).Where(type => type != null);

            interfaceTypes.ForEach(type =>
            {
                Log.Debug("Applying '{0}' on '{1}'", GetType().Name, type.InterfaceType);

                Container.RegisterType(type.InterfaceType, type.ImplementationType, registrationType: RegistrationType);
            });
        }
        #endregion
    }
}