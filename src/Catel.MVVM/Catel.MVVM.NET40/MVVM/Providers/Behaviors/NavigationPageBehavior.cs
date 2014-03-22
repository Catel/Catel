// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationPageBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using System.Windows.Controls;
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Reflection;

    /// <summary>
    /// A <see cref="System.Windows.Interactivity.Behavior"/> implementation for a <see cref="Page"/>. 
    /// </summary>
    public class NavigationPageBehavior : MVVMBehaviorBase<Page, NavigationPageLogic>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets a value indicating whether navigating away from the page should save the view model.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if navigating away should save the view model; otherwise, <c>false</c>.
        /// </value>
        public bool NavigatingAwaySavesViewModel
        {
            get { return Logic.NavigatingAwaySavesViewModel; }
            set { Logic.NavigatingAwaySavesViewModel = value; }
        }

        /// <summary>
        /// Creates the logic required for MVVM.
        /// </summary>
        /// <returns>The <see cref="LogicBase"/> implementation uses by this behavior.</returns>
        protected override NavigationPageLogic CreateLogic()
        {
            var associatedObjectType = AssociatedObject.GetType();
            if (!associatedObjectType.ImplementsInterfaceEx<IPage>())
            {
                string error = string.Format("Type '{0}' does not implement IPage, make sure to implement the interface correctly", associatedObjectType);
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            return new NavigationPageLogic((IPage)AssociatedObject, ViewModelType);
        }
    }
}
