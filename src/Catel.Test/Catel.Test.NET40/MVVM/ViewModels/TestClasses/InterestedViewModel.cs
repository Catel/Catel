// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestedViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    /// Interested view model.
    /// </summary>
    [InterestedIn(typeof (InterestingViewModel))]
    public class InterestedViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// Register the InterestedValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData InterestedValueProperty = RegisterProperty("InterestedValue", typeof (string));

        /// <summary>
        /// Register the CommandHasBeenExecuted property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CommandHasBeenExecutedProperty = RegisterProperty("CommandHasBeenExecuted", typeof (bool));

        /// <summary>
        /// Register the CommandHasBeenExecutedWithParameter property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CommandHasBeenExecutedWithParameterProperty = RegisterProperty("CommandHasBeenExecutedWithParameter", typeof (bool));

        /// <summary>
        /// Register the RegisteredCommandHasBeenExecuted property so it is known in the class.
        /// </summary>
        public static readonly PropertyData RegisteredCommandHasBeenExecutedProperty = RegisterProperty("RegisteredCommandHasBeenExecuted", typeof (bool));

        /// <summary>
        /// Register the RegisteredCommandHasBeenExecutedWithParameter property so it is known in the class.
        /// </summary>
        public static readonly PropertyData RegisteredCommandHasBeenExecutedWithParameterProperty = RegisterProperty("RegisteredCommandHasBeenExecutedWithParameter", typeof (bool));

        /// <summary>
        /// Register the NotRegisteredCommandHasBeenExecuted property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NotRegisteredCommandHasBeenExecutedProperty = RegisterProperty("NotRegisteredCommandHasBeenExecuted", typeof (bool));

        /// <summary>
        /// Register the NotRegisteredCommandHasBeenExecutedWithParameter property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NotRegisteredCommandHasBeenExecutedWithParameterProperty = RegisterProperty("NotRegisteredCommandHasBeenExecutedWithParameter", typeof (bool));

        /// <summary>
        /// Register the ViewModelEvents property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ViewModelEventsProperty = RegisterProperty("ViewModelEvents", typeof (List<ViewModelEvent>));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InterestedViewModel"/> class.
        /// </summary>
        public InterestedViewModel()
            : base()
        {
            ViewModelEvents = new List<ViewModelEvent>();
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
        /// Gets or sets the interested value.
        /// </summary>
        public string InterestedValue
        {
            get { return GetValue<string>(InterestedValueProperty); }
            set { SetValue(InterestedValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets a boolean that shows whether a command has been executed on the other view-model.
        /// </summary>
        public bool CommandHasBeenExecuted
        {
            get { return GetValue<bool>(CommandHasBeenExecutedProperty); }
            set { SetValue(CommandHasBeenExecutedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a boolean that shows whether the command has been executed with the right command parameter.
        /// </summary>
        public bool CommandHasBeenExecutedWithParameter
        {
            get { return GetValue<bool>(CommandHasBeenExecutedWithParameterProperty); }
            set { SetValue(CommandHasBeenExecutedWithParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the a boolean that shows whether the registered command has been executed on the other view-model.
        /// </summary>
        public bool RegisteredCommandHasBeenExecuted
        {
            get { return GetValue<bool>(RegisteredCommandHasBeenExecutedProperty); }
            set { SetValue(RegisteredCommandHasBeenExecutedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the a boolean that shows whether the registered command has been executed on the other view-model with the right command parameter.
        /// </summary>
        public bool RegisteredCommandHasBeenExecutedWithParameter
        {
            get { return GetValue<bool>(RegisteredCommandHasBeenExecutedWithParameterProperty); }
            set { SetValue(RegisteredCommandHasBeenExecutedWithParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the a boolean that shows whether the NotRegistered command has been executed on the other view-model.
        /// </summary>
        public bool NotRegisteredCommandHasBeenExecuted
        {
            get { return GetValue<bool>(NotRegisteredCommandHasBeenExecutedProperty); }
            set { SetValue(NotRegisteredCommandHasBeenExecutedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the a boolean that shows whether the NotRegistered command has been executed on the other view-model with the right command parameter.
        /// </summary>
        public bool NotRegisteredCommandHasBeenExecutedWithParameter
        {
            get { return GetValue<bool>(NotRegisteredCommandHasBeenExecutedWithParameterProperty); }
            set { SetValue(NotRegisteredCommandHasBeenExecutedWithParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the list of received view model events.
        /// </summary>
        public List<ViewModelEvent> ViewModelEvents
        {
            get { return GetValue<List<ViewModelEvent>>(ViewModelEventsProperty); }
            set { SetValue(ViewModelEventsProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when a property has changed for a view model type that the current view model is interested in. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnViewModelPropertyChanged(IViewModel viewModel, string propertyName)
        {
            if (viewModel is InterestingViewModel)
            {
                InterestedValue = ((InterestingViewModel) viewModel).InterestingValue;
            }
        }

        /// <summary>
        /// Called when a command for a view model type that the current view model is interested in has been executed. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter used during the execution.</param>
        protected override void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
            if (viewModel is InterestingViewModel)
            {
                string tag = (command.Tag is string) ? (string) command.Tag : string.Empty;

                if (tag == "test")
                {
                    CommandHasBeenExecuted = true;

                    if ((commandParameter is string) && (((string) commandParameter) == "parameter"))
                    {
                        CommandHasBeenExecutedWithParameter = true;
                    }
                }

                if (tag == "unregistered")
                {
                    NotRegisteredCommandHasBeenExecuted = true;

                    if ((commandParameter is string) && (((string) commandParameter) == "parameter"))
                    {
                        NotRegisteredCommandHasBeenExecutedWithParameter = true;
                    }
                }

                if (tag == "registered")
                {
                    RegisteredCommandHasBeenExecuted = true;

                    if ((commandParameter is string) && (((string) commandParameter) == "parameter"))
                    {
                        RegisteredCommandHasBeenExecutedWithParameter = true;
                    }
                }
            }
        }

        protected override void OnViewModelEvent(IViewModel viewModel, ViewModelEvent viewModelEvent, System.EventArgs e)
        {
            if (viewModel is InterestingViewModel)
            {
                ViewModelEvents.Add(viewModelEvent);
            }
        }
        #endregion
    }
}