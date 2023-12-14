namespace Catel.Runtime
{
    /// <summary>
    /// Info about a reference which can provide a unique reference.
    /// </summary>
    public class ReferenceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceInfo" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="id">The unique identifier.</param>
        /// <param name="isFirstUsage">if set to <c>true</c>, this is the first usage of this instance.</param>
        public ReferenceInfo(object instance, int? id, bool isFirstUsage)
        {
            Instance = instance;
            Id = id;
            IsFirstUsage = isFirstUsage;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int? Id { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this is the first usage of this instance.
        /// </summary>
        /// <value><c>true</c> if this instance is the first usage of this instance; otherwise, <c>false</c>.</value>
        public bool IsFirstUsage { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has called the serializing method on the model.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has called serializing; otherwise, <c>false</c>.
        /// </value>
        internal bool HasCalledSerializing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has called the serialized method on the model.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has called serializing; otherwise, <c>false</c>.
        /// </value>
        internal bool HasCalledSerialized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has called the deserializing method on the model.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has called serializing; otherwise, <c>false</c>.
        /// </value>
        internal bool HasCalledDeserializing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has called the deserialized method on the model.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has called serializing; otherwise, <c>false</c>.
        /// </value>
        internal bool HasCalledDeserialized { get; set; }
    }
}
