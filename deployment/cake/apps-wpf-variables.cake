#l "buildserver.cake"

// Generic
var DeploymentsShare = GetBuildServerVariable("DeploymentsShare");
var Channel = GetBuildServerVariable("Channel");
var UpdateDeploymentsShare = bool.Parse(GetBuildServerVariable("UpdateDeploymentsShare", "true"));

// Inno Setup

// Squirrel

// Azure sync
var AzureDeploymentsStorageConnectionString = GetBuildServerVariable("AzureDeploymentsStorageConnectionString");


var WpfApps = WpfAppsToBuild ?? new string[] { };