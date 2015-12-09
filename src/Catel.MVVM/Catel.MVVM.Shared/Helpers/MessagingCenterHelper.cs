#if XAMARIN_FORMS

namespace Catel
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Catel.Reflection;
    using Xamarin.Forms;

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

    /// <summary>
    ///     The Messaging Center Helper
    /// </summary>
    public static class MessagingCenterHelper
    {
        public static void Send(Type typeOfSender, object sender, string message, IArgumentsProxy argumentsProxy)
        {
            Argument.IsOfType(() => sender, typeOfSender);

            var type = typeof (MessagingCenter);
            //// TODO: Use reflection API instead but reflection API requires some fixes.
            var methodInfo =
                type.GetRuntimeMethods()
                    .FirstOrDefault(
                        info =>
                            info.Name == "Send" && info.GetGenericArguments().Length == 2 &&
                            info.GetParameters().Length == 3);
            var makeGenericMethod = methodInfo.MakeGenericMethod(typeof (Page), argumentsProxy.InternalType);
            makeGenericMethod.Invoke(type, new[] {sender, message, argumentsProxy.Object});
        }

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
            return new AlertArgumentsesProxy(caption, message, positiveButton, negativeButton);
        }

        /// <summary>
        ///     The alert arguments proxy.
        /// </summary>
        private sealed class AlertArgumentsesProxy : IArgumentsProxy
        {
            private readonly PropertyInfo _propertyInfo;

            /// <summary>
            ///     Initializes a new instance of the <see cref="AlertArgumentsesProxy" /> class.
            /// </summary>
            /// <param name="caption">The caption</param>
            /// <param name="message">The message</param>
            /// <param name="positiveButton">The positive button text</param>
            /// <param name="negativeButton">The negative button text</param>
            public AlertArgumentsesProxy(string caption, string message, string positiveButton, string negativeButton)
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

        public static class Messages
        {
            public const string SendAlert = "Xamarin.SendAlert";
        }

        public static void SendAlert(Page sender, IArgumentsProxy arguments)
        {
            Send(typeof(Page), sender, Messages.SendAlert, arguments);
        }
    }
}

#endif