using System;
using Catel.IoC;
using Catel.MVVM;
using Xamarin.Forms;

namespace Catel.Xamarin.Forms
{
    public class Application<TViewModel> : Application where TViewModel : IViewModel
    {
        protected Application()
        {
            // TODO: ModuleInit should work instead this.
            Core.ModuleInitializer.Initialize();
            ModuleInitializer.Initialize();

            // TODO: Improve this approach.
            var viewLocator = this.GetDependencyResolver().Resolve<IViewLocator>();
            var viewModelFactory = this.GetDependencyResolver().Resolve<IViewModelFactory>();
            var resolveView = viewLocator.ResolveView(typeof(TViewModel));
            ApplicationPage = (Page)Activator.CreateInstance(resolveView);
            ApplicationPage.BindingContext = viewModelFactory.CreateViewModel<TViewModel>(null);
        }

        protected Page ApplicationPage { get; private set; }
    }
}