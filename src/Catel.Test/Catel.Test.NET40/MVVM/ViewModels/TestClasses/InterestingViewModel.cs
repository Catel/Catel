// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestingViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    /// Interesting view model.
    /// </summary>
    public class InterestingViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// Register the InterestingValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData InterestingValueProperty = RegisterProperty("InterestingValue", typeof (string));
        #endregion

        #region Commands

        #region Properties
        /// <summary>
        /// Gets the TestCommand command.
        /// </summary>
        public Command<object> TestCommand { get; private set; }

        /// <summary>
        /// Gets the RegisteredCommand command.
        /// </summary>
        public Command<object> RegisteredCommand { get; private set; }

        /// <summary>
        /// Gets the NotRegisteredCommand command.
        /// </summary>
        public Command<object> NotRegisteredCommand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method to invoke when the TestCommand command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private void OnTestCommandExecute(object parameter)
        {
            // Just do something here, not really required for unit testing
        }

        /// <summary>
        /// Method to invoke when the RegisteredCommand command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private void OnRegisteredCommandExecute(object parameter)
        {
            // Just do something here, not really required for unit testing
        }

        /// <summary>
        /// Method to invoke when the NotRegisteredCommand command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private void OnUnregisteredCommandExecute(object parameter)
        {
            // Just do something here, not really required for unit testing
        }
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InterestingViewModel"/> class.
        /// </summary>
        public InterestingViewModel()
            : base()
        {
            TestCommand = new Command<object>(OnTestCommandExecute, null, "test");
            TestCommand.AutomaticallyDispatchEvents = false;

            RegisteredCommand = new Command<object>(OnRegisteredCommandExecute, null, "registered");
            RegisteredCommand.AutomaticallyDispatchEvents = false;

            NotRegisteredCommand = new Command<object>(OnUnregisteredCommandExecute, null, "unregistered");
            NotRegisteredCommand.AutomaticallyDispatchEvents = false;

            InitializeViewModel();
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
        /// Gets or sets the interesting value.
        /// </summary>
        public string InterestingValue
        {
            get { return GetValue<string>(InterestingValueProperty); }
            set { SetValue(InterestingValueProperty, value); }
        }
        #endregion
    }
}