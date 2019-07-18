#l "tools-variables.cake"

#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"

using System.Xml.Linq;

//-------------------------------------------------------------

private void EnsureChocolateyLicenseFile(string projectName)
{
    // Required for Chocolatey

    var projectDirectory = GetProjectDirectory(projectName);
    var outputDirectory = GetProjectOutputDirectory(projectName);

    // Check if it already exists
    var fileName = string.Format("{0}/LICENSE.txt", outputDirectory);
    if (!FileExists(fileName))
    {
        Information("Creating Chocolatey license file for '{0}'", projectName);

        // Option 1: Copy from root
        var sourceFile = "./LICENSE";
        if (FileExists(sourceFile))
        {
            Information("Using license file from repository");

            CopyFile(sourceFile, fileName);
            return;
        }

        // Option 2: use expression (PackageLicenseExpression)
        throw new Exception("Cannot find ./LICENSE, which is required for Chocolatey");
    }
}

//-------------------------------------------------------------

private void EnsureChocolateyVerificationFile(string projectName)
{
    // Required for Chocolatey

    var projectDirectory = GetProjectDirectory(projectName);
    var outputDirectory = GetProjectOutputDirectory(projectName);

    // Check if it already exists
    var fileName = string.Format("{0}/VERIFICATION.txt", outputDirectory);
    if (!FileExists(fileName))
    {
        Information("Creating Chocolatey verification file for '{0}'", projectName);
        
        System.IO.File.WriteAllText(fileName, @"VERIFICATION
Verification is intended to assist the Chocolatey moderators and community
in verifying that this package's contents are trustworthy.
 
<Include details of how to verify checksum contents>
<If software vendor, explain that here - checksum verification instructions are optional>");
    }
}

//-------------------------------------------------------------

private string GetToolsNuGetRepositoryUrls(string projectName)
{
    // Allow per project overrides via "NuGetRepositoryUrlFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "ToolsNuGetRepositoryUrlsFor", ToolsNuGetRepositoryUrls);
}

//-------------------------------------------------------------

private string GetToolsNuGetRepositoryApiKeys(string projectName)
{
    // Allow per project overrides via "NuGetRepositoryApiKeyFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "ToolsNuGetRepositoryApiKeysFor", ToolsNuGetRepositoryApiKeys);
}

//-------------------------------------------------------------

private void ValidateToolsInput()
{
    // No validation required (yet)
}

//-------------------------------------------------------------

private bool HasTools()
{
    return Tools != null && Tools.Count > 0;
}

//-------------------------------------------------------------

private async Task PrepareForToolsAsync()
{
    if (!HasTools())
    {
        return;
    }

    // Check whether projects should be processed, `.ToList()` 
    // is required to prevent issues with foreach
    foreach (var tool in Tools.ToList())
    {
        if (!ShouldProcessProject(tool))
        {
            Tools.Remove(tool);
        }
    }

    if (IsLocalBuild && Target.ToLower().Contains("packagelocal"))
    {
        foreach (var tool in Tools)
        {
            var cacheDirectory = Environment.ExpandEnvironmentVariables(string.Format("%userprofile%/.nuget/packages/{0}/{1}", tool, VersionNuGet));

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

private void UpdateInfoForTools()
{
    if (!HasTools())
    {
        return;
    }

    foreach (var tool in Tools)
    {
        Information("Updating version for tool '{0}'", tool);

        var projectFileName = GetProjectFileName(tool);

        TransformConfig(projectFileName, new TransformationCollection 
        {
            { "Project/PropertyGroup/PackageVersion", VersionNuGet }
        });
    }
}

//-------------------------------------------------------------

private void BuildTools()
{
    if (!HasTools())
    {
        return;
    }
    
    foreach (var tool in Tools)
    {
        LogSeparator("Building tool '{0}'", tool);

        var projectFileName = GetProjectFileName(tool);
        
        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Quiet,
            //Verbosity = Verbosity.Diagnostic,
            ToolVersion = MSBuildToolVersion.Default,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        ConfigureMsBuild(msBuildSettings, tool);
        
        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = GetProjectOutputDirectory(tool);
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

private void PackageTools()
{
    if (!HasTools())
    {
        return;
    }

    foreach (var tool in Tools)
    {
        LogSeparator("Packaging tool '{0}'", tool);

        var projectDirectory = string.Format("./src/{0}", tool);
        var projectFileName = string.Format("{0}/{1}.csproj", projectDirectory, tool);
        var outputDirectory = GetProjectOutputDirectory(tool);
        Information("Output directory: '{0}'", outputDirectory);

        // Step 1: remove intermediate files to ensure we have the same results on the build server, somehow NuGet 
        // targets tries to find the resource assemblies in [ProjectName]\obj\Release\net46\de\[ProjectName].resources.dll',
        // we won't run a clean on the project since it will clean out the actual output (which we still need for packaging)

        Information("Cleaning intermediate files for tool '{0}'", tool);

        var binFolderPattern = string.Format("{0}/bin/{1}/**.dll", projectDirectory, ConfigurationName);

        Information("Deleting 'bin' directory contents using '{0}'", binFolderPattern);

        var binFiles = GetFiles(binFolderPattern);
        DeleteFiles(binFiles);

        var objFolderPattern = string.Format("{0}/obj/{1}/**.dll", projectDirectory, ConfigurationName);

        Information("Deleting 'bin' directory contents using '{0}'", objFolderPattern);

        var objFiles = GetFiles(objFolderPattern);
        DeleteFiles(objFiles);

        Information(string.Empty);

        // Step 2: Ensure chocolatey stuff
        EnsureChocolateyLicenseFile(tool);
        EnsureChocolateyVerificationFile(tool);

        // Step 3: Go packaging!
        Information("Using 'msbuild' to package '{0}'", tool);

        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Quiet,
            //Verbosity = Verbosity.Diagnostic,
            ToolVersion = MSBuildToolVersion.Default,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        ConfigureMsBuild(msBuildSettings, tool, "pack");

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
        
        // As described in the this issue: https://github.com/NuGet/Home/issues/4360
        // we should not use IsTool, but set BuildOutputTargetFolder instead
        msBuildSettings.WithProperty("BuildOutputTargetFolder", "tools");
        msBuildSettings.WithProperty("NoDefaultExcludes", "true");
        //msBuildSettings.WithProperty("IsTool", "true");

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

private async Task DeployToolsAsync()
{
    if (!HasTools())
    {
        return;
    }

    foreach (var tool in Tools)
    {
        if (!ShouldDeployProject(tool))
        {
            Information("Tool '{0}' should not be deployed", tool);
            continue;
        }

        LogSeparator("Deploying tool '{0}'", tool);

        var packageToPush = string.Format("{0}/{1}.{2}.nupkg", OutputRootDirectory, tool, VersionNuGet);
        var nuGetRepositoryUrls = GetToolsNuGetRepositoryUrls(tool);
        var nuGetRepositoryApiKeys = GetToolsNuGetRepositoryApiKeys(tool);

        var nuGetServers = GetNuGetServers(nuGetRepositoryUrls, nuGetRepositoryApiKeys);
        if (nuGetServers.Count == 0)
        {
            throw new Exception("No NuGet repositories specified, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to the default public NuGet feed");
        }

        Information("Found '{0}' target NuGet servers to push tool '{1}'", nuGetServers.Count, tool);

        foreach (var nuGetServer in nuGetServers)
        {
            Information("Pushing to '{0}'", nuGetServer);

            NuGetPush(packageToPush, new NuGetPushSettings
            {
                Source = nuGetServer.Url,
                ApiKey = nuGetServer.ApiKey
            });
        }

        await NotifyAsync(tool, string.Format("Deployed to NuGet store(s)"), TargetType.Tool);
    }
}

//-------------------------------------------------------------

Task("UpdateInfoForTools")
    .IsDependentOn("Clean")
    .Does(() =>
{
    UpdateSolutionAssemblyInfo();
    UpdateInfoForTools();
});

//-------------------------------------------------------------

Task("BuildTools")
    .IsDependentOn("UpdateInfoForTools")
    .Does(() =>
{
    BuildTools();
});

//-------------------------------------------------------------

Task("PackageTools")
    .IsDependentOn("BuildTools")
    .Does(() =>
{
    PackageTools();
});

//-------------------------------------------------------------

Task("DeployTools")
    .IsDependentOn("PackageTools")
    .Does(async () =>
{
    await DeployToolsAsync();
});