// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModelWithValidationTags.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    /// Test view model with validation tags.
    /// </summary>
    public class TestViewModelWithValidationTags : ViewModelBase
    {
        public TestViewModelWithValidationTags()
        {
            DeferValidationUntilFirstSaveCall = false;
        }

        #region Methods
        /// <summary>
        /// Validates the field values of this object. Override this method to enable
        /// validation of field values.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                validationResults.Add(new FieldValidationResult(FirstNameProperty, ValidationResultType.Error, "First name is required") { Tag = "PersonValidation" });
            }

            if (string.IsNullOrEmpty(LastName))
            {
                validationResults.Add(new FieldValidationResult(LastNameProperty, ValidationResultType.Error, "Last name is required") { Tag = "PersonValidation" });
            }
        }
        #endregion

        #region Constants
        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("FirstName");

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData MiddleNameProperty = RegisterProperty<string>("MiddleName");

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData LastNameProperty = RegisterProperty<string>("LastName");
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName
        {
            get { return GetValue<string>(MiddleNameProperty); }
            set { SetValue(MiddleNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        #endregion
    }
}
