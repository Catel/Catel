// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN_FORMS

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Reflection;

    using Xamarin.Forms;

    public partial class MessageService
    {
        /// <summary>
        ///     Shows the message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The message result.</returns>
        /// <exception cref="ArgumentException">The <paramref name="message" /> is <c>null</c> or whitespace.</exception>
        protected virtual async Task<MessageResult> ShowMessageBoxAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            var messageResult = MessageResult.Cancel;

            var modalStack = Application.Current.MainPage.Navigation.ModalStack;
            var currentPage = modalStack[modalStack.Count - 1];
            if (currentPage is NavigationPage)
            {
                currentPage = (currentPage as NavigationPage).CurrentPage ?? currentPage;
            }

            if (currentPage != null)
            {
                var assembly = typeof (Application).GetAssemblyEx();
                var alertArgumentType = assembly.GetType("Xamarin.Forms.AlertArguments");
                var propertyInfo = alertArgumentType.GetPropertyEx("Result");
                var constructor = alertArgumentType.GetConstructorsEx()[0];

                var configurationResultMap = new Dictionary<MessageButton, Configuration>();
                configurationResultMap.Add(MessageButton.OK, new Configuration("OK", MessageResult.OK, null, MessageResult.None));
                configurationResultMap.Add(MessageButton.OKCancel, new Configuration("OK", MessageResult.OK, "Cancel", MessageResult.Cancel));
                configurationResultMap.Add(MessageButton.YesNo, new Configuration("Yes", MessageResult.Yes, "No", MessageResult.No));

                if (!configurationResultMap.ContainsKey(button))
                {
                    throw new ArgumentOutOfRangeException("button");
                }

                var configuration = configurationResultMap[button];
                var invoke = constructor.Invoke(new object[] {caption, message, configuration.PositiveButton, configuration.NegativeButton});
                var type = typeof(MessagingCenter);

                //// TODO: Use reflection API instead but reflection API requires some fixes.
                var methodInfo = type.GetRuntimeMethods().FirstOrDefault(info => info.Name == "Send" && info.GetGenericArguments().Length == 2 && info.GetParameters().Length == 3);
                var makeGenericMethod = methodInfo.MakeGenericMethod(typeof(Page), alertArgumentType);
                makeGenericMethod.Invoke(type, new[] {currentPage, "Xamarin.SendAlert", invoke});
                var value = (TaskCompletionSource<bool>) propertyInfo.GetValue(invoke);

                await value.Task;

                messageResult = value.Task.Result ? configuration.PositiveResult: configuration.NegativeResult;
            }

            return messageResult;
        }

        /// <summary>
        /// The configuration class.
        /// </summary>
        private class Configuration
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Configuration"/> class.
            /// </summary>
            /// <param name="positiveButton">The positive button text</param>
            /// <param name="positiveResult">The positive result</param>
            /// <param name="negativeButton">The negative button text</param>
            /// <param name="negativeResult">The negative result</param>
            public Configuration(string positiveButton, MessageResult positiveResult, string negativeButton, MessageResult negativeResult)
            {
                PositiveButton = positiveButton;
                PositiveResult = positiveResult;
                NegativeButton = negativeButton;
                NegativeResult = negativeResult;
            }

            /// <summary>
            /// Gets the positive button text.
            /// </summary>
            public string PositiveButton { get; }

            /// <summary>
            /// Gets the positive result.
            /// </summary>
            public MessageResult PositiveResult { get; }

            /// <summary>
            /// Gets the negative button text.
            /// </summary>
            public string NegativeButton { get; }

            /// <summary>
            /// Gets the negative result.
            /// </summary>
            public MessageResult NegativeResult { get; }
        }
    }
}

#endif