namespace Catel.Test.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

#if NET
    [Serializable]
#endif
    public class ObjectWithPrivateMembers : ComparableModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithPrivateMembers()
        {
        }

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithPrivateMembers(string privateMemberValue)
        {
            // Store values
            PrivateMember = privateMemberValue;
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithPrivateMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the public member.
        /// </summary>
        public string PublicMember
        {
            get { return GetValue<string>(PublicMemberProperty); }
            set { SetValue(PublicMemberProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PublicMemberProperty = RegisterProperty("PublicMember", typeof(string), "Public member");

        /// <summary>
        ///   Gets or sets the private member.
        /// </summary>
        private string PrivateMember
        {
            get { return GetValue<string>(PrivateMemberProperty); }
            set { SetValue(PrivateMemberProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PrivateMemberProperty = RegisterProperty("PrivateMember", typeof(string), "Private member");
        #endregion
    }
}