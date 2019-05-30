#l "github-pages-variables.cake"

#addin "nuget:?package=Cake.Git&version=0.19.0"

//-------------------------------------------------------------

private string GetGitHubPagesRepositoryUrl(string projectName)
{
    // Allow per project overrides via "GitHubPagesRepositoryUrlFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "GitHubPagesRepositoryUrlFor", GitHubPagesRepositoryUrl);
}

//-------------------------------------------------------------

private string GetGitHubPagesBranchName(string projectName)
{
    // Allow per project overrides via "GitHubPagesBranchNameFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "GitHubPagesBranchNameFor", GitHubPagesBranchName);
}

//-------------------------------------------------------------

private string GetGitHubPagesEmail(string projectName)
{
    // Allow per project overrides via "GitHubPagesEmailFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "GitHubPagesEmailFor", GitHubPagesApiToken);
}

//-------------------------------------------------------------

private string GetGitHubPagesUserName(string projectName)
{
    // Allow per project overrides via "GitHubPagesUserNameFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "GitHubPagesUserNameFor", GitHubPagesApiToken);
}

//-------------------------------------------------------------

private string GetGitHubPagesApiToken(string projectName)
{
    // Allow per project overrides via "GitHubPagesApiTokenFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "GitHubPagesApiTokenFor", GitHubPagesApiToken);
}

//-------------------------------------------------------------

private void ValidateGitHubPagesInput()
{
    if (!HasGitHubPages())
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(GitHubPagesRepositoryUrl))
    {
        throw new Exception("GitHubPagesRepositoryUrl must be defined");
    }

    if (string.IsNullOrWhiteSpace(GitHubPagesBranchName))
    {
        throw new Exception("GitHubPagesBranchName must be defined");
    }
                
    if (string.IsNullOrWhiteSpace(GitHubPagesEmail))
    {
        throw new Exception("GitHubPagesEmail must be defined");
    }

    if (string.IsNullOrWhiteSpace(GitHubPagesUserName))
    {
        throw new Exception("GitHubPagesUserName must be defined");
    }

    if (string.IsNullOrWhiteSpace(GitHubPagesApiToken))
    {
        throw new Exception("GitHubPagesApiToken must be defined");
    }
}

//-------------------------------------------------------------

private bool HasGitHubPages()
{
    return GitHubPages != null && GitHubPages.Count > 0;
}

//-------------------------------------------------------------

private async Task PrepareForGitHubPagesAsync()
{
    if (!HasGitHubPages())
    {
        return;
    }

    // Check whether projects should be processed, `.ToList()` 
    // is required to prevent issues with foreach
    foreach (var gitHubPage in GitHubPages.ToList())
    {
        if (!ShouldProcessProject(gitHubPage))
        {
            GitHubPages.Remove(gitHubPage);
        }
    }
}

//-------------------------------------------------------------

private void UpdateInfoForGitHubPages()
{
    if (!HasGitHubPages())
    {
        return;
    }

    foreach (var gitHubPage in GitHubPages)
    {
        Information("Updating version for GitHub page '{0}'", gitHubPage);

        var projectFileName = GetProjectFileName(gitHubPage);

        TransformConfig(projectFileName, new TransformationCollection 
        {
            { "Project/PropertyGroup/PackageVersion", VersionNuGet }
        });
    }
}

//-------------------------------------------------------------

private void BuildGitHubPages()
{
    if (!HasGitHubPages())
    {
        return;
    }

    foreach (var gitHubPage in GitHubPages)
    {
        LogSeparator("Building GitHub page '{0}'", gitHubPage);

        var projectFileName = GetProjectFileName(gitHubPage);
        
        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
            ToolVersion = MSBuildToolVersion.Default,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        ConfigureMsBuild(msBuildSettings, gitHubPage);

        // Always disable SourceLink
        msBuildSettings.WithProperty("EnableSourceLink", "false");

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, gitHubPage);
        Information("Output directory: '{0}'", outputDirectory);
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);

        MSBuild(projectFileName, msBuildSettings);
    }
}

//-------------------------------------------------------------

private void PackageGitHubPages()
{
    if (!HasGitHubPages())
    {
        return;
    }

    foreach (var gitHubPage in GitHubPages)
    {
      LogSeparator("Packaging GitHub pages '{0}'", gitHubPage);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", gitHubPage);

        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, gitHubPage);
        Information("Output directory: '{0}'", outputDirectory);

        Information("1) Using 'dotnet publish' to package '{0}'", gitHubPage);

        var msBuildSettings = new DotNetCoreMSBuildSettings();

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", outputDirectory);
        msBuildSettings.WithProperty("ConfigurationName", ConfigurationName);
        msBuildSettings.WithProperty("PackageVersion", VersionNuGet);

        var publishSettings = new DotNetCorePublishSettings
        {
            MSBuildSettings = msBuildSettings,
            OutputDirectory = outputDirectory,
            Configuration = ConfigurationName
        };

        DotNetCorePublish(projectFileName, publishSettings);
    }
}

//-------------------------------------------------------------

private void DeployGitHubPages()
{
    if (!HasGitHubPages())
    {
        return;
    }
    
    foreach (var gitHubPage in GitHubPages)
    {
        if (!ShouldDeployProject(gitHubPage))
        {
            Information("GitHub page '{0}' should not be deployed", gitHubPage);
            continue;
        }

        LogSeparator("Deploying GitHub page '{0}'", gitHubPage);

        Warning("Only Blazor apps are supported as GitHub pages");

        var temporaryDirectory = GetTempDirectory("gh-pages", gitHubPage);

        CleanDirectory(temporaryDirectory);

        var repositoryUrl = GetGitHubPagesRepositoryUrl(gitHubPage);
        var branchName = GetGitHubPagesBranchName(gitHubPage);
        var email = GetGitHubPagesEmail(gitHubPage);
        var userName = GetGitHubPagesUserName(gitHubPage);
        var apiToken = GetGitHubPagesApiToken(gitHubPage);

        Information("1) Cloning repository '{0}' using branch name '{1}'", repositoryUrl, branchName);

        GitClone(repositoryUrl, temporaryDirectory, userName, apiToken, new GitCloneSettings
        {
            BranchName = branchName
        });

        Information("2) Updating the GitHub pages branch with latest source");

        // Special directory we need to distribute (e.g. output\Release\Blazorc.PatternFly.Example\Blazorc.PatternFly.Example\dist)
        var sourceDirectory = string.Format("{0}/{1}/{1}/dist", OutputRootDirectory, gitHubPage);
        var sourcePattern = string.Format("{0}/**/*", sourceDirectory);

        Debug("Copying all files from '{0}' => '{1}'", sourcePattern, temporaryDirectory);

        CopyFiles(sourcePattern, temporaryDirectory, true);

        Information("3) Committing latest GitHub pages");

        GitAddAll(temporaryDirectory);
        GitCommit(temporaryDirectory, "Build server", email, string.Format("Auto-update GitHub pages: '{0}'", VersionNuGet));

        Information("4) Pushing code back to repository '{0}'", repositoryUrl);

        GitPush(temporaryDirectory, userName, apiToken);
    }
}