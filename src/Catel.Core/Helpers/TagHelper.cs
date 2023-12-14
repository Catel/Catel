namespace Catel
{
    /// <summary>
    /// Helper class for comparing tags.
    /// </summary>
    public static class TagHelper
    {
        /// <summary>
        /// Compares the <paramref name="firstTag"/> with the <paramref name="secondTag"/>.
        /// <para/>
        /// This method is introduced because a string comparison fails when using ==.
        /// </summary>
        /// <param name="firstTag">The first tag.</param>
        /// <param name="secondTag">The second tag.</param>
        /// <returns>
        /// 	<c>true</c> if the tags are equal; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// When both tags are <c>null</c>, the tags are considered equal.
        /// </remarks>
        public static bool AreTagsEqual(object? firstTag, object? secondTag)
        {
            return ObjectHelper.AreEqual(firstTag, secondTag);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the tag. This method also handles <c>null</c>, in that
        /// case it will return "null".
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public static string? ToString(object? tag)
        {
            return ObjectToStringHelper.ToString(tag);
        }
    }
}
