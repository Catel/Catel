// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    /// <summary>
    /// Implementation of the <see cref="IAccelerometerService" />.
    /// </summary>
    public partial class AccelerometerService : SensorServiceBase<IAccelerometerValue, AccelerometerValueChangedEventArgs>, IAccelerometerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerService"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        public AccelerometerService(IDispatcherService dispatcherService)
            : base(dispatcherService)
        {
            Initialize();
        }

        /// <summary>
        /// Starts the sensor service so it's retrieving data.
        /// </summary>
        public override void Start()
        {
            if (IsSupported)
            {
                StartSensor();
            }
        }

        /// <summary>
        /// Stops the sensor service so it's no longer retrieving data.
        /// </summary>
        public override void Stop()
        {
            StopSensor();
        }

        partial void Initialize();
        partial void StartSensor();
        partial void StopSensor();
    }
}