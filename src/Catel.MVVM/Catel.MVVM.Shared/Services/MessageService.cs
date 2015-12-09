// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

#if ANDROID
    using Android.App;
#elif IOS

#elif NETFX_CORE
    using global::Windows.UI.Popups;
#else
    using System.Windows;
    using Windows;
#endif

    /// <summary>
    /// Message service that implements the <see cref="IMessageService"/>.
    /// </summary>
    public partial class MessageService : ViewModelServiceBase, IMessageService
    {
        private readonly IDispatcherService _dispatcherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService"/> is <c>null</c>.</exception>
        public MessageService(IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => dispatcherService);

            _dispatcherService = dispatcherService;
        }

        #region Methods
#if !XAMARIN
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
                throw new NotSupportedInPlatformException("MessageBox class does not support MessageButton '{0}'", button);
            }
        }
#endif
        #endregion

        #region IMessageService Members
        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "Async overload", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public virtual Task<MessageResult> ShowError(Exception exception)
        {
            return ShowErrorAsync(exception);
        }

        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public virtual Task<MessageResult> ShowErrorAsync(Exception exception)
        {
            Argument.IsNotNull("exception", exception);

            return ShowError(exception.Message, string.Empty);
        }

        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "Async overload", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public virtual Task<MessageResult> ShowError(string message, string caption = "")
        {
            return ShowErrorAsync(message, caption);
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
                caption = Catel.ResourceHelper.GetString("ErrorTitle");
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Error;

            return Show(message, caption, button, icon);
        }

        /// <summary>
        /// Shows a warning message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "Async overload", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public virtual Task<MessageResult> ShowWarning(string message, string caption = "")
        {
            return ShowWarningAsync(message, caption);
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
                caption = Catel.ResourceHelper.GetString("WarningTitle");
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Warning;

            return Show(message, caption, button, icon);
        }

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "Async overload", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public virtual Task<MessageResult> ShowInformation(string message, string caption = "")
        {
            return ShowInformationAsync(message, caption);
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
                caption = Catel.ResourceHelper.GetString("InfoTitle");
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Information;

            return Show(message, caption, button, icon);
        }

        /// <summary>
        /// Shows the specified message and returns the result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The <see cref="MessageResult"/>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "Async overload", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public virtual Task<MessageResult> Show(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
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
#endregion
    }
}