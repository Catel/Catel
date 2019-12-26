namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// ModelC Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET || NETCORE
    [Serializable]
#endif
    [KnownType(typeof(ModelA)), KnownType(typeof(ModelB))]
    public class ModelC : ComparableModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ModelC() { }
        #endregion

        /// <summary>
        /// Gets or sets the D property.
        /// </summary>
        public string D
        {
            get { return GetValue<string>(DProperty); }
            set { SetValue(DProperty, value); }
        }

        /// <summary>
        /// Register the D property so it is known in the class.
        /// </summary>
        public static readonly PropertyData DProperty = RegisterProperty("D", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the E property.
        /// </summary>
        public Model E
        {
            get { return GetValue<Model>(EProperty); }
            set { SetValue(EProperty, value); }
        }

        /// <summary>
        /// Register the E property so it is known in the class.
        /// </summary>
        public static readonly PropertyData EProperty = RegisterProperty("E", typeof(Model), null);
    }
}
