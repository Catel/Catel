// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoWorkEventHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    /// <summary>
    /// Represents the method that will handle the <see cref="BackgroundWorker.DoWork" /> event of a <see cref="BackgroundWorker" /> class.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="DoWorkEventArgs" /> that contains the event data.</param>
    public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs e);
}