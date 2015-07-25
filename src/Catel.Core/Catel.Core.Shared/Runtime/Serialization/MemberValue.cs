// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Diagnostics;

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
        Field,

        /// <summary>
        /// The member is a collection.
        /// </summary>
        Collection,

        /// <summary>
        /// The member is a dictionary.
        /// </summary>
        Dictionary
    }

    /// <summary>
    /// Member value which represents the serialization info of a specific member.
    /// </summary>
    [DebuggerDisplay("{Name} => {Value} ({ActualMemberType})")]
    public class MemberValue
    {
        private object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberValue" /> class.
        /// </summary>
        /// <param name="memberGroup">Group of the member.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public MemberValue(SerializationMemberGroup memberGroup, Type modelType, Type memberType, string name, object value)
        {
            MemberGroup = memberGroup;
            ModelType = modelType;
            MemberType = memberType;
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
        public Type MemberType { get; private set; }

        /// <summary>
        /// Gets the actual type of the value.
        /// </summary>
        /// <value>The actual type of the value.</value>
        public Type ActualMemberType { get; private set; }

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
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;

                ActualMemberType = (value != null) ? value.GetType() : null;
            }
        }
        #endregion
    }
}