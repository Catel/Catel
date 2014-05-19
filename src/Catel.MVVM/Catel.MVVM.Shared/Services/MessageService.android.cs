// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Services
{
    using System;
    using global::Android.App;

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
        protected virtual MessageResult ShowMessageBox(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            var messageResult = MessageResult.Cancel;
            var context = Catel.Android.ContextHelper.CurrentContext;
            var builder = new AlertDialog.Builder(context);

            switch (button)
            {
                case MessageButton.OK:
                    builder.SetPositiveButton("OK", (sender, e) => { messageResult = MessageResult.OK; });
                    break;

                case MessageButton.OKCancel:
                    builder.SetPositiveButton("OK", (sender, e) => { messageResult = MessageResult.OK; });
                    builder.SetCancelable(true);
                    break;

                case MessageButton.YesNo:
                    builder.SetPositiveButton("Yes", (sender, e) => { messageResult = MessageResult.Yes; });
                    builder.SetNegativeButton("No", (sender, e) => { messageResult = MessageResult.No; });
                    break;

                case MessageButton.YesNoCancel:
                    builder.SetPositiveButton("Yes", (sender, e) => { messageResult = MessageResult.Yes; });
                    builder.SetNegativeButton("No", (sender, e) => { messageResult = MessageResult.No; });
                    builder.SetCancelable(true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("button");
            }

            builder.SetMessage(message).SetTitle(caption);

            builder.Show();

            return messageResult;
        }
    }
}

#endif