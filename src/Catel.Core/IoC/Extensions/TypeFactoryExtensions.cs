namespace Catel.IoC
{
    using System;
    using Catel.Reflection;

    /// <summary>
    /// Extension methods for the <see cref="ITypeFactory"/>.
    /// </summary>
    public static class TypeFactoryExtensions
    {

        public static T CreateRequiredInstance<T>(this ITypeFactory typeFactory)
        {
            return (T)CreateRequiredInstance(typeFactory, typeof(T));
        }

        public static object CreateRequiredInstance(this ITypeFactory typeFactory, Type typeToConstruct)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(typeToConstruct);

            var model = typeFactory.CreateInstance(typeToConstruct);
            if (model is null)
            {
                throw CreateFailedToCreateRequiredTypeException(typeToConstruct);
            }

            return model;
        }

        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public static T? CreateInstance<T>(this ITypeFactory typeFactory)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            return (T?)typeFactory.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Creates an instance of the specified type using dependency injection.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public static T? CreateInstanceWithTag<T>(this ITypeFactory typeFactory, object tag)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            return (T?)typeFactory.CreateInstanceWithTag(typeof(T), tag);
        }

        public static T CreateRequiredInstanceWithParameters<T>(this ITypeFactory typeFactory, Type typeToConstruct, params object?[] parameters)
        {
            return (T)CreateRequiredInstanceWithParameters(typeFactory, typeToConstruct, parameters);
        }

        public static object CreateRequiredInstanceWithParameters(this ITypeFactory typeFactory, Type typeToConstruct, params object?[] parameters)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(typeToConstruct);

            var model = typeFactory.CreateInstanceWithParameters(typeToConstruct, parameters);
            if (model is null)
            {
                throw CreateFailedToCreateRequiredTypeException(typeToConstruct);
            }

            return model;
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory"/> is <c>null</c>.</exception>
        public static T? CreateInstanceWithParameters<T>(this ITypeFactory typeFactory, params object[] parameters)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            return (T?)typeFactory.CreateInstanceWithParameters(typeof(T), parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public static T? CreateInstanceWithParametersWithTag<T>(this ITypeFactory typeFactory, object tag, params object[] parameters)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            return (T?)typeFactory.CreateInstanceWithParametersWithTag(typeof(T), tag, parameters);
        }

        public static T CreateRequiredInstanceWithParametersAndAutoCompletion<T>(this ITypeFactory typeFactory, params object?[] parameters)
        {
            return (T)CreateRequiredInstanceWithParametersAndAutoCompletion(typeFactory, typeof(T), parameters);
        }

        public static object CreateRequiredInstanceWithParametersAndAutoCompletion(this ITypeFactory typeFactory, Type typeToConstruct, params object?[] parameters)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);
            ArgumentNullException.ThrowIfNull(typeToConstruct);

            var model = typeFactory.CreateInstanceWithParametersAndAutoCompletion(typeToConstruct, parameters);
            if (model is null)
            {
                throw CreateFailedToCreateRequiredTypeException(typeToConstruct);
            }

            return model;
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory"/> is <c>null</c>.</exception>
        public static T? CreateInstanceWithParametersAndAutoCompletion<T>(this ITypeFactory typeFactory, params object[] parameters)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            return (T?)typeFactory.CreateInstanceWithParametersAndAutoCompletion(typeof(T), parameters);
        }

        /// <summary>
        /// Creates an instance of the specified type using the specified parameters as injection values.
        /// <para />
        /// This method will also auto-complete any additional dependencies that can be resolved from the <see cref="IServiceLocator" />.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="tag">The preferred tag when resolving dependencies.</param>
        /// <param name="parameters">The parameters to inject.</param>
        /// <returns>The instantiated type using dependency injection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public static T? CreateInstanceWithParametersAndAutoCompletionWithTag<T>(this ITypeFactory typeFactory, object tag, params object[] parameters)
        {
            ArgumentNullException.ThrowIfNull(typeFactory);

            return (T?)typeFactory.CreateInstanceWithParametersAndAutoCompletionWithTag(typeof(T), tag, parameters);
        }

        private static Exception CreateFailedToCreateRequiredTypeException(Type type)
        {
            return new CatelException($"Cannot create instance of type '{type.GetSafeFullName()}'");
        }
    }
}
