namespace Catel.MVVM.CSLA
{
    using System;

    /// <summary>
    /// View model for CSLA view models.
    /// </summary>
    [CLSCompliant(false)]
    public interface IViewModel : Catel.MVVM.IViewModel, Csla.Xaml.IViewModel
    {
    }
}