namespace Catel.ThirdPartyNotices;

public interface IThirdPartyNotice
{
    string Content { get; }
    string Title { get; }
    string Url { get; }
}