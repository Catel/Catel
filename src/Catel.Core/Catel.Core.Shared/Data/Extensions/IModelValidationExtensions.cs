// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelValidationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    /// <summary>
    /// Extensions for model validation.
    /// </summary>
    public static class IModelValidationExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the validation context of the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The validation context.</returns>
        public static IValidationContext GetValidationContext(this IModelValidation model)
        {
            Argument.IsNotNull("model", model);

            return model.ValidationContext;
        }
        #endregion
    }
}