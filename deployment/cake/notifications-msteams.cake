#addin "nuget:?package=Cake.MicrosoftTeams&version=0.9.0"

var MsTeamsWebhookUrl = GetBuildServerVariable("MsTeamsWebhookUrl", showValue: false);
var MsTeamsWebhookUrlForErrors = GetBuildServerVariable("MsTeamsWebhookUrlForErrors", MsTeamsWebhookUrl, showValue: false);

//-------------------------------------------------------------

public string GetMsTeamsWebhookUrl(string project, TargetType targetType)
{
    // Allow per target overrides via "MsTeamsWebhookUrlFor[TargetType]"
    var targetTypeUrl = GetTargetSpecificConfigurationValue(targetType, "MsTeamsWebhookUrlFor", string.Empty);
    if (!string.IsNullOrEmpty(targetTypeUrl))
    {
        return targetTypeUrl;
    }

    // Allow per project overrides via "MsTeamsWebhookUrlFor[ProjectName]"
    var projectTypeUrl = GetProjectSpecificConfigurationValue(project, "MsTeamsWebhookUrlFor", string.Empty);
    if (!string.IsNullOrEmpty(projectTypeUrl))
    {
        return projectTypeUrl;
    }

    // Return default fallback
    return MsTeamsWebhookUrl;
}

//-------------------------------------------------------------

public string GetMsTeamsTarget(string project, TargetType targetType, NotificationType notificationType)
{
    if (notificationType == NotificationType.Error)
    {
        return MsTeamsWebhookUrlForErrors;
    }

    return GetMsTeamsWebhookUrl(project, targetType);
}

//-------------------------------------------------------------

public async Task NotifyMsTeamsAsync(string project, string message, TargetType targetType, NotificationType notificationType)
{
    var targetWebhookUrl = GetMsTeamsTarget(project, targetType, notificationType);
    if (string.IsNullOrWhiteSpace(targetWebhookUrl))
    {
        return;
    }

    var messageCard = new MicrosoftTeamsMessageCard 
    {
        title = project,
        summary = notificationType.ToString(),
        sections = new []
        {
            new MicrosoftTeamsMessageSection
            {
                activityTitle = notificationType.ToString(),
                activitySubtitle = message,
                activityText = " ",
                activityImage = "https://raw.githubusercontent.com/cake-build/graphics/master/png/cake-small.png",
                facts = new [] 
                {
                    new MicrosoftTeamsMessageFacts { name ="Project", value = project },
                    new MicrosoftTeamsMessageFacts { name ="Version", value = VersionFullSemVer },
                    new MicrosoftTeamsMessageFacts { name ="CakeVersion", value = Context.Environment.Runtime.CakeVersion.ToString() },
                    //new MicrosoftTeamsMessageFacts { name ="TargetFramework", value = Context.Environment.Runtime .TargetFramework.ToString() },
                },
            }
        }
    };

    var result = MicrosoftTeamsPostMessage(messageCard, new MicrosoftTeamsSettings 
    {
        IncomingWebhookUrl = targetWebhookUrl
    });

    if (result != System.Net.HttpStatusCode.OK)
    {
        Warning(string.Format("MsTeams result: {0}", result));
    }
}