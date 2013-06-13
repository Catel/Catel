// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleTemplate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager.Models
{
    using System;
    using Data;
    using IoC;
    using Logging;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// The module template.
    /// </summary>
    public class ModuleTemplate : ModelBase
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>Register the ModuleName property so it is known in the class.</summary>
        public static readonly PropertyData ModuleNameProperty = RegisterProperty("ModuleName", typeof (string));

        /// <summary>Register the State property so it is known in the class.</summary>
        public static readonly PropertyData StateProperty = RegisterProperty("State", typeof (string));

        /// <summary>Register the Enabled property so it is known in the class.</summary>
        public static readonly PropertyData EnabledProperty = RegisterProperty("Enabled", typeof (bool), null, (sender, args) =>
            {
                var value = (bool) args.NewValue;

                var instance = (ModuleTemplate) sender;

                var moduleManager = ServiceLocator.Default.ResolveType<IModuleManager>();

                if (moduleManager == null)
                {
                    return;
                }

                if (!value)
                {
                    return;
                }

                try
                {
                    var moduleName = (string) instance.GetValue(ModuleNameProperty);
                    moduleManager.LoadModule(moduleName);
                    instance.SetValue(StateProperty, "Active");
                    instance.SetValue(TimeProperty, DateTime.Now.ToString("HH:mm:ss"));
                    Log.Info(string.Format("Module '{0}' loaded successfully.", moduleName));
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            });

        /// <summary>Register the Time property so it is known in the class.</summary>
        public static readonly PropertyData TimeProperty = RegisterProperty("Time", typeof (string));
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
            get { return GetValue<string>(ModuleNameProperty); }
            set { SetValue(ModuleNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State
        {
            get { return GetValue<string>(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ModuleTemplate"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return GetValue<bool>(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public string Time
        {
            get { return GetValue<string>(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }
        #endregion
    }
}