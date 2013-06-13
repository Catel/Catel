// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompassService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    /// <summary>
    /// Interface for retrieving the compass information.
    /// </summary>
    public interface ICompassService : ISensorService<ICompassValue, CompassValueChangedEventArgs>
    {
    }
}