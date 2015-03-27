// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using Logging;

    /// <summary>
    /// The output log listener.
    /// </summary>
    [ObsoleteEx(Replacement = "Orc.Controls, see https://github.com/wildgums/orc.controls", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class OutputLogListener : LogListenerBase
    {
        /// <summary>
        /// The output log listener.
        /// </summary>
        public OutputLogListener()
        {
        }
    }
}

#endif