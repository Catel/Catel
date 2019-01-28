namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

#if NET || NETCORE
    [Serializable]
#endif
    public class Child : SavableModelBase<Child>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        protected Child()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Child" /> class.
        /// </summary>
        /// <param name = "parent">The parent.</param>
        /// <param name = "name">The name.</param>
        public Child(Parent parent, string name)
        {
            // Store values
            Name = name;
        }

#if NET || NETCORE
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected Child(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the name of the child.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);
        #endregion
    }
}
