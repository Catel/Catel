// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionTester.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;
    using LinqExpression = System.Linq.Expressions.Expression;

    /// <summary>
    /// Class that helps test methods for expected exceptions.
    /// </summary>
    public static class ExceptionTester
    {
        /// <summary>
        /// Calls the method and checks for the exception.
        /// <para />
        /// If no exception is thrown by the method, this method will throw an exception. If the wrong
        /// exception is thrown by the delegate, this method will thrown an exception as well.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="exceptionValidator">The exception validator. If <c>null</c>, the exception will not be validated custom.</param>
        /// <returns>The exception so it can be further analyzed if required.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static TException CallMethodAndExpectException<TException>(Action action, Func<TException, bool> exceptionValidator = null)
            where TException : Exception
        {
            Argument.IsNotNull("action", action);

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (!(ex is TException))
                {
                    throw new Exception(string.Format("Expected exception of type '{0}', but actual type is '{1}'.\n\nInner exception: {2}",
                        typeof (TException).FullName, ex.GetType().FullName, ex.Message));
                }

                if (exceptionValidator != null)
                {
                    if (!exceptionValidator((TException) ex))
                    {
                        throw new Exception(string.Format("Right exception was thrown, but custom exception validation check failed.\n\nInner exception: {0}", ex.Message));
                    }
                }

                return (TException)ex;
            }

            throw new Exception(string.Format("Expected exception of type '{0}', but no exception was thrown", typeof (TException).FullName));
        }
    }
}