﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionUtil.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Linq
{
#if WINDOWS_PHONE
    using Catel.Reflection;
#endif

    using System;
    using System.Linq.Expressions;
    using LinqExpression = System.Linq.Expressions.Expression;

    /// <summary>
    /// General purpose Expression utilities
    /// </summary>
    /// <remarks>
    /// Code originally found at http://www.yoda.arachsys.com/csharp/miscutil/.
    /// </remarks>
    public static class ExpressionUtil
    {
        /// <summary>
        /// Create a function delegate representing a unary operation
        /// </summary>
        /// <typeparam name="TArg1">The parameter type</typeparam>
        /// <typeparam name="TResult">The return type</typeparam>
        /// <param name="body">Body factory</param>
        /// <returns>Compiled function delegate</returns>
        public static Func<TArg1, TResult> CreateExpression<TArg1, TResult>(
            Func<LinqExpression, UnaryExpression> body)
        {
            ParameterExpression inp = LinqExpression.Parameter(typeof (TArg1), "inp");

            try
            {
                return LinqExpression.Lambda<Func<TArg1, TResult>>(body(inp), inp).Compile();
            }
            catch (Exception ex)
            {
                string msg = ex.Message; // avoid capture of ex itself
                return delegate { throw new InvalidOperationException(msg); };
            }
        }

        /// <summary>
        /// Create a function delegate representing a binary operation
        /// </summary>
        /// <typeparam name="TArg1">The first parameter type</typeparam>
        /// <typeparam name="TArg2">The second parameter type</typeparam>
        /// <typeparam name="TResult">The return type</typeparam>
        /// <param name="body">Body factory</param>
        /// <returns>Compiled function delegate</returns>
        public static Func<TArg1, TArg2, TResult> CreateExpression<TArg1, TArg2, TResult>(
            Func<LinqExpression, LinqExpression, BinaryExpression> body)
        {
            return CreateExpression<TArg1, TArg2, TResult>(body, false);
        }

        /// <summary>
        /// Create a function delegate representing a binary operation
        /// </summary>
        /// <param name="castArgsToResultOnFailure">
        /// If no matching operation is possible, attempt to convert
        /// TArg1 and TArg2 to TResult for a match? For example, there is no
        /// "decimal operator /(decimal, int)", but by converting TArg2 (int) to
        /// TResult (decimal) a match is found.
        /// </param>
        /// <typeparam name="TArg1">The first parameter type</typeparam>
        /// <typeparam name="TArg2">The second parameter type</typeparam>
        /// <typeparam name="TResult">The return type</typeparam>
        /// <param name="body">Body factory</param>
        /// <returns>Compiled function delegate</returns>
        public static Func<TArg1, TArg2, TResult> CreateExpression<TArg1, TArg2, TResult>(
            Func<LinqExpression, LinqExpression, BinaryExpression> body, bool castArgsToResultOnFailure)
        {
            ParameterExpression lhs = LinqExpression.Parameter(typeof (TArg1), "lhs");
            ParameterExpression rhs = LinqExpression.Parameter(typeof (TArg2), "rhs");
            try
            {
                try
                {
                    return LinqExpression.Lambda<Func<TArg1, TArg2, TResult>>(body(lhs, rhs), lhs, rhs).Compile();
                }
                catch (InvalidOperationException)
                {
                    if (castArgsToResultOnFailure && !( // if we show retry                                                        
                                                      typeof (TArg1) == typeof (TResult) && // and the args aren't
                                                      typeof (TArg2) == typeof (TResult)))
                    {
                        // already "TValue, TValue, TValue"...
                        // convert both lhs and rhs to TResult (as appropriate)
                        LinqExpression castLhs = typeof (TArg1) == typeof (TResult) ?
                                                                                        (LinqExpression) lhs :
                                                                                                                 (LinqExpression) LinqExpression.Convert(lhs, typeof (TResult));
                        LinqExpression castRhs = typeof (TArg2) == typeof (TResult) ?
                                                                                        (LinqExpression) rhs :
                                                                                                                 (LinqExpression) LinqExpression.Convert(rhs, typeof (TResult));

                        return LinqExpression.Lambda<Func<TArg1, TArg2, TResult>>(
                            body(castLhs, castRhs), lhs, rhs).Compile();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message; // avoid capture of ex itself
                return delegate { throw new InvalidOperationException(msg); };
            }
        }
    }
}