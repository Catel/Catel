// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationObject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Class containing information about a (de)serialized value.
    /// </summary>
    public class SerializationObject
    {
        private readonly object _memberValue;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationObject" /> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberGroup">Group of the member.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberValue">The member value.</param>
        private SerializationObject(Type modelType, SerializationMemberGroup memberGroup, string memberName, object memberValue)
        {
            ModelType = modelType;

            MemberGroup = memberGroup;
            MemberName = memberName;
            _memberValue = memberValue;
        }
        #endregion

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the group of the member.
        /// </summary>
        /// <value>The group of the member.</value>
        public SerializationMemberGroup MemberGroup { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string MemberName { get; private set; }

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <value>The member value.</value>
        /// <exception cref="InvalidOperationException">The <see cref="IsSuccessful"/> is false and this member cannot be used.</exception>
        public object MemberValue
        {
            get
            {
                if (!IsSuccessful)
                {
                    throw new InvalidOperationException();
                }

                return _memberValue;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is successful.
        /// </summary>
        /// <value><c>true</c> if this instance is successful; otherwise, <c>false</c>.</value>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Creates an instance of the <see cref="SerializationObject" /> which represents a failed deserialized value.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberGroup">Type of the member.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>SerializationObject.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="memberName" /> is <c>null</c> or whitespace.</exception>
        public static SerializationObject FailedToDeserialize(Type modelType, SerializationMemberGroup memberGroup, string memberName)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNullOrWhitespace("memberName", memberName);

            var obj = new SerializationObject(modelType, memberGroup, memberName, null);
            obj.IsSuccessful = false;

            return obj;            
        }

        /// <summary>
        /// Creates an instance of the <see cref="SerializationObject" /> which represents a succeeded deserialized value.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberGroup">Type of the member.</param>
        /// <param name="memberName">Name of the property.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>SerializationObject.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="memberName" /> is <c>null</c> or whitespace.</exception>
        public static SerializationObject SucceededToDeserialize(Type modelType, SerializationMemberGroup memberGroup, string memberName, object memberValue)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNullOrWhitespace("memberName", memberName);

            var obj = new SerializationObject(modelType, memberGroup, memberName, memberValue);
            obj.IsSuccessful = true;

            return obj;
        }
    }
}