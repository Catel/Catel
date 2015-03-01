// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperBase.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System.Windows;
    using Microsoft.Practices.Prism.Modularity;
    using IoC;

#if NET
    using Windows;
#endif

    /// <summary>
    /// The service locator bootstrapper that allows simple instantiation of the shell.
    /// </summary>
    /// <typeparam name="TShell">
    /// The shell type
    /// </typeparam>
    public abstract class BootstrapperBase<TShell> : BootstrapperBase
#if SILVERLIGHT
        where TShell : UIElement
#else
        where TShell : System.Windows.Window
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperBase{TShell}" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        protected BootstrapperBase(IServiceLocator serviceLocator = null)
            : base(serviceLocator)
        {
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>
        /// The shell of the application.
        /// </returns>
        protected override DependencyObject CreateShell()
        {
            var shell = Container.ResolveType<TShell>();

            return shell;
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        protected override void InitializeShell()
        {
#if SILVERLIGHT
            Application.Current.RootVisual = (UIElement)Shell;
#else
            var shell = Shell as System.Windows.Window;
            if (shell != null)
            {
                Application.Current.MainWindow = shell;

                shell.Owner = null;
                
                shell.Show();
                shell.BringWindowToTop();
            }
#endif
        }

        /// <summary>
        /// The configure container.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            if (!Container.IsTypeRegistered<TShell>() && !typeof(TShell).IsAbstract)
            {
                Container.RegisterType<TShell, TShell>();
            }
        }
    }

    /// <summary>
    /// The service locator bootstrapper that allows simple instantiation of the shell and the module catalog.
    /// </summary>
    /// <typeparam name="TShell">
    /// The shell type
    /// </typeparam>
    /// <typeparam name="TModuleCatalog">
    /// The module catalog type
    /// </typeparam>
    public abstract class BootstrapperBase<TShell, TModuleCatalog> : BootstrapperBase<TShell>
#if SILVERLIGHT
        where TShell : UIElement
#else
        where TShell : System.Windows.Window
#endif
        where TModuleCatalog : class, IModuleCatalog, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperBase{TShell, TModuleCatalog}" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        protected BootstrapperBase(IServiceLocator serviceLocator = null)
            : base(serviceLocator)
        {
        }

        /// <summary>
        /// Creates the <see cref="T:Microsoft.Practices.Prism.Modularity.IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new ModuleCatalog.
        /// </remarks>
        /// <returns>
        /// The instance of <typeparamref name="TModuleCatalog"/> type.
        /// </returns>
        protected override sealed IModuleCatalog CreateModuleCatalog()
        {
            return new TModuleCatalog();
        }

        /// <summary>
        /// Gets the default <see cref="IModuleCatalog"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IModuleCatalog"/> instance.</value>
        protected new TModuleCatalog ModuleCatalog
        {
            get
            {
                return base.ModuleCatalog as TModuleCatalog;
            }
        }
    }
}