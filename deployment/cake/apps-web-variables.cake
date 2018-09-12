#l "./buildserver.cake"

var OctopusRepositoryUrl = GetBuildServerVariable("OctopusRepositoryUrl");
var OctopusRepositoryApiKey = GetBuildServerVariable("OctopusRepositoryApiKey");
var OctopusDeploymentTarget = GetBuildServerVariable("OctopusDeploymentTarget", "Staging");

//-------------------------------------------------------------

List<string> _webApps;

public List<string> WebApps
{
    get 
    {
        if (_webApps is null)
        {
            _webApps = new List<string>();
        }

        return _webApps;
    }
}