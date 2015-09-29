// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneBootstrapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Windows;
    using Logging;

    /// <summary>
    /// Standalone bootstrapper to make it possible to run Prism without initialization.
    /// </summary>
    public class StandaloneBootstrapper : BootstrapperBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates the shell.
        /// </summary>
        /// <returns>DependencyObject.</returns>
        protected override DependencyObject CreateShell()
        {
            return new DependencyObject();
        }
    }
}