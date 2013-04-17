// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.FluentValidation.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    /// The person view model.
    /// </summary>
    public class PersonViewModel : ViewModelBase
    {
        #region Constants and Fields

        /// <summary>
        /// The first name property.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("PersonFirstName", typeof(string));

        /// <summary>
        /// The last name property.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("PersonLastName", typeof(string));

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets PersonFirstName.
        /// </summary>
        [DisplayName("First name")]
        public string PersonFirstName
        {
            get
            {
                return this.GetValue<string>(FirstNameProperty);
            }

            set
            {
                this.SetValue(FirstNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets PersonLastName.
        /// </summary>
        [DisplayName("Last name")]
        public string PersonLastName
        {
            get
            {
                return this.GetValue<string>(LastNameProperty);
            }

            set
            {
                this.SetValue(LastNameProperty, value);
            }
        }

        #endregion
    }
}