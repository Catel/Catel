#l "components-variables.cake"

#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"

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
    return Components != null && Components.Length > 0;
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
            Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, component);
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

private void PackageComponents()
{
    if (!HasComponents())
    {
        return;
    }

    foreach (var component in Components)
    {
        LogSeparator("Packaging component '{0}'", component);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", component);

        // Note: we have a bug where UAP10.0 cannot be packaged, for details see 
        // https://github.com/dotnet/cli/issues/9303
        // 
        // Therefore we will use VS instead for packing and lose the ability to sign
        var useDotNetPack = true;

        var projectFileContents = FileReadText(projectFileName);
        if (!string.IsNullOrWhiteSpace(projectFileContents))
        {
            useDotNetPack = !projectFileContents.ToLower().Contains("uap10.0");
        }

        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, component);
        Information("Output directory: '{0}'", outputDirectory);

        if (useDotNetPack)
        {
            Information("Using 'dotnet pack' to package '{0}'", component);

            var msBuildSettings = new DotNetCoreMSBuildSettings();

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);
            msBuildSettings.WithProperty("ConfigurationName", ConfigurationName);
            msBuildSettings.WithProperty("PackageVersion", VersionNuGet);

            var packSettings = new DotNetCorePackSettings
            {
                MSBuildSettings = msBuildSettings,
                OutputDirectory = OutputRootDirectory,
                Configuration = ConfigurationName,
                NoBuild = true,
            };

            DotNetCorePack(projectFileName, packSettings);
        }
        else
        {
            Warning("Using Visual Studio to pack instead of 'dotnet pack' for '{0}' because UAP 10.0 project was detected. Unfortunately assemblies will not be signed inside the NuGet package", component);

            var msBuildSettings = new MSBuildSettings 
            {
                Verbosity = Verbosity.Minimal, // Verbosity.Diagnostic
                ToolVersion = MSBuildToolVersion.VS2017,
                Configuration = ConfigurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);
            msBuildSettings.WithProperty("ConfigurationName", ConfigurationName);
            msBuildSettings.WithProperty("PackageVersion", VersionNuGet);
            msBuildSettings = msBuildSettings.WithTarget("pack");

            MSBuild(projectFileName, msBuildSettings);
        }
        
        LogSeparator();
    }

    var codeSign = (!IsCiBuild && !string.IsNullOrWhiteSpace(CodeSignCertificateSubjectName));
    if (codeSign)
    {
        // For details, see https://docs.microsoft.com/en-us/nuget/create-packages/sign-a-package
        // nuget sign MyPackage.nupkg -CertificateSubjectName <MyCertSubjectName> -Timestamper <TimestampServiceURL>
        var filesToSign = GetFiles(string.Format("{0}/*.nupkg", OutputRootDirectory));

        foreach (var fileToSign in filesToSign)
        {
            Information("Signing NuGet package '{0}'", fileToSign);

            var exitCode = StartProcess(NuGetExe, new ProcessSettings
            {
                Arguments = string.Format("sign \"{0}\" -CertificateSubjectName \"{1}\" -Timestamper \"{2}\"", fileToSign, CodeSignCertificateSubjectName, CodeSignTimeStampUri)
            });

            Information("Signing NuGet package exited with '{0}'", exitCode);
        }
    }
}

//-------------------------------------------------------------

private void DeployComponents()
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
            Error("NuGet repository is empty, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to the default public NuGet feed");
            return;
        }

        NuGetPush(packageToPush, new NuGetPushSettings
        {
            Source = nuGetRepositoryUrl,
            ApiKey = nuGetRepositoryApiKey
        });
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
    .Does(() =>
{
    DeployComponents();
});