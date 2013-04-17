// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalContainerNotSupportedException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Exception class in case an external container is not supported by the <see cref="IServiceLocator"/> implementation.
    /// </summary>
    public class ExternalContainerNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalContainerNotSupportedException"/> class.
        /// </summary>
        /// <param name="supportedContainers">The supported containers.</param>
        public ExternalContainerNotSupportedException(string[] supportedContainers)
            : base(FormatMessage(supportedContainers))
        {
            SupportedContainers = supportedContainers;
        }

        /// <summary>
        /// Gets the names of the supported containers.
        /// </summary>
        /// <value>The supported containers.</value>
        public string[] SupportedContainers { get; private set; }

        /// <summary>
        /// Formats the error message.
        /// </summary>
        /// <param name="supportedContainers">The supported containers.</param>
        /// <returns>The formatted error message.</returns>
        private static string FormatMessage(IEnumerable<string> supportedContainers)
        {
            var message = new StringBuilder();
            message.AppendLine("The specified container is not supported. Please use one of the following:");

            foreach (string supportedContainer in supportedContainers)
            {
                message.AppendLine("  * " + supportedContainer);
            }

            return message.ToString();
        }
    }
}
