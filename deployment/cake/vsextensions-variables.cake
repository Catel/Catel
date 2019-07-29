#l "buildserver.cake"

var VsExtensionsPublisherName = GetBuildServerVariable("VsExtensionsPublisherName", showValue: true);
var VsExtensionsPersonalAccessToken = GetBuildServerVariable("VsExtensionsPersonalAccessToken", showValue: false);

//-------------------------------------------------------------

List<string> _vsExtensions;

public List<string> VsExtensions
{
    get 
    {
        if (_vsExtensions is null)
        {
            _vsExtensions = new List<string>();
        }

        return _vsExtensions;
    }
}