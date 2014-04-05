// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

#if ANDROID
    using Android.App;
#elif IOS

#elif NETFX_CORE
    using global::Windows.UI.Popups;
    using global::System.Threading.Tasks;
#else
    using System.Windows;
    using Windows;
#endif

    /// <summary>
    /// Message service that implements the <see cref="IMessageService"/>.
    /// </summary>
    public partial class MessageService : ViewModelServiceBase, IMessageService
    {
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
        /// <param name="completedCallback">The callback to invoke when the message is completed. Can be <c>null</c>.</param>
        /// <remarks>
        /// There is no garantuee that the method will be executed asynchronous, only that the <paramref name="completedCallback"/>
        /// will be invoked when the message is dismissed.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public virtual void ShowError(Exception exception, Action completedCallback)
        {
            Argument.IsNotNull("exception", exception);

            ShowError(exception.Message, string.Empty, completedCallback);
        }

        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="completedCallback">The callback to invoke when the message is completed. Can be <c>null</c>.</param>
        /// <remarks>
        /// There is no garantuee that the method will be executed asynchronous, only that the <paramref name="completedCallback"/>
        /// will be invoked when the message is dismissed.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual void ShowError(string message, string caption = "", Action completedCallback = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Catel.ResourceHelper.GetString("ErrorTitle");
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Error;

            if (completedCallback != null)
            {
                ShowAsync(message, caption, button, icon, result => completedCallback());
            }
            else
            {
                Show(message, caption, button, icon);
            }
        }

        /// <summary>
        /// Shows a warning message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="completedCallback">The callback to invoke when the message is completed. Can be <c>null</c>.</param>
        /// <remarks>
        /// There is no garantuee that the method will be executed asynchronous, only that the <paramref name="completedCallback"/>
        /// will be invoked when the message is dismissed.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual void ShowWarning(string message, string caption = "", Action completedCallback = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Catel.ResourceHelper.GetString("WarningTitle");
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Warning;

            if (completedCallback != null)
            {
                ShowAsync(message, caption, button, icon, result => completedCallback());
            }
            else
            {
                Show(message, caption, button, icon);
            }
        }

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="completedCallback">The callback to invoke when the message is completed. Can be <c>null</c>.</param>
        /// <remarks>
        /// There is no garantuee that the method will be executed asynchronous, only that the <paramref name="completedCallback"/>
        /// will be invoked when the message is dismissed.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual void ShowInformation(string message, string caption = "", Action completedCallback = null)
        {
            if (string.IsNullOrEmpty(caption))
            {
                caption = Catel.ResourceHelper.GetString("InfoTitle");
            }

            const MessageButton button = MessageButton.OK;
            const MessageImage icon = MessageImage.Information;

            if (completedCallback != null)
            {
                ShowAsync(message, caption, button, icon, result => completedCallback());
            }
            else
            {
                Show(message, caption, button, icon);
            }
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
        public virtual MessageResult Show(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
#if NETFX_CORE
            var task = ShowMessageBox(message, caption, button, icon);
            task.Wait();
            return task.Result;
#else
            return ShowMessageBox(message, caption, button, icon);
#endif
        }

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="completedCallback">The callback to invoke when the message is completed. Can be <c>null</c>.</param>
        /// <remarks>
        /// There is no garantuee that the method will be executed asynchronous, only that the <paramref name="completedCallback"/>
        /// will be invoked when the message is dismissed.
        /// </remarks>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        public virtual void ShowAsync(string message, string caption = "", MessageButton button = MessageButton.OK,
            MessageImage icon = MessageImage.None, Action<MessageResult> completedCallback = null)
        {
            var result = Show(message, caption, button, icon);
            if (completedCallback != null)
            {
                completedCallback(result);
            }
        }
        #endregion
    }
}