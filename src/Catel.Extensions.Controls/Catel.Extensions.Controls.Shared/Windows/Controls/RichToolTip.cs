// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RichToolTip.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Navigation;
    using System.Windows.Threading;

    /// <summary>
    /// A kind-of Tooltip implementation that stays open once element is hovered and the content inside is responsive
    /// <para />
    /// It corresponds to most of the TooltipService attached properties so use them as you wish
    /// <para />
    /// Usage: (Like Tooltip)
    /// <![CDATA[
    /// <Control ToolTipService.Placement="Right">
    ///     <xmlns-pfx:RichToolTip.PopupContent>
    ///         <TextBlock Text="This will be displayed in the popup" />
    ///     </xmlns-pfx:RichToolTip.PopupContent>
    /// </Control>
    /// 
    /// <Control ToolTipService.Placement="Right">
    ///     <xmlns-pfx:RichToolTip.PopupContent>
    ///         <RichToolTip Placement="..." PlacementTarget="..." HorizontalOffset=".." and so on>
    ///             <TextBlock Text="This will be displayed in the popup" />
    ///         </RichToolTip>
    ///     </xmlns-pfx:RichToolTip.PopupContent>
    /// </Control>
    /// ]]>
    /// <para />
    /// Known Issues:
    /// 1 - I didn't have the time nor the strength to care about repositioning. I simply hide the popup whenever it would need repositioning. (Window movement, etc..) But it's ok since it's the default behavior of popup overall.
    /// 2 - XBap mode sets transparency through a hack! supported only in full trust.
    /// 3 - In XBap mode, moving the mouse slowly towards the popup will cause it to hide
    /// 4 - In XBap mode, moving the mouse over the element shows the tooltip even when the browser isn't the active window
    /// </summary>
    /// <remarks>
    /// Originally found at http://blogs.microsoft.co.il/blogs/zuker/archive/2009/01/18/wpf-popups-and-tooltip-behavior-solution.aspx
    /// </remarks>
    public class RichToolTip : ContentControl
    {
        #region Fields
        const int AnimationDurationInMs = 200;
        const int ShowDeferredMilliseconds = 500;
        const bool AnimationEnabledDefault = true;

        delegate void Action();
        Popup _parentPopup;

        static RichToolTip lastShownPopup;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the related object.
        /// </summary>
        /// <value>The related object.</value>
        public UIElement RelatedObject { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable animation or not
        /// </summary>
        /// <value><c>true</c> if animation should be enabled; otherwise, <c>false</c>.</value>
        public bool EnableAnimation { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="RichToolTip"/> class.
        /// </summary>
        static RichToolTip()
        {
            EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseDownEvent, new MouseButtonEventHandler(OnElementMouseDown), true);
            EventManager.RegisterClassHandler(typeof(RichToolTip), ButtonBase.ClickEvent, new RoutedEventHandler(OnButtonBaseClick), false);
            //EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseEnterEvent, new MouseEventHandler(element_MouseEnter), true);
            //EventManager.RegisterClassHandler(typeof(UIElement), UIElement.MouseLeaveEvent, new MouseEventHandler(element_MouseLeave), true);
            EventManager.RegisterClassHandler(typeof(Selector), Selector.SelectionChangedEvent, new SelectionChangedEventHandler(selector_SelectionChangedEvent), true);

            //only in XBap mode
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                EventManager.RegisterClassHandler(typeof(NavigationWindow), UIElement.MouseLeaveEvent, new RoutedEventHandler(OnNavigationWindowMouseLeaveEvent), true);
            }
            else
            {
                EventManager.RegisterClassHandler(typeof(Window), Window.SizeChangedEvent, new RoutedEventHandler(OnWindowSizeChanged), true);
            }

            CommandManager.RegisterClassCommandBinding(typeof(RichToolTip), new CommandBinding(CloseCommand, ExecuteCloseCommand));

            InitStoryboards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichToolTip"/> class.
        /// </summary>
        public RichToolTip()
        {
            Loaded += ContentTooltip_Loaded;
            Unloaded += ContentTooltip_Unloaded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichToolTip"/> class.
        /// </summary>
        /// <param name="relatedObject">The related object.</param>
        public RichToolTip(UIElement relatedObject)
            : this()
        {
            Load(relatedObject);
        }
        #endregion

        #region Loading
        /// <summary>
        /// Loads the specified related object.
        /// </summary>
        /// <param name="relatedObject">The related object.</param>
        internal void Load(UIElement relatedObject)
        {
            RelatedObject = relatedObject;

            FrameworkElement fe = relatedObject as FrameworkElement;

            if (fe == null)
            {
                throw new InvalidOperationException("The element is not supported");
            }

            RelatedObject.MouseEnter += element_MouseEnter;
            RelatedObject.MouseLeave += element_MouseLeave;

            fe.Unloaded += RelatedObject_Unloaded;

            BindRootVisual();
        }

        private void RelatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            RelatedObject.MouseEnter -= element_MouseEnter;
            RelatedObject.MouseLeave -= element_MouseLeave;
        }

        private void ContentTooltip_Unloaded(object sender, RoutedEventArgs e)
        {
            UnbindRootVisual();
        }

        private void ContentTooltip_Loaded(object sender, RoutedEventArgs e)
        {
            BindRootVisual();
        }
        #endregion

        #region Popup Creation
        private static readonly Type PopupType = typeof(Popup);
        private static readonly Type PopupSecurityHelperType = PopupType.GetNestedType("PopupSecurityHelper", BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly FieldInfo Popup_secHelper = PopupType.GetField("_secHelper", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo PopupSecurityHelper_isChildPopupInitialized = PopupSecurityHelperType.GetField("_isChildPopupInitialized", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo PopupSecurityHelper_isChildPopup = PopupSecurityHelperType.GetField("_isChildPopup", BindingFlags.Instance | BindingFlags.NonPublic);

        void HookupParentPopup()
        {
            //Create the Popup and attach the CustomControl to it.
            _parentPopup = new Popup();

            //THIS IS A HACK!
            //This enables transparency on the popup - needed for XBap versions!
            //NOTE - this requires that the xbap app will run in full trust
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                try
                {
                    new ReflectionPermission(PermissionState.Unrestricted).Demand();

                    DoPopupHacks();
                }
                catch (SecurityException) { }
            }

            _parentPopup.AllowsTransparency = true;

            Popup.CreateRootPopup(_parentPopup, this);
        }

        void DoPopupHacks()
        {
            object secHelper = Popup_secHelper.GetValue(_parentPopup);
            PopupSecurityHelper_isChildPopupInitialized.SetValue(secHelper, true);
            PopupSecurityHelper_isChildPopup.SetValue(secHelper, false);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Close command.
        /// </summary>
        public static RoutedCommand CloseCommand = new RoutedCommand("Close", typeof(RichToolTip));
        static void ExecuteCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            HideLastShown(true);
        }
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Placement property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = ToolTipService.PlacementProperty.AddOwner(typeof(RichToolTip));

        /// <summary>
        /// Gets or sets the placement.
        /// </summary>
        /// <value>The placement.</value>
        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// Placement target property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty = ToolTipService.PlacementTargetProperty.AddOwner(typeof(RichToolTip));

        /// <summary>
        /// Gets or sets the placement target.
        /// </summary>
        /// <value>The placement target.</value>
        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        /// <summary>
        /// Placement rectangle property.
        /// </summary>
        public static readonly DependencyProperty PlacementRectangleProperty = ToolTipService.PlacementRectangleProperty.AddOwner(typeof(RichToolTip));

        /// <summary>
        /// Gets or sets the placement rectangle.
        /// </summary>
        /// <value>The placement rectangle.</value>
        public Rect PlacementRectangle
        {
            get { return (Rect)GetValue(PlacementRectangleProperty); }
            set { SetValue(PlacementRectangleProperty, value); }
        }

        /// <summary>
        /// Horizontal offset property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = ToolTipService.HorizontalOffsetProperty.AddOwner(typeof(RichToolTip));

        /// <summary>
        /// Gets or sets the horizontal offset.
        /// </summary>
        /// <value>The horizontal offset.</value>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// Vertical offset property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = ToolTipService.VerticalOffsetProperty.AddOwner(typeof(RichToolTip));

        /// <summary>
        /// Gets or sets the vertical offset.
        /// </summary>
        /// <value>The vertical offset.</value>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// IsOpen property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = Popup.IsOpenProperty.AddOwner(typeof(RichToolTip), 
            new FrameworkPropertyMetadata( false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichToolTip ctrl = (RichToolTip)d;

            if ((bool)e.NewValue)
            {
                if (ctrl._parentPopup == null)
                {
                    ctrl.HookupParentPopup();
                }
            }
        }

        #endregion

        #region Attached Properties

        #region HideOnClick
        /// <summary>
        /// Gets the hide on click.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetHideOnClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(HideOnClickProperty);
        }

        /// <summary>
        /// Sets the hide on click.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetHideOnClick(DependencyObject obj, bool value)
        {
            obj.SetValue(HideOnClickProperty, value);
        }

        /// <summary>
        /// Hide on click property.
        /// </summary>
        public static readonly DependencyProperty HideOnClickProperty =
            DependencyProperty.RegisterAttached("HideOnClick", typeof(bool), typeof(RichToolTip), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region PopupContent
        /// <summary>
        /// Gets the content of the popup.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static object GetPopupContent(DependencyObject obj)
        {
            return obj.GetValue(PopupContentProperty);
        }

        /// <summary>
        /// Sets the content of the popup.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetPopupContent(DependencyObject obj, object value)
        {
            obj.SetValue(PopupContentProperty, value);
        }

        /// <summary>
        /// Popup content property.
        /// </summary>
        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.RegisterAttached("PopupContent", typeof(object), typeof(RichToolTip), new FrameworkPropertyMetadata(OnPopupContentChanged));

        private static void OnPopupContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = o as UIElement;
            if (element == null)
            {
                throw new InvalidOperationException("Can't hook to events other than UI Element");
            }

            if (e.NewValue != null)
            {
                RichToolTip popup = e.NewValue as RichToolTip;

                if (popup != null)
                {
                    popup.Load(element);
                }
                else
                {
                    popup = new RichToolTip(element);

                    Binding binding = new Binding
                    {
                        Path = new PropertyPath(PopupContentProperty),
                        Mode = BindingMode.OneWay,
                        Source = o,
                    };
                    popup.SetBinding(ContentProperty, binding);
                }

                //popup.SetBinding(DataContextProperty, new Binding { Source = element, Path = new PropertyPath(DataContextProperty) });

                SetContentTooltipWrapper(o, popup);
            }
        }
        #endregion

        #region ContentTooltipWrapper
        internal static RichToolTip GetContentTooltipWrapper(DependencyObject obj)
        {
            return (RichToolTip)obj.GetValue(ContentTooltipWrapperProperty);
        }

        internal static void SetContentTooltipWrapper(DependencyObject obj, RichToolTip value)
        {
            obj.SetValue(ContentTooltipWrapperProperty, value);
        }

        internal static readonly DependencyProperty ContentTooltipWrapperProperty =
            DependencyProperty.RegisterAttached("ContentTooltipWrapper", typeof(RichToolTip), typeof(RichToolTip));
        #endregion

        #endregion

        #region Root Visual Binding
        bool boundToRoot = false;
        bool hasParentWindow = false;
        Window parentWindow = null;

        void BindRootVisual()
        {
            if (!boundToRoot)
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    parentWindow = RelatedObject.FindLogicalAncestorByType<Window>() ?? RelatedObject.FindVisualAncestorByType<Window>();

                    if (parentWindow != null)
                    {
                        hasParentWindow = true;

                        parentWindow.Deactivated += window_Deactivated;
                        parentWindow.LocationChanged += window_LocationChanged;
                    }
                }

                boundToRoot = true;
            }
        }

        void UnbindRootVisual()
        {
            if (boundToRoot)
            {
                if (parentWindow != null)
                {
                    parentWindow.Deactivated -= window_Deactivated;
                    parentWindow.LocationChanged -= window_LocationChanged;
                }

                boundToRoot = false;
            }
        }
        #endregion

        #region Animations & Intervals
        static DispatcherTimer _timer;
        static Storyboard showStoryboard;
        static Storyboard hideStoryboard;
        bool setRenderTransform;

        static void InitStoryboards()
        {
            showStoryboard = new Storyboard();
            hideStoryboard = new Storyboard();

            TimeSpan duration = TimeSpan.FromMilliseconds(AnimationDurationInMs);

            DoubleAnimation animation = new DoubleAnimation(1, duration, FillBehavior.Stop);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            showStoryboard.Children.Add(animation);

            animation = new DoubleAnimation(0.1, 1, duration, FillBehavior.Stop);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0).(1)", FrameworkElement.RenderTransformProperty, ScaleTransform.ScaleXProperty));
            showStoryboard.Children.Add(animation);

            animation = new DoubleAnimation(0.1, 1, duration, FillBehavior.Stop);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0).(1)", FrameworkElement.RenderTransformProperty, ScaleTransform.ScaleYProperty));
            showStoryboard.Children.Add(animation);

            animation = new DoubleAnimation(0, duration, FillBehavior.Stop);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            hideStoryboard.Children.Add(animation);

            hideStoryboard.Completed += delegate { OnAnimationCompleted(); };
        }

        static void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(ShowDeferredMilliseconds);
        }

        static void ResetTimer(RichToolTip tooltip)
        {
            if (_timer != null)
            {
                _timer.Tick -= tooltip.ShowDeferred;
                _timer.Stop();
            }
        }

        void Animate(bool show)
        {
            if (show)
            {
                if (!setRenderTransform)
                {
                    RenderTransform = new ScaleTransform();

                    setRenderTransform = true;
                }

                showStoryboard.Begin(this);
            }
            else
            {
                hideStoryboard.Begin(this);
            }
        }

        static void OnAnimationCompleted()
        {
            HideLastShown(false);
        }
        #endregion

        #region Event Invocations
        void element_MouseEnter(object sender, MouseEventArgs e)
        {
            DependencyObject o = sender as DependencyObject;

            //implementation for static subscribing:
            //if (o != null)
            //{
            //    RichToolTip tooltip = GetContentTooltipWrapper(o);

            //    if (tooltip != null && (tooltip != lastShownPopup || tooltip.RelatedObject != o))
            //    {
            //        tooltip.Show();
            //    }
            //}

            if (!IsShown() && this != lastShownPopup)
            {
                if (!hasParentWindow || parentWindow.IsActive)
                {
                    Show(true);
                }
            }
        }

        void element_MouseLeave(object sender, MouseEventArgs e)
        {
            //implementation for static subscribing:
            //DependencyObject o = sender as DependencyObject;

            //if (o != null)
            //{
            //    RichToolTip tooltip = GetContentTooltipWrapper(o);

            //    if (tooltip != null)
            //    {
            //        ResetTimer(tooltip);
            //    }
            //}

            ResetTimer(this);
        }

        static void OnNavigationWindowMouseLeaveEvent(object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            new Action(() =>
            {
                if (lastShownPopup != null && !lastShownPopup.IsMouseOver)
                {
                    HideLastShown(false);
                }
            }));
        }

        void window_LocationChanged(object sender, EventArgs e)
        {
            if (IsShown())
            {
                HideLastShown(false);
            }
        }

        void window_Deactivated(object sender, EventArgs e)
        {
            if (IsShown())
            {
                HideLastShown(false);
            }
        }

        static void OnWindowSizeChanged(object sender, RoutedEventArgs e)
        {
            HideLastShown();
        }

        static void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lastShownPopup != null && lastShownPopup.IsShown())
            {
                RichToolTip popup;

                DependencyObject o = e.OriginalSource as DependencyObject;
                if (!TryFindPopupParent(e.OriginalSource, out popup))
                {
                    HideLastShown(true);
                }
            }
        }

        static void OnButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (lastShownPopup != null && lastShownPopup.IsShown())
            {
                DependencyObject o = e.OriginalSource as DependencyObject;

                bool hide = GetHideOnClick(o);
                if (hide)
                {
                    HideLastShown(true);

                    e.Handled = true;
                }
            }
        }

        static void selector_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            HideLastShown();
        }

        static bool TryFindPopupParent(object source, out RichToolTip popup)
        {
            popup = null;
            UIElement element = source as UIElement;

            if (element != null)
            {
                popup = element.FindVisualAncestorByType<RichToolTip>();

                if (popup == null)
                {
                    popup = element.FindLogicalAncestorByType<RichToolTip>();
                }

                return popup != null;
            }

            return false;
        }
        #endregion

        #region Show / Hide
        bool showAnimate = AnimationEnabledDefault;

        bool IsShown()
        {
            return IsOpen;
        }

        /// <summary>
        /// Shows the specified animate.
        /// </summary>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        public void Show(bool animate)
        {
            showAnimate = animate;

            if (_timer == null)
            {
                InitTimer();
            }

            _timer.Tick += ShowDeferred;

            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        private void ShowDeferred(object sender, EventArgs e)
        {
            ResetTimer(this);

            HideLastShown(false);

            ShowInternal();

            if (showAnimate && EnableAnimation)
            {
                Animate(true);
            }
            else
            {
                this.Opacity = 1;
            }

            lastShownPopup = this;
        }

        private void ShowInternal()
        {
            Visibility = Visibility.Visible;

            IsOpen = true;
        }

        static void HideLastShown()
        {
            HideLastShown(false);
        }

        static void HideLastShown(bool animate)
        {
            if (lastShownPopup != null)
            {
                lastShownPopup.Hide(animate);
            }
        }

        /// <summary>
        /// Hides the specified animate.
        /// </summary>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        public void Hide(bool animate)
        {
            if (animate && EnableAnimation)
            {
                Animate(false);
            }
            else
            {
                HideInternal();
            }
        }

        private void HideInternal()
        {
            Visibility = Visibility.Collapsed;

            IsOpen = false;

            lastShownPopup = null;
        }

        #endregion
    }
}

#endif