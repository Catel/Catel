namespace Catel.Test.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// DynamicObject Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class DynamicObject : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public DynamicObject()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected DynamicObject(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        // TODO: Define your custom properties here using the dataprop code snippet
        #endregion

        #region Methods
        public static PropertyData RegisterProperty(string name, Type type)
        {
            return ModelBase.RegisterProperty(name, type);
        }

        public new void InitializePropertyAfterConstruction(PropertyData propertyData)
        {
            base.InitializePropertyAfterConstruction(propertyData);
        }

        public void SetValue<TValue>(string propertyName, TValue value)
        {
            base.SetValue(propertyName, value);
        }

        public new TValue GetValue<TValue>(string propertyName)
        {
            return base.GetValue<TValue>(propertyName);
        }
        #endregion
    }
}