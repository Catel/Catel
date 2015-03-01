// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Tasks;

    using IoC;
    using Logging;

    using Microsoft.Practices.Prism;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;

    using MVVM;
    using Services;
    using MVVM.Tasks;

    using Reflection;

    /// <summary>
    /// The service locator bootstrapper.
    /// </summary>
    public abstract class BootstrapperBase : Bootstrapper
    {
        #region Fields

        /// <summary>
        /// The use default configuration.
        /// </summary>
        private bool _useDefaultConfiguration = true;

        private readonly IServiceLocator _serviceLocator;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperBase"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        protected BootstrapperBase(IServiceLocator serviceLocator = null)
        {
            _serviceLocator = serviceLocator ?? ServiceLocator.Default;

            ViewModelServiceHelper.RegisterDefaultViewModelServices(_serviceLocator);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the default <see cref="IServiceLocator"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IServiceLocator"/> instance.</value>
        protected IServiceLocator Container { get; private set; }
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the logger is created.
        /// </summary>
        public event EventHandler<EventArgs> CreatedLogger;

        /// <summary>
        /// Occurs when the module catalog is created.
        /// </summary>
        public event EventHandler<EventArgs> CreatedModuleCatalog;

        /// <summary>
        /// Occurs when the module catalog is configured.
        /// </summary>
        public event EventHandler<EventArgs> ConfiguredModuleCatalog;

        /// <summary>
        /// Occurs when the service locator container is created.
        /// </summary>
        public event EventHandler<EventArgs> CreatedServiceLocatorContainer;

        /// <summary>
        /// Occurs when the service locator container is configured.
        /// </summary>
        public event EventHandler<EventArgs> ConfiguredServiceLocatorContainer;

        /// <summary>
        /// Occurs when the service locator is configured.
        /// </summary>
        public event EventHandler<EventArgs> ConfiguredServiceLocator;

        /// <summary>
        /// Occurs when the region adapters are configured.
        /// </summary>
        public event EventHandler<EventArgs> ConfiguredRegionAdapters;

        /// <summary>
        /// Occurs when the default region behaviors are configured.
        /// </summary>
        public event EventHandler<EventArgs> ConfiguredDefaultRegionBehaviors;

        /// <summary>
        /// Occurs when the framework exception types are registered.
        /// </summary>
        public event EventHandler<EventArgs> RegisteredFrameworkExceptionTypes;

        /// <summary>
        /// Occurs when the shell is created.
        /// </summary>
        public event EventHandler<EventArgs> CreatedShell;

        /// <summary>
        /// Occurs when the modules are initialized.
        /// </summary>
        public event EventHandler<EventArgs> InitializedModules;

        /// <summary>
        /// Occurs when the shell is initialized.
        /// </summary>
        public event EventHandler<EventArgs> InitializedShell;

        /// <summary>
        /// Occurs when the bootstrapper is completed.
        /// </summary>
        public event EventHandler<EventArgs> BootstrapperCompleted;
        #endregion

        #region Methods

        /// <summary>
        /// The create container.
        /// </summary>
        /// <returns>The current instance of <see cref="IServiceLocator" />.</returns>
        private IServiceLocator CreateContainer()
        {
            return _serviceLocator;
        }

        /// <summary>
        /// Runs the bootstrapper.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If set to <c>true</c>, the tasks should run with the default configuration.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the Logger is not successfully initialized.
        /// -or-
        /// The ModuleCatalog is not successfully initialized.
        /// -or-
        /// The ServiceLocator is not successfully initialized.
        /// </exception>
        public override void Run(bool runWithDefaultConfiguration)
        {
            var tasks = CreateInitializationTasks(runWithDefaultConfiguration);

            foreach (var task in tasks)
            {
                task.Execute();
            }
        }

        /// <summary>
        /// Runs the bootstrapper using the <see cref="ISplashScreenService" />.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="runWithDefaultConfiguration">If set to <c>true</c>, the tasks should run with the default configuration.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the Logger is not successfully initialized.
        /// -or-
        /// The ModuleCatalog is not successfully initialized.
        /// -or-
        /// The ServiceLocator is not successfully initialized.
        /// </exception>
        public void RunWithSplashScreen<TViewModel>(bool runWithDefaultConfiguration = true)
            where TViewModel : IProgressNotifyableViewModel
        {
            var tasks = CreateInitializationTasks(runWithDefaultConfiguration);

            var splashScreenService = _serviceLocator.ResolveType<ISplashScreenService>();

            foreach (var task in tasks)
            {
                splashScreenService.Enqueue(task);
            }

            splashScreenService.CommitAsync<TViewModel>();
        }

        /// <summary>
        /// Creates the initialization tasks.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If set to <c>true</c>, the tasks should run with the default configuration.</param>
        /// <returns>Array of tasks to execute to complete the initialization.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the Logger is not successfully initialized.
        /// -or-
        /// The ModuleCatalog is not successfully initialized.
        /// -or-
        /// The ServiceLocator is not successfully initialized.
        /// </exception>
        protected virtual ITask[] CreateInitializationTasks(bool runWithDefaultConfiguration)
        {
            _useDefaultConfiguration = runWithDefaultConfiguration;

            var taskFactory = ServiceLocator.Default.ResolveType<IBootstrapperTaskFactory>();

            var actions = new List<ITask>();

            actions.Add(taskFactory.CreateCreateLoggerTask(() =>
            {
                Logger = CreateLogger();
                /*
                This condition is always false.
                if (Logger == null)
                {
                    throw new InvalidOperationException("The ILoggerFacade is required and cannot be null");
                }
                */

                CreatedLogger.SafeInvoke(this);

                Logger.Log("Created logger", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateCreateModuleCatalogTask(() =>
            {
                Logger.Log("Creating module catalog", Category.Debug, Priority.Low);

                ModuleCatalog = CreateModuleCatalog();
                if (ModuleCatalog == null)
                {
                    throw new InvalidOperationException("The IModuleCatalog is required and cannot be null in order to initialize the modules");
                }

                CreatedModuleCatalog.SafeInvoke(this);

                Logger.Log("Created module catalog", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateConfigureModuleCatalogTask(() =>
            {
                Logger.Log("Configuring module catalog", Category.Debug, Priority.Low);

                ConfigureModuleCatalog();

                ConfiguredModuleCatalog.SafeInvoke(this);

                Logger.Log("Configured module catalog", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateCreateServiceLocatorContainerTask(() =>
            {
                Logger.Log("Creating service locator container", Category.Debug, Priority.Low);

                Container = CreateContainer();

                CreatedServiceLocatorContainer.SafeInvoke(this);

                Logger.Log("Created service locator container", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateConfigureServiceLocatorContainerTask(() =>
            {
                Logger.Log("Configuring the service locator container", Category.Debug, Priority.Low);

                ConfigureContainer();

                ConfiguredServiceLocatorContainer.SafeInvoke(this);

                Logger.Log("Configured the container", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateConfigureServiceLocatorTask(() =>
            {
                Logger.Log("Configuring service locator", Category.Debug, Priority.Low);

                ConfigureServiceLocator();

                ConfiguredServiceLocator.SafeInvoke(this);

                Logger.Log("Configured service locator", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateConfigureRegionAdaptersTask(() =>
            {
                Logger.Log("Configuring region adapters", Category.Debug, Priority.Low);

                ConfigureRegionAdapterMappings();

                ConfiguredRegionAdapters.SafeInvoke(this);

                Logger.Log("Configured region adapters", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateConfigureDefaultRegionBehaviorsTask(() =>
            {
                Logger.Log("Configuring default region behaviors", Category.Debug, Priority.Low);

                ConfigureDefaultRegionBehaviors();

                ConfiguredDefaultRegionBehaviors.SafeInvoke(this);

                Logger.Log("Configured default region behaviors", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateRegisterFrameworkExceptionTypesTask(() =>
            {
                Logger.Log("Registering Framework Exception Types", Category.Debug, Priority.Low);

                RegisterFrameworkExceptionTypes();

                RegisteredFrameworkExceptionTypes.SafeInvoke(this);

                Logger.Log("Registered Framework Exception Types", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateCreateShellTask(() =>
            {
                Logger.Log("Creating the shell", Category.Debug, Priority.Low);
                var dispatcherService = Container.ResolveType<IDispatcherService>();

                dispatcherService.Invoke(() => Shell = CreateShell());

                if (Shell != null)
                {
                    Logger.Log("Setting the RegionManager", Category.Debug, Priority.Low);
                    dispatcherService.Invoke(() => RegionManager.SetRegionManager(Shell, Container.ResolveType<IRegionManager>()));

                    Logger.Log("Updating Regions", Category.Debug, Priority.Low);
                    dispatcherService.Invoke(RegionManager.UpdateRegions);

                    CreatedShell.SafeInvoke(this);
                }

                Logger.Log("Created the shell", Category.Debug, Priority.Low);
            }));

            actions.Add(taskFactory.CreateInitializeModulesTask(() =>
            {
                if (Container.IsTypeRegistered<IModuleManager>())
                {
                    Logger.Log("Initializing modules", Category.Debug, Priority.Low);

                    InitializeModules();

                    InitializedModules.SafeInvoke(this);

                    Logger.Log("Initialized modules", Category.Debug, Priority.Low);
                }
            }));

            IList<ITask> bootTasks = new List<ITask>();
            InitializeBootTasks(bootTasks);
            actions.AddRange(bootTasks);

            actions.Add(taskFactory.CreateInitializingShellTask(() =>
            {
                if (Shell != null)
                {
                    Logger.Log("Initializing the shell", Category.Debug, Priority.Low);

                    var dispatcherService = Container.ResolveType<IDispatcherService>();
                    dispatcherService.Invoke(InitializeShell);

                    InitializedShell.SafeInvoke(this);
                }

                BootstrapperCompleted.SafeInvoke(this);

                Logger.Log("Bootstrapper sequence completed", Category.Debug, Priority.Low);
            }));

            return actions.ToArray();
        }

        /// <summary>
        /// Initialize boot tasks. 
        /// </summary>
        /// <param name="bootTasks">The additional boot tasks.</param>
        /// <remarks>Override this method to add additional tasks that will be executed before shell initialization.</remarks>
        protected virtual void InitializeBootTasks(IList<ITask> bootTasks)
        {
        }

        /// <summary>
        /// Create the <see cref="T:Microsoft.Practices.Prism.Logging.ILoggerFacade" /> used by the bootstrapper.
        /// </summary>
        /// <returns>An implementation of <see cref="ILoggerFacade" />.</returns>
        /// <remarks>The base implementation returns a new TextLogger.</remarks>
        protected override sealed ILoggerFacade CreateLogger()
        {
            return new LoggerFacadeAdapter(LogManager.GetCurrentClassLogger());
        }

        /// <summary>
        /// The configure container.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Container.RegisterInstance<ILoggerFacade>(Logger);
            Container.RegisterInstance<IModuleCatalog>(ModuleCatalog);

            if (_useDefaultConfiguration)
            {
                Container.RegisterTypeIfNotYetRegistered<Microsoft.Practices.ServiceLocation.IServiceLocator, ServiceLocatorAdapter>();
                Container.RegisterTypeIfNotYetRegistered<IModuleInitializer, Microsoft.Practices.Prism.Modularity.ModuleInitializer>();
                Container.RegisterTypeIfNotYetRegistered<IModuleManager, ModuleManager>();
                Container.RegisterTypeIfNotYetRegistered<RegionAdapterMappings, RegionAdapterMappings>();
                Container.RegisterTypeIfNotYetRegistered<IRegionManager, RegionManager>();
                Container.RegisterTypeIfNotYetRegistered<IEventAggregator, EventAggregator>();
                Container.RegisterTypeIfNotYetRegistered<IRegionViewRegistry, RegionViewRegistry>();
                Container.RegisterTypeIfNotYetRegistered<IRegionBehaviorFactory, RegionBehaviorFactory>();
                Container.RegisterTypeIfNotYetRegistered<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(RegistrationType.Transient);
                Container.RegisterTypeIfNotYetRegistered<IRegionNavigationJournal, RegionNavigationJournal>(RegistrationType.Transient);
                Container.RegisterTypeIfNotYetRegistered<IRegionNavigationService, RegionNavigationService>(RegistrationType.Transient);
                Container.RegisterTypeIfNotYetRegistered<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            }
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator"/>.
        /// </summary>
        protected override sealed void ConfigureServiceLocator()
        {
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => Container.ResolveType<Microsoft.Practices.ServiceLocation.IServiceLocator>());
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog.
        /// </summary>
        protected override void InitializeModules()
        {
            var moduleManager = Container.ResolveType<IModuleManager>();

            int registeredModulesCount = 0;
            Logger.Log("Registering currently available modules automatically", Category.Debug, Priority.Low);

            foreach (var module in ModuleCatalog.Modules)
            {
                if (RegisterModule(module))
                {
                    registeredModulesCount++;
                }
            }

            Logger.Log(string.Format("Registered '{0}' available modules automatically", registeredModulesCount), Category.Debug, Priority.Low);

            moduleManager.ModuleDownloadProgressChanged += OnModuleDownloadProgressChanged;

            moduleManager.Run();
        }

        /// <summary>
        /// Called when the <see cref="IModuleManager" /> raises the <see cref="IModuleManager.ModuleDownloadProgressChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Practices.Prism.Modularity.ModuleDownloadProgressChangedEventArgs" /> instance containing the event data.</param>
        private void OnModuleDownloadProgressChanged(object sender, ModuleDownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                RegisterModule(e.ModuleInfo);
            }
        }

        /// <summary>
        /// Registers the module.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <returns>The register module.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        private bool RegisterModule(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull("moduleInfo", moduleInfo);

            var moduleType = TypeCache.GetType(moduleInfo.ModuleType);
            if (moduleType == null)
            {
                Logger.Log(string.Format("Cannot register module type '{0}' automatically, type not found", moduleInfo.ModuleType), Category.Warn, Priority.High);
                return false;
            }

            Logger.Log(string.Format("Registering module type '{0}' automatically", moduleType.FullName), Category.Debug, Priority.Low);
            Container.RegisterType(moduleType, moduleType);

            return true;
        }

        #endregion

        #region Nested type: LoggerFacadeAdapter
        /// <summary>
        /// The logger facade adapter.
        /// </summary>
        public class LoggerFacadeAdapter : LogListenerBase, ILoggerFacade
        {
            #region Constants

            /// <summary>
            /// The pattern.
            /// </summary>
            private const string CatelToLoggerFacadeMessagePattern = @"\A(?:\[[^]]+\]\s\[(None|Low|Medium|High)\]\s(.+))\Z";

            /// <summary>
            /// The pattern 2.
            /// </summary>
            private const string CatelMessagePattern = @"\A(?:\[[^]]+\]\s(.+))\Z";
            #endregion

            #region Fields

            /// <summary>
            /// The _regex 2.
            /// </summary>
            private readonly Regex _catelRegex = new Regex(CatelMessagePattern, RegexOptions.IgnorePatternWhitespace);

            /// <summary>
            /// The _regex.
            /// </summary>
            private readonly Regex _catelToLoggerFacadeRegex = new Regex(CatelToLoggerFacadeMessagePattern, RegexOptions.IgnorePatternWhitespace);

            /// <summary>
            /// The log.
            /// </summary>
            private readonly ILog _log;
            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LoggerFacadeAdapter"/> class. 
            /// </summary>
            /// <param name="log">
            /// The log.
            /// </param>
            /// <param name="relayCatelMessageToLoggerFacade">
            /// Option to relay catel message to logger facade
            /// </param>
            /// <exception cref="System.ArgumentNullException">
            /// The <paramref name="log"/> is <c>null</c>.
            /// </exception>
            public LoggerFacadeAdapter(ILog log, bool relayCatelMessageToLoggerFacade = false)
            {
                Argument.IsNotNull("log", log);
                _log = log;
                if (relayCatelMessageToLoggerFacade)
                {
                    LogManager.AddListener(this);
                }
            }

            #endregion

            #region ILoggerFacade Members

            /// <summary>
            /// The log.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="category">
            /// The category.
            /// </param>
            /// <param name="priority">
            /// The priority.
            /// </param>
            void ILoggerFacade.Log(string message, Category category, Priority priority)
            {
                switch (category)
                {
                    case Category.Debug:
                        _log.Debug(priority, message);
                        break;

                    case Category.Warn:
                        _log.Warning(priority, message);
                        break;

                    case Category.Exception:
                        _log.Error(priority, message);
                        break;

                    case Category.Info:
                        _log.Info(priority, message);
                        break;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            /// <param name="time">The time.</param>
            protected override void Debug(ILog log, string message, object extraData, DateTime time)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Debug);
            }

            /// <summary>
            /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            /// <param name="time">The time.</param>
            protected override void Info(ILog log, string message, object extraData, DateTime time)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Info);
            }

            /// <summary>
            /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            /// <param name="time">The time.</param>
            protected override void Warning(ILog log, string message, object extraData, DateTime time)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Warn);
            }

            /// <summary>
            /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            /// <param name="time">The time.</param>
            protected override void Error(ILog log, string message, object extraData, DateTime time)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Exception);
            }

            /// <summary>
            /// The relay log to logger facade if required.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="category">The category.</param>
            private void RelayLogMessageToLoggerFacadeIfRequired(string message, Category category)
            {
                Match match = _catelToLoggerFacadeRegex.Match(message);
                if (!match.Success)
                {
                    Match match2 = _catelRegex.Match(message);
                    if (match2.Success)
                    {
                        (this as ILoggerFacade).Log(match2.Groups[1].Value, category, Priority.None);
                    }
                }
            }

            #endregion
        }
        #endregion
    }
}