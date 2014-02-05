// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Enum representing all member group.
    /// </summary>
    public enum SerializationMemberGroup
    {
        /// <summary>
        /// The member is a catel property.
        /// </summary>
        CatelProperty,

        /// <summary>
        /// The member is a regular property.
        /// </summary>
        RegularProperty,

        /// <summary>
        /// The member is a field.
        /// </summary>
        Field
    }

    /// <summary>
    /// Member value which represents the serialization info of a specific member.
    /// </summary>
    public class MemberValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberValue" /> class.
        /// </summary>
        /// <param name="memberGroup">Group of the member.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public MemberValue(SerializationMemberGroup memberGroup, Type modelType, Type type, string name, object value)
        {
            MemberGroup = memberGroup;
            ModelType = modelType;
            Type = type;
            Name = name;
            Value = value;
        }

        #region Properties
        /// <summary>
        /// Gets the group of the member.
        /// </summary>
        /// <value>The group of the member.</value>
        public SerializationMemberGroup MemberGroup { get; private set; }

        /// <summary>
        /// Gets the type of the member.
        /// <para />
        /// This is the actual member type as it is defined on the type. This is <c>not</c> a wrapper around
        /// the <c>value.GetType()</c>.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the type of the model which this member value is a member of.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
        #endregion
    }
}