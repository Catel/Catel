// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartUpInfoProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System.Collections.Generic;
#if NET
    using System.Diagnostics;
#endif
#if SILVERLIGHT
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
        private List<string> _argumentList;
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
                if (_argumentList == null)
                {
                    _argumentList = new List<string>();

                    var arguments = Process.GetCurrentProcess().StartInfo.Arguments;

                    string current = string.Empty;
                    for (int i = 0; i < Process.GetCurrentProcess().StartInfo.Arguments.Length; i++)
                    {
                        if (arguments[i] == '\"')
                        {
                            int j = i + 1;
                            while (j < arguments.Length && arguments[j] != '\"')
                            {
                                current += arguments[j++];
                            }

                            i = j - 1;
                        }
                        else if (arguments[i] == ' ')
                        {
                            if (current != string.Empty)
                            {
                                _argumentList.Add(current);
                                current = string.Empty;
                            }
                        }
                        else
                        {
                            current += arguments[i];
                        }
                    }

                    if (current != string.Empty)
                    {
                        _argumentList.Add(current);
                    }
                }

                return _argumentList.ToArray();
            }
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Gets the silverlight application initialization parameters.
        /// </summary>
        public IDictionary<string, string> InitParms
        {
            get { return Application.Current.Host.InitParams; }
        }
#endif
        #endregion
    }
}