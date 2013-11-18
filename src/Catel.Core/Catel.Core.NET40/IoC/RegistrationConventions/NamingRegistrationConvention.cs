// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamingRegistrationConvention.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Collections;
    using Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class NamingRegistrationConvention : RegistrationConventionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NamingRegistrationConvention"/> class.
        /// </summary>
        /// <param name="serviceLocator"></param>
        /// <param name="registrationType"></param>
        public NamingRegistrationConvention(IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton) : base(serviceLocator, registrationType)
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
                    var implementationTypeName = interfaceType.Name.Replace("I", "").Trim();

                    var implementationType =
                        typesToHandle.FirstOrDefault(typeToHandle => TagHelper.AreTagsEqual(typeToHandle.Name, implementationTypeName));

                    if (implementationType != null && interfaceType.IsAssignableFrom(implementationType))
                    {
                        Container.RegisterType(interfaceType, implementationType, registrationType: RegistrationType);
                    }
                }
                else
                {
                    var implementationType = type;
                    var interfaceTypeName = string.Format("I{0}", implementationType.Name);

                    var interfaceType =
                        typesToHandle.FirstOrDefault(row => TagHelper.AreTagsEqual(row.Name, interfaceTypeName));

                    if (interfaceType != null && interfaceType.IsAssignableFrom(implementationType))
                    {
                        Container.RegisterType(interfaceType, implementationType, registrationType: RegistrationType);
                    }
                }
            });
        }
        #endregion
    }
}