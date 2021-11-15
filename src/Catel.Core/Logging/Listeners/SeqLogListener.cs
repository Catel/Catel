// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeqLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

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
    public class SeqLogListener : BatchLogListenerBase, IDisposable
    {
#region Constants
        private const string ApiKeyHeaderName = "X-Seq-ApiKey";
        private const string BulkUploadResource = "api/events/raw";
#endregion

#region Fields
        private readonly IJsonLogFormatter _jsonLogFormatter;

        private WebClient _webClient;

        private readonly object _syncObj = new object();
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
                await textWriter.WriteAsync("{\"events\":[");

                var logEntries = batchEntries.Select(batchEntry => FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData, batchEntry.Data, FastDateTime.Now)).Aggregate((log1, log2) => string.Format("{0},{1}", log1, log2));

                await textWriter.WriteAsync(logEntries);
                await textWriter.WriteAsync("]}");

                var message = textWriter.ToString();

                InitializeWebClient();

                _webClient.UploadStringAsync(new Uri(WebApiUrl), message);
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        private string WebApiUrl
        {
            get
            {
                var baseUri = ServerUrl;
                if (string.IsNullOrWhiteSpace(baseUri))
                {
                    return string.Empty;
                }

                if (!baseUri.EndsWith("/"))
                {
                    baseUri += "/";
                }

                return string.Format("{0}{1}", baseUri, BulkUploadResource);
            }
        }

        private void InitializeWebClient()
        {
            lock (_syncObj)
            {
                if (_webClient is null)
                {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    _webClient = new WebClient 
                    { 
                        Encoding = Encoding.UTF8 
                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                    _webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

                    if (!string.IsNullOrWhiteSpace(ApiKey))
                    {
                        _webClient.Headers.Add(ApiKeyHeaderName, ApiKey);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _webClient?.Dispose();
        }

        #endregion

    }

}
#endif
