﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Services
{
    using System;

    /// <summary>
    /// Service that allows scheduling of actions in the future.
    /// </summary>
    public interface ISchedulerService
    {
        /// <summary>
        /// Schedules the specified action in a relative <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="timeSpan"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeSpan"/> is in the past.</exception>
        void Schedule(Action action, TimeSpan timeSpan);

        /// <summary>
        /// Schedules the specified action in an absolute <see cref="DateTime"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="dateTime">The date time.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dateTime"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dateTime"/> is in the past.</exception>
        void Schedule(Action action, DateTime dateTime);
    }
}