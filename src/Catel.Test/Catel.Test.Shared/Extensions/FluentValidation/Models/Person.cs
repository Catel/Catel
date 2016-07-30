// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.FluentValidation.Models
{
    /// <summary>
    /// The person.
    /// </summary>
    public class Person
    {
        #region Properties

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        public string LastName { get; set; }
        #endregion
    }
}

#endif