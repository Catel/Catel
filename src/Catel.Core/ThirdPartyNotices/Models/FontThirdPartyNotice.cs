namespace Catel.ThirdPartyNotices;

using System;

public class FontThirdPartyNotice : ThirdPartyNotice
{
    public FontThirdPartyNotice(string fontName, string fontUrl)
    {
        ArgumentNullException.ThrowIfNull(fontName);
        ArgumentNullException.ThrowIfNull(fontUrl);

        Title = fontName;
        Url = fontUrl;
    }
}
