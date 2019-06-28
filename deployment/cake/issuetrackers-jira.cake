#tool "nuget:?package=JiraCli&version=1.2.0-beta0002"

var JiraUrl = GetBuildServerVariable("JiraUrl", showValue: true);
var JiraUsername = GetBuildServerVariable("JiraUsername", showValue: true);
var JiraPassword = GetBuildServerVariable("JiraPassword", showValue: false);
var JiraProjectName = GetBuildServerVariable("JiraProjectName", showValue: true);

//-------------------------------------------------------------

public bool IsJiraAvailable()
{
    if (string.IsNullOrWhiteSpace(JiraUrl))
    {
        return false;
    }

    if (string.IsNullOrWhiteSpace(JiraProjectName))
    {
        return false;
    }

    return true;
}

//-------------------------------------------------------------

public async Task CreateAndReleaseVersionInJiraAsync()
{
    if (!IsJiraAvailable())
    {
        Information("JIRA is not available, skipping JIRA integration");
        return;
    }

    var version = VersionFullSemVer;

    Information("Releasing version '{0}' in JIRA", version);

    // Example call:
    // JiraCli.exe -url %JiraUrl% -user %JiraUsername% -pw %JiraPassword% -action createandreleaseversion 
    // -project %JiraProjectName% -version %GitVersion_FullSemVer% -merge %IsOfficialBuild%

    var nugetPath = Context.Tools.Resolve("JiraCli.exe");
    StartProcess(nugetPath, new ProcessSettings 
    {
        Arguments = new ProcessArgumentBuilder()
            .AppendSwitch("-url", JiraUrl)
            .AppendSwitch("-user", JiraUsername)
            .AppendSwitchSecret("-pw", JiraPassword)
            .AppendSwitch("-action", "createandreleaseversion")
            .AppendSwitch("-project", JiraProjectName)
            .AppendSwitch("-version", version)
            .AppendSwitch("-merge", IsOfficialBuild.ToString())
    });

    Information("Released version in JIRA");
}