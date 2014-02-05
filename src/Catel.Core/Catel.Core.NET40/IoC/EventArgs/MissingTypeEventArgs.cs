// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingTypeEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Event arguments for the <see cref="IServiceLocator.MissingType"/> event.
    /// <para>
    /// </para>
    /// These event arguments will be fired. To resolve a type, set either the <see cref="ImplementingInstance"/>
    /// or <see cref="ImplementingType"/>. If both are filled, the instance will be used.
    /// </summary>
    public class MissingTypeEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTypeEventArgs"/> class. 
        /// </summary>
        /// <param name="interfaceType">
        /// Type of the interface.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> is <c>null</c>.</exception>
        public MissingTypeEventArgs(Type interfaceType)
        {
            Argument.IsNotNull("interfaceType", interfaceType);

            InterfaceType = interfaceType;
            RegistrationType = RegistrationType.Singleton;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the interface that is currently unresolved.
        /// </summary>
        /// <value>The type of the interface.</value>
        public Type InterfaceType { get; private set; }

        /// <summary>
        /// Gets or sets the implementing instance.
        /// <para />
        /// Set if the registration of an instance is required.
        /// </summary>
        /// <value>The implementing instance.</value>
        public object ImplementingInstance { get; set; }

        /// <summary>
        /// Gets or sets the implementing type.
        /// <para />
        /// Set if the registration of a type is required.
        /// </summary>
        /// <value>The implementing type.</value>
        public Type ImplementingType { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets the life style of the type that will be registered. 
        /// </summary>
        /// <remarks>
        /// If the <see cref="ImplementingInstance"/> is set then this value will be ignored.
        /// </remarks>
        public RegistrationType RegistrationType { get; set; }
        #endregion
    }
}