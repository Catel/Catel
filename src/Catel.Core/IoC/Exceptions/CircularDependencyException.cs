namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Exception which can be used to easily detect circular dependencies inside the <see cref="ServiceLocator"/>.
    /// <para />
    /// This exception is used instead of letting the .NET framework throw a <c>StackOverflowException</c> which
    /// is much harder to debug.
    /// </summary>
    public class CircularDependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException" /> class.
        /// </summary>
        /// <param name="duplicateRequestInfo">Type request that occurred second time.</param>
        /// <param name="typePath">The type path.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typePath"/> is <c>null</c>.</exception>
        internal CircularDependencyException(TypeRequestInfo duplicateRequestInfo, ITypeRequestPath typePath, string message)
            : base(message)
        {
            DuplicateRequestInfo = duplicateRequestInfo;
            TypePath = typePath;
        }

        /// <summary>
        /// Duplicated type request
        /// </summary>
        public TypeRequestInfo DuplicateRequestInfo { get; private set; }
        
        /// <summary>
        /// Gets the type path.
        /// </summary>
        /// <value>The type path.</value>
        public ITypeRequestPath TypePath { get; private set; }
    }
}
