using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catel.IoC.Exceptions
{
    /// <summary>
    /// Exception class in case an requested type from <see cref="IServiceLocator"/> is not registred.
    /// </summary>
    class MissingRegistredTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingRegistredTypeException"/> class.
        /// </summary>
        /// <param name="requestedType">The requested type.</param>
        public MissingRegistredTypeException(Type requestedType)
            : base("The specified type is not registered. Please register type before using it.")
        {
            RequestedType = requestedType;
        }

        /// <summary>
        /// Gets the requested type.
        /// </summary>
        /// <value>The type.</value>
        public Type RequestedType { get; private set; }
    }
}
