#l "./buildserver.cake"

var WindowsStoreAppId = GetBuildServerVariable("WindowsStoreAppId", string.Empty);
var WindowsStoreClientId = GetBuildServerVariable("WindowsStoreClientId", string.Empty);
var WindowsStoreClientSecret = GetBuildServerVariable("WindowsStoreClientSecret", string.Empty);
var WindowsStoreTenantId = GetBuildServerVariable("WindowsStoreTenantId", string.Empty);

var UwpApps = UwpAppsToBuild ?? new string[] { };