// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyPropertyChanging.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if (NETFX_CORE && !UWP) || PCL

namespace System.ComponentModel
{
    /// <summary>
    /// Handler for the <see cref="INotifyPropertyChanging.PropertyChanging"/> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> instance containing the event data.</param>
    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);

    /// <summary>
    /// INotifyPropertyChanging implementation for platforms not supporting INotifyPropertyChanging.
    /// </summary>
    public interface INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs when a property is about to change on the object.
        /// </summary>
        event PropertyChangingEventHandler PropertyChanging;
    }
}

#endif