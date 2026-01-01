namespace Catel.Tests.ViewModels
{
    using System;
    using Catel.MVVM;

    public class AutoClosingViewModel : ViewModelBase
    {
        public AutoClosingViewModel(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }
    }
}
