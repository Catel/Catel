// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Services
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Service that allows scheduling of actions in the future.
    /// </summary>
    public partial class SchedulerService : ViewModelServiceBase, ISchedulerService
    {
        /// <summary>
        /// Schedules the specified action in a relative <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="timeSpan"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeSpan"/> is in the past.</exception>
        public virtual void Schedule(Action action, TimeSpan timeSpan)
        {
            Argument.IsNotNull("action", action);
            Argument.IsNotNull("timeSpan", timeSpan);
            Argument.IsMinimal("timeSpan", (int)timeSpan.TotalMilliseconds, 0);

            var timer = new DispatcherTimer();
            timer.Interval = timeSpan;
            timer.Tick += (sender, e) =>
                { 
                    action();

                    timer.Stop();
                };

            timer.Start();
        }

        /// <summary>
        /// Schedules the specified action in an absolute <see cref="DateTime"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dateTime"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dateTime"/> is in the past.</exception>
        public virtual void Schedule(Action action, DateTime dateTime)
        {
            Argument.IsNotNull("action", action);
            Argument.IsNotNull("dateTime", dateTime);

            if (dateTime <= FastDateTime.Now)
            {
                throw new ArgumentOutOfRangeException("dateTime", "The date/time cannot be in the past");
            }

            Schedule(action, dateTime - FastDateTime.Now);
        }
    }
}

#endif