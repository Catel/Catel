namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// ModelB Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [Serializable]
    public class ModelB : Model
    {
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ModelB() { }

        /// <summary>
        /// Gets or sets property C.
        /// </summary>
        public string C
        {
            get { return GetValue<string>(CProperty); }
            set { SetValue(CProperty, value); }
        }

        /// <summary>
        /// Register the C property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData CProperty = RegisterProperty("C", string.Empty);
    }
}
