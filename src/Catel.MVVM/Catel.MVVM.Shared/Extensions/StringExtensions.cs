// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the a unique name for a control. This is sometimes required in some frameworks.
        /// <para />
        /// The name is made unique by appending a unique guid.
        /// </summary>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>System.String.</returns>
        public static string GetUniqueControlName(this string controlName)
        {
            var random = Guid.NewGuid().ToString();
            random = random.Replace("-", string.Empty);

            var name = string.Format("{0}_{1}", controlName, random);
            return name;
        }
    }
}