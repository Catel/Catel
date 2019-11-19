namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
