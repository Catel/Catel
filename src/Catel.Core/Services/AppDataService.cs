namespace Catel.Services
{
    using Catel.IO;

    public class AppDataService : IAppDataService
    {
        public virtual string GetApplicationDataDirectory(ApplicationDataTarget applicationDataTarget)
        {
            var applicationDataDirectory = Catel.IO.Path.GetApplicationDataDirectory(applicationDataTarget);
            return applicationDataDirectory;
        }
    }
}
