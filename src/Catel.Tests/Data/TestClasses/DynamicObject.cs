namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    /// <summary>
    /// DynamicObject Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [Serializable]
    public class DynamicObject : ModelBase
    {
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public DynamicObject()
        {
        }

        public static IPropertyData RegisterProperty(string name, Type type)
        {
            return ModelBase.RegisterProperty(name, type, null);
        }

        public new void InitializePropertyAfterConstruction(IPropertyData propertyData)
        {
            ArgumentNullException.ThrowIfNull(propertyData);

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
    }
}
