// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunWorkerCompletedEventHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    /// <summary>
    /// Represents the method that will handle the <see cref="BackgroundWorker.RunWorkerCompleted" /> event of a <see cref="BackgroundWorker" /> class.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="RunWorkerCompletedEventArgs" /> that contains the event data.</param>
    public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);
}