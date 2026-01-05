namespace Catel.IoC
{
    public interface IInitializeAtStartup : IConstructAtStartup
    {
        void Initialize();
    }
}
