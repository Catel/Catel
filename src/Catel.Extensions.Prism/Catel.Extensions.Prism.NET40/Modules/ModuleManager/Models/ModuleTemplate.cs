// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleTemplate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager.Models
{
    using Catel.Data;

    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// The module template.
    /// </summary>
    public class ModuleTemplate : ObservableObject
    {
        #region Fields

        /// <summary>
        /// The module info.
        /// </summary>
        private readonly ModuleInfo _moduleInfo;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        public ModuleTemplate(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);

            _moduleInfo = moduleInfo;

            RaisePropertyChanged(() => ModuleName);
            RaisePropertyChanged(() => Enabled);
            RaisePropertyChanged(() => State);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>
        /// The name of the module.
        /// </value>
        public string ModuleName
        {
            get { return _moduleInfo.ModuleName; }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public ModuleState State
        {
            get { return _moduleInfo.State; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ModuleTemplate"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return _moduleInfo.State != ModuleState.NotStarted; }
        }

        #endregion
    }
}