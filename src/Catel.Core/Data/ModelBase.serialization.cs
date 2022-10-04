namespace Catel.Data
{
    using ISerializable = Catel.Runtime.Serialization.ISerializable;

    public partial class ModelBase
    {
        /// <summary>
        /// Called when the object is being serialized.
        /// </summary>
        protected virtual void OnSerializing()
        {
        }

        /// <summary>
        /// Called when the object has been serialized.
        /// </summary>
        protected virtual void OnSerialized()
        {
            
        }

        /// <summary>
        /// Starts the serialization.
        /// </summary>
        void ISerializable.StartSerialization()
        {
            OnSerializing();
        }

        /// <summary>
        /// Finishes the serialization.
        /// </summary>
        void ISerializable.FinishSerialization()
        {
            OnSerialized();
        }

        /// <summary>
        /// Called when the object is being deserialized.
        /// </summary>
        protected virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Begins the deserialization.
        /// </summary>
        void ISerializable.StartDeserialization()
        {
            //Log.Debug($"Start deserialization of '{GetType().Name}'");

            OnDeserializing();
        }

        /// <summary>
        /// Finishes the deserialization.
        /// </summary>
        void ISerializable.FinishDeserialization()
        {
            //Log.Debug($"Finish deserialization of '{GetType().Name}'");

            OnDeserialized();
        }
    }
}
