// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleManagerView.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager
{
    using System;
    using IoC;
    using MVVM;
    using ViewModels;

    /// <summary>
    /// Interaction logic for ModuleManagerView.xaml.
    /// </summary>
    public partial class ModuleManagerView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleManagerView"/> class.
        /// </summary>
        public ModuleManagerView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the type of the view model. If this method returns <c>null</c>, the view model type will be retrieved by naming
        /// convention using the <see cref="IViewModelLocator"/> registered in the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks></remarks>
        protected override Type GetViewModelType()
        {
            return typeof(ModuleManagerViewModel);
        }
        #endregion
    }
}