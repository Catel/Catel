#l "buildserver.cake"

var ToolsNuGetRepositoryUrls = GetBuildServerVariable("ToolsNuGetRepositoryUrls", showValue: true);
var ToolsNuGetRepositoryApiKeys = GetBuildServerVariable("ToolsNuGetRepositoryApiKeys", showValue: false);

//-------------------------------------------------------------

List<string> _tools;

public List<string> Tools
{
    get 
    {
        if (_tools is null)
        {
            _tools = new List<string>();
        }

        return _tools;
    }
}