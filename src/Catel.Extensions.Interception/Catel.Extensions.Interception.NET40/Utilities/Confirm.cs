// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Confirm.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Utilities
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class Confirm
    {
        #region Assertions
        /// <summary>
        /// Assertions the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public static void Assertion(Func<bool> predicate)
        {
            Assertion(predicate());
        }

        /// <summary>
        /// Assertions the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="message">The message.</param>
        public static void Assertion(Func<bool> predicate, string message)
        {
            Assertion(predicate(), new InvalidOperationException(message));
        }

        /// <summary>
        /// Assertions the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exception">The exception.</param>
        public static void Assertion(Func<bool> predicate, Exception exception)
        {
            if (!predicate())
            {
                throw exception;
            }
        }

        /// <summary>
        /// Assertions the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> condition.</param>
        public static void Assertion(bool condition)
        {
            Assertion(condition,
                      "Unable to assert statement. This usually happens when an invalid state is detected in the application.");
        }

        /// <summary>
        /// Assertions the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        public static void Assertion(bool condition, string message)
        {
            Assertion(condition, new InvalidOperationException(message));
        }

        /// <summary>
        /// Assertions the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="exception">The exception.</param>
        public static void Assertion(bool condition, Exception exception)
        {
            if (!condition)
            {
                throw exception;
            }
        }
        #endregion
    }
}