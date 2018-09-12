#l "buildserver.cake"

// Copyright info
var Company = GetBuildServerVariable("Company");
var StartYear = GetBuildServerVariable("StartYear");

// Versioning
var VersionMajorMinorPatch = GetBuildServerVariable("GitVersion_MajorMinorPatch", "unknown");
var VersionFullSemVer = GetBuildServerVariable("GitVersion_FullSemVer", "unknown");
var VersionNuGet = GetBuildServerVariable("GitVersion_NuGetVersion", "unknown");

if (VersionNuGet == "unknown")
{
    Information("No version info specified, falling back to GitVersion");

    // Fallback to GitVersion
    var gitVersion = GitVersion(new GitVersionSettings 
    {
        UpdateAssemblyInfo = false
    });
    
    VersionMajorMinorPatch = gitVersion.MajorMinorPatch;
    VersionFullSemVer = gitVersion.FullSemVer;
    VersionNuGet = gitVersion.NuGetVersionV2;
}

// NuGet
var NuGetPackageSources = GetBuildServerVariable("NuGetPackageSources");
var NuGetExe = "./tools/nuget.exe";
var NuGetLocalPackagesDirectory = "c:\\source\\_packages";

// Solution / build info
var SolutionName = GetBuildServerVariable("SolutionName");
var SolutionAssemblyInfoFileName = "./src/SolutionAssemblyInfo.cs";
var SolutionFileName = string.Format("./src/{0}", string.Format("{0}.sln", SolutionName));
var IsCiBuild = bool.Parse(GetBuildServerVariable("IsCiBuild", "False"));
var IsAlphaBuild = bool.Parse(GetBuildServerVariable("IsAlphaBuild", "False"));
var IsBetaBuild = bool.Parse(GetBuildServerVariable("IsBetaBuild", "False"));
var IsOfficialBuild = bool.Parse(GetBuildServerVariable("IsOfficialBuild", "False"));
var ConfigurationName = GetBuildServerVariable("ConfigurationName", "Release");
var OutputRootDirectory = GetBuildServerVariable("OutputRootDirectory", string.Format("./output/{0}", ConfigurationName));

// Code signing
var CodeSignWildCard = GetBuildServerVariable("CodeSignWildcard");
var CodeSignCertificateSubjectName = GetBuildServerVariable("CodeSignCertificateSubjectName", Company);
var CodeSignTimeStampUri = GetBuildServerVariable("CodeSignTimeStampUri", "http://timestamp.comodoca.com/authenticode");

// Repository info
var RepositoryUrl = GetBuildServerVariable("RepositoryUrl");
var RepositoryBranchName = GetBuildServerVariable("RepositoryBranchName");
var RepositoryCommitId = GetBuildServerVariable("RepositoryCommitId");

// SonarQube
var SonarUrl = GetBuildServerVariable("SonarUrl");
var SonarUsername = GetBuildServerVariable("SonarUsername");
var SonarPassword = GetBuildServerVariable("SonarPassword");
var SonarProject = GetBuildServerVariable("SonarProject", SolutionName);

//-------------------------------------------------------------

// Update some variables (like expanding paths, etc)

OutputRootDirectory = System.IO.Path.GetFullPath(OutputRootDirectory);

//-------------------------------------------------------------

List<string> _testProjects;

public List<string> TestProjects
{
    get 
    {
        if (_testProjects is null)
        {
            _testProjects = new List<string>();
        }

        return _testProjects;
    }
}