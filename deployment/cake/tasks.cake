#addin "nuget:?package=MagicChunks"
#addin "nuget:?package=Cake.FileHelpers"

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

	foreach (var projectToPackage in projectsToPackage)
	{
		Information("Updating version for package '{0}'", projectToPackage);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", projectToPackage);

        TransformConfig(projectFileName, new TransformationCollection 
        {
            { "Project/PropertyGroup/PackageVersion", versionNuGet }
        });	
    }
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

Task("CodeSign")
    .IsDependentOn("Build")
    .ContinueOnError()
    .Does(() =>
{
    if (isCiBuild)
    {
        Information("Skipping code signing because this is a CI build");
        return;
    }

    var exeSignFilesSearchPattern = outputRootDirectory + string.Format("/**/*{0}*.exe", codeSignWildCard);
    var dllSignFilesSearchPattern = outputRootDirectory + string.Format("/**/*{0}*.dll", codeSignWildCard);

    List<FilePath> filesToSign = new List<FilePath>();

	Information("Searching for files to code sign using '{0}'", exeSignFilesSearchPattern);

    filesToSign.AddRange(GetFiles(exeSignFilesSearchPattern));

	Information("Searching for files to code sign using '{0}'", dllSignFilesSearchPattern);

    filesToSign.AddRange(GetFiles(dllSignFilesSearchPattern));

    Information("Found '{0}' files to code sign, this can take a few minutes", filesToSign.Count);

    Sign(filesToSign, new SignToolSignSettings 
    {
        AppendSignature = false,
        TimeStampUri = new Uri(codeSignTimeStampUri),
        CertSubjectName = codeSignCertificateSubjectName
    });
});

//-------------------------------------------------------------

Task("Package")
	.IsDependentOn("CodeSign")
	.Does(() =>
{
	foreach (var projectToPackage in projectsToPackage)
	{
		Information("Packaging '{0}'", projectToPackage);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", projectToPackage);

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

        if (useDotNetPack)
        {
            var packSettings = new DotNetCorePackSettings
            {
                Configuration = configurationName,
                NoBuild = true,
            };

            DotNetCorePack(projectFileName, packSettings);
        }
        else
        {
            Warning("Using Visual Studio to pack instead of 'dotnet pack' because UAP 10.0 project was detected. Unfortunately assemblies will not be signed inside the NuGet package");

            var msBuildSettings = new MSBuildSettings 
            {
                Verbosity = Verbosity.Minimal, // Verbosity.Diagnostic
                ToolVersion = MSBuildToolVersion.VS2017,
                Configuration = configurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            msBuildSettings.Properties["ConfigurationName"] = new List<string>(new [] { configurationName });
            msBuildSettings.Properties["PackageVersion"] = new List<string>(new [] { versionNuGet });
            msBuildSettings = msBuildSettings.WithTarget("pack");

            MSBuild(projectFileName, msBuildSettings);
        }
	}
});

//-------------------------------------------------------------

Task("Default")
	.IsDependentOn("Build");

//-------------------------------------------------------------

RunTarget(target);