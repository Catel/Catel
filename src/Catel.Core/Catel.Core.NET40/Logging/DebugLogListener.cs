// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
    using System.Windows.Threading;
#endif

    /// <summary>
    ///   Implementation of the <see cref = "ILogListener" /> that writes code to the debug or output window.
    /// </summary>
    public class DebugLogListener : LogListenerBase
    {
#if SL4 || SL5
        private ScriptObject _console = null;
        private readonly Dispatcher _dispatcher = System.Windows.Deployment.Current.Dispatcher;

        /// <summary>
        /// Initializes the console for silverlight applications.
        /// </summary>
        /// <returns><c>true</c> if the console is available; otherwise <c>false</c>.</returns>
        private bool InitializeConsole()
        {
            try
            {
                if (_console == null)
                {
                    if (!_dispatcher.CheckAccess())
                    {
                        return false;
                    }
                    
                    var window = HtmlPage.Window;
                    var isConsoleAvailable = (bool)window.Eval("typeof(console) != 'undefined' && typeof(console.log) != 'undefined'");
                    if (isConsoleAvailable)
                    {
                        var createLogFunction = (bool)window.Eval("typeof(catellog) === 'undefined'");
                        if (createLogFunction)
                        {
                            // Load the logging function into global scope:
                            const string logFunction = @"function catellog(msg) { console.log(msg); }";
                            string code = string.Format(@"if(window.execScript) {{ window.execScript('{0}'); }} else {{ eval.call(null, '{0}'); }}", logFunction);
                            window.Eval(code);
                        }

                        _console = window.Eval("catellog") as ScriptObject;
                    }                        
                }
            }
            catch (Exception)
            {
            }

            return (_console != null);
        }
#endif

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            string consoleMessage = FormatLogEvent(log, message, logEvent, extraData);

#if NETFX_CORE
            System.Diagnostics.Debug.WriteLine(consoleMessage);
#elif WINDOWS_PHONE
            System.Diagnostics.Debug.WriteLine(consoleMessage);
#elif SL4 || SL5
            if (InitializeConsole())
            {
                _dispatcher.BeginInvoke(() => _console.InvokeSelf(consoleMessage));
            }

            System.Diagnostics.Debug.WriteLine(consoleMessage);
#elif NET
            switch (logEvent)
            {
                case LogEvent.Debug:
                    System.Diagnostics.Trace.WriteLine(consoleMessage);
                    break;

                case LogEvent.Info:
                    System.Diagnostics.Trace.TraceInformation(consoleMessage);
                    break;

                case LogEvent.Warning:
                    System.Diagnostics.Trace.TraceWarning(consoleMessage);
                    break;

                case LogEvent.Error:
                    System.Diagnostics.Trace.TraceError(consoleMessage);
                    break;
            }
#endif
        }
    }
}