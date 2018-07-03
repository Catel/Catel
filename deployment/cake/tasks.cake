#l "generic-tasks.cake"
#l "apps-uwp-tasks.cake"
#l "apps-wpf-tasks.cake"
#l "components-tasks.cake"

#addin "nuget:?package=Cake.Sonar&version=1.1.0"

#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.3.0"

var Target = GetBuildServerVariable("Target", "Default");

Information("Running target '{0}'", Target);
Information("Using output directory '{0}'", OutputRootDirectory);

//-------------------------------------------------------------

private void BuildTestProjects()
{
    foreach (var testProject in TestProjects)
    {
        Information("Building test project '{0}'", testProject);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", testProject);
        
        var msBuildSettings = new MSBuildSettings
        {
            Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        // Force disable SonarQube
        msBuildSettings.WithProperty("SonarQubeExclude", "true");

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, testProject);
        Information("Output directory: '{0}'", outputDirectory);
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);

        MSBuild(projectFileName, msBuildSettings);
    }
}

//-------------------------------------------------------------

Task("UpdateInfo")
    .Does(() =>
{
    UpdateSolutionAssemblyInfo();
    
    UpdateInfoForComponents();
    UpdateInfoForUwpApps();
    UpdateInfoForWpfApps();
});

//-------------------------------------------------------------

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("UpdateInfo")
    .Does(() =>
{
    var enableSonar = !string.IsNullOrWhiteSpace(SonarUrl);
    if (enableSonar)
    {
        SonarBegin(new SonarBeginSettings 
        {
            // SonarQube info
            Url = SonarUrl,
            Login = SonarUsername,
            Password = SonarPassword,

            // Project info
            Key = SonarProject,
            // Branch only works with the branch plugin
            //Branch = RepositoryBranchName,
            Version = VersionFullSemVer,
            
            // Minimize extreme logging
            Verbose = false,
            Silent = true,
        });
    }
    else
    {
        Information("Skipping Sonar integration since url is not specified");
    }

    BuildComponents();
    BuildUwpApps();
    BuildWpfApps();

    if (!string.IsNullOrWhiteSpace(SonarUrl))
    {
        SonarEnd(new SonarEndSettings 
        {
            Login = SonarUsername,
            Password = SonarPassword,
        });
    }

    BuildTestProjects();
});

//-------------------------------------------------------------

Task("Package")
    // Make sure we have the temporary "project.assets.json" in case we need to package with Visual Studio
    .IsDependentOn("RestorePackages")
    // Make sure to update if we are running on a new agent so we can sign nuget packages
    .IsDependentOn("UpdateNuGet")
    .IsDependentOn("CodeSign")
    .Does(() =>
{
    PackageComponents();
    PackageUwpApps();
    PackageWpfApps();
});

//-------------------------------------------------------------
// Wrapper tasks since we don't want to add "Build" as a 
// dependency to "Package" because we want to run in multiple
// stages
//-------------------------------------------------------------

Task("BuildAndPackage")
    .IsDependentOn("Build")
    .IsDependentOn("Package");

//-------------------------------------------------------------

Task("Default")
	.IsDependentOn("Build");

//-------------------------------------------------------------

RunTarget(Target);