// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Argument.expression.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    using Data;

    /// <summary>
    /// Argument validator class to help validating arguments that are passed into a method.
    /// <para />
    /// This class automatically adds thrown exceptions to the log file.
    /// </summary>
    public static partial class Argument
    {
        #region Methods

        /// <summary>
        /// The get parameter info.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// The type of the parameter.
        /// </typeparam>
        /// <returns>
        /// The <see cref="ParameterInfo{T}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        private static ParameterInfo<T> GetParameterInfo<T>(Expression<Func<T>> expression)
        {
            IsNotNull("expression", expression);
            IsOfType("expression.Body", expression.Body, typeof(MemberExpression));

            var parameterExpression = (MemberExpression)expression.Body;
            var parameterInfo = new ParameterInfo<T>(parameterExpression.Member.Name, expression.Compile().Invoke());

            return parameterInfo;
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        /// The parameter type. 
        /// </typeparam>
        /// <param name="expression">
        /// The expression
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotNull<T>(Expression<Func<T>> expression)
        {
            var parameterInfo = GetParameterInfo(expression);
            IsNotNull(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="expression"/> value is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotNullOrEmpty(Expression<Func<string>> expression)
        {
            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmpty(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="expression"/> value is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotNullOrEmpty(Expression<Func<Guid>> expression)
        {
            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmpty(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="expression"/> value is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotNullOrEmpty(Expression<Func<Guid?>> expression)
        {
            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmpty(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or a whitespace.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="expression"/> value is <c>null</c> or a whitespace.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotNullOrWhitespace(Expression<Func<string>> expression)
        {
            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrWhitespace(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
        /// </summary>
        /// <param name="expression">
        /// The expression
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="expression"/> value is <c>null</c> or an empty array.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotNullOrEmptyArray(Expression<Func<Array>> expression)
        {
            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmptyArray(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="minimumValue">
        /// The minimum value.
        /// </param>
        /// <param name="maximumValue">
        /// The maximum value.
        /// </param>
        /// <param name="validation">
        /// The validation function to call for validation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="validation"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(Expression<Func<T>> expression, T minimumValue, T maximumValue, Func<T, T, T, bool> validation)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsNotOutOfRange(parameterInfo.Name, parameterInfo.Value, minimumValue, maximumValue, validation);
        }

#if NET

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="minimumValue">
        /// The minimum value.
        /// </param>
        /// <param name="maximumValue">
        /// The maximum value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(Expression<Func<T>> expression, T minimumValue, T maximumValue)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsNotOutOfRange(parameterInfo.Name, parameterInfo.Value, minimumValue, maximumValue);
        }

#endif

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="minimumValue">
        /// The minimum value.
        /// </param>
        /// <param name="maximumValue">
        /// The maximum value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotOutOfRange(Expression<Func<int>> expression, int minimumValue, int maximumValue)
        {
            ParameterInfo<int> parameterInfo = GetParameterInfo(expression);
            IsNotOutOfRange(parameterInfo.Name, parameterInfo.Value, minimumValue, maximumValue);
        }

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="minimumValue">
        /// The minimum value.
        /// </param>
        /// <param name="validation">
        /// The validation function to call for validation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="validation"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMinimal<T>(Expression<Func<T>> expression, T minimumValue, Func<T, T, bool> validation)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsMinimal(parameterInfo.Name, parameterInfo.Value, minimumValue, validation);
        }

#if NET

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="minimumValue">
        /// The minimum value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMinimal<T>(Expression<Func<T>> expression, T minimumValue)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsMinimal(parameterInfo.Name, parameterInfo.Value, minimumValue);
        }

#endif

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="minimumValue">
        /// The minimum value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMinimal(Expression<Func<int>> expression, int minimumValue)
        {
            ParameterInfo<int> parameterInfo = GetParameterInfo(expression);
            IsMinimal(parameterInfo.Name, parameterInfo.Value, minimumValue);
        }

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="maximumValue">
        /// The maximum value.
        /// </param>
        /// <param name="validation">
        /// The validation function to call for validation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="validation"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMaximum<T>(Expression<Func<T>> expression, T maximumValue, Func<T, T, bool> validation)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsMaximum(parameterInfo.Name, parameterInfo.Value, maximumValue, validation);
        }

#if NET

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="maximumValue">
        /// The maximum value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMaximum<T>(Expression<Func<T>> expression, T maximumValue)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsMaximum(parameterInfo.Name, parameterInfo.Value, maximumValue);
        }

#endif

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="maximumValue">
        /// The maximum value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="expression"/> value is out of range.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMaximum(Expression<Func<int>> expression, int maximumValue)
        {
            ParameterInfo<int> parameterInfo = GetParameterInfo(expression);
            IsMaximum(parameterInfo.Name, parameterInfo.Value, maximumValue);
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> value implements the specified <paramref name="interfaceType"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="interfaceType">
        /// The type of the interface to check for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="interfaceType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="expression"/> value does not implement the <paramref name="interfaceType"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void ImplementsInterface<T>(Expression<Func<T>> expression, Type interfaceType) where T : class
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type)
            {
                ImplementsInterface(parameterInfo.Name, parameterInfo.Value as Type, interfaceType);
            }
            else
            {
                ImplementsInterface(parameterInfo.Name, parameterInfo.Value.GetType(), interfaceType);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> value implements at least one of the specified <paramref name="interfaceTypes"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="interfaceTypes">
        /// The types of the interfaces to check for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="interfaceTypes"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="expression"/> value does not implement at least one of the <paramref name="interfaceTypes"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void ImplementsOneOfTheInterfaces<T>(Expression<Func<T>> expression, Type[] interfaceTypes) where T : class
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type)
            {
                ImplementsOneOfTheInterfaces(parameterInfo.Name, parameterInfo.Value as Type, interfaceTypes);
            }
            else
            {
                ImplementsOneOfTheInterfaces(parameterInfo.Name, parameterInfo.Value.GetType(), interfaceTypes);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> value is of the specified <paramref name="requiredType"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="requiredType">
        /// The type to check for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requiredType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="expression"/> value is not of type <paramref name="requiredType"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public static void IsOfType<T>(Expression<Func<T>> expression, Type requiredType) where T : class
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type)
            {
                IsOfType(parameterInfo.Name, parameterInfo.Value as Type, requiredType);
            }
            else
            {
                IsOfType(parameterInfo.Name, parameterInfo.Value.GetType(), requiredType);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> value is of at least one of the specified <paramref name="requiredTypes"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression type.
        /// </param>
        /// <param name="requiredTypes">
        /// The types to check for.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requiredTypes"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="expression"/> value is not at least one of the <paramref name="requiredTypes"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsOfOneOfTheTypes<T>(Expression<Func<T>> expression, Type[] requiredTypes) where T : class
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type)
            {
                IsOfOneOfTheTypes(parameterInfo.Name, parameterInfo.Value as Type, requiredTypes);
            }
            else
            {
                IsOfOneOfTheTypes(parameterInfo.Name, parameterInfo.Value.GetType(), requiredTypes);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> value is not of the specified <paramref name="notRequiredType"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="notRequiredType">
        /// The type to check for.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notRequiredType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="expression"/> value is of type <paramref name="notRequiredType"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotOfType<T>(Expression<Func<T>> expression, Type notRequiredType) where T : class
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type)
            {
                IsNotOfType(parameterInfo.Name, parameterInfo.Value as Type, notRequiredType);
            }
            else
            {
                IsNotOfType(parameterInfo.Name, parameterInfo.Value.GetType(), notRequiredType);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> value is not of any of the specified <paramref name="notRequiredTypes"/>.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="notRequiredTypes">
        /// The types to check for.
        /// </param>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notRequiredTypes"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="expression"/> value is of one of the <paramref name="notRequiredTypes"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotOfOneOfTheTypes<T>(Expression<Func<T>> expression, Type[] notRequiredTypes) where T : class
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type)
            {
                IsNotOfOneOfTheTypes(parameterInfo.Name, parameterInfo.Value as Type, notRequiredTypes);
            }
            else
            {
                IsNotOfOneOfTheTypes(parameterInfo.Name, parameterInfo.Value.GetType(), notRequiredTypes);
            }
        }

        /// <summary>
        /// Determines whether the specified argument doesn't match with a given pattern.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="regexOptions">
        /// The regular expression options.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> value matches with the given <paramref name="pattern"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsNotMatch(Expression<Func<string>> expression, string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            ParameterInfo<string> parameterInfo = GetParameterInfo(expression);
            IsNotMatch(parameterInfo.Name, parameterInfo.Value, pattern, regexOptions);
        }

        /// <summary>
        /// Determines whether the specified argument match with a given pattern.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <param name="regexOptions">
        /// The regular expression options.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> value doesn't match with the given <paramref name="pattern"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsMatch(Expression<Func<string>> expression, string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            ParameterInfo<string> parameterInfo = GetParameterInfo(expression);
            IsMatch(parameterInfo.Name, parameterInfo.Value, pattern, regexOptions);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="validation">
        /// The validation function.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="validation"/> code returns <c>false</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="validation"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, Func<T, bool> validation)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, parameterInfo.Value, validation);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="validation">
        /// The validation function.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="validation"/> code returns <c>false</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="validation"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, Func<bool> validation)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, parameterInfo.Value, validation);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="validation">
        /// The validation result.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="validation"/> code returns <c>false</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="validation"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, bool validation)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, parameterInfo.Value, validation);
        }       
        
        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">
        /// The value type.
        /// </typeparam>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="validator">
        /// The validator.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <see cref="IValueValidator{TValue}.IsValid"/> of  <paramref name="validator"/> returns <c>false</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="validator"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, IValueValidator<T> validator)
        {
            ParameterInfo<T> parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, parameterInfo.Value, validator);
        }

        #endregion

        #region Nested type: ParameterInfo

        /// <summary>
        /// The parameter info.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter value.
        /// </typeparam>
        private class ParameterInfo<T>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterInfo{T}"/> class. 
            /// </summary>
            /// <param name="name">
            /// The parameter name. 
            /// </param>
            /// <param name="value">
            /// The parameter value.
            /// </param>
            public ParameterInfo(string name, T value)
            {
                this.Value = value;
                this.Name = name;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the value.
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }
            #endregion
        }
        #endregion
    }
}