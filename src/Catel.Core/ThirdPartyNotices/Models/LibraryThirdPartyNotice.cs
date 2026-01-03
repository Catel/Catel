namespace Catel.ThirdPartyNotices
{
    public class LibraryThirdPartyNotice : ResourceBasedThirdPartyNotice
    {
        private const string RelativeResourcePath = "Resources.ThirdPartyNotices.library.txt";

        public LibraryThirdPartyNotice(string title, string url)
            : base(title, url, title, RelativeResourcePath)
        {
        }

        public LibraryThirdPartyNotice(string title, string url, string assemblyName)
            : base(title, url, assemblyName, RelativeResourcePath)
        {
        }

        public LibraryThirdPartyNotice(string title, string url, string assemblyName, string rootNamespace)
            : base(title, url, assemblyName, rootNamespace, RelativeResourcePath)
        {
        }
    }
}
