// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.Windows;
    using Windows;

#if NETFX_CORE
    using global::Windows.UI.Popups;
    using global::System.Threading.Tasks;
#endif

    /// <summary>
	/// Message service that implements the <see cref="IMessageService"/> by using the <see cref="MessageBox"/> class.
	/// </summary>
	public class MessageService : ViewModelServiceBase, IMessageService
	{
		#region Methods
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

#if NET
		/// <summary>
		/// Translates the message image.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <returns>
		/// Corresponding <see cref="MessageBoxImage"/>.
		/// </returns>
		protected static MessageBoxImage TranslateMessageImage(MessageImage image)
		{
            return Enum<MessageBoxImage>.ConvertFromOtherEnumValue(image);
		}
#endif

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

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The message result.</returns>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
#if NETFX_CORE
        protected async virtual Task<MessageResult> ShowMessageBox(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
#else
        protected virtual MessageResult ShowMessageBox(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
#endif
        {
            Argument.IsNotNullOrWhitespace("message", message);

            var result = MessageBoxResult.None;
            var messageBoxButton = TranslateMessageButton(button);

#if NET
            var messageBoxImage = TranslateMessageImage(icon);

            var activeWindow = Application.Current.GetActiveWindow();
            if (activeWindow != null)
            {
                result = MessageBox.Show(activeWindow, message, caption, messageBoxButton, messageBoxImage);
            }
            else
            {
                result = MessageBox.Show(message, caption, messageBoxButton, messageBoxImage);
            }
#elif NETFX_CORE
            // TODO: Add translations for system

            var messageDialog = new MessageDialog(message, caption);

            if (Enum<MessageButton>.Flags.IsFlagSet(button, MessageButton.OK) || 
                Enum<MessageButton>.Flags.IsFlagSet(button, MessageButton.OKCancel))
            {
                messageDialog.Commands.Add(new UICommand("OK", cmd => result = MessageBoxResult.OK));
            }

            if (Enum<MessageButton>.Flags.IsFlagSet(button, MessageButton.YesNo) ||
                Enum<MessageButton>.Flags.IsFlagSet(button, MessageButton.YesNoCancel))
            {
                messageDialog.Commands.Add(new UICommand("Yes", cmd => result = MessageBoxResult.Yes));
                messageDialog.Commands.Add(new UICommand("No", cmd => result = MessageBoxResult.No));
            }

            if (Enum<MessageButton>.Flags.IsFlagSet(button, MessageButton.OKCancel) ||
                Enum<MessageButton>.Flags.IsFlagSet(button, MessageButton.YesNoCancel))
            {
                messageDialog.Commands.Add(new UICommand("Cancel", cmd => result = MessageBoxResult.Cancel));
                messageDialog.CancelCommandIndex = (uint)messageDialog.Commands.Count - 1;
            }

            await messageDialog.ShowAsync();
#else
            result = MessageBox.Show(message, caption, messageBoxButton);
#endif

            return TranslateMessageBoxResult(result);
        }
        #endregion
    }
}