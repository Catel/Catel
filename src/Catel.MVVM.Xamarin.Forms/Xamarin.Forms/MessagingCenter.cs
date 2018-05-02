// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCenter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Xamarin.Forms
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::Xamarin.Forms;
    using global::Xamarin.Forms.Internals;


    /// <summary>
    ///     The Messaging Center Helper
    /// </summary>
    internal static class MessagingCenter
    {
        /// <summary>
        /// Sends the specified type of sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="typeOfSender">The type of sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="alertArguments">The arguments proxy.</param>
        /// <returns>System.Threading.Tasks.TaskCompletionSource&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeOfSender"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="sender"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="alertArguments"/> is <c>null</c>.</exception>
        private static TaskCompletionSource<bool> Send(object sender, Type typeOfSender, string message, AlertArguments alertArguments)
        {
            Argument.IsNotNull(() => sender);
            Argument.IsNotNull(() => typeOfSender);
            Argument.IsOfType(() => sender, typeOfSender);
            Argument.IsNotNull(() => alertArguments);

            var type = typeof(global::Xamarin.Forms.MessagingCenter);
            //// TODO: Use reflection API instead but reflection API requires some fixes.
            var methodInfo = type.GetRuntimeMethods().FirstOrDefault(info => info.Name == "Send" && info.GetGenericArguments().Length == 2 && info.GetParameters().Length == 3);
            var makeGenericMethod = methodInfo.MakeGenericMethod(typeof(Page), typeof(AlertArguments));
            makeGenericMethod.Invoke(type, new[] { sender, message, alertArguments });

            return alertArguments.Result;
        }

        /// <summary>
        /// Sends the alert.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Threading.Tasks.TaskCompletionSource&lt;System.Boolean&gt;.</returns>
        private static void SendAlertMessage(Page sender, AlertArguments arguments)
        {
            Send(sender, typeof(Page), Messages.SendAlert, arguments);
        }

        /// <summary>
        /// Sends the alert.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="message">The message.</param>
        /// <param name="positiveButton">The positive button.</param>
        /// <param name="negativeButton">The negative button.</param>
        /// <returns>System.Threading.Tasks.TaskCompletionSource&lt;System.Boolean&gt;.</returns>
        public static async Task<bool> SendAlertAsync(Page sender, string caption, string message, string positiveButton, string negativeButton)
        {
            var argument = new AlertArguments(caption, message, positiveButton, negativeButton);
            SendAlertMessage(sender, argument);
            return await argument.Result.Task;
        }

        /// <summary>
        /// The messages.
        /// </summary>
        private static class Messages
        {
            /// <summary>
            /// The send alert message.
            /// </summary>
            public const string SendAlert = "Xamarin.SendAlert";
        }
    }
}