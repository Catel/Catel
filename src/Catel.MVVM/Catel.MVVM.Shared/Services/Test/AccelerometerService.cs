// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services.Test
{
    /// <summary>
    /// Test implementation of the accelerometer service.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.AccelerometerService service = (Test.AccelerometerService)GetService<IAccelerometerService>();
    /// 
    /// // Queue the next value (and then wait 5 seconds)
    /// var testData = new SensorTestData(new AccelerometerValue(new DateTimeOffset(DateTime.Now, new TimeSpan(200)), 1d, 2d, 3d));
    /// service.ExpectedValues.Add(testData);
    /// 
    /// // Go to the next value manually
    /// service.ProceedToNextValue();
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class AccelerometerService : SensorServiceBase<IAccelerometerValue, AccelerometerValueChangedEventArgs>, IAccelerometerService
    {
        // No implementation required, all done by abstract base class!
    }
}
