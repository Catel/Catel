#l "generic-variables.cake"

#addin "nuget:?package=MagicChunks&version=2.0.0.119"
#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"
#addin "nuget:?package=Cake.DependencyCheck&version=1.2.0"

#tool "nuget:?package=DependencyCheck.Runner.Tool&include=./**/dependency-check.sh&include=./**/dependency-check.bat"
#tool "nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2018.1.3"

//-------------------------------------------------------------

private void ValidateRequiredInput(string parameterName)
{
    if (!Parameters.ContainsKey(parameterName))
    {
        throw new Exception(string.Format("Parameter '{0}' is required but not defined", parameterName));
    }
}

//-------------------------------------------------------------

private void ValidateGenericInput()
{
    ValidateRequiredInput("SolutionName");
    ValidateRequiredInput("Company");
    ValidateRequiredInput("RepositoryUrl");
}

//-------------------------------------------------------------

private void CleanUpCode(bool failOnChanges)
{
    Information("Cleaning up code using dotnet-format");

    // --check: return non-0 exit code if changes are needed
    // --dry-run: don't save files

    // Note: disabled for now, see:
    // * https://github.com/onovotny/MSBuildSdkExtras/issues/164
    // * https://github.com/microsoft/msbuild/issues/4376
    // var arguments = new List<string>();

    // //arguments.Add("--dry-run");

    // if (failOnChanges)
    // {
    //     arguments.Add("--check");
    // }

    // DotNetCoreTool(null, "format", string.Join(" ", arguments),
    //     new DotNetCoreToolSettings
    //     {
    //         WorkingDirectory = "./src/"
    //     });
}

//-------------------------------------------------------------

private void VerifyDependencies(string pathToScan = "./src/**/*.csproj")
{
    Information("Verifying dependencies for security vulnerabilities in '{0}'", pathToScan);

    DependencyCheck(new DependencyCheckSettings
    {
        Project = SolutionName,
        Scan = pathToScan,
        FailOnCVSS = "0",
        Format = "HTML",
        Data = "%temp%/dependency-check/data"
    });
}

//-------------------------------------------------------------

private void UpdateSolutionAssemblyInfo()
{
    Information("Updating assembly info to '{0}'", VersionFullSemVer);

    var assemblyInfoParseResult = ParseAssemblyInfo(SolutionAssemblyInfoFileName);

    var assemblyInfo = new AssemblyInfoSettings 
    {
        Company = assemblyInfoParseResult.Company,
        Version = VersionMajorMinorPatch,
        FileVersion = VersionMajorMinorPatch,
        InformationalVersion = VersionFullSemVer,
        Copyright = string.Format("Copyright © {0} {1} - {2}", Company, StartYear, DateTime.Now.Year)
    };

    CreateAssemblyInfo(SolutionAssemblyInfoFileName, assemblyInfo);
}

//-------------------------------------------------------------

Task("UpdateNuGet")
    .ContinueOnError()
    .Does(() => 
{
    Information("Making sure NuGet is using the latest version");

    var exitCode = StartProcess(NuGetExe, new ProcessSettings
    {
        Arguments = "update -self"
    });

    var newNuGetVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(NuGetExe);
    var newNuGetVersion = newNuGetVersionInfo.FileVersion;

    Information("Updating NuGet.exe exited with '{0}', version is '{1}'", exitCode, newNuGetVersion);
});

//-------------------------------------------------------------

Task("RestorePackages")
    .IsDependentOn("UpdateNuGet")
    .ContinueOnError()
    .Does(() =>
{
    var projects = GetFiles("./**/*.csproj");
    var solutions = GetFiles("./**/*.sln");
    
    var allFiles = new List<FilePath>();
    //allFiles.AddRange(projects);
    allFiles.AddRange(solutions);

    foreach(var file in allFiles)
    {
        RestoreNuGetPackages(file);
    }
});

//-------------------------------------------------------------

// Note: it might look weird that this is dependent on restore packages,
// but to clean, the msbuild projects must be able to load. However, they need
// some targets files that come in via packages

Task("Clean")
    .IsDependentOn("RestorePackages")
    .ContinueOnError()
    .Does(() => 
{
    var platforms = new Dictionary<string, PlatformTarget>();
    platforms["AnyCPU"] = PlatformTarget.MSIL;
    platforms["x86"] = PlatformTarget.x86;
    platforms["x64"] = PlatformTarget.x64;
    platforms["arm"] = PlatformTarget.ARM;

    foreach (var platform in platforms)
    {
        try
        {
            Information("Cleaning output for platform '{0}'", platform.Value);

            var msBuildSettings = new MSBuildSettings
            {
                Verbosity = Verbosity.Minimal,
                ToolVersion = MSBuildToolVersion.Default,
                Configuration = ConfigurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = platform.Value
            };

            ConfigureMsBuild(msBuildSettings, platform.Key, "clean");

            msBuildSettings.Targets.Add("Clean");

            MSBuild(SolutionFileName, msBuildSettings);
        }
        catch (System.Exception ex)
        {
            Warning("Failed to clean output for platform '{0}': {1}", platform.Value, ex.Message);
        }
    }

    if (DirectoryExists(OutputRootDirectory))
    {
        DeleteDirectory(OutputRootDirectory, new DeleteDirectorySettings()
        {
            Force = true,
            Recursive = true
        });
    }
});

//-------------------------------------------------------------

Task("VerifyDependencies")
    .IsDependentOn("Prepare")
    .Does(async () =>
{
    if (DependencyCheckDisabled)
    {
        Information("Dependency analysis is disabled");
        return;
    }

    VerifyDependencies();
});

//-------------------------------------------------------------

Task("CleanupCode")
    .Does(() => 
{
    CleanUpCode(true);
});

//-------------------------------------------------------------

Task("CodeSign")
    .ContinueOnError()
    .Does(() =>
{
    if (IsCiBuild)
    {
        Information("Skipping code signing because this is a CI build");
        return;
    }

    if (IsLocalBuild)
    {
        Information("Skipping code signing because this is a local package build");
        return;
    }

    if (string.IsNullOrWhiteSpace(CodeSignCertificateSubjectName))
    {
        Information("Skipping code signing because the certificate subject name was not specified");
        return;
    }

    List<FilePath> filesToSign = new List<FilePath>();

    // Note: only code-sign components & wpf apps, skip test projects & uwp apps
    var projectsToCodeSign = new List<string>();
    projectsToCodeSign.AddRange(Components);
    projectsToCodeSign.AddRange(WpfApps);

    foreach (var projectToCodeSign in projectsToCodeSign)
    {
        var codeSignWildCard = CodeSignWildCard;
        if (string.IsNullOrWhiteSpace(codeSignWildCard))
        {
            // Empty, we need to override with project name for valid default value
            codeSignWildCard = projectToCodeSign;
        }
    
        var projectFilesToSign = new List<FilePath>();

        var outputDirectory = string.Format("{0}/{1}", OutputRootDirectory, projectToCodeSign);

        var exeSignFilesSearchPattern = string.Format("{0}/**/*{1}*.exe", outputDirectory, codeSignWildCard);
        Information(exeSignFilesSearchPattern);
        projectFilesToSign.AddRange(GetFiles(exeSignFilesSearchPattern));

        var dllSignFilesSearchPattern = string.Format("{0}/**/*{1}*.dll", outputDirectory, codeSignWildCard);
        Information(dllSignFilesSearchPattern);
        projectFilesToSign.AddRange(GetFiles(dllSignFilesSearchPattern));

        Information("Found '{0}' files to code sign for '{1}'", projectFilesToSign.Count, projectToCodeSign);

        filesToSign.AddRange(projectFilesToSign);
    }

    if (filesToSign.Count == 0)
    {
        Information("Found no files to sign, skipping code signing process...");
        return;
    }

    Information("Found '{0}' files to code sign using subject name '{1}', this can take a few minutes...", filesToSign.Count, CodeSignCertificateSubjectName);

    var signToolSignSettings = new SignToolSignSettings 
    {
        AppendSignature = false,
        TimeStampUri = new Uri(CodeSignTimeStampUri),
        CertSubjectName = CodeSignCertificateSubjectName
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