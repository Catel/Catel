// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    public abstract class ServiceBase : IService
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }
    }
}
