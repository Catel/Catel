// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Environment.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System.Windows;
    using Data;
    using System;
    using IoC;
    using Logging;
    using MVVM;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

#if !NET
    using System.ComponentModel;
#else
    using System.ComponentModel;
    using System.Diagnostics;
#endif

    /// <summary>
    /// Class containing environment information.
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// The default value for dependency properties that use a Properties.Resources value. Such values should be set in the constructor
        /// of the control instead of the dependency property to allow runtime switching of languages.
        /// </summary>
        public const string DefaultMultiLingualDependencyPropertyValue = "SET IN CONSTRUCTOR TO SUPPORT RUNTIME LANGUAGE SWITCHING";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets or sets a value indicating whether the environment is currently in design mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the environment is in design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInDesignMode
        {
            get
            {
                if (_isInDesignMode.HasValue)
                {
                    return _isInDesignMode.Value;
                }

#if NET
                var prop = DesignerProperties.IsInDesignModeProperty;
                _isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

                if (!BypassDevEnvCheck)
                {
                    if (!_isInDesignMode.Value && EnvironmentHelper.IsProcessHostedByTool)
                    {
                        _isInDesignMode = true;
                    }
                }
#elif NETFX_CORE
                _isInDesignMode = global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
                _isInDesignMode = DesignerProperties.IsInDesignTool;
#endif

                return _isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the "devenv.exe" check should be bypassed. By default, the <see cref="IsInDesignMode"/>
        /// also checks whether the current process is "devenv.exe".
        /// <para />
        /// This behavior is not very useful when using Catel in Visual Studio extensions, so it is possible to bypass that specific check.
        /// </summary>
        /// <value><c>true</c> if the check should be bypassed; otherwise, <c>false</c>.</value>
        public static bool BypassDevEnvCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether property change notifications are currently disabled for all instances.
        /// </summary>
        /// <value><c>true</c> if property change notifications should be disabled for all instances; otherwise, <c>false</c>.</value>
        public static bool DisablePropertyChangeNotifications
        {
            get { return ModelBase.DisablePropertyChangeNotifications; }
            set { ModelBase.DisablePropertyChangeNotifications = value; }
        }

        /// <summary>
        /// Registers the default view model services in the default <see cref="ServiceLocator"/>. This call can come in handy when the 
        /// services should be accessed before any view model is created.
        /// </summary>
        public static void RegisterDefaultViewModelServices()
        {
            ViewModelServiceHelper.RegisterDefaultViewModelServices(ServiceLocator.Default);
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Gets the main window of the application. This is a convenience call to easily allow the retrieval of the main window
        /// for each target framework (WPF, SL, WP7).
        /// </summary>
        public static Window MainWindow
        {
            get
            {
#if NETFX_CORE
                return Window.Current;
#else
                var application = Application.Current;
                if (application == null)
                {
                    return null;
                }

                try
                {
                    return application.MainWindow;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to retrieve the applications MainWindow, returning null");
                    return null;
                }
#endif
            }
        }
#endif
    }
}