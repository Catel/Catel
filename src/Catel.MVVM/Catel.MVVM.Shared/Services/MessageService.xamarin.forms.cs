// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Catel.Extensions;
using Catel.Reflection;
using Xamarin.Forms;

#if XAMARIN_FORMS

namespace Catel.Services
{
    public partial class MessageService
    {
        /// <summary>
        /// Configuration and Result Map.
        /// </summary>
        private readonly Dictionary<MessageButton, Configuration> _configurationResultMap = new Dictionary<MessageButton, Configuration>();

        /// <summary>
        /// Called at the end of constructors.
        /// </summary>
        partial void Initialize()
        {
            _configurationResultMap.Add(MessageButton.OK, new Configuration("OK", MessageResult.OK, null, MessageResult.None));
            _configurationResultMap.Add(MessageButton.OKCancel, new Configuration("OK", MessageResult.OK, "Cancel", MessageResult.Cancel));
            _configurationResultMap.Add(MessageButton.YesNo, new Configuration("Yes", MessageResult.Yes, "No", MessageResult.No));
        }

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

            var currentPage = Application.Current.CurrentPage();
            if (currentPage != null)
            {
                if (!_configurationResultMap.ContainsKey(button))
                {
                    throw new ArgumentOutOfRangeException("button");
                }

                var configuration = _configurationResultMap[button];

                var argument = new AlertArgumentProxy(caption, message, configuration.PositiveButton, configuration.NegativeButton);
                MessagingCenterHelper.Send(typeof(Page), currentPage, "Xamarin.SendAlert", argument.InternalType, argument.Object);

                await argument.Result.Task;

                messageResult = argument.Result.Task.Result ? configuration.PositiveResult: configuration.NegativeResult;
            }

            return messageResult;
        }

        /// <summary>
        /// The alert argument proxy.
        /// </summary>
        private sealed class AlertArgumentProxy
        {
            private readonly PropertyInfo _propertyInfo;

            /// <summary>
            ///     Initializes a new instance of the <see cref="AlertArgumentProxy" /> class.
            /// </summary>
            /// <param name="caption">The caption</param>
            /// <param name="message">The message</param>
            /// <param name="positiveButton">The positive button text</param>
            /// <param name="negativeButton">The negative button text</param>
            public AlertArgumentProxy(string caption, string message, string positiveButton, string negativeButton)
            {
                var assembly = typeof (Application).GetAssemblyEx();
                InternalType = assembly.GetType("Xamarin.Forms.AlertArguments");

                _propertyInfo = InternalType.GetPropertyEx("Result");
                var constructor = InternalType.GetConstructorsEx()[0];
                Object = constructor.Invoke(new object[] {caption, message, positiveButton, negativeButton});
            }

            /// <summary>
            /// 
            /// </summary>
            public object Object { get; }

            /// <summary>
            /// 
            /// </summary>
            public Type InternalType { get; }

            /// <summary>
            /// 
            /// </summary>
            public TaskCompletionSource<bool> Result => (TaskCompletionSource<bool>) _propertyInfo.GetValue(Object);
        }

        /// <summary>
        ///     The configuration class.
        /// </summary>
        private sealed class Configuration
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="Configuration" /> class.
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
            ///     Gets the positive button text.
            /// </summary>
            public string PositiveButton { get; }

            /// <summary>
            ///     Gets the positive result.
            /// </summary>
            public MessageResult PositiveResult { get; }

            /// <summary>
            ///     Gets the negative button text.
            /// </summary>
            public string NegativeButton { get; }

            /// <summary>
            ///     Gets the negative result.
            /// </summary>
            public MessageResult NegativeResult { get; }
        }
    }
}

#endif