// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Converters
{
    /// <summary>
    /// Interface for all value converters.
    /// </summary>
    public interface IValueConverter 
#if XAMARIN
        : Xamarin.Forms.IValueConverter
#elif NETFX_CORE
        : global::Windows.UI.Xaml.Data.IValueConverter
#else
        : System.Windows.Data.IValueConverter
#endif
    {
    }
}
