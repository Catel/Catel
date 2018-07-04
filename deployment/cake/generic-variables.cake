#l "buildserver.cake"

// Copyright info
var Company = GetBuildServerVariable("Company", DefaultCompany);

// Versioning
var VersionMajorMinorPatch = GetBuildServerVariable("GitVersion_MajorMinorPatch", "3.0.0");
var VersionFullSemVer = GetBuildServerVariable("GitVersion_FullSemVer", "3.0.0-alpha.1");
var VersionNuGet = GetBuildServerVariable("GitVersion_NuGetVersion", "3.0.0-alpha0001");

// NuGet
var NuGetPackageSources = GetBuildServerVariable("NuGetPackageSources", string.Empty);
var NuGetExe = "./tools/nuget.exe";

// Solution / build info
var SolutionName = GetBuildServerVariable("SolutionName", DefaultSolutionName);
var SolutionAssemblyInfoFileName = "./src/SolutionAssemblyInfo.cs";
var SolutionFileName = string.Format("./src/{0}", SolutionName);
var IsCiBuild = bool.Parse(GetContinuaCIVariable("IsCiBuild", "False"));
var IsOfficialBuild = bool.Parse(GetContinuaCIVariable("IsOfficialBuild", "False"));
var ConfigurationName = GetBuildServerVariable("ConfigurationName", "Release");
var OutputRootDirectory = GetBuildServerVariable("OutputRootDirectory", string.Format("./output/{0}", ConfigurationName));

// Code signing
var CodeSignWildCard = GetBuildServerVariable("CodeSignWildcard", SolutionName);
var CodeSignCertificateSubjectName = GetBuildServerVariable("CodeSignCertificateSubjectName", Company);
var CodeSignTimeStampUri = GetBuildServerVariable("CodeSignTimeStampUri", "http://timestamp.comodoca.com/authenticode");

// Repository info
var RepositoryUrl = GetBuildServerVariable("RepositoryUrl", DefaultRepositoryUrl);
var RepositoryBranchName = GetBuildServerVariable("RepositoryBranchName", string.Empty);
var RepositoryCommitId = GetBuildServerVariable("RepositoryCommitId", string.Empty);

// SonarQube
var SonarUrl = GetBuildServerVariable("SonarUrl", string.Empty);
var SonarUsername = GetBuildServerVariable("SonarUsername", string.Empty);
var SonarPassword = GetBuildServerVariable("SonarPassword", string.Empty);
var SonarProject = GetBuildServerVariable("SonarProject", SolutionName);

//-------------------------------------------------------------

// Update some variables (like expanding paths, etc)

OutputRootDirectory = System.IO.Path.GetFullPath(OutputRootDirectory);
var TestProjects = TestProjectsToBuild ?? new string[] { };