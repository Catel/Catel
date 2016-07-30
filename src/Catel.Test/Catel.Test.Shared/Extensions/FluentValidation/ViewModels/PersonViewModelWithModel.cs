// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonViewModelWithModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.FluentValidation.ViewModels
{
    using Catel.Data;
    using Catel.MVVM;
    using Catel.Test.Extensions.FluentValidation.Models;

    /// <summary>
    /// The person view model with model.
    /// </summary>
    public class PersonViewModelWithModel : ViewModelBase
    {
        #region Constants

        /// <summary>Register the Person property so it is known in the class.</summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person), default(Person), (s, e) => ((PersonViewModelWithModel)s).OnPersonChanged(e));
        #endregion

        #region Constructors
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Person.
        /// </summary>
        [Model]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Occurs when the value of the Person property is changed.
        /// </summary>
        /// <param name="e">
        /// The event argument
        /// </param>
        private void OnPersonChanged(AdvancedPropertyChangedEventArgs e)
        {
        }

        #endregion
    }
}

#endif