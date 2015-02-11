﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using Catel.MVVM.Views;
    

#if WINDOWS_PHONE && SILVERLIGHT
    using System.Windows;
    using Catel.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
#endif

#if XAMARIN || (WINDOWS_PHONE && SILVERLIGHT)
    /// <summary>
    /// Phone service.
    /// </summary>
    public class PhoneService : IPhoneService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneService"/> class.
        /// </summary>
        public PhoneService()
	    {
#if (WINDOWS_PHONE && SILVERLIGHT)
            StartupMode = (StartupMode)PhoneApplicationService.Current.StartupMode;
#endif
	    }

        /// <summary>
        /// Gets the startup mode.
        /// </summary>
        /// <value>The startup mode.</value>
        public StartupMode StartupMode { get; private set; }
    }
#endif
}