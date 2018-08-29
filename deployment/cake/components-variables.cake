#l "buildserver.cake"

var NuGetRepositoryUrl = GetBuildServerVariable("NuGetRepositoryUrl");
var NuGetRepositoryApiKey = GetBuildServerVariable("NuGetRepositoryApiKey");

var Components = ComponentsToBuild ?? new string[] { };