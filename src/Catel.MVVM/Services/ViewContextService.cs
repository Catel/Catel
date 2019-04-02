namespace Catel.Services
{
    using Catel.MVVM.Views;

    public class ViewContextService : IViewContextService
    {
        public object GetContext(IView view)
        {
            return view.DataContext;
        }
    }
}
