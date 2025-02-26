namespace Catel.Data
{
    using System.IO;
    using Catel.Runtime.Serialization;

    public static partial class ModelBaseExtensions
    {
        /// <summary>
        /// Saves the specified model to the stream using the serializer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="serializer">The serializer.</param>
        public static void Save(this ModelBase model, Stream stream, ISerializer serializer)
        {
            serializer.Serialize(model, stream, null);
        }

        /// <summary>
        /// Saves the specified model to the file using the specified serializer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="serializer">The serializer.</param>
        public static void Save(this ModelBase model, string filePath, ISerializer serializer)
        {
            Argument.IsNotNullOrWhitespace("filePath", filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                Save(model, fileStream, serializer);
            }
        }
    }
}
