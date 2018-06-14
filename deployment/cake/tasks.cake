#addin "nuget:?package=MagicChunks"
#addin "nuget:?package=Cake.FileHelpers"
#addin "nuget:?package=Cake.Sonar"
#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool"

Information("Running target '{0}'", target);
Information("Using output directory '{0}'", outputRootDirectory);

var nuGetExe = System.IO.Path.GetFullPath(outputRootDirectory + "/../../tools/nuget.exe");

//-------------------------------------------------------------

Task("UpdateNuGet")
    .ContinueOnError()
    .Does(() => 
{
    Information("Making sure NuGet is using the latest version");

    var exitCode = StartProcess(nuGetExe, new ProcessSettings
    {
        Arguments = "update -self"
    });

    var newNuGetVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(nuGetExe);
    var newNuGetVersion = newNuGetVersionInfo.FileVersion;

    Information("Updating NuGet.exe exited with '{0}', version is '{1}'", exitCode, newNuGetVersion);
});

//-------------------------------------------------------------

Task("RestorePackages")
    .IsDependentOn("UpdateNuGet")
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

Task("SonarBegin")
    .ContinueOnError()
    .Does(() =>
{
    if (string.IsNullOrWhiteSpace(sonarUrl))
    {
        Information("Skipping Sonar integration since url is not specified");
        return;
    }

    SonarBegin(new SonarBeginSettings {
        Url = sonarUrl,
        Login = sonarUsername,
        Password = sonarPassword,
        Verbose = true,
        Key = sonarProject
    });
});

//-------------------------------------------------------------

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("UpdateInfo")
    .IsDependentOn("SonarBegin")
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

Task("SonarEnd")
    .IsDependentOn("Build")
    .ContinueOnError()
    .Does(() =>
{
    if (string.IsNullOrWhiteSpace(sonarUrl))
    {
        // No need to log, we already did
        return;
    }

    SonarEnd(new SonarEndSettings {
        Login = sonarUsername,
        Password = sonarPassword,
     });
});

//-------------------------------------------------------------

Task("CodeSign")
    .IsDependentOn("SonarEnd")
    .ContinueOnError()
    .Does(() =>
{
    if (isCiBuild)
    {
        Information("Skipping code signing because this is a CI build");
        return;
    }

    if (string.IsNullOrWhiteSpace(codeSignCertificateSubjectName))
    {
        Information("Skipping code signing because the certificate subject name was not specified");
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

    var signToolSignSettings = new SignToolSignSettings 
    {
        AppendSignature = false,
        TimeStampUri = new Uri(codeSignTimeStampUri),
        CertSubjectName = codeSignCertificateSubjectName
    };

    Sign(filesToSign, signToolSignSettings);

    // Note parallel doesn't seem to be faster in an example repository:
    // 1 thread:   1m 30s
    // 4 threads:  1m 30s
    // 10 threads: 1m 30s
    // Parallel.ForEach(filesToSign, new ParallelOptions 
    //     { 
    //         MaxDegreeOfParallelism = 10 
    //     },
    //     fileToSign => 
    //     { 
    //         Sign(fileToSign, signToolSignSettings);
    //     });
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

    var codeSign = (!isCiBuild && !string.IsNullOrWhiteSpace(codeSignCertificateSubjectName));
    if (codeSign)
    {
        // For details, see https://docs.microsoft.com/en-us/nuget/create-packages/sign-a-package
        // nuget sign MyPackage.nupkg -CertificateSubjectName <MyCertSubjectName> -Timestamper <TimestampServiceURL>
        var filesToSign = GetFiles(string.Format("{0}/*.nupkg", outputRootDirectory));

        foreach (var fileToSign in filesToSign)
        {
            Information("Signing NuGet package '{0}'", fileToSign);

            var exitCode = StartProcess(nuGetExe, new ProcessSettings
            {
                Arguments = string.Format("sign \"{0}\" -CertificateSubjectName \"{1}\" -Timestamper \"{2}\"", fileToSign, codeSignCertificateSubjectName, codeSignTimeStampUri)
            });

            Information("Signing NuGet package exited with '{0}'", exitCode);
        }
    }
});

//-------------------------------------------------------------

Task("Default")
	.IsDependentOn("Build");

//-------------------------------------------------------------

RunTarget(target);