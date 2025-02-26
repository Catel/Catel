﻿namespace Catel.Data
{
    using System;

    using Catel.Caching;
    using Catel.Reflection;

    /// <summary>
    /// Validator provider that provides the validator based on attributes on models.
    /// </summary>
    public class AttributeValidatorProvider : ValidatorProviderBase
    {
        private readonly ICacheStorage<Type, IValidator?> _validatorPerType = new CacheStorage<Type, IValidator?>(storeNullValues: true);
        private readonly IServiceProvider _serviceProvider;

        public AttributeValidatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Provides an access point to allow a custom implementation in order to retrieve the available validator for the specified type.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <returns>The <see cref="IValidator" /> for the specified type or <c>null</c> if no validator is available for the specified type.</returns>
        protected override IValidator? GetValidator(Type targetType)
        {
            return _validatorPerType.GetFromCacheOrFetch(targetType, () =>
            {
                if (targetType.TryGetAttribute<ValidateModelAttribute>(out var attribute))
                {
                    var validator = _serviceProvider.GetService(attribute.ValidatorType) as IValidator;
                    return validator;
                }

                return null;
            });
        }
    }
}
