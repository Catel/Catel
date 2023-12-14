namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// ModelBase Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [KnownType(typeof(ModelA)), KnownType(typeof(ModelB)), Serializable]
    public class Model : ComparableModelBase
    {
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Model() { }

        /// <summary>
        /// Gets or sets the A property.
        /// </summary>
        public string A
        {
            get { return GetValue<string>(AProperty); }
            set { SetValue(AProperty, value); }
        }

        /// <summary>
        /// Register the A property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData AProperty = RegisterProperty("A", string.Empty);
    }
}
