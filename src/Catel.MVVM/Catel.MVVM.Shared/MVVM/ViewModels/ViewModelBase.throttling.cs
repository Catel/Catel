// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.throttling.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Services;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows.Threading;
#endif

    public partial class ViewModelBase
    {
        #region Fields
        /// <summary>
        /// The throttling timer.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly DispatcherTimer _throttlingTimer = new DispatcherTimer();

        /// <summary>
        /// The throttling rate.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private TimeSpan _throttlingRate = new TimeSpan(0);

        /// <summary>
        /// A value indicating whether throttling is enabled.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _isThrottlingEnabled;

        /// <summary>
        /// A value indicating whether throttling is currently being handled.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _isHandlingThrottlingNotifications;

        /// <summary>
        /// Lock object for throttling.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly object _throttlingLockObject = new object();

        /// <summary>
        /// The properties queue used when throttling is enabled.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private Dictionary<string, DateTime> _throttlingQueue = new Dictionary<string, DateTime>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RaisePropertyChanged"/> will be dispatched using
        /// the <see cref="IDispatcherService"/>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        [ExcludeFromValidation]
        protected bool DispatchPropertyChangedEvent { get; set; }

        /// <summary>
        /// Gets or sets the throttling rate.
        /// <para />
        /// When throttling is enabled, the view model will raise property changed event in a timely manner to
        /// reduce the number of updates the view has to do based on the properties.
        /// </summary>
        /// <value>The throttling rate.</value>
        [ExcludeFromValidation]
        protected TimeSpan ThrottlingRate
        {
            get
            {
                return _throttlingRate;
            }
            set
            {
                Log.Debug("Updating throttling rate of view model '{0}' to an interval of '{1}' ms", UniqueIdentifier, value.TotalMilliseconds);

                _throttlingRate = value;
                if (_throttlingRate.TotalMilliseconds.Equals(0d))
                {
                    _isThrottlingEnabled = false;

                    _throttlingTimer.Stop();

                    Log.Debug("Throttling is disabled because the throttling rate is set to 0");
                }
                else
                {
                    _isThrottlingEnabled = true;

                    _throttlingTimer.Stop();
                    _throttlingTimer.Interval = _throttlingRate;
                    _throttlingTimer.Start();

                    Log.Debug("Throttling is enabled because the throttling rate is set to '{0}' ms", _throttlingRate.TotalMilliseconds);
                }
            }
        }
        #endregion

        #region Methods
        partial void InitializeThrottling()
        {
            _throttlingTimer.Tick += (sender, e) => OnThrottlingTimerTick();
        }

        partial void UninitializeThrottling()
        {
            _isThrottlingEnabled = false;
            _throttlingTimer.Stop();
        }

        /// <summary>
        /// Called when the throttling timer ticks.
        /// </summary>
        private void OnThrottlingTimerTick()
        {
            Dictionary<string, DateTime> throttlingQueue;

            lock (_throttlingLockObject)
            {
                throttlingQueue = _throttlingQueue;
                _throttlingQueue = new Dictionary<string, DateTime>();
            }

            if (throttlingQueue.Count == 0)
            {
                return;
            }

            _isHandlingThrottlingNotifications = true;

            foreach (var throttledProperty in throttlingQueue)
            {
                RaisePropertyChanged(throttledProperty.Key);
            }

            _isHandlingThrottlingNotifications = false;
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
            if (_isThrottlingEnabled && !_isHandlingThrottlingNotifications)
            {
                lock (_throttlingLockObject)
                {
                    _throttlingQueue[e.PropertyName] = DateTime.Now;
                }

                return;
            }

            if (DispatchPropertyChangedEvent)
            {
                _dispatcherService.BeginInvokeIfRequired(() => base.RaisePropertyChanged(sender, e));
            }
            else
            {
                base.RaisePropertyChanged(sender, e);
            }
        }
        #endregion
    }
}

#endif