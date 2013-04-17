// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPhoneApplicationPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Phone.Controls
{
    using MVVM;

    /// <summary>
    /// Interface defining functionality for the <see cref="PhoneApplicationPage"/>.
    /// </summary>
    public interface IPhoneApplicationPage : IViewModelContainer
    {
        /// <summary>
        /// Gets or sets a value indicating whether navigating away from the page should save the view model.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if navigating away should save the view model; otherwise, <c>false</c>.
        /// </value>
        bool NavigatingAwaySavesViewModel { get; set; }

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
        bool BackKeyCancelsViewModel { get; set; }
    }
}
