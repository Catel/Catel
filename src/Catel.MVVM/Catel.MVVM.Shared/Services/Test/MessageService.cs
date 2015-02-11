﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Test implementation of the <see cref="IMessageService"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.MessageService service = (Test.MessageService)GetService<IMessageService>();
    /// 
    /// // Queue the next expected result
    /// service.ExpectedResults.Add(MessageResult.Yes);
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class MessageService : IMessageService
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        public MessageService()
        {
            ExpectedResults = new Queue<MessageResult>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the queue of expected results.
        /// </summary>
        /// <value>The expected results.</value>
        public Queue<MessageResult> ExpectedResults { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public async Task<MessageResult> ShowError(Exception exception)
        {
            return MessageResult.None;
        }

        /// <summary>
        /// Shows an error message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="caption"/> is <c>null</c> or whitespace.</exception>
        public async Task<MessageResult> ShowError(string message, string caption = "")
        {
            return MessageResult.None;
        }

        /// <summary>
        /// Shows a warning message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="caption"/> is <c>null</c> or whitespace.</exception>
        public async Task<MessageResult> ShowWarning(string message, string caption = "")
        {
            return MessageResult.None;
        }

        /// <summary>
        /// Shows an information message to the user and allows a callback operation when the message is completed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="caption"/> is <c>null</c> or whitespace.</exception>
        public async Task<MessageResult> ShowInformation(string message, string caption = "")
        {
            return MessageResult.None;
        }

        /// <summary>
        /// Shows the specified message and returns the result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>The <see cref="MessageResult"/>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="message"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="caption"/> is <c>null</c> or whitespace.</exception>
        public async Task<MessageResult> Show(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            if (ExpectedResults.Count == 0)
            {
                throw new Exception(ResourceHelper.GetString("NoExpectedResultsInQueueForUnitTest"));
            }

            return ExpectedResults.Dequeue();
        }
        #endregion
    }
}
