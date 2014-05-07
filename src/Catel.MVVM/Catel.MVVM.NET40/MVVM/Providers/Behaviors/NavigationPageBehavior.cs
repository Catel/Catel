// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationPageBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using Logging;
    using Views;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// A behavior implementation for a <see cref="Page"/>. 
    /// </summary>
    public class NavigationPageBehavior : MVVMBehaviorBase<Page, PageLogic>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates the logic required for MVVM.
        /// </summary>
        /// <returns>The <see cref="LogicBase"/> implementation uses by this behavior.</returns>
        protected override PageLogic CreateLogic()
        {
            var associatedObjectType = AssociatedObject.GetType();
            if (!associatedObjectType.ImplementsInterfaceEx<IPage>())
            {
                string error = string.Format("Type '{0}' does not implement IPage, make sure to implement the interface correctly", associatedObjectType);
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            return new PageLogic((IPage)AssociatedObject, ViewModelType);
        }
    }
}
