// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceContextExtension.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.ServiceModel;
    using IoC;

    /// <summary>
    /// </summary>
    public class InstanceContextExtension : IExtension<InstanceContext>
    {
        #region Fields
        /// <summary>
        ///     The _child container
        /// </summary>
        private IServiceLocator _childContainer;
        #endregion

        #region IExtension<InstanceContext> Members
        /// <summary>
        ///     Attaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Attach(InstanceContext owner)
        {
        }

        /// <summary>
        ///     Detaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Detach(InstanceContext owner)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Gets the child container.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator" /> is <c>null</c>.</exception>
        public IServiceLocator GetChildContainer(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            return _childContainer ?? (_childContainer = serviceLocator);
        }

        /// <summary>
        ///     Disposes the of child container.
        /// </summary>
        public void DisposeChildContainer()
        {
            _childContainer = null;
        }
        #endregion
    }
}