// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneApplicationPageBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls.MVVMProviders
{
    using System.Windows.Interactivity;
    using Logic;
    using MVVM;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// A <see cref="Behavior"/> implementation for a <see cref="PhoneApplicationPage"/>.
    /// </summary>
    public class PhoneApplicationPageBehavior : MVVMBehaviorBase<PhoneApplicationPage, PhoneApplicationPageLogic>
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
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
        /// Gets or sets a value indicating whether the back key cancels the view model. This
        /// means that <see cref="IViewModel.CancelViewModel"/> will be called when the back key is pressed.
        /// <para/>
        /// If this property is <c>false</c>, the <see cref="IViewModel.SaveViewModel"/> will be called instead.
        /// <para/>
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the back key cancels the view model; otherwise, <c>false</c>.
        /// </value>
        public bool BackKeyCancelsViewModel
        {
            get { return Logic.BackKeyCancelsViewModel; }
            set { Logic.BackKeyCancelsViewModel = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the logic required for MVVM.
        /// </summary>
        /// <returns>The <see cref="LogicBase"/> implementation uses by this behavior.</returns>
        protected override PhoneApplicationPageLogic CreateLogic()
        {
            return new PhoneApplicationPageLogic(AssociatedObject, ViewModelType);
        }
        #endregion
    }
}
