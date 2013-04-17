#pragma warning disable 1591    // 1591 = missing xml

namespace Catel.Windows.PopupBehavior
{
    /// <summary>
    /// Popup orientation.
    /// </summary>
    /// <remarks>
    /// Original code can be found at: http://kentb.blogspot.com/2010/07/silverlight-popup-with-target-placement.html
    /// </remarks>
    public class PopupOrientation
    {
        public PopupPlacement Placement
        {
            get;
            set;
        }

        public PopupHorizontalAlignment HorizontalAlignment
        {
            get;
            set;
        }

        public PopupVerticalAlignment VerticalAlignment
        {
            get;
            set;
        }
    }
}