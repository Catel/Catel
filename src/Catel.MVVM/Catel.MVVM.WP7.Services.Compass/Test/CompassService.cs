// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompassService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    /// <summary>
    /// Test implementation of the compass service.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.CompassService service = (Test.CompassService)GetService<IAccelerometerService>();
    /// 
    /// // Queue the next value (and then wait 5 seconds)
    /// var testData = new SensorTestData(new CompassValue(/* construct value here */);
    /// service.ExpectedValues.Add(testData);
    /// 
    /// // Go to the next value manually
    /// service.ProceedToNextValue();
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class CompassService : SensorServiceBase<ICompassValue, CompassValueChangedEventArgs>, ICompassService
    {
        // No implementation required, all done by abstract base class!
    }
}
