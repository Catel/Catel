// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

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
        [Catel.ComponentModel.DisplayName("First name")]
        public string PersonFirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets PersonLastName.
        /// </summary>
        [Catel.ComponentModel.DisplayName("Last name")]
        public string PersonLastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        #endregion
    }
}

#endif