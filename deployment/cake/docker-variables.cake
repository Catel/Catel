#l "buildserver.cake"

var DockerRegistryUrl = GetBuildServerVariable("DockerRegistryUrl");
var DockerRegistryApiKey = GetBuildServerVariable("DockerRegistryApiKey");

//-------------------------------------------------------------

List<string> _dockerImages;

public List<string> DockerImages
{
    get 
    {
        if (_dockerImages is null)
        {
            _dockerImages = new List<string>();
        }

        return _dockerImages;
    }
}