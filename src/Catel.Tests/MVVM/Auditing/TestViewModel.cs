// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.MVVM.Auditing
{
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    /// Test view model.
    /// </summary>
    public class TestViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// Register the TestProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TestPropertyProperty = RegisterProperty("TestProperty", typeof (string), "defaultvalue");
        #endregion

        #region Commands

        #region Properties
        /// <summary>
        /// Gets the TestCommand command.
        /// </summary>
        public Command<string> TestCommand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method to invoke when the TestCommand command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private void OnTestCommandExecute(string parameter)
        {
            // Empty by purpose   
        }
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel"/> class.
        /// </summary>
        public TestViewModel()
        {
            TestCommand = new Command<string>(OnTestCommandExecute);
            TestCommand.AutomaticallyDispatchEvents = false;

#pragma warning disable 4014
            InitializeViewModelAsync();
#pragma warning restore 4014
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "View model title"; }
        }

        /// <summary>
        /// Gets or sets the test property.
        /// </summary>
        public string TestProperty
        {
            get { return GetValue<string>(TestPropertyProperty); }
            set { SetValue(TestPropertyProperty, value); }
        }
        #endregion
    }
}