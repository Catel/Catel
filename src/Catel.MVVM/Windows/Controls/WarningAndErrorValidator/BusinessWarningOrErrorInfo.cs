namespace Catel.Windows.Controls
{
    using System.Globalization;

    /// <summary>
    /// Information class about business warnings and errors.
    /// </summary>
    internal class BusinessWarningOrErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessWarningOrErrorInfo"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BusinessWarningOrErrorInfo(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (obj.GetHashCode() != GetHashCode())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", Message).GetHashCode();
        }
    }
}
