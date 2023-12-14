namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Attribute to specify the serialization modifier attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SerializerModifierAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerModifierAttribute"/> class.
        /// </summary>
        /// <param name="serializerModifierType">Type of the serializer modifier.</param>
        public SerializerModifierAttribute(Type serializerModifierType)
        {
            SerializerModifierType = serializerModifierType;
        }

        /// <summary>
        /// Gets the type of the serializer modifier.
        /// </summary>
        /// <value>The type of the serializer modifier.</value>
        public Type SerializerModifierType { get; private set; }
    }
}
