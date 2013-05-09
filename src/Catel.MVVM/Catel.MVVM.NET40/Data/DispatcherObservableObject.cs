// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherObservableObject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using IoC;
    using MVVM.Services;

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
            var serviceLocator = ServiceLocator.Default;
            serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();

            _dispatcherService = serviceLocator.ResolveType<IDispatcherService>();
        }

        /// <summary>
        /// Raises the <see cref="ObservableObject.PropertyChanging"/> event.
        /// <para/>
        /// This is the one and only method that actually raises the <see cref="ObservableObject.PropertyChanging"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> instance containing the event data.</param>
        protected override void RaisePropertyChanging(object sender, AdvancedPropertyChangingEventArgs e)
        {
            _dispatcherService.BeginInvoke(() => base.RaisePropertyChanging(sender, e));
        }

        /// <summary>
        /// Raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// <para/>
        /// This is the one and only method that actually raises the <see cref="ObservableObject.PropertyChanged"/> event. All other
        /// methods are (and should be) just overloads that eventually call this method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void RaisePropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            _dispatcherService.BeginInvoke(() => base.RaisePropertyChanged(sender, e));
        }
    }
}