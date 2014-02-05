// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWindow.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Controls;
    using MVVM;

    /// <summary>
    /// <see cref="Window"/> class that implements the <see cref="InfoBarMessageControl"/> and
    /// the default buttons, according to the <see cref="DataWindowMode"/>. Also supports MVVM out
    /// of the box by using the <see cref="ViewModelBase"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [ObsoleteEx(Replacement = "Catel.Windows.DataWindow", TreatAsErrorFromVersion = "3.3", RemoveInVersion = "4.0")]
    public class DataWindow<TViewModel> : DataWindow
        where TViewModel : class, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindow"/> class.
        /// </summary>
        /// <param name="mode"><see cref="DataWindowMode"/>.</param>
        /// <param name="additionalButtons">The additional buttons.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <param name="setOwnerAndFocus">if set to <c>true</c>, set the main window as owner window and focus the window.</param>
        /// <param name="infoBarMessageControlGenerationMode">The info bar message control generation mode.</param>
        public DataWindow(DataWindowMode mode = DataWindowMode.OkCancel, IEnumerable<DataWindowButton> additionalButtons = null, 
            DataWindowDefaultButton defaultButton = DataWindowDefaultButton.OK, bool setOwnerAndFocus = true, 
            InfoBarMessageControlGenerationMode infoBarMessageControlGenerationMode = InfoBarMessageControlGenerationMode.Inline)
            : this(null, mode, additionalButtons, defaultButton, setOwnerAndFocus, infoBarMessageControlGenerationMode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindow&lt;TViewModel&gt;"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <remarks>
        /// Explicit constructor with view model injection, required for <see cref="Activator.CreateInstance(System.Type)"/> which
        /// does not seem to support default parameter values.
        /// </remarks>
        public DataWindow(IViewModel viewModel)
            : this(viewModel, DataWindowMode.OkCancel)
        {
            // Do not remove this constructor, see remarks
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="mode"><see cref="DataWindowMode"/>.</param>
        /// <param name="additionalButtons">The additional buttons.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <param name="setOwnerAndFocus">if set to <c>true</c>, set the main window as owner window and focus the window.</param>
        /// <param name="infoBarMessageControlGenerationMode">The info bar message control generation mode.</param>
        public DataWindow(IViewModel viewModel, DataWindowMode mode, IEnumerable<DataWindowButton> additionalButtons = null,
            DataWindowDefaultButton defaultButton = DataWindowDefaultButton.OK, bool setOwnerAndFocus = true, 
            InfoBarMessageControlGenerationMode infoBarMessageControlGenerationMode = InfoBarMessageControlGenerationMode.Inline)
            : base(viewModel, mode, additionalButtons, defaultButton, setOwnerAndFocus, infoBarMessageControlGenerationMode)
        {
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public new TViewModel ViewModel
        {
            get { return base.ViewModel as TViewModel; }
        }

        /// <summary>
        /// Gets the type of the view model. If this method returns <c>null</c>, the view model type will be retrieved by naming convention.
        /// </summary>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks></remarks>
        protected override Type GetViewModelType()
        {
            return typeof(TViewModel);
        }
    }
}