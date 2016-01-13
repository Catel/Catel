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
    using System.Threading.Tasks;

    using global::Xamarin.Forms;
    using IoC;

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
            var dependencyResolver = this.GetDependencyResolver();

            var languageService = dependencyResolver.Resolve<ILanguageService>();

            _configurationResultMap.Add(MessageButton.OK, new Configuration(languageService.GetString("OK"), MessageResult.OK, null, MessageResult.None));
            _configurationResultMap.Add(MessageButton.OKCancel, new Configuration(languageService.GetString("OK"), MessageResult.OK,
                languageService.GetString("Cancel"), MessageResult.Cancel));
            _configurationResultMap.Add(MessageButton.YesNo, new Configuration(languageService.GetString("Yes"), MessageResult.Yes,
                languageService.GetString("No"), MessageResult.No));
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
            if (!_configurationResultMap.ContainsKey(button))
            {
                throw new ArgumentOutOfRangeException("button");
            }

            var messageResult = MessageResult.None;

            var currentPage = Application.Current.CurrentPage();
            if (currentPage != null)
            {
                var configuration = _configurationResultMap[button];
                var result = Xamarin.Forms.MessagingCenter.SendAlert(currentPage, caption, message, configuration.PositiveButton, configuration.NegativeButton);
                await result.Task;
                messageResult = result.Task.Result ? configuration.PositiveResult: configuration.NegativeResult;
            }

            return messageResult;
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