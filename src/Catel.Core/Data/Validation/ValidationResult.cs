// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValidationResult.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// Base class for validation results.
    /// </summary>
    public abstract class ValidationResult : IValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="validationResultType">Type of the validation result.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResultType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        protected ValidationResult(ValidationResultType validationResultType, string message)
        {
            Argument.IsNotNull("validationResultType", validationResultType);
            Argument.IsNotNull("message", message);

            ValidationResultType = validationResultType;
            Message = message;
        }

        /// <summary>
        /// Gets the type of the validation result.
        /// </summary>
        /// <value>The type of the validation result.</value>
        public ValidationResultType ValidationResultType { get; private set; }

        /// <summary>
        /// Gets or sets the validation result message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks>
        /// This value has a public setter so it is possible to customize the message
        /// in derived classes.
        /// <para />
        /// One should be careful and know what they are doing when overwriting an error message.
        /// </remarks>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the tag that allows grouping of validations.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }
    }

    /// <summary>
    /// Field validation result.
    /// </summary>
    public class FieldValidationResult : ValidationResult, IFieldValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValidationResult"/> class.
        /// </summary>
        /// <param name="property">The property data.</param>
        /// <param name="validationResultType">Type of the validation result.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public FieldValidationResult(PropertyData property, ValidationResultType validationResultType, string messageFormat, params object[] args)
            : this(property.Name, validationResultType, messageFormat, args) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValidationResult"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="validationResultType">Type of the validation result.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public FieldValidationResult(string propertyName, ValidationResultType validationResultType, string messageFormat, params object[] args)
            : base(validationResultType, (args is null || args.Length == 0) ? messageFormat : string.Format(messageFormat, args))
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("messageFormat", messageFormat);

            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var value = string.Format("{0} (Field: {1} | Tag: {2})", Message, PropertyName, ObjectToStringHelper.ToString(Tag));
            return value;
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing a warning.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public static FieldValidationResult CreateWarning(PropertyData propertyData, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("propertyData", propertyData);
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            return CreateWarning(propertyData.Name, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing a warning.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public static FieldValidationResult CreateWarning(string propertyName, string messageFormat, params object[] args)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            return new FieldValidationResult(propertyName, ValidationResultType.Warning, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult" /> containing a warning.
        /// </summary>
        /// <typeparam name="TProperty">The type of themodel.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="FieldValidationResult" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat" /> is <c>null</c> or whitespace.</exception>
        public static FieldValidationResult CreateWarning<TProperty>(Expression<Func<TProperty>> propertyExpression, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);
            return new FieldValidationResult(propertyName, ValidationResultType.Warning, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing a warning.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        public static FieldValidationResult CreateWarningWithTag(PropertyData propertyData, string message, object tag)
        {
            var warning = CreateWarning(propertyData, message);
            warning.Tag = tag;

            return warning;
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing a warning.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        public static FieldValidationResult CreateWarningWithTag(string propertyName, string message, object tag)
        {
            var warning = CreateWarning(propertyName, message);
            warning.Tag = tag;

            return warning;
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult" /> containing a warning.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The <see cref="FieldValidationResult" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message" /> is <c>null</c>.</exception>
        public static FieldValidationResult CreateWarningWithTag<TProperty>(Expression<Func<TProperty>> propertyExpression, string message, object tag)
        {
            var warning = CreateWarning(propertyExpression, message);
            warning.Tag = tag;

            return warning;
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing an error.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public static FieldValidationResult CreateError(PropertyData propertyData, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("propertyData", propertyData);
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            return CreateError(propertyData.Name, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing an error.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public static FieldValidationResult CreateError(string propertyName, string messageFormat, params object[] args)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            return new FieldValidationResult(propertyName, ValidationResultType.Error, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult" /> containing an error.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="FieldValidationResult" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat" /> is <c>null</c> or whitespace.</exception>
        public static FieldValidationResult CreateError<TProperty>(Expression<Func<TProperty>> propertyExpression, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("propertyName", propertyExpression);
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            var propertyName = ExpressionHelper.GetPropertyName(propertyExpression);
            return new FieldValidationResult(propertyName, ValidationResultType.Error, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing an error.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        public static FieldValidationResult CreateErrorWithTag(PropertyData propertyData, string message, object tag)
        {
            var error = CreateError(propertyData, message);
            error.Tag = tag;

            return error;
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult"/> containing an error.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The <see cref="FieldValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        public static FieldValidationResult CreateErrorWithTag(string propertyName, string message, object tag)
        {
            var error = CreateError(propertyName, message);
            error.Tag = tag;

            return error;
        }

        /// <summary>
        /// Creates a <see cref="FieldValidationResult" /> containing an error.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The <see cref="FieldValidationResult" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="message" /> is <c>null</c>.</exception>
        public static FieldValidationResult CreateErrorWithTag<TProperty>(Expression<Func<TProperty>> propertyExpression, string message, object tag)
        {
            var error = CreateError(propertyExpression, message);
            error.Tag = tag;

            return error;
        }
    }

    /// <summary>
    /// Business rule validation result.
    /// </summary>
    public class BusinessRuleValidationResult : ValidationResult, IBusinessRuleValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleValidationResult"/> class.
        /// </summary>
        /// <param name="validationResultType">Type of the validation result.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public BusinessRuleValidationResult(ValidationResultType validationResultType, string messageFormat, params object[] args)
            : base(validationResultType, (args is null || args.Length == 0) ? messageFormat : string.Format(messageFormat, args))
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var value = string.Format("{0} (Tag: {1})", Message, ObjectToStringHelper.ToString(Tag));
            return value;
        }

        /// <summary>
        /// Creates a <see cref="BusinessRuleValidationResult"/> containing a warning.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="BusinessRuleValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public static BusinessRuleValidationResult CreateWarning(string messageFormat, params object[] args)
        {
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            return new BusinessRuleValidationResult(ValidationResultType.Warning, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="BusinessRuleValidationResult"/> containing a warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The <see cref="BusinessRuleValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        public static BusinessRuleValidationResult CreateWarningWithTag(string message, object tag)
        {
            var warning = CreateWarning(message);
            warning.Tag = tag;

            return warning;
        }

        /// <summary>
        /// Creates a <see cref="BusinessRuleValidationResult"/> containing an error.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// The <see cref="BusinessRuleValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="messageFormat"/> is <c>null</c> or whitespace.</exception>
        public static BusinessRuleValidationResult CreateError(string messageFormat, params object[] args)
        {
            Argument.IsNotNullOrWhitespace("messageFormat", messageFormat);

            return new BusinessRuleValidationResult(ValidationResultType.Error, messageFormat, args);
        }

        /// <summary>
        /// Creates a <see cref="BusinessRuleValidationResult"/> containing an error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The <see cref="BusinessRuleValidationResult"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        public static BusinessRuleValidationResult CreateErrorWithTag(string message, object tag)
        {
            var error = CreateError(message);
            error.Tag = tag;

            return error;
        }
    }
}
