// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdvancedNotifyPropertyChanged.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.ComponentModel;

    /// <summary>
    /// Interface defining advanced additional functionality for the <see cref="INotifyPropertyChanged"/> interface. This
    /// interface still supports the "old" way, so this can perfectly be used by any other class.
    /// </summary>
    public interface IAdvancedNotifyPropertyChanged : INotifyPropertyChanged
    {
    }
}