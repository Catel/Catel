// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
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
	}
}
