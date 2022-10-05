namespace Catel.Services
{
    /// <summary>
    /// Interface defining a language source.
    /// </summary>
    public interface ILanguageSource
    {
        /// <summary>
        /// Gets the source for the current language source.
        /// </summary>
        /// <returns>The source string.</returns>
        string GetSource();
    }
}
