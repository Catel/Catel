namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

    public static class IMessageServiceExtensions
    {
        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="messageService">The message service.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static Task<MessageResult> ShowErrorAsync(this IMessageService messageService, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(messageService);
            ArgumentNullException.ThrowIfNull(exception);

            return messageService.ShowErrorAsync(exception.Message, string.Empty);
        }
    }
}
