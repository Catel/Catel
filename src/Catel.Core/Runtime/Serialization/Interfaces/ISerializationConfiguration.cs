namespace Catel.Runtime.Serialization
{
    using System.Globalization;

    /// <summary>
    /// Serialization configuration.
    /// </summary>
    public interface ISerializationConfiguration
    {
        /// <summary>
        /// Gets or sets the culture used for serialization.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        CultureInfo Culture { get; set; }
    }
}
