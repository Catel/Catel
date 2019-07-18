#l "notifications-msteams.cake"
//#l "notifications-slack.cake"

//-------------------------------------------------------------

public enum NotificationType
{
    Info,

    Error
}

//-------------------------------------------------------------

public async Task NotifyDefaultAsync(string project, string message, TargetType targetType = TargetType.Unknown)
{
    await NotifyAsync(project, message, targetType, NotificationType.Info);
}

//-------------------------------------------------------------

public async Task NotifyErrorAsync(string project, string message, TargetType targetType = TargetType.Unknown)
{
    await NotifyAsync(project, string.Format("ERROR: {0}", message), targetType, NotificationType.Error);
}

//-------------------------------------------------------------

public async Task NotifyAsync(string project, string message, TargetType targetType = TargetType.Unknown, NotificationType notificationType = NotificationType.Info)
{
    await NotifyMsTeamsAsync(project, message, targetType, notificationType);

    // TODO: Add more notification systems here such as Slack
}