namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// ModelBase Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [KnownType(typeof(ModelA)), KnownType(typeof(ModelB)), Serializable]
#endif
    public class Model : ComparableModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Model() { }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Model(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

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
        public static readonly PropertyData AProperty = RegisterProperty("A", typeof(string), string.Empty);
    }
}