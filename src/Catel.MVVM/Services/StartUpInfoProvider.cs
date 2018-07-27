// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartUpInfoProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Services
{
    /// <summary>
    /// 
    /// </summary>
    [ObsoleteEx(Message = "Use Orc.CommandLine instead since that has better support for parsing command lines", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
    public class StartUpInfoProvider : IStartUpInfoProvider
    {
        /// <summary>
        /// The command argument list.
        /// </summary>
        private string[] _arguments;
        #region IStartUpInfoProvider Members
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
        #endregion
    }
}

#endif
