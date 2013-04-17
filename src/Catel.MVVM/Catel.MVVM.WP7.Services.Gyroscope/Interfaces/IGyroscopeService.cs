// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGyroscopeService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    /// <summary>
    /// Interface for retrieving gyroscope information.
    /// </summary>
    public interface IGyroscopeService : ISensorService<IGyroscopeValue, GyroscopeValueChangedEventArgs>
    {
    }
}