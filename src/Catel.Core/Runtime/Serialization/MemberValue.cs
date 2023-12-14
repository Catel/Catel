namespace Catel.Runtime.Serialization
{
    using Catel.Reflection;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Member value which represents the serialization info of a specific member.
    /// </summary>
    [DebuggerDisplay("{Name} => {Value} ({ActualMemberType})")]
    public class MemberValue
    {
        private object? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberValue" /> class.
        /// </summary>
        /// <param name="memberGroup">Group of the member.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="name">The name.</param>
        /// <param name="nameForSerialization">The name for serialization.</param>
        /// <param name="value">The value.</param>
        public MemberValue(SerializationMemberGroup memberGroup, Type modelType, Type memberType, string name, string nameForSerialization, object? value)
        {
            MemberGroup = memberGroup;
            ModelType = modelType;
            ModelTypeName = modelType.GetSafeFullName(false);
            MemberType = memberType;
            MemberTypeName = memberType.GetSafeFullName(false);
            Name = name;
            NameForSerialization = nameForSerialization;
            Value = value;
        }

        /// <summary>
        /// Gets the group of the member.
        /// </summary>
        /// <value>The group of the member.</value>
        public SerializationMemberGroup MemberGroup { get; private set; }

        /// <summary>
        /// Gets the type of the model which this member value is a member of.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the name of the model type, which should be a cached version of <c>ModelType.GetSafeFullName(false);</c>.
        /// </summary>
        /// <value>The name of the model type.</value>
        public string ModelTypeName { get; private set; }

        /// <summary>
        /// Gets the type of the member.
        /// <para />
        /// This is the actual member type as it is defined on the type. This is <c>not</c> a wrapper around
        /// the <c>value.GetType()</c>.
        /// </summary>
        /// <value>The type.</value>
        public Type MemberType { get; private set; }

        /// <summary>
        /// Gets the name of the model type, which should be a cached version of <c>ModelType.GetSafeFullName(false);</c>.
        /// </summary>
        /// <value>The name of the model type.</value>
        public string MemberTypeName { get; private set; }

        /// <summary>
        /// Gets the actual type of the value.
        /// </summary>
        /// <value>The actual type of the value.</value>
        public Type? ActualMemberType { get; private set; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the name for serialization. This is a name mapped based on attributes
        /// like DataMember("something"), etc.
        /// </summary>
        /// <value>The name for serialization.</value>
        public string NameForSerialization { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object? Value
        {
            get { return _value; }
            set
            {
                _value = value;

                ActualMemberType = (value is not null) ? value.GetType() : null;
            }
        }

        /// <summary>
        /// Gets the the best member type. Code is equal to <c>memberValue.ActualMemberType ?? memberValue.MemberType</c>.
        /// </summary>
        /// <returns>Type.</returns>
        public Type GetBestMemberType()
        {
            return ActualMemberType ?? MemberType;
        }
    }
}
