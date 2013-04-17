// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationResult.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    /// <summary>
    /// Dummy class to allow code sharing between WPF and Silverlight.
    /// </summary>
    public class ValidationResult
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is internal to make this object inaccessible to the outside assemblies.
        /// </remarks>
        internal ValidationResult()
        {   
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the content of the error.
        /// </summary>
        /// <value>The content of the error.</value>
        public object ErrorContent { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the validation result is valid.
        /// </summary>
        /// <value><c>true</c> if the validation result is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; internal set; }
        #endregion
    }
}
