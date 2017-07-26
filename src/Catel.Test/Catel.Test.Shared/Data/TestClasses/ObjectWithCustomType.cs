namespace Catel.Test.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

#if NET
    [Serializable]
#endif
    public class ObjectWithCustomType : ComparableModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithCustomType()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithCustomType(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        ///   Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the gender.
        /// </summary>
        public Gender Gender
        {
            get { return GetValue<Gender>(GenderProperty); }
            set { SetValue(GenderProperty, value); }
        }

        /// <summary>
        ///   Register the Gender property so it is known in the class.
        /// </summary>
        public static readonly PropertyData GenderProperty = RegisterProperty("Gender", typeof(Gender), Gender.Male);
        #endregion

        #region Methods
        #endregion
    }
}