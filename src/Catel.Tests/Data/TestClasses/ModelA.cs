namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// ModelA Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET || NETCORE
    [Serializable]
#endif
    public class ModelA : Model
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ModelA() { }
        #endregion

        /// <summary>
        /// Gets or sets property B.
        /// </summary>
        public string B
        {
            get { return GetValue<string>(BProperty); }
            set { SetValue(BProperty, value); }
        }

        /// <summary>
        /// Register the B property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BProperty = RegisterProperty("B", typeof(string), string.Empty);
    }
}
