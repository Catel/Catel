var _dotNetCoreCache = new Dictionary<string, bool>();

//-------------------------------------------------------------

private void LogSeparator(string messageFormat, params object[] args)
{
    Information("");
    Information("----------------------------------------");
    Information(messageFormat, args);
    Information("----------------------------------------");
    Information("");
}

//-------------------------------------------------------------

private void LogSeparator()
{
    Information("");
    Information("----------------------------------------");
    Information("");
}

//-------------------------------------------------------------

private string GetProjectDirectory(string projectName)
{
    var projectDirectory = string.Format("./src/{0}/", projectName);
    return projectDirectory;
}

//-------------------------------------------------------------

private string GetProjectOutputDirectory(string projectName)
{
    var projectDirectory = string.Format("{0}/{1}", OutputRootDirectory, projectName);
    return projectDirectory;
}

//-------------------------------------------------------------

private string GetProjectFileName(string projectName)
{
    var fileName = string.Format("{0}{1}.csproj", GetProjectDirectory(projectName), projectName);
    return fileName;
}

//-------------------------------------------------------------

private string GetProjectSlug(string projectName)
{
    var slug = projectName.Replace(".", "").Replace(" ", "");
    return slug;
}

//-------------------------------------------------------------

private string GetProjectSpecificConfigurationValue(string projectName, string configurationPrefix, string fallbackValue)
{
    // Allow per project overrides via "[configurationPrefix][projectName]"
    var slug = GetProjectSlug(projectName);
    var keyToCheck = string.Format("{0}{1}", configurationPrefix, slug);

    var value = GetBuildServerVariable(keyToCheck, fallbackValue);
    return value;
}

//-------------------------------------------------------------

private bool IsDotNetCoreProject(string projectName)
{
    var projectFileName = GetProjectFileName(projectName);

    if (!_dotNetCoreCache.TryGetValue(projectFileName, out var isDotNetCore))
    {
        isDotNetCore = false;

        var lines = System.IO.File.ReadAllLines(projectFileName);
        foreach (var line in lines)
        {
            // Match both *TargetFramework* and *TargetFrameworks* 
            var lowerCase = line.ToLower();
            if (lowerCase.Contains("targetframework"))
            {
                if (lowerCase.Contains("netcore"))
                {
                    isDotNetCore = true;
                    break;
                }
            }
        }

        _dotNetCoreCache[projectFileName] = isDotNetCore;
    }

    return _dotNetCoreCache[projectFileName];
}

//-------------------------------------------------------------

private bool ShouldDeployProject(string projectName)
{
    // Allow the build server to configure this via "Deploy[ProjectName]"
    var slug = GetProjectSlug(projectName);
    var keyToCheck = string.Format("Deploy{0}", slug);

    var value = GetBuildServerVariable(keyToCheck, "True");
    
    Information("Value for '{0}': {1}", keyToCheck, value);
    
    var shouldDeploy = bool.Parse(value);
    return shouldDeploy;
}