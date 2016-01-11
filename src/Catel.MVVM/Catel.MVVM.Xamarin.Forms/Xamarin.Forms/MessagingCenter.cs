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
    using Reflection;

    /// <summary>
    ///     The arguments proxy interface.
    /// </summary>
    public interface IArgumentsProxy
    {
        /// <summary>
        ///     The object.
        /// </summary>
        object Object { get; }

        /// <summary>
        ///     The internal type.
        /// </summary>
        Type InternalType { get; }

        /// <summary>
        ///     The result.
        /// </summary>
        TaskCompletionSource<bool> Result { get; }
    }

    public static class ArgumentsProxyFactory
    {
        /// <summary>
        ///     Creates a proxy for <see cref="Xamarin.Forms.AlertArgument" />.
        /// </summary>
        /// <param name="caption">The caption</param>
        /// <param name="message">The message</param>
        /// <param name="positiveButton">The positive button text</param>
        /// <param name="negativeButton">The negative button text</param>
        /// <returns>The proxy instance</returns>
        public static IArgumentsProxy CreateAlertArgument(string caption, string message, string positiveButton,
            string negativeButton)
        {
            return new AlertArgumentsProxy(caption, message, positiveButton, negativeButton);
        }

        /// <summary>
        ///     The alert arguments proxy.
        /// </summary>
        private sealed class AlertArgumentsProxy : IArgumentsProxy
        {
            /// <summary>
            /// The property info
            /// </summary>
            private readonly PropertyInfo _propertyInfo;

            /// <summary>
            ///     Initializes a new instance of the <see cref="AlertArgumentsProxy" /> class.
            /// </summary>
            /// <param name="caption">The caption</param>
            /// <param name="message">The message</param>
            /// <param name="positiveButton">The positive button text</param>
            /// <param name="negativeButton">The negative button text</param>
            public AlertArgumentsProxy(string caption, string message, string positiveButton, string negativeButton)
            {
                var assembly = typeof (Application).GetAssemblyEx();
                InternalType = assembly.GetType("Xamarin.Forms.AlertArguments");

                _propertyInfo = InternalType.GetPropertyEx("Result");
                var constructor = InternalType.GetConstructorsEx()[0];
                Object = constructor.Invoke(new object[] {caption, message, positiveButton, negativeButton});
            }

            /// <summary>
            ///     The task completion source.
            /// </summary>
            public TaskCompletionSource<bool> Result => (TaskCompletionSource<bool>) _propertyInfo.GetValue(Object);

            /// <summary>
            ///     The object.
            /// </summary>
            public object Object { get; }

            /// <summary>
            ///     The internal type.
            /// </summary>
            public Type InternalType { get; }
        }
    }

    /// <summary>
    ///     The Messaging Center Helper
    /// </summary>
    public class MessagingCenter
    {
        private static MessagingCenter _current;

        private MessagingCenter()
        {
        }

        public static MessagingCenter Current
        {
            get { return _current ?? (_current = new MessagingCenter()); }
        }

        /// <summary>
        /// Sends the specified type of sender.
        /// </summary>
        /// <param name="typeOfSender">The type of sender.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="argumentsProxy">The arguments proxy.</param>
        /// <returns>System.Threading.Tasks.TaskCompletionSource&lt;System.Boolean&gt;.</returns>
        private static TaskCompletionSource<bool> Send(Type typeOfSender, object sender, string message, IArgumentsProxy argumentsProxy)
        {
            Argument.IsOfType(() => sender, typeOfSender);

            var type = typeof(global::Xamarin.Forms.MessagingCenter);
            //// TODO: Use reflection API instead but reflection API requires some fixes.
            var methodInfo = type.GetRuntimeMethods().FirstOrDefault(info => info.Name == "Send" && info.GetGenericArguments().Length == 2 && info.GetParameters().Length == 3);
            var makeGenericMethod = methodInfo.MakeGenericMethod(typeof(Page), argumentsProxy.InternalType);
            makeGenericMethod.Invoke(type, new[] { sender, message, argumentsProxy.Object });

            return argumentsProxy.Result;
        }

        /// <summary>
        /// Sends the alert.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Threading.Tasks.TaskCompletionSource&lt;System.Boolean&gt;.</returns>
        private static TaskCompletionSource<bool> SendAlert(Page sender, IArgumentsProxy arguments)
        {
            return Send(typeof(Page), sender, Messages.SendAlert, arguments);
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
        public TaskCompletionSource<bool> SendAlert(Page sender, string caption, string message, string positiveButton, string negativeButton)
        {
            var argument = ArgumentsProxyFactory.CreateAlertArgument(caption, message, positiveButton, negativeButton);
            SendAlert(sender, argument);
            return argument.Result;
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