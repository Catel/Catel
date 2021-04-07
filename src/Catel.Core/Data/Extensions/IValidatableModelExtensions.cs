// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidatableModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Validatable model extensions.
    /// </summary>
    public static class IValidatableModelExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the validation context for a complete object graph by also checking the properties and recursive 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The validation context for the whole object graph.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public static IValidationContext GetValidationContextForObjectGraph(this IValidatableModel model)
        {
            Argument.IsNotNull("model", model);

            var validationContext = new ValidationContext();

            validationContext.AddModelValidation(model, new List<IValidatableModel>());

            return validationContext;
        }

        private static void AddModelValidation(this ValidationContext validationContext, IValidatableModel model, List<IValidatableModel> handledModels)
        {
            Argument.IsNotNull("validationContext", validationContext);

            if (handledModels.Any(x => ReferenceEquals(x, model)))
            {
                return;
            }

            handledModels.Add(model);

            validationContext.SynchronizeWithContext(model.ValidationContext, true);

            var propertyDataManager = PropertyDataManager.Default;
            var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(model.GetType());

            foreach (var property in catelTypeInfo.GetCatelProperties())
            {
                var propertyValue = model.GetValue<object>(property.Key);
                var enumerable = propertyValue as IEnumerable;
                if (enumerable is not null && !(propertyValue is string))
                {
                    foreach (var item in enumerable)
                    {
                        var modelItem = item as IValidatableModel;
                        if (modelItem is not null)
                        {
                            validationContext.AddModelValidation(modelItem, handledModels);
                        }
                    }
                }

                var propertyModel = propertyValue as IValidatableModel;
                if (propertyModel is not null)
                {
                    validationContext.AddModelValidation(propertyModel, handledModels);
                }
            }
        }
        #endregion
    }
}
