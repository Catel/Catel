// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Data;
    using FluentValidation;
    using Reflection;

    using IValidator = Data.IValidator;

    /// <summary>
    /// The fluent validator provider.
    /// </summary>
    /// <remarks>
    /// This class will automatically retrieve the right fluent validation class associated with the view models. 
    /// </remarks>
    public class FluentValidatorProvider : ValidatorProviderBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidatorProvider"/> class.
        /// </summary>
        public FluentValidatorProvider()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            ValidatorOptions.DisplayNameResolver = (type, member, expression) =>
            {
                Catel.ComponentModel.DisplayNameAttribute displayNameAttribute;
                string displayName = member.Name;
                if (member.TryGetAttribute(out displayNameAttribute))
                {
                    displayName = displayNameAttribute.DisplayName;
                }

                return displayName;
            };
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets a validator for the specified target type.
        /// </summary>
        /// <remarks>
        /// This method only searches for fluent validators on the assembly which the <paramref name="targetType"/> belongs to, 
        /// and creates adapters that allow fluent validator talks with catel validation approach. 
        /// </remarks>
        /// <param name="targetType">
        ///   The target type.
        /// </param>
        /// <returns>
        /// The <see cref="IValidator"/> for the specified type or <c>null</c> if no validator is available for the specified type. 
        /// If only one Validator is found an instance of <see cref="FluentValidatorToCatelValidatorAdapter"/> is returned, otherwise a
        /// <see cref="CompositeValidator"/> is created from a collection of it's corresponding <see cref="FluentValidatorToCatelValidatorAdapter"/>.
        /// </returns>
        protected override IValidator GetValidator(Type targetType)
        {
            IValidator validator = null;

            // NOTE: Patch for performance issue the validator of a viewmodel must be in the same assembly of the view model.

            var assembly = targetType.GetAssemblyEx();
            var exportedTypes = assembly.GetExportedTypesEx();

            var validatorTypes = new List<Type>();
            foreach (var exportedType in exportedTypes)
            {
                if (typeof(FluentValidation.IValidator).IsAssignableFromEx(exportedType))
                {
                    var currentType = exportedType;
                    bool found = false;
                    while (!found && currentType != typeof(object))
                    {
                        if (currentType != null)
                        {
                            var typeInfo = currentType.GetTypeInfo();
                            var genericArguments = currentType.GetGenericArgumentsEx();

                            found = typeInfo.IsGenericType && genericArguments.FirstOrDefault(type => type.IsAssignableFromEx(targetType)) != null;
                            if (!found)
                            {
                                currentType = typeInfo.BaseType;
                            }
                        }
                    }

                    if (found)
                    {
                        validatorTypes.Add(exportedType);
                    }
                }
            }

            if (validatorTypes.Count > 0)
            {
                validator = FluentValidatorToCatelValidatorAdapter.From(validatorTypes);
            }

            return validator;
        }

        #endregion
    }
}