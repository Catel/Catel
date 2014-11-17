// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunWorkerCompletedEventHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE || PCL

namespace System.ComponentModel
{
    /// <summary>
    /// RunWorkerCompleted method definition.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);
}

#endif