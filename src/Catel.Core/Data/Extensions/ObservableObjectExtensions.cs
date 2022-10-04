namespace Catel.Data
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="ObservableObject"/> class.
    /// </summary>
    public static class ObservableObjectExtensions
    {
        /// <summary>
        /// Raises the property changed event for the specified <see cref="ObservableObject"/>.
        /// </summary>
        /// <param name="sender">The observable object.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sender"/> is <c>null</c>.</exception>
        public static void RaiseAllPropertiesChanged(this ObservableObject sender)
        {
            sender.RaisePropertyChanged(string.Empty);
        }
    }
}
