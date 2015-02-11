// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.wpf.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NET

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Windows;

    public partial class MessageService
    {
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

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The message result.</returns>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        protected virtual Task<MessageResult> ShowMessageBox(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            Argument.IsNotNullOrWhitespace("message", message);

            var tcs = new TaskCompletionSource<MessageResult>();

            _dispatcherService.BeginInvoke(() =>
            {
                MessageBoxResult result;
                var messageBoxButton = TranslateMessageButton(button);
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

                tcs.SetResult(TranslateMessageBoxResult(result));
            });

            return tcs.Task;
        }
    }
}

#endif