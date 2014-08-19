// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeqLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
        /// The seq server url.
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Provide an seq server api key.
        /// </summary>
        public string ApiKey { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Formats the log infos.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <param name="logEvent"></param>
        /// <param name="extraData"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        protected override string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            var messageResult = _jsonLogFormatter.FormatLogEvent(log, message, logEvent, time);

            return messageResult;
        }

        /// <summary>
        /// Writes the bacth log entries.
        /// </summary>
        /// <param name="batchEntries"></param>
        /// <exception cref="ArgumentNullException">The <paramref name="batchEntries"/> is <c>null</c>.</exception>
        protected override Task WriteBatch(List<LogBatchEntry> batchEntries)
        {
            Argument.IsNotNull("batchEntries", batchEntries);

            try
            {
                var textWriter = new StringWriter();
                textWriter.Write("{\"events\":[");

                var logEntries = batchEntries.Select(
                    batchEntry =>
                        FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData, DateTime.Now))
                    .Aggregate((log1, log2) => string.Format("{0},{1}", log1, log2));

                textWriter.Write(logEntries);

                textWriter.Write("]}");

                var message = textWriter.ToString();

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

                }

                if (!string.IsNullOrWhiteSpace(ApiKey))
                {
                    _webClient.Headers.Add(ApiKeyHeaderName, ApiKey);
                }

                return Task.Factory.StartNew(() => _webClient.UploadString(_webApiUrl, message));
            }
            catch (Exception)
            {
                // Swallow
            }

            return null;
        }

        #endregion
    }

}
#endif