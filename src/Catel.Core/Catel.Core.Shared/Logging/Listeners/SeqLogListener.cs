// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeqLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET
namespace Catel.Logging
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///  Log listener which writes all data to a Seq server.
    /// </summary>
    public class SeqLogListener : BatchLogListenerBase
    {
        #region Constants
        private const string ApiKeyHeaderName = "X-Seq-ApiKey";
        private const string BulkUploadResource = "api/events/raw";
        #endregion

        #region Fields
        private readonly IJsonLogFormatter _jsonLogFormatter;
        private readonly object _lock = new object();

        private WebClient _webClient;
        private string _webApiUrl;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SeqLogListener"/> class.
        /// </summary>
        public SeqLogListener()
        {
            _jsonLogFormatter = new JsonLogFormatter();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the seq server url.
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the seq server api key.
        /// </summary>
        public string ApiKey { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Formats the log event to a message which can be written to a log persistence storage.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        /// <returns>The formatted log event.</returns>
        protected override string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            var messageResult = _jsonLogFormatter.FormatLogEvent(log, message, logEvent, time);

            return messageResult;
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        /// <returns>Task so this can be done asynchronously.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="batchEntries"/> is <c>null</c>.</exception>
        protected override async Task WriteBatchAsync(List<LogBatchEntry> batchEntries)
        {
            Argument.IsNotNull("batchEntries", batchEntries);

            try
            {
                var textWriter = new StringWriter();
                textWriter.Write("{\"events\":[");

                var logEntries = batchEntries.Select(
                    batchEntry => FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData, batchEntry.Data, FastDateTime.Now))
                    .Aggregate((log1, log2) => string.Format("{0},{1}", log1, log2));

                textWriter.Write(logEntries);
                textWriter.Write("]}");

                var message = textWriter.ToString();

                lock (_lock)
                {
                    if (_webClient == null)
                    {
                        var baseUri = ServerUrl;
                        if (!baseUri.EndsWith("/"))
                        {
                            baseUri += "/";
                        }

                        _webApiUrl = string.Format("{0}{1}", baseUri, BulkUploadResource);

                        _webClient = new WebClient {Encoding = Encoding.UTF8};
                        _webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

                        if (!string.IsNullOrWhiteSpace(ApiKey))
                        {
                            _webClient.Headers.Add(ApiKeyHeaderName, ApiKey);
                        }
                    }
                }

                _webClient.UploadString(_webApiUrl, message);
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        #endregion
    }

}
#endif