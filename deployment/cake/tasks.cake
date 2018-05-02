Information("Running target '{0}'", target);
Information("Using output directory '{0}'", outputRootDirectory);

//-------------------------------------------------------------

Task("RestorePackages")
	.Does(() =>
{
	var solutions = GetFiles("./**/*.sln");
	
	foreach(var solution in solutions)
	{
		Information("Restoring packages for {0}", solution);
		
        var nuGetRestoreSettings = new NuGetRestoreSettings();

        if (!string.IsNullOrWhiteSpace(nuGetPackageSources))
        {
            var sources = new List<string>();

            foreach (var splitted in nuGetPackageSources.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                sources.Add(splitted);
            }
            
            if (sources.Count > 0)
            {
                nuGetRestoreSettings.Source = sources;
            }
        }

        NuGetRestore(solution, nuGetRestoreSettings);
	}
});

//-------------------------------------------------------------

// Note: it might look weird that this is dependent on restore packages,
// but to clean, the msbuild projects must be able to load. However, they need
// some targets files that come in via packages

Task("Clean")
    .IsDependentOn("RestorePackages")
	.Does(() => 
{
	if (DirectoryExists(outputRootDirectory))
	{
		DeleteDirectory(outputRootDirectory, new DeleteDirectorySettings()
        {
            Force = true,
            Recursive = true    
        });
	}

    foreach (var platform in platforms)
    {
		Information("Cleaning output for platform '{0}'", platform.Value);

        MSBuild(solutionFileName, configurator =>
            configurator.SetConfiguration(configurationName)
                .SetVerbosity(Verbosity.Minimal)
                .SetMSBuildPlatform(MSBuildPlatform.x86)
                .SetPlatformTarget(platform.Value)
                .WithTarget("Clean"));
    }
});

//-------------------------------------------------------------

Task("UpdateInfo")
	.Does(() =>
{
    Information("Updating assembly info to '{0}'", versionFullSemVer);

	var assemblyInfoParseResult = ParseAssemblyInfo(solutionAssemblyInfoFileName);

    var assemblyInfo = new AssemblyInfoSettings {
        Company = assemblyInfoParseResult.Company,
        Version = versionMajorMinorPatch,
        FileVersion = versionMajorMinorPatch,
        InformationalVersion = versionFullSemVer,
        Copyright = string.Format("Copyright © {0} {1} - {2}", company, startYear, DateTime.Now.Year)
    };

	CreateAssemblyInfo(solutionAssemblyInfoFileName, assemblyInfo);
});

//-------------------------------------------------------------

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("UpdateInfo")
	.Does(() =>
{
	var msBuildSettings = new MSBuildSettings {
		Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
		ToolVersion = MSBuildToolVersion.VS2017,
		Configuration = configurationName,
		MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
		PlatformTarget = PlatformTarget.MSIL
	};

    // TODO: Enable GitLink / SourceLink, see RepositoryUrl, RepositoryBranchName, RepositoryCommitId variables

	MSBuild(solutionFileName, msBuildSettings);
});

//-------------------------------------------------------------

Task("Package")
	.IsDependentOn("Build")
	.Does(() =>
{
	foreach (var projectToPackage in projectsToPackage)
	{
		Information("Packaging '{0}'", projectToPackage);

		var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Minimal, // Verbosity.Diagnostic
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = configurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
	    };

        msBuildSettings.Properties["ConfigurationName"] = new List<string>(new [] { configurationName });
	    msBuildSettings.Properties["PackageVersion"] = new List<string>(new [] { versionNuGet });

        msBuildSettings = msBuildSettings.WithTarget("pack");

        var projectFileName = string.Format("./src/{0}/{0}.csproj", projectToPackage);
	    MSBuild(projectFileName, msBuildSettings);
	}
});

//-------------------------------------------------------------

Task("Default")
	.IsDependentOn("Build");

//-------------------------------------------------------------

RunTarget(target);