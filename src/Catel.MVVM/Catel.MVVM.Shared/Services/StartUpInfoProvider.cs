// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartUpInfoProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Services
{
#if SILVERLIGHT
    using System.Collections.Generic;

    using System.Windows;
#endif

    /// <summary>
    /// 
    /// </summary>
    public class StartUpInfoProvider : IStartUpInfoProvider
    {
#if NET
        /// <summary>
        /// The command argument list.
        /// </summary>
        private string[] _arguments;
#endif

        #region IStartUpInfoProvider Members
#if NET
        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        public string[] Arguments
        {
            get
            {
        	    if (_arguments == null)
        	    {
        	        var commandLine = System.Environment.GetCommandLineArgs();

                    _arguments = new string[commandLine.Length - 1];

                    if (_arguments.Length > 0)
        	        {
                        System.Array.Copy(commandLine, 1, _arguments, 0, _arguments.Length);
        	        }
        	    }
        	
                return _arguments;
            }
        }
#endif

#if SL5
        /// <summary>
        /// Gets the silverlight application initialization parameters.
        /// </summary>
        public IDictionary<string, string> InitParams
        {
            get { return Application.Current.Host.InitParams; }
        }
#endif
        #endregion
    }
}

#endif