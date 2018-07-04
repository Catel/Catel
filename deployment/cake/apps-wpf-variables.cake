#l "buildserver.cake"

// Generic
var DeploymentsShare = GetBuildServerVariable("DeploymentsShare", string.Empty);
var Channel = GetBuildServerVariable("Channel", string.Empty);
var UpdateDeploymentsShare = bool.Parse(GetBuildServerVariable("UpdateDeploymentsShare", "true"));

// Inno Setup

// Squirrel


var WpfApps = WpfAppsToBuild ?? new string[] { };