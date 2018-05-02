// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.winrt.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using global::Windows.UI.Popups;

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
        protected virtual async Task<MessageResult> ShowMessageBoxAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            // TODO: Add translations for system

            var result = MessageBoxResult.None;
            var messageBoxButton = TranslateMessageButton(button);
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

            return TranslateMessageBoxResult(result);
        }
    }
}

#endif