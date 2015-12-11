// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCenter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Xamarin.Forms
{
    using System;
    using Catel.IoC;
    using Catel.MVVM;
    using Forms;
    using Page = global::Xamarin.Forms.Page;
    using Application = global::Xamarin.Forms.Application;

    public class Application<TViewModel> : Application 
        where TViewModel : IViewModel
    {
        protected Application()
        {
            var serviceLocator = ServiceLocator.Default;

            // TODO: ModuleInit should work instead this.
            var coreModule = new CoreModule();
            coreModule.Initialize(serviceLocator);

            var mvvmModule = new MVVMModule();
            mvvmModule.Initialize(serviceLocator);

            // TODO: Improve this approach.
            var viewLocator = serviceLocator.ResolveType<IViewLocator>();
            var viewModelFactory = serviceLocator.ResolveType<IViewModelFactory>();
            var resolveView = viewLocator.ResolveView(typeof (TViewModel));
            ApplicationPage = (Page) Activator.CreateInstance(resolveView);
            ApplicationPage.BindingContext = viewModelFactory.CreateViewModel<TViewModel>(null);
        }

        protected Page ApplicationPage { get; }
    }
}