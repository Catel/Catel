// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GyroscopeService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    /// <summary>
    /// Test implementation of the gyroscope service.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.GyroscopeService service = (Test.GyroscopeService)GetService<IGyroscopeService>();
    /// 
    /// // Queue the next value (and then wait 5 seconds)
    /// var testData = new SensorTestData(new GyroscopeValue(/* construct value here */);
    /// service.ExpectedValues.Add(testData);
    /// 
    /// // Go to the next value manually
    /// service.ProceedToNextValue();
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class GyroscopeService : SensorServiceBase<IGyroscopeValue, GyroscopeValueChangedEventArgs>, IGyroscopeService
    {
        // No implementation required, all done by abstract base class!
    }
}
