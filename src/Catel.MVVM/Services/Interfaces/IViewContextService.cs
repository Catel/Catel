namespace Catel.Services
{
    using Catel.MVVM.Views;

    public interface IViewContextService
    {
        object GetContext(IView view);
    }
}
