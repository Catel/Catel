#l "docker-variables.cake"

#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"

//-------------------------------------------------------------

private string GetDockerRegistryUrl(string projectName)
{
    // Allow per project overrides via "DockerRegistryUrlFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "DockerRegistryUrlFor", DockerRegistryUrl);
}

//-------------------------------------------------------------

private string GetDockerRegistryApiKey(string projectName)
{
    // Allow per project overrides via "DockerRegistryApiKeyFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(projectName, "DockerRegistryApiKeyFor", DockerRegistryApiKey);
}

//-------------------------------------------------------------

private void ValidateDockerImagesInput()
{
    // No validation required (yet)
}

//-------------------------------------------------------------

private bool HasDockerImages()
{
    return DockerImages != null && DockerImages.Count > 0;
}

//-------------------------------------------------------------

private void UpdateInfoForDockerImages()
{
    if (!HasDockerImages())
    {
        return;
    }

    foreach (var dockerImage in DockerImages)
    {
        Information("Updating version for docker image '{0}'", dockerImage);

        var projectFileName = GetProjectFileName(dockerImage);

        TransformConfig(projectFileName, new TransformationCollection 
        {
            { "Project/PropertyGroup/PackageVersion", VersionNuGet }
        });
    }
}

//-------------------------------------------------------------

private void BuildDockerImages()
{
    if (!HasDockerImages())
    {
        return;
    }
    
    foreach (var dockerImage in DockerImages)
    {
        LogSeparator("Building docker image '{0}'", dockerImage);

        var projectFileName = GetProjectFileName(dockerImage);
        
        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, dockerImage);
        Information("Output directory: '{0}'", outputDirectory);
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);

        // SourceLink specific stuff
        msBuildSettings.WithProperty("PublishRepositoryUrl", "true");
        
        // For SourceLink to work, the .csproj should contain something like this:
        // <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0-beta-63127-02 " PrivateAssets="all" />
        
        MSBuild(projectFileName, msBuildSettings);
    }
}

//-------------------------------------------------------------

private void PackageDockerImages()
{
    if (!HasDockerImages())
    {
        return;
    }

    foreach (var dockerImage in DockerImages)
    {
        LogSeparator("Packaging docker image '{0}'", dockerImage);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", dockerImage);

        // TODO: How to pack

        
        LogSeparator();
    }
}

//-------------------------------------------------------------

private void DeployDockerImages()
{
    if (!HasDockerImages())
    {
        return;
    }

    foreach (var dockerImage in DockerImages)
    {
        if (!ShouldDeployProject(dockerImage))
        {
            Information("Docker image '{0}' should not be deployed", dockerImage);
            continue;
        }

        LogSeparator("Deploying docker image '{0}'", dockerImage);

        var imageToPush = string.Format("{0}/{1}.{2}.nupkg", OutputRootDirectory, dockerImage, VersionNuGet);
        var dockerRegistryUrl = GetDockerRegistryUrl(dockerImage);
        var dockerRegistryApiKey = GetDockerRegistryApiKey(dockerImage);

        if (string.IsNullOrWhiteSpace(dockerRegistryUrl))
        {
            Error("Docker registry url is empty, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to some default public registry");
            return;
        }

        // TODO: Push to registry
    }
}

//-------------------------------------------------------------

Task("UpdateInfoForDockerImages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    UpdateSolutionAssemblyInfo();
    UpdateInfoForDockerImages();
});

//-------------------------------------------------------------

Task("BuildDockerImages")
    .IsDependentOn("UpdateInfoForDockerImages")
    .Does(() =>
{
    BuildDockerImages();
});

//-------------------------------------------------------------

Task("PackageDockerImages")
    .IsDependentOn("BuildDockerImages")
    .Does(() =>
{
    PackageDockerImages();
});

//-------------------------------------------------------------

Task("DeployDockerImages")
    .IsDependentOn("PackageDockerImages")
    .Does(() =>
{
    DeployDockerImages();
});