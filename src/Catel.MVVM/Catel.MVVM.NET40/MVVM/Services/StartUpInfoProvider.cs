// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartUpInfoProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
#if NET
    using System.Diagnostics;

    using Catel.Windows;

#endif
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
            get { return _arguments ?? (_arguments = CommandLineHelper.Parse(Process.GetCurrentProcess().StartInfo.Arguments)); }
        }
#endif

#if SL4 || SL5
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