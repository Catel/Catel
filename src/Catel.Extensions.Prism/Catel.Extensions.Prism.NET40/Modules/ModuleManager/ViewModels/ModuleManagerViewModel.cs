// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleManagerViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Data;
    using MVVM;
    using Microsoft.Practices.Prism.Modularity;
    using Models;

    /// <summary>
    /// Module manager view model.
    /// </summary>
    public class ModuleManagerViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>Register the ApplicationModules property so it is known in the class.</summary>
        public static readonly PropertyData ApplicationModulesProperty = RegisterProperty("ApplicationModules", typeof (ObservableCollection<ModuleTemplate>));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManagerViewModel"/> class.
        /// </summary>
        public ModuleManagerViewModel()
        {
            var knownModules = GetService<IModuleInfoManager>().KnownModules;
            var tempModules = knownModules.Select(moduleInfo => new ModuleTemplate
                {
                    ModuleName = moduleInfo.ModuleName,
                    Enabled = moduleInfo.InitializationMode == InitializationMode.WhenAvailable,
                    State = moduleInfo.InitializationMode == InitializationMode.WhenAvailable ? "Active" : "OnDemand"
                }).ToList();

            var sorted = tempModules.OrderBy(module => module.ModuleName);
            ApplicationModules = new ObservableCollection<ModuleTemplate>(sorted);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the application modules.
        /// </summary>
        /// <value>
        /// The application modules.
        /// </value>
        public ObservableCollection<ModuleTemplate> ApplicationModules
        {
            get { return GetValue<ObservableCollection<ModuleTemplate>>(ApplicationModulesProperty); }
            set { SetValue(ApplicationModulesProperty, value); }
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
    }
}