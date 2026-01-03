namespace Catel.ThirdPartyNotices
{
    using System;
    using System.IO;

    public class FileBasedThirdPartyNotice : ThirdPartyNotice
    {
        public FileBasedThirdPartyNotice(string title, string url, string fileName)
        {
            ArgumentNullException.ThrowIfNull(title);
            ArgumentNullException.ThrowIfNull(fileName);

            Title = title;
            Url = url;
            Content = File.ReadAllText(fileName);
        }
    }
}
