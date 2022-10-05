namespace Catel
{
    using System;

    /// <summary>
    /// Uri extension methods.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Gets the safe URI string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The safe URI string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uri" /> is <c>null</c>.</exception>
        public static string GetSafeUriString(this Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            var safeUri = uri.ToString();
            while (safeUri.StartsWith("//"))
            {
                safeUri = safeUri.Remove(0, 1);
            }

            uri = new Uri(safeUri, UriKind.RelativeOrAbsolute);
            return IsAbsoluteUrl(uri.ToString()) ? uri.AbsoluteUri : uri.OriginalString;
        }

        /// <summary>
        /// Determines whether the specified url is an absolute url or not.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns><c>true</c> if the specified url is an absolute url; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="url"/> is <c>null</c>.</exception>
        public static bool IsAbsoluteUrl(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result);
        }
    }
}
