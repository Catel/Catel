// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeValidatorProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;

    using Catel.Caching;
    using Catel.IoC;
    using Catel.Reflection;

    /// <summary>
    /// Validator provider that provides the validator based on attributes on models.
    /// </summary>
    public class AttributeValidatorProvider : ValidatorProviderBase
    {
        private readonly ICacheStorage<Type, IValidator> _validatorPerType = new CacheStorage<Type, IValidator>(storeNullValues: true); 

        #region Methods
        /// <summary>
        /// Provides an access point to allow a custom implementation in order to retrieve the available validator for the specified type.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <returns>The <see cref="IValidator" /> for the specified type or <c>null</c> if no validator is available for the specified type.</returns>
        protected override IValidator GetValidator(Type targetType)
        {
            return _validatorPerType.GetFromCacheOrFetch(targetType, () =>
            {
                ValidateModelAttribute attribute;
                if (AttributeHelper.TryGetAttribute(targetType, out attribute))
                {
                    var validator = TypeFactory.Default.CreateInstance(attribute.ValidatorType) as IValidator;
                    return validator;
                }

                return null;
            });
        }
        #endregion
    }
}