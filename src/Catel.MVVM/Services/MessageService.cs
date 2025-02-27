﻿namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Message service that implements the <see cref="IMessageService"/>.
    /// </summary>
    public partial class MessageService : ViewModelServiceBase, IMessageService
    {
        private readonly IDispatcherService _dispatcherService;
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="languageService">The language service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="languageService"/> is <c>null</c>.</exception>
        public MessageService(IDispatcherService dispatcherService, ILanguageService languageService)
        {
            ArgumentNullException.ThrowIfNull(dispatcherService);
            ArgumentNullException.ThrowIfNull(languageService);

            _dispatcherService = dispatcherService;
            _languageService = languageService;

            Initialize();
        }

        partial void Initialize();

        /// <summary>
        /// Translates the message box result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// Corresponding <see cref="MessageResult"/>.
        /// </returns>
        protected static MessageResult TranslateMessageBoxResult(MessageBoxResult result)
        {
            return Enum<MessageResult>.ConvertFromOtherEnumValue(result);
        }

        /// <summary>
        /// Translates the message button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>
        /// Corresponding <see cref="MessageBoxButton"/>.
        /// </returns>
        protected static MessageBoxButton TranslateMessageButton(MessageButton button)
        {
            try
            {
                return Enum<MessageBoxButton>.ConvertFromOtherEnumValue(button);
            }
            catch (Exception)
            {
                throw new NotSupportedInPlatformException($"MessageBox class does not support MessageButton '{Enum<MessageButton>.ToString(button)}'");
            }
        }

        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual Task<MessageResult> ShowErrorAsync(string message, string caption = "")
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = _languageService.GetString("ErrorTitle") ?? string.Empty;
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Error;

            return ShowAsync(message, caption, button, icon);
        }

        /// <summary>
        /// Shows a warning message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual Task<MessageResult> ShowWarningAsync(string message, string caption = "")
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = _languageService.GetString("WarningTitle") ?? string.Empty;
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Warning;

            return ShowAsync(message, caption, button, icon);
        }

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual Task<MessageResult> ShowInformationAsync(string message, string caption = "")
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = _languageService.GetString("InfoTitle") ?? string.Empty;
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Information;

            return ShowAsync(message, caption, button, icon);
        }

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual Task<MessageResult> ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK,
          MessageImage icon = MessageImage.None)
        {
            return ShowMessageBoxAsync(message, caption, button, icon);
        }
    }
}
