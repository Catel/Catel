namespace Catel
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using Catel.Logging;
    using Data;

    /// <summary>
    /// Argument validator class to help validating arguments that are passed into a method.
    /// <para />
    /// This class automatically adds thrown exceptions to the log file.
    /// </summary>
    public static partial class Argument
    {
        /// <summary>
        /// The get parameter info.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="ParameterInfo{T}" />.</returns>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        private static ParameterInfo<T> GetParameterInfo<T>(Expression<Func<T>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterExpression = (MemberExpression)expression.Body;
            var parameterInfo = new ParameterInfo<T>(parameterExpression.Member.Name, expression.Compile().Invoke());

            return parameterInfo;
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="expression" /> value is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNull<T>(Expression<Func<T>> expression)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotNull(parameterInfo.Name, parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> value is <c>null</c> or empty.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmpty(Expression<Func<string>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmpty(parameterInfo.Name, (string)parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not empty.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> value is <c>null</c> or empty.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotEmpty(Expression<Func<Guid>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotEmpty(parameterInfo.Name, (Guid)parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> value is <c>null</c> or empty.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmpty(Expression<Func<Guid?>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmpty(parameterInfo.Name, (Guid?)parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or a whitespace.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> value is <c>null</c> or a whitespace.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrWhitespace(Expression<Func<string>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrWhitespace(parameterInfo.Name, (string)parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <exception cref="ArgumentException">If <paramref name="expression" /> value is <c>null</c> or an empty array.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmptyArray(Expression<Func<Array>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotNullOrEmptyArray(parameterInfo.Name, (Array)parameterInfo.Value);
        }

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="validation">The validation function to call for validation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validation" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expression" /> value is out of range.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(Expression<Func<T>> expression, T minimumValue, T maximumValue, Func<T, T, T, bool> validation)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotOutOfRange(parameterInfo.Name, (T)parameterInfo.Value, minimumValue, maximumValue, validation);
        }

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expression" /> value is out of range.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(Expression<Func<T>> expression, T minimumValue, T maximumValue)
            where T : IComparable
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotOutOfRange(parameterInfo.Name, (T)parameterInfo.Value, minimumValue, maximumValue);
        }

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="validation">The validation function to call for validation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validation" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expression" /> value is out of range.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMinimal<T>(Expression<Func<T>> expression, T minimumValue, Func<T, T, bool> validation)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsMinimal(parameterInfo.Name, (T)parameterInfo.Value, minimumValue, validation);
        }

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expression" /> value is out of range.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMinimal<T>(Expression<Func<T>> expression, T minimumValue)
            where T : IComparable
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsMinimal(parameterInfo.Name, (T)parameterInfo.Value, minimumValue);
        }

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="validation">The validation function to call for validation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validation" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expression" /> value is out of range.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMaximum<T>(Expression<Func<T>> expression, T maximumValue, Func<T, T, bool> validation)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsMaximum(parameterInfo.Name, (T)parameterInfo.Value, maximumValue, validation);
        }

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expression" /> value is out of range.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMaximum<T>(Expression<Func<T>> expression, T maximumValue)
            where T : IComparable
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsMaximum(parameterInfo.Name, (T)parameterInfo.Value, maximumValue);
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression" /> value implements the specified <paramref name="interfaceType" />.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="interfaceType">The type of the interface to check for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> value does not implement the <paramref name="interfaceType" />.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void ImplementsInterface<T>(Expression<Func<T>> expression, Type interfaceType)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type parameterType)
            {
                ImplementsInterface(parameterInfo.Name, parameterType, interfaceType);
            }
            else
            {
                ImplementsInterface(parameterInfo.Name, parameterInfo.Value.GetType(), interfaceType);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression" /> value implements at least one of the specified <paramref name="interfaceTypes" />.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="interfaceTypes">The types of the interfaces to check for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> value does not implement at least one of the <paramref name="interfaceTypes" />.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void ImplementsOneOfTheInterfaces<T>(Expression<Func<T>> expression, Type[] interfaceTypes)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type parameterType)
            {
                ImplementsOneOfTheInterfaces(parameterInfo.Name, parameterType, interfaceTypes);
            }
            else
            {
                ImplementsOneOfTheInterfaces(parameterInfo.Name, parameterInfo.Value.GetType(), interfaceTypes);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression" /> value is of the specified <paramref name="requiredType" />.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="requiredType">The type to check for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> value is not of type <paramref name="requiredType" />.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        public static void IsOfType<T>(Expression<Func<T>> expression, Type requiredType)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);
            ArgumentNullException.ThrowIfNull(requiredType);

            var parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type parameterType)
            {
                IsOfType(parameterInfo.Name, parameterType, requiredType);
            }
            else
            {
                IsOfType(parameterInfo.Name, parameterInfo.Value.GetType(), requiredType);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression" /> value is of at least one of the specified <paramref name="requiredTypes" />.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="expression">The expression type.</param>
        /// <param name="requiredTypes">The types to check for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="requiredTypes" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> value is not at least one of the <paramref name="requiredTypes" />.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsOfOneOfTheTypes<T>(Expression<Func<T>> expression, Type[] requiredTypes)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type parameterType)
            {
                IsOfOneOfTheTypes(parameterInfo.Name, parameterType, requiredTypes);
            }
            else
            {
                IsOfOneOfTheTypes(parameterInfo.Name, parameterInfo.Value.GetType(), requiredTypes);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression" /> value is not of the specified <paramref name="notRequiredType" />.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="notRequiredType">The type to check for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> value is of type <paramref name="notRequiredType" />.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOfType<T>(Expression<Func<T>> expression, Type notRequiredType)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type parameterType)
            {
                IsNotOfType(parameterInfo.Name, parameterType, notRequiredType);
            }
            else
            {
                IsNotOfType(parameterInfo.Name, parameterInfo.Value.GetType(), notRequiredType);
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="expression" /> value is not of any of the specified <paramref name="notRequiredTypes" />.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="notRequiredTypes">The types to check for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> value is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> value is of one of the <paramref name="notRequiredTypes" />.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOfOneOfTheTypes<T>(Expression<Func<T>> expression, Type[] notRequiredTypes)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            if (parameterInfo.Value is Type parameterType)
            {
                IsNotOfOneOfTheTypes(parameterInfo.Name, parameterType, notRequiredTypes);
            }
            else
            {
                IsNotOfOneOfTheTypes(parameterInfo.Name, parameterInfo.Value.GetType(), notRequiredTypes);
            }
        }

        /// <summary>
        /// Determines whether the specified argument doesn't match with a given pattern.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="regexOptions">The regular expression options.</param>
        /// <exception cref="System.ArgumentException">The <paramref name="pattern" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotMatch(Expression<Func<string>> expression, string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsNotMatch(parameterInfo.Name, (string)parameterInfo.Value, pattern, regexOptions);
        }

        /// <summary>
        /// Determines whether the specified argument match with a given pattern.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="regexOptions">The regular expression options.</param>
        /// <exception cref="System.ArgumentException">The <paramref name="pattern" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMatch(Expression<Func<string>> expression, string pattern, RegexOptions regexOptions = RegexOptions.None)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsMatch(parameterInfo.Name, (string)parameterInfo.Value, pattern, regexOptions);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="validation">The validation function.</param>
        /// <exception cref="ArgumentException">If the <paramref name="validation" /> code returns <c>false</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, Func<T, bool> validation)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, (T)parameterInfo.Value, validation);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="validation">The validation function.</param>
        /// <exception cref="ArgumentException">If the <paramref name="validation"/> code returns <c>false</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression"/> body is not of type <see cref="MemberExpression"/>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression"/> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, Func<bool> validation)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, (T)parameterInfo.Value, validation);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="validation">The validation result.</param>
        /// <exception cref="ArgumentException">If the <paramref name="validation" /> code returns <c>false</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, bool validation)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, (T)parameterInfo.Value, validation);
        }

        /// <summary>
        /// Determines whether the specified argument is valid.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="validator">The validator.</param>
        /// <exception cref="ArgumentException">If the <see cref="IValueValidator{TValue}.IsValid" /> of  <paramref name="validator" /> returns <c>false</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="expression" /> body is not of type <see cref="MemberExpression" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="expression" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsValid<T>(Expression<Func<T>> expression, IValueValidator<T> validator)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var parameterInfo = GetParameterInfo(expression);
            IsValid(parameterInfo.Name, (T)parameterInfo.Value, validator);
        }

        /// <summary>
        /// The parameter info.
        /// </summary>
        private class ParameterInfo<T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterInfo{T}" /> class.
            /// </summary>
            /// <param name="name">The parameter name.</param>
            /// <param name="value">The value.</param>
            public ParameterInfo(string name, T value)
            {
                Name = name;
                Value = value;
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name { get; private set; }
        }
    }
}
