namespace Catel.Data
{
    using System;

    /// <summary>
    ///   Provider of <see cref="IValidator" /> classes. This provider can be used to retrieve specific validators for specific types.
    /// </summary>
    public interface IValidatorProvider
    {
        /// <summary>
        ///   Gets a validator for the specified target type.
        /// </summary>
        /// <typeparam name="TTargetType"> The target type. </typeparam>
        /// <returns> The <see cref="IValidator" /> for the specified type or <c>null</c> if no validator is available for the specified type. </returns>
        IValidator GetValidator<TTargetType>();

        /// <summary>
        ///   Gets a validator for the specified target type.
        /// </summary>
        /// <param name="targetType"> The target type. </param>
        /// <returns> The <see cref="IValidator" /> for the specified type or <c>null</c> if no validator is available for the specified type. </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> is <c>null</c>.</exception>
        IValidator GetValidator(Type targetType);
    }
}
