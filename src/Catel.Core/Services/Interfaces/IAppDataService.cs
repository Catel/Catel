namespace Catel.Services
{
    public interface IAppDataService
    {
        string GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget applicationDataTarget);
    }
}
