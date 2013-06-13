// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using IoC;

    /// <summary>
	/// Base class for services.
	/// </summary>
    public abstract class ServiceBase : IService
	{
		#region Fields
		#endregion

		#region Constructors
		#endregion

		#region Properties
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }
        #endregion

		#region Methods
        /// <summary>
        /// Gets the service from the <see cref="ServiceLocator"/>.
        /// </summary>
        /// <typeparam name="TService">The service.</typeparam>
        /// <returns>The service resolved from the service locator.</returns>
        protected TService GetService<TService>(object tag = null)
        {
            return (TService)ServiceLocator.Default.ResolveType(typeof(TService), tag);
        }
		#endregion
	}
}
