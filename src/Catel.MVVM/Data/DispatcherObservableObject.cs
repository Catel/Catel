// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherObservableObject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.ComponentModel;
    using IoC;
    using Services;

    /// <summary>
    /// Implementation of the <see cref="ObservableObject"/> class that will dispatch all change notifications
    /// to the UI thread using the <see cref="IDispatcherService"/>.
    /// </summary>
    public class DispatcherObservableObject : ObservableObject
    {
        /// <summary>
        /// The dispatcher service used to dispatch all calls.
        /// </summary>
        private static readonly IDispatcherService _dispatcherService;

        /// <summary>
        /// Initializes the <see cref="DispatcherObservableObject"/> class.
        /// </summary>
        static DispatcherObservableObject()
        {
            var serviceLocator = IoCConfiguration.DefaultServiceLocator;
            serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();

            _dispatcherService = serviceLocator.ResolveType<IDispatcherService>();
        }

        /// <summary>
        /// Raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// <para/>
        /// This is the one and only method that actually raises the <see cref="ObservableObject.PropertyChanged"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _dispatcherService.BeginInvokeIfRequired(() => base.RaisePropertyChanged(sender, e));
        }
    }
}
