namespace Catel.Data
{
    using System.IO;
    using Runtime.Serialization;

    /// <summary>
    /// ISaveable model extensions.
    /// </summary>
    public static class ISavableModelExtensions
    {
        /// <summary>
        /// Saves the object to a file using a specific formatting.
        /// </summary>
        /// <param name="model">The model to save.</param>
        /// <param name="fileName">Filename of the file that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        public static void Save(this ISavableModel model, string fileName, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            var fileInfo = new FileInfo(fileName);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }

            using (Stream stream = new FileStream(fileName, FileMode.Create))
            {
                model.Save(stream, serializer, configuration);
            }
        }
    }
}
