// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.CSLA
{
    using System;

    /// <summary>
    /// View model for CSLA view models.
    /// </summary>
    [CLSCompliant(false)]
    [ObsoleteEx(Message = "We are considering to remove CSLA support. See https://catelproject.atlassian.net/browse/CTL-671", 
        TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public interface IViewModel : Catel.MVVM.IViewModel, Csla.Xaml.IViewModel
    {
    }
}