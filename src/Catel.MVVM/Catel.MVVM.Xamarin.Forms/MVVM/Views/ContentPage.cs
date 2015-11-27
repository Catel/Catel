// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views  
{
    using System;
    using MVVM;

    /// <summary>
    /// 
    /// </summary>
    public class ContentPage : global::Xamarin.Forms.ContentPage, IView
    {
        public IViewModel ViewModel { get; }

        public event EventHandler<EventArgs> ViewModelChanged;

        public object DataContext { get; set; }

        public object Tag { get; set; }

        public event EventHandler<EventArgs> Loaded;

        public event EventHandler<EventArgs> Unloaded;

        public event EventHandler<DataContextChangedEventArgs> DataContextChanged;
    }
}