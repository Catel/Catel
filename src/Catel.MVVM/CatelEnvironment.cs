// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelEnvironment.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System.Windows;
    using Data;
    using System;
    using System.ComponentModel;
    using IoC;
    using Logging;
    using MVVM;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

#if NET
    using System.Diagnostics;
#endif

#if XAMARIN_FORMS
    using Xamarin.Forms;
#endif

    /// <summary>
    /// Class containing environment information.
    /// </summary>
    public static class CatelEnvironment
    {
        /// <summary>
        /// The default value for dependency properties that use a Properties.Resources value. Such values should be set in the constructor
        /// of the control instead of the dependency property to allow runtime switching of languages.
        /// </summary>
        public const string DefaultMultiLingualDependencyPropertyValue = "SET IN CONSTRUCTOR TO SUPPORT RUNTIME LANGUAGE SWITCHING";

        private static bool _bypassDevEnvCheck;
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets or sets a value indicating whether the environment is currently in design mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if the environment is in design mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = GetIsInDesignMode(true);
                }

                if (_isInDesignMode.Value)
                {
                    DesignTimeHelper.InitializeDesignTime();
                }

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
        public static bool BypassDevEnvCheck
        {
            get { return _bypassDevEnvCheck; }
            set
            {
                _bypassDevEnvCheck = value;
                _isInDesignMode = null;
            }
        }

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

#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Gets the main window of the application.
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
                    Log.Warning(ex, "Failed to retrieve the application MainWindow, returning null");
                    return null;
                }
#endif
            }
        }
#endif

        /// <summary>
        /// Gets whether the software is currently in design mode.
        /// <para />
        /// Note that unless the <see cref="IsInDesignMode" />, the value is not cached but always determined at runtime.
        /// </summary>
        /// <param name="initializeDesignTime">if set to <c>true</c>, automatically call <see cref="DesignTimeHelper.InitializeDesignTime"/> if in design mode.</param>
        /// <returns><c>true</c> if the software is in design mode, <c>false</c> otherwise.</returns>
        public static bool GetIsInDesignMode(bool initializeDesignTime)
        {
            bool? isInDesignMode = null;

#if NET
            var prop = DesignerProperties.IsInDesignModeProperty;
            isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

            if (!isInDesignMode.Value)
            {
                isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }

            if (!BypassDevEnvCheck && !isInDesignMode.Value && EnvironmentHelper.IsProcessHostedByTool)
            {
                isInDesignMode = true;
            }
#elif NETFX_CORE
            isInDesignMode = global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#elif XAMARIN || XAMARIN_FORMS
            isInDesignMode = false;
#else
            isInDesignMode = DesignerProperties.IsInDesignTool;
#endif

            if (initializeDesignTime && isInDesignMode.Value)
            {
                DesignTimeHelper.InitializeDesignTime();
            }

            return isInDesignMode.Value;
        }
    }
}
