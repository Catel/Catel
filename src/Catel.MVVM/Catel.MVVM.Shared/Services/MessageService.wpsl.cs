// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.wpsl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class MessageService
    {
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

            return new Task<MessageResult>(() =>
            {
                var messageBoxButton = TranslateMessageButton(button);
                var result = MessageBox.Show(message, caption, messageBoxButton);

                return TranslateMessageBoxResult(result);
            });
        }
    }
}

#endif