// Customize this file when using a different issue tracker
// #l "buildserver-github.cake"
#l "issuetrackers-jira.cake"

//-------------------------------------------------------------

public async Task CreateAndReleaseVersionAsync()
{
    LogSeparator("Creating and releasing version");

    await CreateAndReleaseVersionInJiraAsync();

    // TODO: Add more issue tracker integrations (such as GitHub)
}
