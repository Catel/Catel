namespace Catel.Data
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Exception when a property is used by the <see cref="ModelBase"/> class that is
    /// not registered by the object.
    /// </summary>
    public class PropertyNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNotRegisteredException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that caused the exception.</param>
        /// <param name="objectType">Type of the object that is trying to register the property.</param>
        public PropertyNotRegisteredException(string propertyName, Type objectType)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is not registered on type '{1}'", propertyName, objectType))
        {
            PropertyName = propertyName;
            ObjectType = objectType;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        /// <value>The property name.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the object type for which the property is already registered.
        /// </summary>
        /// <value>The object type for which the property is already registered.</value>
        public Type ObjectType { get; private set; }
    }
}
