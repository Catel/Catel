// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if SL5

namespace Catel.Logging
{
    using System;
    using System.Windows.Browser;
    using System.Windows.Threading;

    public partial class DebugLogListener
    {
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
                if (_console == null && HtmlPage.IsEnabled)
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

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            string consoleMessage = FormatLogEvent(log, message, logEvent, extraData, time);

            if (InitializeConsole())
            {
                _dispatcher.BeginInvoke(() => _console.InvokeSelf(consoleMessage));
            }

            System.Diagnostics.Debug.WriteLine(consoleMessage);
        }
    }
}

#endif