namespace Catel.Configuration
{
    using Runtime.Serialization;

    /// <summary>
    /// Dynamic configuration serializer modifier.
    /// </summary>
    public class DynamicConfigurationSerializerModifier : SerializerModifierBase
    {
        private readonly ISerializationManager _serializationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicConfigurationSerializerModifier"/> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        public DynamicConfigurationSerializerModifier(ISerializationManager serializationManager)
        {
            _serializationManager = serializationManager;
        }

        /// <summary>
        /// Called when the object is about to be serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public override void OnSerializing(ISerializationContext context, object model)
        {
            _serializationManager.Clear(model.GetType());

            base.OnSerializing(context, model);
        }
    }
}
