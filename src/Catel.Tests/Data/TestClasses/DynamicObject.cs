namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// DynamicObject Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET || NETCORE
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
        #endregion

        #region Properties
        // TODO: Define your custom properties here using the dataprop code snippet
        #endregion

        #region Methods
        public static IPropertyData RegisterProperty(string name, Type type)
        {
            return ModelBase.RegisterProperty(name, type, null);
        }

        public new void InitializePropertyAfterConstruction(IPropertyData propertyData)
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
