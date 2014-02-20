// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleManagerViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Catel.Data;
    using Catel.Logging;
    using Catel.Modules.ModuleManager.Models;
    using Catel.MVVM;
    using Catel.Services;

    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Module manager view model.
    /// </summary>
    public class ModuleManagerViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>Register the Modules property so it is known in the class.</summary>
        public static readonly PropertyData ModulesProperty = RegisterProperty<ModuleManagerViewModel, ObservableCollection<ModuleTemplate>>(model => model.Modules);
        #endregion

        #region Fields
        /// <summary>
        /// The module manager.
        /// </summary>
        private readonly IModuleManager _moduleManager;

        /// <summary>
        /// The please wait service 
        /// </summary>
        private readonly IPleaseWaitService _pleaseWaitService;

        /// <summary>
        /// The lock
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// Indicates if any module is loading
        /// </summary>
        private bool _isLoading;

        /// <summary>
        /// 
        /// </summary>
        private readonly List<string> _modules = new List<string>();

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManagerViewModel" /> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleManager"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="pleaseWaitService"/> is <c>null</c>.</exception>
        public ModuleManagerViewModel(IModuleCatalog moduleCatalog, IModuleManager moduleManager, IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => moduleManager);
            Argument.IsNotNull(() => moduleCatalog);
            Argument.IsNotNull(() => pleaseWaitService);
     
            _moduleManager = moduleManager;
            _pleaseWaitService = pleaseWaitService;
            
            var modules = moduleCatalog.Modules;

            var moduleTemplates = modules.Select(moduleInfo => new ModuleTemplate(moduleInfo)).OrderBy(template => template.ModuleName);

            Modules = new ObservableCollection<ModuleTemplate>(moduleTemplates);
            LoadModuleCommand = new Command<ModuleTemplate>(LoadModuleCommandExecute, LoadModuleCommandCanExecute);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The load module
        /// </summary>
        public Command<ModuleTemplate> LoadModuleCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the application modules.
        /// </summary>
        /// <value>
        /// The application modules.
        /// </value>
        public ObservableCollection<ModuleTemplate> Modules
        {
            get { return GetValue<ObservableCollection<ModuleTemplate>>(ModulesProperty); }
            private set { SetValue(ModulesProperty, value); }
        }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Module Manager"; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Can execute the load module command.
        /// </summary>
        /// <param name="moduleTemplate"></param>
        /// <returns></returns>
        private bool LoadModuleCommandCanExecute(ModuleTemplate moduleTemplate)
        {
            return moduleTemplate.State == ModuleState.NotStarted;
        }

        /// <summary>
        /// Called when the view model is initialized
        /// </summary>
        protected override void Initialize()
        {
            _moduleManager.ModuleDownloadProgressChanged += ModuleManagerOnModuleDownloadProgressChanged;
            _moduleManager.LoadModuleCompleted += ModuleManagerOnLoadModuleCompleted;
        }

        /// <summary>
        /// Called when a module loading process is completed
        /// </summary>
        /// <param name="sender">The module type loade</param>
        /// <param name="loadModuleCompletedEventArgs">The event argument</param>
        private void ModuleManagerOnLoadModuleCompleted(object sender, LoadModuleCompletedEventArgs loadModuleCompletedEventArgs)
        {
            string moduleName = loadModuleCompletedEventArgs.ModuleInfo.ModuleName;
            ModuleTemplate moduleTemplate = Modules.FirstOrDefault(template => template.ModuleName == moduleName);
            if (moduleTemplate != null)
            {
                moduleTemplate.RaisePropertyChanged(() => moduleTemplate.Enabled);
                moduleTemplate.RaisePropertyChanged(() => moduleTemplate.State);
            }
          
            lock (_syncObj)
            {
                _modules.Remove(moduleName);
                if (_modules.Count == 0)
                {
                    _isLoading = false;
                    _pleaseWaitService.Hide();
                }
            }
        }

        /// <summary>
        /// Called when the view model is closed
        /// </summary>
        protected override void Close()
        {
            _moduleManager.ModuleDownloadProgressChanged -= ModuleManagerOnModuleDownloadProgressChanged;
            _moduleManager.LoadModuleCompleted -= ModuleManagerOnLoadModuleCompleted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="moduleDownloadProgressChangedEventArgs"></param>
        private void ModuleManagerOnModuleDownloadProgressChanged(object sender, ModuleDownloadProgressChangedEventArgs moduleDownloadProgressChangedEventArgs)
        {
            lock (_syncObj)
            {
                string moduleName = moduleDownloadProgressChangedEventArgs.ModuleInfo.ModuleName;
                if (!_modules.Contains(moduleName))
                {
                    _modules.Add(moduleName);
                }
            }
        }

        /// <summary>
        /// The load module template.
        /// </summary>
        private void LoadModuleCommandExecute(ModuleTemplate moduleTemplate)
        {
            _isLoading = true;
            _pleaseWaitService.Show(string.Format(CultureInfo.InvariantCulture, "Starting to load module '{0}'", moduleTemplate.ModuleName));

            new Thread(MonitorThread).Start();
            try
            {
                _moduleManager.LoadModule(moduleTemplate.ModuleName);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// Monitor thread.
        /// </summary>
        /// <param name="state"></param>
        private void MonitorThread(object state)
        {
            var cursorPerModule = new Dictionary<string, int>();
            string[] messages = { "Currently downloading module '{0}'", "The module '{0}' will be available soon", "Hang in there, the module '{0}' is almost here" };
            while (_isLoading)
            {
                string currentModuleName = string.Empty;
                lock (_syncObj)
                {
                    if (_modules.Count > 0)
                    {
                        currentModuleName = _modules.LastOrDefault();
                    }
                }

                if (!string.IsNullOrEmpty(currentModuleName))
                {
                    if (!cursorPerModule.ContainsKey(currentModuleName))
                    {
                        cursorPerModule[currentModuleName] = 0;
                    }

                    if (cursorPerModule[currentModuleName] < messages.Length)
                    {
                        _pleaseWaitService.UpdateStatus(string.Format(CultureInfo.InvariantCulture, messages[cursorPerModule[currentModuleName]++], currentModuleName));
                    }      
                }

                Thread.Sleep(5000);
            }
        }
        #endregion
    }
}