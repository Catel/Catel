// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    /// <summary>
    /// Display name attribute.
    /// </summary>
    /// <remarks>
    /// Use in order to display a final user readable string associated to the property in the validation message.
    /// </remarks>
    /// <example>
    /// <code>
    ///  <![CDATA[
    /// public class PersonViewModel : ViewModelBase
    /// {
    ///    [DisplayName("First name")]
    ///    property PersonFirstName { get; set;}
    /// }
    /// ]]>
    ///  </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayNameAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <exception cref="ArgumentException">If <paramref name="displayName"/> is <c>null</c> or a whitespace.</exception>
        public DisplayNameAttribute(string displayName)
        {
            Argument.IsNotNullOrWhitespace("displayName", displayName);

            DisplayName = displayName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; private set; }

        #endregion
    }
}