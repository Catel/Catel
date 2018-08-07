#l "./buildserver.cake"

var WindowsStoreAppId = GetBuildServerVariable("WindowsStoreAppId");
var WindowsStoreClientId = GetBuildServerVariable("WindowsStoreClientId");
var WindowsStoreClientSecret = GetBuildServerVariable("WindowsStoreClientSecret");
var WindowsStoreTenantId = GetBuildServerVariable("WindowsStoreTenantId");

var UwpApps = UwpAppsToBuild ?? new string[] { };