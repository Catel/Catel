// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationObject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Class containing information about a (de)serialized value.
    /// </summary>
    public class SerializationObject
    {
        private readonly object _propertyValue;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationObject" /> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        private SerializationObject(Type modelType, string propertyName, object propertyValue)
        {
            ModelType = modelType;
            PropertyName = propertyName;
            _propertyValue = propertyValue;
        }
        #endregion

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <value>The property value.</value>
        /// <exception cref="InvalidOperationException">The <see cref="IsSuccessful"/> is false and this property cannot be used.</exception>
        public object PropertyValue
        {
            get
            {
                if (!IsSuccessful)
                {
                    throw new InvalidOperationException();
                }

                return _propertyValue;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is successful.
        /// </summary>
        /// <value><c>true</c> if this instance is successful; otherwise, <c>false</c>.</value>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Creates an instance of the <see cref="SerializationObject"/> which represents a failed deserialized value.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>SerializationObject.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static SerializationObject FailedToDeserialize(Type modelType, string propertyName)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var obj = new SerializationObject(modelType, propertyName, null);
            obj.IsSuccessful = false;

            return obj;            
        }

        /// <summary>
        /// Creates an instance of the <see cref="SerializationObject"/> which represents a succeeded deserialized value.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static SerializationObject SucceededToDeserialize(Type modelType, string propertyName, object propertyValue)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var obj = new SerializationObject(modelType, propertyName, propertyValue);
            obj.IsSuccessful = true;

            return obj;
        }
    }
}