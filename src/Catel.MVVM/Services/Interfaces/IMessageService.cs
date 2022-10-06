namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Available message results.
    /// </summary>
    /// <remarks>
    /// Although directly extracted from the WPF <c>MessageBoxResult</c>, this enum provides more flexibility to use
    /// other ways to show messages to the user instead of the default <c>MessageBox</c>.
    /// </remarks>
    public enum MessageResult
    {
        /// <summary>
        /// No result available.
        /// </summary>
        None,

        /// <summary>
        /// Message is acknowledged.
        /// </summary>
        OK,

        /// <summary>
        /// Message is canceled.
        /// </summary>
        Cancel,

        /// <summary>
        /// Message is acknowledged with yes.
        /// </summary>
        Yes,

        /// <summary>
        /// Message is acknowledged with no.
        /// </summary>
        No
    }

    /// <summary>
    /// Available message buttons.
    /// </summary>
    /// <remarks>
    /// Although directly extracted from the WPF <c>MessageBoxButton</c>, this enum provides more flexibility to use
    /// other ways to show messages to the user instead of the default <c>MessageBox</c>.
    /// </remarks>
    [Flags]
    public enum MessageButton
    {
        /// <summary>
        /// OK button.
        /// </summary>
        OK = 1,

        /// <summary>
        /// OK and Cancel buttons.
        /// </summary>
        OKCancel = 2,

        /// <summary>
        /// Yes and No buttons.
        /// </summary>
        YesNo = 4,

        /// <summary>
        /// Yes, No and Cancel buttons.
        /// </summary>
        YesNoCancel = 8
    }

    /// <summary>
    /// Available message images.
    /// </summary>
    /// <remarks>
    /// Although directly extracted from the WPF <c>MessageBoxImage</c>, this enum provides more flexibility to use
    /// other ways to show messages to the user instead of the default <c>MessageBox</c>.
    /// </remarks>
    public enum MessageImage
    {
        /// <summary>
        /// Show no image.
        /// </summary>
        None,

        /// <summary>
        /// Information image.
        /// </summary>
        Information,

        /// <summary>
        /// Question image.
        /// </summary>
        Question,

        /// <summary>
        /// Exclamation image.
        /// </summary>
        Exclamation,

        /// <summary>
        /// Error image.
        /// </summary>
        Error,

        /// <summary>
        /// Stop image.
        /// </summary>
        Stop,

        /// <summary>
        /// Warning image.
        /// </summary>
        Warning
    }

    /// <summary>
    /// Interface for the message service.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        Task<MessageResult> ShowErrorAsync(string message, string caption = "");

        /// <summary>
        /// Shows a warning message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        Task<MessageResult> ShowWarningAsync(string message, string caption = "");

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        Task<MessageResult> ShowInformationAsync(string message, string caption = "");

        /// <summary>
        /// Shows the specified message and returns the result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The <see cref="MessageResult"/>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None);
    }
}
