// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandlerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using Callbacks;
    using IoC;
    using Services;

    /// <summary>
    /// Represents the handler base implementation.
    /// </summary>
    public class HandlerBase : ServiceBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerBase" /> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, <see cref="ServiceLocator.Default" /> will be used.</param>
        /// <param name="typeFactory">The type factory. If <c>null</c>, <see cref="Catel.IoC.TypeFactory.Default" /> will be used.</param>
        /// <param name="targetInstanceToUse">The target instance you want use in proxy instanciation.</param>
        public HandlerBase(Type serviceType, object tag = null, IServiceLocator serviceLocator = null, object targetInstanceToUse = null, ITypeFactory typeFactory = null)
        {
            ServiceType = serviceType;
            Tag = tag;
            TypeFactory = typeFactory ?? IoC.TypeFactory.Default;
            Container = serviceLocator ?? ServiceLocator.Default;
            TargetInstanceToUse = targetInstanceToUse;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        protected internal Type ServiceType { get; private set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        protected internal object Tag { get; private set; }

        /// <summary>
        ///     Gets the default <see cref="ITypeFactory" /> for the application.
        /// </summary>
        /// <value>
        ///     The default <see cref="ITypeFactory" /> instance.
        /// </value>
        protected internal ITypeFactory TypeFactory { get; private set; }

        /// <summary>
        ///     Gets the specified target instance to use in proxy instanciation.
        /// </summary>
        /// <value>
        ///     The specified target instance.
        /// </value>
        protected internal object TargetInstanceToUse { get; private set; }

        /// <summary>
        ///     Gets the default <see cref="IServiceLocator" /> for the application.
        /// </summary>
        /// <value>
        ///     The default <see cref="IServiceLocator" /> instance.
        /// </value>
        protected IServiceLocator Container { get; private set; }

        /// <summary>
        ///     Gets the implemented types.
        /// </summary>
        /// <value>
        ///     The implemented types.
        /// </value>
        protected internal virtual IList<Type> ImplementedTypes { get; protected set; }

        /// <summary>
        ///     Gets the callbacks.
        /// </summary>
        /// <value>
        ///     The callbacks.
        /// </value>
        protected internal virtual ICacheStorage<IMemberDefinition, CallbackCollection> Callbacks { get; protected set; }

        /// <summary>
        ///     Gets the intercepted members.
        /// </summary>
        /// <value>
        ///     The intercepted members.
        /// </value>
        protected internal virtual IList<IMemberDefinition> InterceptedMembers { get; protected set; }
        #endregion
    }
}