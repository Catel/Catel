namespace Catel.Data
{
    /// <summary>
    /// Object adapter allowing to customize reflection and member mappings.
    /// </summary>
    public interface IObjectAdapter
    {
        /// <summary>
        /// Gets the member value of the instance.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to retrieve.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The member value to update.</param>
        /// <returns><c>true</c> if the member was retrieved; otherwise <c>false</c>.</returns>
        bool GetMemberValue<TValue>(object instance, string memberName, ref TValue value);

        /// <summary>
        /// Sets the member value of the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the member was set successfully; otherwise <c>false</c>.</returns>
        bool SetMemberValue<TValue>(object instance, string memberName, TValue value);
    }
}
