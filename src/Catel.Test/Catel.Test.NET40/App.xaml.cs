// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRT.Test
{
#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : UserControl
    {
        #region Constructors
        public App()
        {
            InitializeComponent();
        }
        #endregion
    }
}