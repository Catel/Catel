#l "./buildserver.cake"

var OctopusRepositoryUrl = GetBuildServerVariable("OctopusRepositoryUrl");
var OctopusRepositoryApiKey = GetBuildServerVariable("OctopusRepositoryApiKey");
var OctopusDeploymentTarget = GetBuildServerVariable("OctopusDeploymentTarget", "Staging");

var WebApps = WebAppsToBuild ?? new string[] { };