// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    
    using IoC;
    using Logging;

    using Microsoft.Practices.Prism;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;

    #if NET
    using Modules.ModuleManager;
    #endif

    using MVVM;
    using MVVM.Services;
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
        /// Initializes the bootstrapper base.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
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

        #region Methods

        /// <summary>
        /// The create container.
        /// </summary>
        /// <returns>
        /// The current instance of <see cref="IServiceLocator"/>.
        /// </returns>
        private IServiceLocator CreateContainer()
        {
            return _serviceLocator;
        }

        /// <summary>
        /// Runs the bootstrapper.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">if set to <c>true</c>, the tasks should run with the default configuration.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the Logger is not successfully initialized
        /// -or-
        /// The ModuleCatalog is not successfully initialized
        /// -or-
        /// The ServiceLocator is not successfully initialized
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
        /// <param name="runWithDefaultConfiguration">if set to <c>true</c>, the tasks should run with the default configuration.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the Logger is not successfully initialized
        /// -or-
        /// The ModuleCatalog is not successfully initialized
        /// -or-
        /// The ServiceLocator is not successfully initialized
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
        /// <param name="runWithDefaultConfiguration">if set to <c>true</c>, the tasks should run with the default configuration.</param>
        /// <returns>Array of tasks to execute to complete the initialization.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the Logger is not successfully initialized
        /// -or-
        /// The ModuleCatalog is not successfully initialized
        /// -or-
        /// The ServiceLocator is not successfully initialized
        /// </exception>
        private ITask[] CreateInitializationTasks(bool runWithDefaultConfiguration)
        {
            _useDefaultConfiguration = runWithDefaultConfiguration;

            var actions = new List<ITask>();

            actions.Add(new ActionTask("Creating logger", x =>
            {
                Logger = CreateLogger();
                /*
                This condition is always false.
                if (Logger == null)
                {
                    throw new InvalidOperationException("The ILoggerFacade is required and cannot be null");
                }
                */

                Logger.Log("Created logger", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Creating module catalog", x =>
            {
                Logger.Log("Creating module catalog", Category.Debug, Priority.Low);

                ModuleCatalog = CreateModuleCatalog();
                if (ModuleCatalog == null)
                {
                    throw new InvalidOperationException("The IModuleCatalog is required and cannot be null in order to initialize the modules");
                }

                Logger.Log("Created module catalog", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Configuring module catalog", x =>
            {
                Logger.Log("Configuring module catalog", Category.Debug, Priority.Low);

                ConfigureModuleCatalog();

                Logger.Log("Configured module catalog", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Creating Service locator container", x =>
            {
                Logger.Log("Creating Service locator container", Category.Debug, Priority.Low);

                Container = CreateContainer();
                /*
                This condition is always false.
                if (Container == null)
                {
                    throw new InvalidOperationException("The IServiceLocator is required and cannot be null");
                }
                */

                Logger.Log("Created Service locator container", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Configuring the container", x =>
            {
                Logger.Log("Configuring the Service Locator container", Category.Debug, Priority.Low);

                ConfigureContainer();

                Logger.Log("Configured the container", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Configuring service locator", x =>
            {
                Logger.Log("Configuring service locator", Category.Debug, Priority.Low);

                ConfigureServiceLocator();

                Logger.Log("Configured service locator", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Configuring region adapters", x =>
            {
                Logger.Log("Configuring region adapters", Category.Debug, Priority.Low);

                ConfigureRegionAdapterMappings();

                Logger.Log("Configured region adapters", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Configuring default region behaviors", x =>
            {
                Logger.Log("Configuring default region behaviors", Category.Debug, Priority.Low);

                ConfigureDefaultRegionBehaviors();

                Logger.Log("Configured default region behaviors", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Registering Framework Exception Types", x =>
            {
                Logger.Log("Registering Framework Exception Types", Category.Debug, Priority.Low);

                RegisterFrameworkExceptionTypes();

                Logger.Log("Registered Framework Exception Types", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Creating the shell", x =>
            {
                Logger.Log("Creating the shell", Category.Debug, Priority.Low);

                Shell = CreateShell();

                if (Shell != null)
                {
                    Logger.Log("Setting the RegionManager", Category.Debug, Priority.Low);
                    RegionManager.SetRegionManager(Shell, Container.ResolveType<IRegionManager>());

                    Logger.Log("Updating Regions", Category.Debug, Priority.Low);
                    RegionManager.UpdateRegions();
                }

                Logger.Log("Created the shell", Category.Debug, Priority.Low);
            }));

            actions.Add(new ActionTask("Initializing modules", x =>
            {
                if (Container.IsTypeRegistered<IModuleManager>())
                {
                    Logger.Log("Initializing modules", Category.Debug, Priority.Low);

                    InitializeModules();

                    Logger.Log("Initialized modules", Category.Debug, Priority.Low);
                }
            }));

            IList<ITask> bootTasks = new List<ITask>();
            InitializeBootTasks(bootTasks);
            actions.AddRange(bootTasks);

            actions.Add(new ActionTask("Starting application", x =>
            {
                if (Shell != null)
                {
                    Logger.Log("Initializing the shell", Category.Debug, Priority.Low);
                    InitializeShell();
                }

                Logger.Log("Bootstrapper sequence completed", Category.Debug, Priority.Low);
            }));

            return actions.ToArray();
        }

        /// <summary>
        /// Initialize boot tasks. 
        /// </summary>
        /// <param name="bootTasks">The aditional boot tasks.</param>
        /// <remarks>Override this method to add aditional tasks that will be executed before shell initialization.</remarks>
        protected virtual void InitializeBootTasks(IList<ITask> bootTasks)
        {
        }

        /// <summary>
        /// Create the <see cref="T:Microsoft.Practices.Prism.Logging.ILoggerFacade"/> used by the bootstrapper.
        /// </summary>
        /// <returns>
        /// An implementation of <see cref="ILoggerFacade"/>.
        /// </returns>
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

#if NET
				Container.RegisterTypeIfNotYetRegistered<IModuleInfoManager, ModuleInfoManager>();
#endif
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
            protected override void Debug(ILog log, string message, object extraData)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Debug);
            }

            /// <summary>
            /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            protected override void Info(ILog log, string message, object extraData)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Info);
            }

            /// <summary>
            /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            protected override void Warning(ILog log, string message, object extraData)
            {
                RelayLogMessageToLoggerFacadeIfRequired(message, Category.Warn);
            }

            /// <summary>
            /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="message">The message.</param>
            /// <param name="extraData">The additional data.</param>
            protected override void Error(ILog log, string message, object extraData)
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