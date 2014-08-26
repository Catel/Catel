// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Text;

    /// <summary>
    /// Extension methods for the <see cref="ModelBase"/> class.
    /// </summary>
    public static class ModelBaseExtensions
    {
        /// <summary>
        /// Returns a message that contains all the current warnings.
        /// </summary>
        /// <param name="model">The model base.</param>
        /// <param name="userFriendlyObjectName">Name of the user friendly object.</param>
        /// <returns>
        /// Warning string or empty in case of no warnings.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public static string GetWarningMessage(this ModelBase model, string userFriendlyObjectName = null)
        {
            Argument.IsNotNull("model", model);

            var validationContext = ((IModelValidation) model).ValidationContext;

            if (!validationContext.HasWarnings)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(userFriendlyObjectName))
            {
                // Use the real entity name (stupid developer that passes a useless value)
                userFriendlyObjectName = model.GetType().Name;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(ResourceHelper.GetString("WarningsFound"), userFriendlyObjectName);
            messageBuilder.Append(GetListMessages(validationContext, ValidationResultType.Warning));

            return messageBuilder.ToString();
        }

        /// <summary>
        /// Returns a message that contains all the current errors.
        /// </summary>
        /// <param name="model">The model base.</param>
        /// <param name="userFriendlyObjectName">Name of the user friendly object.</param>
        /// <returns>
        /// Error string or empty in case of no errors.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public static string GetErrorMessage(this ModelBase model, string userFriendlyObjectName = null)
        {
            Argument.IsNotNull("model", model);

            var validationContext = ((IModelValidation)model).ValidationContext;

            if (!validationContext.HasErrors)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(userFriendlyObjectName))
            {
                // Use the real entity name (stupid developer that passes a useless value)
                userFriendlyObjectName = model.GetType().Name;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(ResourceHelper.GetString("ErrorsFound"), userFriendlyObjectName);
            messageBuilder.Append(GetListMessages(validationContext, ValidationResultType.Error));

            return messageBuilder.ToString();
        }

        /// <summary>
        /// Gets the list messages.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <param name="validationResult">The validation result.</param>
        /// <returns>
        /// String representing the output of all items in the fields an business object.
        /// </returns>
        /// <remarks>
        /// This method is used to create a message string for field warnings or errors and business warnings
        /// or errors. Just pass the right dictionary and list to this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        private static string GetListMessages(IValidationContext validationContext, ValidationResultType validationResult)
        {
            Argument.IsNotNull("validationContext", validationContext);

            var messageBuilder = new StringBuilder();

            switch (validationResult)
            {
                case ValidationResultType.Warning:
                    foreach (var field in validationContext.GetFieldWarnings())
                    {
                        messageBuilder.AppendLine("* {0}", field.Message);
                    }

                    foreach (var businessItem in validationContext.GetBusinessRuleWarnings())
                    {
                        messageBuilder.AppendLine("* {0}", businessItem.Message);
                    }
                    break;

                case ValidationResultType.Error:
                    foreach (var field in validationContext.GetFieldErrors())
                    {
                        messageBuilder.AppendLine("* {0}", field.Message);
                    }

                    foreach (var businessItem in validationContext.GetBusinessRuleErrors())
                    {
                        messageBuilder.AppendLine("* {0}", businessItem.Message);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("validationResult");
            }

            return messageBuilder.ToString();
        }

        /// <summary>
        /// Gets the validation context for a complete object graph by also checking the properties and recursive 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The validation context for the whole object graph.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public static IValidationContext GetValidationContextForObjectGraph(this IModel model)
        {
            Argument.IsNotNull("model", model);

            var validationContext = new ValidationContext();

            validationContext.AddModelValidation(model, new List<IModel>());

            return validationContext;
        }

        private static void AddModelValidation(this ValidationContext validationContext, IModel model, List<IModel> handledModels)
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
                var propertyValue = model.GetValue(property.Key);
                var enumerable = propertyValue as IEnumerable;
                if (enumerable != null && !(propertyValue is string))
                {
                    foreach (var item in enumerable)
                    {
                        var modelItem = item as IModel;
                        if (modelItem != null)
                        {
                            validationContext.AddModelValidation(modelItem, handledModels);
                        }
                    }
                }

                var propertyModel = propertyValue as IModel;
                if (propertyModel != null)
                {
                    validationContext.AddModelValidation(propertyModel, handledModels);
                }
            }
        }
    }
}