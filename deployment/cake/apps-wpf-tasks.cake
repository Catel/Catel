#l "apps-wpf-variables.cake"

#tool "Squirrel.Windows" 

#addin nuget:?package=Cake.Squirrel&version=0.13.0
#addin nuget:?package=MagicChunks&version=2.0.0.119

//-------------------------------------------------------------

private bool HasWpfApps()
{
    return WpfApps != null && WpfApps.Length > 0;
}

//-------------------------------------------------------------

private void UpdateInfoForWpfApps()
{
    if (!HasWpfApps())
    {
        return;
    }

    // No specific implementation required for now    
}

//-------------------------------------------------------------

private void BuildWpfApps()
{
    if (!HasWpfApps())
    {
        return;
    }
    
    foreach (var wpfApp in WpfApps)
    {
        Information("Building WPF app '{0}'", wpfApp);

        var projectFileName = string.Format("./src/{0}/{0}.csproj", wpfApp);
        
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
        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, wpfApp);
        Information("Output directory: '{0}'", outputDirectory);
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);

        MSBuild(projectFileName, msBuildSettings);
    }
}

//-------------------------------------------------------------

private void PackageWpfAppUsingInnoSetup(string wpfApp, string channel)
{
    var innoSetupTemplateDirectory = string.Format("./deployment/innosetup/{0}", wpfApp);
    if (!DirectoryExists(innoSetupTemplateDirectory))
    {
        Information("Skip packaging of WPF app '{0}' using Inno Setup since no Inno Setup template is present");
        return;
    }

    Information("Packaging WPF app '{0}' using Inno Setup", wpfApp);

    var installersOnDeploymentsShare = string.Format("{0}/{1}/installer", DeploymentsShare, wpfApp);
    CreateDirectory(installersOnDeploymentsShare);

    var setupPostfix = string.Empty;
    if (!string.Equals(channel, "stable", StringComparison.OrdinalIgnoreCase))
    {
        setupPostfix = string.Format("_{0}", channel.ToLower());
    }

    var innoSetupOutputRoot = string.Format("{0}/innosetup/{1}", OutputRootDirectory, wpfApp);
    var innoSetupReleasesRoot = string.Format("{0}/releases", innoSetupOutputRoot);
    var innoSetupOutputIntermediate = string.Format("{0}/intermediate", innoSetupOutputRoot);

    CreateDirectory(innoSetupReleasesRoot);
    CreateDirectory(innoSetupOutputIntermediate);

    // Set up InnoSetup template
    CopyDirectory(innoSetupTemplateDirectory, innoSetupOutputIntermediate);

    var innoSetupScriptFileName = string.Format("{0}/setup.iss", innoSetupOutputIntermediate);
    var fileContents = System.IO.File.ReadAllText(innoSetupScriptFileName);
    fileContents = fileContents.Replace("[VERSION]", VersionMajorMinorPatch);
    fileContents = fileContents.Replace("[VERSION_DISPLAY]", VersionFullSemVer);
    fileContents = fileContents.Replace("[WIZARDIMAGEFILE]", string.Format("logo_large{0}", setupPostfix));

    var signTool = string.Empty;
    if (!string.IsNullOrWhiteSpace(CodeSignCertificateSubjectName))
    {
        signTool = string.Format("SignTool={0}", CodeSignCertificateSubjectName);
    }

    fileContents = fileContents.Replace("[SIGNTOOL]", signTool);
    System.IO.File.WriteAllText(innoSetupScriptFileName, fileContents);

    // Copy all files to the intermediate directory so Inno Setup knows what to do
    var appSourceDirectory = string.Format("{0}/{1}/**/*", OutputRootDirectory, wpfApp);
    var appTargetDirectory = innoSetupOutputIntermediate;

    Information("Copying files from '{0}' => '{1}'", appSourceDirectory, appTargetDirectory);

    CopyFiles(appSourceDirectory, appTargetDirectory, true);

    Information("Generating Inno Setup packages, this can take a while, especially when signing is enabled...");

    InnoSetup(innoSetupScriptFileName, new InnoSetupSettings
    {
        OutputDirectory = innoSetupReleasesRoot
    });

    if (UpdateDeploymentsShare)
    {
        Information("Copying Inno Setup files to deployments share at '{0}'", installersOnDeploymentsShare);

        // Copy the following files:
        // - Setup.exe => [wpfApp]-[version].exe
        // - Setup.exe => [wpfApp]-[channel].exe

        var installerSourceFile = string.Format("{0}/{1}_{2}.exe", innoSetupReleasesRoot, wpfApp, VersionFullSemVer);
        CopyFile(installerSourceFile, string.Format("{0}/{1}_{2}.exe", installersOnDeploymentsShare, wpfApp, VersionFullSemVer));
        CopyFile(installerSourceFile, string.Format("{0}/{1}{2}.exe", installersOnDeploymentsShare, wpfApp, setupPostfix));
    }   
}
//-------------------------------------------------------------

private void PackageWpfAppUsingSquirrel(string wpfApp, string channel)
{
    var squirrelOutputRoot = string.Format("{0}/squirrel/{1}/{2}", OutputRootDirectory, wpfApp, channel);
    var squirrelReleasesRoot = string.Format("{0}/releases", squirrelOutputRoot);
    var squirrelOutputIntermediate = string.Format("{0}/intermediate", squirrelOutputRoot);

    var nuSpecTemplateFileName = string.Format("./deployment/squirrel/template/{0}.nuspec", wpfApp);
    var nuSpecFileName = string.Format("{0}/{1}.nuspec", squirrelOutputIntermediate, wpfApp);
    var nuGetFileName = string.Format("{0}/{1}.{2}.nupkg", squirrelOutputIntermediate, wpfApp, VersionNuGet);

    if (!FileExists(nuSpecTemplateFileName))
    {
        Information("Skip packaging of WPF app '{0}' using Squirrel since no Squirrel template is present");
        return;
    }

    Information("Packaging WPF app '{0}' using Squirrel", wpfApp);

    CreateDirectory(squirrelReleasesRoot);
    CreateDirectory(squirrelOutputIntermediate);

    // Set up Squirrel nuspec
    CopyFile(nuSpecTemplateFileName, nuSpecFileName);

    TransformConfig(nuSpecFileName,
        new TransformationCollection {
            { "package/metadata/version", VersionNuGet },
            { "package/metadata/authors", Company },
            { "package/metadata/owners", Company },
            { "package/metadata/copyright", string.Format("Copyright © {0} {1} - {2}", Company, StartYear, DateTime.Now.Year) },
        });

    // Copy all files to the lib so Squirrel knows what to do
    var appSourceDirectory = string.Format("{0}/{1}", OutputRootDirectory, wpfApp);
    var appTargetDirectory = string.Format("{0}/lib", squirrelOutputIntermediate);

    Information("Copying files from '{0}' => '{1}'", appSourceDirectory, appTargetDirectory);

    CopyDirectory(appSourceDirectory, appTargetDirectory);

    // Create NuGet package
    NuGetPack(nuSpecFileName, new NuGetPackSettings
    {
        OutputDirectory = squirrelOutputIntermediate,
    });

    // Copy deployments share to the intermediate root so we can locally create the Squirrel releases
    var releasesSourceDirectory = string.Format("{0}/{1}/{2}", DeploymentsShare, wpfApp, channel);
    var releasesTargetDirectory = squirrelReleasesRoot;

    Information("Copying releases from '{0}' => '{1}'", releasesSourceDirectory, releasesTargetDirectory);

    CopyDirectory(releasesSourceDirectory, releasesTargetDirectory);

    // Squirrelify!
    var squirrelSettings = new SquirrelSettings();
    squirrelSettings.NoMsi = false;
    squirrelSettings.ReleaseDirectory = squirrelReleasesRoot;
    squirrelSettings.LoadingGif = "./deployment/squirrel/loading.gif";

    // Note: this is not really generic, but this is where we store our icons file, we can
    // always change this in the future
    var iconFileName = "./design/logo/logo.ico";
    squirrelSettings.Icon = iconFileName;
    squirrelSettings.SetupIcon = iconFileName;

    if (!string.IsNullOrWhiteSpace(CodeSignCertificateSubjectName))
    {
        squirrelSettings.SigningParameters = string.Format("/a /t {0} /n {1}", CodeSignTimeStampUri, CodeSignCertificateSubjectName);
    }

    Information("Generating Squirrel packages, this can take a while, especially when signing is enabled...");

    Squirrel(nuGetFileName, squirrelSettings);

    if (UpdateDeploymentsShare)
    {
        Information("Copying updated Squirrel files back to deployments share at '{0}'", releasesSourceDirectory);

        // Copy the following files:
        // - [version]-full.nupkg
        // - [version]-full.nupkg
        // - Setup.exe => Setup.exe & WpfApp.exe
        // - Setup.msi
        // - RELEASES            

        var squirrelFiles = GetFiles(string.Format("{0}/{1}-{2}*.nupkg", squirrelReleasesRoot, wpfApp, VersionNuGet));
        CopyFiles(squirrelFiles, releasesSourceDirectory);
        CopyFile(string.Format("{0}/Setup.exe", squirrelReleasesRoot), string.Format("{0}/Setup.exe", releasesSourceDirectory));
        CopyFile(string.Format("{0}/Setup.exe", squirrelReleasesRoot), string.Format("{0}/{1}.exe", releasesSourceDirectory, wpfApp));
        CopyFile(string.Format("{0}/Setup.msi", squirrelReleasesRoot), string.Format("{0}/Setup.msi", releasesSourceDirectory));
        CopyFile(string.Format("{0}/RELEASES", squirrelReleasesRoot), string.Format("{0}/RELEASES", releasesSourceDirectory));
    }    
}

//-------------------------------------------------------------

private void PackageWpfApps()
{
    if (!HasWpfApps())
    {
        return;
    }
    
    if (string.IsNullOrWhiteSpace(DeploymentsShare))
    {
        Warning("DeploymentsShare variable is not set, cannot package WPF apps");
        return;
    }

    var channels = new List<string>();

    if (IsOfficialBuild)
    {
        // All channels
        channels.Add("alpha");
        channels.Add("beta");
        channels.Add("stable");
    }
    else
    {
        // Single channel        
        channels.Add(Channel);
    }

    foreach (var wpfApp in WpfApps)
    {
        foreach (var channel in channels)
        {
            Information("Packaging WPF app '{0}' for channel '{1}'", wpfApp, channel);

            PackageWpfAppUsingInnoSetup(wpfApp, channel);
            PackageWpfAppUsingSquirrel(wpfApp, channel);
        }
    }
}

//-------------------------------------------------------------

Task("UpdateInfoForWpfApps")
    .IsDependentOn("Clean")
    .Does(() =>
{
    UpdateSolutionAssemblyInfo();
    UpdateInfoForWpfApps();
});

//-------------------------------------------------------------

Task("BuildWpfApps")
    .IsDependentOn("UpdateInfoForWpfApps")
    .Does(() =>
{
    BuildWpfApps();
});

//-------------------------------------------------------------

Task("PackageWpfApps")
    .IsDependentOn("BuildWpfApps")
    .Does(() =>
{
    PackageWpfApps();
});