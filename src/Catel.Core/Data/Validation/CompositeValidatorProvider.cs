namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Allows the combination of several validator providers into a single validator provider. 
    /// </summary>
    /// <remarks>
    /// This class can be used to unify instances of <see cref="IValidatorProvider"/> into a single one and provides several 
    /// sources in order to retrieve <see cref="IValidator"/> instances. 
    /// </remarks>
    public class CompositeValidatorProvider : ValidatorProviderBase
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// The validator providers.
        /// </summary>
        private readonly HashSet<IValidatorProvider> _validatorProviders = new HashSet<IValidatorProvider>();

        /// <summary>
        /// Add the validator provider to this composite validator provider.
        /// </summary>
        /// <param name="validatorProvider">The validator provider.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="validatorProvider" /> is <c>null</c>.</exception>
        public void Add(IValidatorProvider validatorProvider)
        {
            ArgumentNullException.ThrowIfNull(validatorProvider);

            lock (_syncObj)
            {
                _validatorProviders.Add(validatorProvider);
            }
        }

        /// <summary>
        /// Determines whether this composite validator provider contains the specified validator provider.
        /// </summary>
        /// <param name="validatorProvider">The validator provider.</param>
        /// <returns><c>true</c> if this composite validator provider contains the specified validator; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validatorProvider" /> is <c>null</c>.</exception>
        public bool Contains(IValidatorProvider validatorProvider)
        {
            ArgumentNullException.ThrowIfNull(validatorProvider);

            lock (_syncObj)
            {
                return _validatorProviders.Contains(validatorProvider);
            }
        }

        /// <summary>
        /// Gets a validator for the specified target type.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <returns>The <see cref="IValidator" /> for the specified type or <c>null</c> if no validator is available for the specified type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> is <c>null</c>.</exception>
        /// <remarks>If there are more than once validator provider and they retrieve more than once validator all of these will be aggregated into a single <see cref="CompositeValidator" />.</remarks>
        protected override IValidator? GetValidator(Type targetType)
        {
            ArgumentNullException.ThrowIfNull(targetType);

            IValidator? validator; 

            lock (_syncObj)
            {
                IList<IValidator> discoveredValidators = (from x in _validatorProviders
                                                          let y = x.GetValidator(targetType)
                                                          where y is not null
                                                          select y).ToList();
                if (discoveredValidators.Count > 1)
                {
                    var composite = new CompositeValidator();
                    foreach (var discoveredValidator in discoveredValidators)
                    {
                        composite.Add(discoveredValidator);
                    }

                    validator = composite;
                }
                else
                {
                    validator = discoveredValidators.FirstOrDefault();
                }
            }

            return validator;
        }

        /// <summary>
        /// Removes the validator provider from this composite validator provider.
        /// </summary>
        /// <param name="validatorProvider">The validator provider.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="validatorProvider" /> is <c>null</c>.</exception>
        public void Remove(IValidatorProvider validatorProvider)
        {
            ArgumentNullException.ThrowIfNull(validatorProvider);

            lock (_syncObj)
            {
                _validatorProviders.Remove(validatorProvider);
            }
        }
    }
}
