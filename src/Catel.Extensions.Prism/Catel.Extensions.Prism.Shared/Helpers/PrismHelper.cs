// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrismHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Windows;
    using IoC;
    using Logging;
    using Microsoft.Practices.Prism.Regions;

    /// <summary>
    /// Prism helper class.
    /// </summary>
    public static class PrismHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Prepares prism without requiring a bootstrapper implementation.
        /// </summary>
        public static void PrepareWithoutBootstrapper()
        {
            var standaloneBootstrapper = new StandaloneBootstrapper();
            standaloneBootstrapper.Run(true);
        }

        /// <summary>
        /// Initializes the main window once it has been created.
        /// </summary>
        public static void InitializeMainWindow()
        {
            Log.Debug("Initializing main window in prism standalone mode");

            var app = Application.Current;
            if (app == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Application.Current == null, create the application before calling this method");
            }

            var mainWindow = app.MainWindow;
            if (mainWindow == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Application.Current.MainWindow == null, create the main window before calling this method");
            }

            var serviceLocator = mainWindow.GetServiceLocator();
            var regionManager = serviceLocator.ResolveType<IRegionManager>();

            RegionManager.SetRegionManager(mainWindow, regionManager);
        }
    }
}