// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWithInvalidMappings.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using Catel.MVVM;

    /// <summary>
    /// View model with invalid mappings.
    /// </summary>
    public class ViewModelWithInvalidMappings : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "View model with invalid mappings"; }
        }
        #endregion
    }
}