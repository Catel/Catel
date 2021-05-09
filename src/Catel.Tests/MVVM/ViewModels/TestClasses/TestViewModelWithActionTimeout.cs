namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System;
    using System.Threading.Tasks;
    using Catel.MVVM;

    public class TestViewModelWithActionTimeout : ViewModelBase
    {
        public int ActionDuration { get; set; }

        protected override async Task<bool> CancelAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(ActionDuration)); 
            
            return await base.CancelAsync();
        }

        protected override async Task<bool> SaveAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(ActionDuration));

            return await base.SaveAsync();
        }
    }
}
