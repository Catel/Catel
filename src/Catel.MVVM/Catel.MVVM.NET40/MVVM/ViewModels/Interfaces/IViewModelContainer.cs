// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelContainer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines a control as a <see cref="IViewModel"/> container.
    /// </summary>
    public interface IViewModelContainer : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        IViewModel ViewModel { get; }

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> property has changed.
        /// </summary>
        event EventHandler<EventArgs> ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;
    }
}
