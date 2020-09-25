namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

#if NET || NETCORE
    [Serializable]
#endif
    public class ObjectWithPrivateConstructor : SavableModelBase<ObjectWithPrivateConstructor>
    {
        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        protected ObjectWithPrivateConstructor()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ObjectWithPrivateConstructor" /> class.
        /// </summary>
        /// <param name = "myValue">My value.</param>
        public ObjectWithPrivateConstructor(string myValue)
        {
            // Store values
            MyValue = myValue;
        }
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets my value.
        /// </summary>
        public string MyValue
        {
            get { return GetValue<string>(MyValueProperty); }
            set { SetValue(MyValueProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData MyValueProperty = RegisterProperty("MyValue", typeof(string), string.Empty);
        #endregion
    }
}
