// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrongViewModelTypeException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Exception in case that a wrong type is used for a view model.
    /// </summary>
    public class WrongViewModelTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongViewModelTypeException"/> class.
        /// </summary>
        /// <param name="actualType">The actual type.</param>
        /// <param name="expectedType">The expected type.</param>
        public WrongViewModelTypeException(Type actualType, Type expectedType)
            : base(string.Format(ResourceHelper.GetString(typeof(WrongViewModelTypeException), "Exceptions", "WrongViewModelType"), expectedType, actualType))
        {
            ActualType = actualType;
            ExpectedType = expectedType;
        }

        /// <summary>
        /// Gets the actual type.
        /// </summary>
        /// <value>The actual type.</value>
        public Type ActualType { get; private set; }

        /// <summary>
        /// Gets the expected type.
        /// </summary>
        /// <value>The expected type.</value>
        public Type ExpectedType { get; private set; }
    }
}