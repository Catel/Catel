namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Type factory which will cache constructors to ensure the best performance available.
    /// </summary>
    public interface ITypeFactory : IDisposable
    {
        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        object CreateInstance(Type typeToConstruct);

        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        object CreateInstanceWithTag(Type typeToConstruct, object? tag);

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        object CreateInstanceWithParameters(Type typeToConstruct, params object[] parameters);

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        object CreateInstanceWithParametersWithTag(Type typeToConstruct, object tag, params object[] parameters);

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct"/> is <c>null</c>.</exception>
        object CreateInstanceWithParametersAndAutoCompletion(Type typeToConstruct, params object[] parameters);

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator" />.
        /// </summary>
        /// <param name="typeToConstruct">The type to construct.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToConstruct" /> is <c>null</c>.</exception>
        object CreateInstanceWithParametersAndAutoCompletionWithTag(Type typeToConstruct, object tag, params object[] parameters);

        /// <summary>
        /// Clears the cache of all constructors.
        /// <para />
        /// This call is normally not necessary since the type factory should keep an eye on the 
        /// <see cref="IServiceLocator.TypeRegistered"/> event to invalidate the cache.
        /// </summary>
        void ClearCache();
    }
}
