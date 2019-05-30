#l "buildserver.cake"

var GitHubPagesRepositoryUrl = GetBuildServerVariable("GitHubPagesRepositoryUrl", RepositoryUrl, showValue: true);
var GitHubPagesBranchName = GetBuildServerVariable("GitHubPagesRepositoryUrl", "gh-pages", showValue: true);
var GitHubPagesEmail = GetBuildServerVariable("GitHubPagesEmail", showValue: true);
var GitHubPagesUserName = GetBuildServerVariable("GitHubPagesUserName", showValue: true);
var GitHubPagesApiToken = GetBuildServerVariable("GitHubPagesApiToken", showValue: false);

//-------------------------------------------------------------

List<string> _gitHubPages;

public List<string> GitHubPages
{
    get 
    {
        if (_gitHubPages is null)
        {
            _gitHubPages = new List<string>();
        }

        return _gitHubPages;
    }
}