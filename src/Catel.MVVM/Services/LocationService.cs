// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using Catel.Logging;

    /// <summary>
    /// Implementation of the <see cref="ILocationService" />
    /// </summary>
    public partial class LocationService : LocationServiceBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationService"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        public LocationService(IDispatcherService dispatcherService)
            : base(dispatcherService)
        {
        }
    }
}