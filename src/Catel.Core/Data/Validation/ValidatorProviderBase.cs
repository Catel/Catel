namespace Catel.Data
{
    using System;

    using Catel.Caching;

    /// <summary>
    /// Base class that implements the <see cref="IValidatorProvider"/> so only the <see cref="GetValidator"/> method
    /// has to be implemented.
    /// </summary>
    public abstract class ValidatorProviderBase : IValidatorProvider
    {
        /// <summary>
        /// The cache storage.
        /// </summary>
        private readonly CacheStorage<Type, IValidator?> _cacheStorage = new CacheStorage<Type, IValidator?>(storeNullValues: true);

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorProviderBase"/> class. 
        /// </summary>
        protected ValidatorProviderBase()
        {
            UseCache = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether use cache that make faster the retrieval of the <see cref="IValidator"/> 
        /// instance for the specified type <see cref="Type"/>.
        /// </summary>
        /// <remarks>By default this property is set to <c>true</c>.</remarks>
        public bool UseCache { get; set; }

        /// <summary>
        /// Gets a validator for the specified target type.
        /// </summary>
        /// <typeparam name="TTargetType">
        /// The target type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IValidator"/> for the specified type or <c>null</c> if no validator is available for the specified type.
        /// </returns>
        IValidator? IValidatorProvider.GetValidator<TTargetType>()
        {
            return ((IValidatorProvider)this).GetValidator(typeof(TTargetType));
        }

        /// <summary>
        /// Gets a validator for the specified target type.
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <returns>
        /// The <see cref="IValidator"/> for the specified type or <c>null</c> if no validator is available for the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="targetType"/> is <c>null</c>.
        /// </exception>
        IValidator? IValidatorProvider.GetValidator(Type targetType)
        {
            return _cacheStorage.GetFromCacheOrFetch(targetType, () => GetValidator(targetType), !UseCache);
        }

        /// <summary>
        /// Provides an access point to allow a custom implementation in order to retrieve the available validator for the specified type.   
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <returns>
        /// The <see cref="IValidator"/> for the specified type or <c>null</c> if no validator is available for the specified type.
        /// </returns>
        protected abstract IValidator? GetValidator(Type targetType);
    }
}
