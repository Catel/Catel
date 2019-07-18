#l "components-variables.cake"

#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"

using System.Xml.Linq;

//-------------------------------------------------------------

private string GetComponentNuGetRepositoryUrl(string projectName)
{
    // Allow per project overrides via "NuGetRepositoryUrlFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "NuGetRepositoryUrlFor", NuGetRepositoryUrl);
}

//-------------------------------------------------------------

private string GetComponentNuGetRepositoryApiKey(string projectName)
{
    // Allow per project overrides via "NuGetRepositoryApiKeyFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "NuGetRepositoryApiKeyFor", NuGetRepositoryApiKey);
}

//-------------------------------------------------------------

private void ValidateComponentsInput()
{
    // No validation required (yet)
}

//-------------------------------------------------------------

private bool HasComponents()
{
    return Components != null && Components.Count > 0;
}

//-------------------------------------------------------------

private async Task PrepareForComponentsAsync()
{
    if (!HasComponents())
    {
        return;
    }

    // Check whether projects should be processed, `.ToList()` 
    // is required to prevent issues with foreach
    foreach (var component in Components.ToList())
    {
        if (!ShouldProcessProject(component))
        {
            Components.Remove(component);
        }
    }

    if (IsLocalBuild && Target.ToLower().Contains("packagelocal"))
    {
        foreach (var component in Components)
        {
            var cacheDirectory = Environment.ExpandEnvironmentVariables(string.Format("%userprofile%/.nuget/packages/{0}/{1}", component, VersionNuGet));

            Information("Checking for existing local NuGet cached version at '{0}'", cacheDirectory);

            var retryCount = 3;

            while (retryCount > 0)
            {
                if (!DirectoryExists(cacheDirectory))
                {
                    break;
                }

                Information("Deleting already existing NuGet cached version from '{0}'", cacheDirectory);
                
                DeleteDirectory(cacheDirectory, new DeleteDirectorySettings()
                {
                    Force = true,
                    Recursive = true
                });

                await System.Threading.Tasks.Task.Delay(1000);

                retryCount--;
            }            
        }
    }
}

//-------------------------------------------------------------

private void UpdateInfoForComponents()
{
    if (!HasComponents())
    {
        return;
    }

    foreach (var component in Components)
    {
        Information("Updating version for component '{0}'", component);

        var projectFileName = GetProjectFileName(component);

        TransformConfig(projectFileName, new TransformationCollection 
        {
            { "Project/PropertyGroup/PackageVersion", VersionNuGet }
        });
    }
}

//-------------------------------------------------------------

private void BuildComponents()
{
    if (!HasComponents())
    {
        return;
    }
    
    foreach (var component in Components)
    {
        LogSeparator("Building component '{0}'", component);

        var projectFileName = GetProjectFileName(component);
        
        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Quiet,
            //Verbosity = Verbosity.Diagnostic,
            ToolVersion = MSBuildToolVersion.Default,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        ConfigureMsBuild(msBuildSettings, component);
        
        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = GetProjectOutputDirectory(component);
        Information("Output directory: '{0}'", outputDirectory);
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);

        // SourceLink specific stuff
        var repositoryUrl = RepositoryUrl;
        if (!SourceLinkDisabled && !IsLocalBuild && !string.IsNullOrWhiteSpace(repositoryUrl))
        {       
            Information("Repository url is specified, enabling SourceLink to commit '{0}/commit/{1}'", repositoryUrl, RepositoryCommitId);

            // TODO: For now we are assuming everything is git, we might need to change that in the future
            // See why we set the values at https://github.com/dotnet/sourcelink/issues/159#issuecomment-427639278
            msBuildSettings.WithProperty("EnableSourceLink", "true");
            msBuildSettings.WithProperty("EnableSourceControlManagerQueries", "false");
            msBuildSettings.WithProperty("PublishRepositoryUrl", "true");
            msBuildSettings.WithProperty("RepositoryType", "git");
            msBuildSettings.WithProperty("RepositoryUrl", repositoryUrl);
            msBuildSettings.WithProperty("RevisionId", RepositoryCommitId);

            InjectSourceLinkInProjectFile(projectFileName);
        }

        MSBuild(projectFileName, msBuildSettings);
    }
}

//-------------------------------------------------------------

private void PackageComponents()
{
    if (!HasComponents())
    {
        return;
    }

    foreach (var component in Components)
    {
        LogSeparator("Packaging component '{0}'", component);

        var projectDirectory = string.Format("./src/{0}", component);
        var projectFileName = string.Format("{0}/{1}.csproj", projectDirectory, component);
        var outputDirectory = GetProjectOutputDirectory(component);
        Information("Output directory: '{0}'", outputDirectory);

        // Step 1: remove intermediate files to ensure we have the same results on the build server, somehow NuGet 
        // targets tries to find the resource assemblies in [ProjectName]\obj\Release\net46\de\[ProjectName].resources.dll',
        // we won't run a clean on the project since it will clean out the actual output (which we still need for packaging)

        Information("Cleaning intermediate files for component '{0}'", component);

        var binFolderPattern = string.Format("{0}/bin/{1}/**.dll", projectDirectory, ConfigurationName);

        Information("Deleting 'bin' directory contents using '{0}'", binFolderPattern);

        var binFiles = GetFiles(binFolderPattern);
        DeleteFiles(binFiles);

        var objFolderPattern = string.Format("{0}/obj/{1}/**.dll", projectDirectory, ConfigurationName);

        Information("Deleting 'bin' directory contents using '{0}'", objFolderPattern);

        var objFiles = GetFiles(objFolderPattern);
        DeleteFiles(objFiles);

        Information(string.Empty);

        // Step 2: Go packaging!
        Information("Using 'msbuild' to package '{0}'", component);

        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Quiet,
            //Verbosity = Verbosity.Diagnostic,
            ToolVersion = MSBuildToolVersion.Default,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        ConfigureMsBuild(msBuildSettings, component, "pack");

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);
        msBuildSettings.WithProperty("ConfigurationName", ConfigurationName);
        msBuildSettings.WithProperty("PackageVersion", VersionNuGet);

        // SourceLink specific stuff
        var repositoryUrl = RepositoryUrl;
        if (!IsLocalBuild && !string.IsNullOrWhiteSpace(repositoryUrl))
        {       
            Information("Repository url is specified, adding commit specific data to package");

            // TODO: For now we are assuming everything is git, we might need to change that in the future
            // See why we set the values at https://github.com/dotnet/sourcelink/issues/159#issuecomment-427639278
            msBuildSettings.WithProperty("PublishRepositoryUrl", "true");
            msBuildSettings.WithProperty("RepositoryType", "git");
            msBuildSettings.WithProperty("RepositoryUrl", repositoryUrl);
            msBuildSettings.WithProperty("RevisionId", RepositoryCommitId);
        }
        
        // Fix for .NET Core 3.0, see https://github.com/dotnet/core-sdk/issues/192, it
        // uses obj/release instead of [outputdirectory]
        msBuildSettings.WithProperty("DotNetPackIntermediateOutputPath", outputDirectory);
        
        msBuildSettings.WithProperty("NoBuild", "true");
        msBuildSettings.Targets.Add("Pack");

        MSBuild(projectFileName, msBuildSettings);

        LogSeparator();
    }

    var codeSign = (!IsCiBuild && !IsLocalBuild && !string.IsNullOrWhiteSpace(CodeSignCertificateSubjectName));
    if (codeSign)
    {
        // For details, see https://docs.microsoft.com/en-us/nuget/create-packages/sign-a-package
        // nuget sign MyPackage.nupkg -CertificateSubjectName <MyCertSubjectName> -Timestamper <TimestampServiceURL>
        var filesToSign = GetFiles(string.Format("{0}/*.nupkg", OutputRootDirectory));

        foreach (var fileToSign in filesToSign)
        {
            Information("Signing NuGet package '{0}' using certificate subject '{1}'", fileToSign, CodeSignCertificateSubjectName);

            var exitCode = StartProcess(NuGetExe, new ProcessSettings
            {
                Arguments = string.Format("sign \"{0}\" -CertificateSubjectName \"{1}\" -Timestamper \"{2}\"", fileToSign, CodeSignCertificateSubjectName, CodeSignTimeStampUri)
            });

            Information("Signing NuGet package exited with '{0}'", exitCode);
        }
    }
}

//-------------------------------------------------------------

private async Task DeployComponentsAsync()
{
    if (!HasComponents())
    {
        return;
    }

    foreach (var component in Components)
    {
        if (!ShouldDeployProject(component))
        {
            Information("Component '{0}' should not be deployed", component);
            continue;
        }

        LogSeparator("Deploying component '{0}'", component);

        var packageToPush = string.Format("{0}/{1}.{2}.nupkg", OutputRootDirectory, component, VersionNuGet);
        var nuGetRepositoryUrl = GetComponentNuGetRepositoryUrl(component);
        var nuGetRepositoryApiKey = GetComponentNuGetRepositoryApiKey(component);

        if (string.IsNullOrWhiteSpace(nuGetRepositoryUrl))
        {
            throw new Exception("NuGet repository is empty, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to the default public NuGet feed");
        }

        NuGetPush(packageToPush, new NuGetPushSettings
        {
            Source = nuGetRepositoryUrl,
            ApiKey = nuGetRepositoryApiKey
        });

        await NotifyAsync(component, string.Format("Deployed to NuGet store"), TargetType.Component);
    }
}

//-------------------------------------------------------------

Task("UpdateInfoForComponents")
    .IsDependentOn("Clean")
    .Does(() =>
{
    UpdateSolutionAssemblyInfo();
    UpdateInfoForComponents();
});

//-------------------------------------------------------------

Task("BuildComponents")
    .IsDependentOn("UpdateInfoForComponents")
    .Does(() =>
{
    BuildComponents();
});

//-------------------------------------------------------------

Task("PackageComponents")
    .IsDependentOn("BuildComponents")
    .Does(() =>
{
    PackageComponents();
});

//-------------------------------------------------------------

Task("DeployComponents")
    .IsDependentOn("PackageComponents")
    .Does(async () =>
{
    await DeployComponentsAsync();
});