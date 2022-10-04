namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Metadata about a member of a type.
    /// </summary>
    public class MemberMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberMetadata" /> class.
        /// </summary>
        /// <param name="containingType">Type of the containing.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="memberGroup">The member group.</param>
        /// <param name="memberName">Name of the member.</param>
        public MemberMetadata(Type containingType, Type memberType, SerializationMemberGroup memberGroup, string memberName)
        {
            ContainingType = containingType;
            MemberType = memberType;
            MemberGroup = memberGroup;
            MemberName = memberName;
            MemberNameForSerialization = memberName;
        }

        /// <summary>
        /// Gets the type of the containing.
        /// </summary>
        /// <value>The type of the containing.</value>
        public Type ContainingType { get; private set; }

        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <value>The type of the member.</value>
        public Type MemberType { get; private set; }

        /// <summary>
        /// Gets the member group.
        /// </summary>
        /// <value>The member group.</value>
        public SerializationMemberGroup MemberGroup { get; private set; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>The name of the member.</value>
        public string MemberName { get; private set; }

        /// <summary>
        /// Gets or sets the member name for serialization. This is a name mapped based on attributes
        /// like DataMember("something"), etc.
        /// </summary>
        /// <value>The member name for serialization.</value>
        public string MemberNameForSerialization { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object? Tag { get; set; }
    }
}
