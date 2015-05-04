// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Data;
    using System.Collections.Generic;
    using Threading;

    /// <summary>
    /// Load behavior of the tabs in the <see cref="TabControl"/>.
    /// </summary>
    public enum LoadTabItemsBehavior
    {
        /// <summary>
        /// Load all tabs using lazy loading, but keeps the tabs in memory afterwards.
        /// </summary>
        LazyLoading,

        /// <summary>
        /// Load all tabs using lazy loading. As soon as a tab is loaded, all other loaded tabs will be unloaded.
        /// </summary>
        LazyLoadingUnloadOthers,

        /// <summary>
        /// Load all tabs as soon as the tab control is loaded.
        /// </summary>
        EagerLoading,

        /// <summary>
        /// Load all tabs when any of the tabs is used for the first time.
        /// </summary>
        EagerLoadingOnFirstUse,

        /// <summary>
        /// Obsolete, use <see cref="LazyLoading"/> instead.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = "LazyLoading", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        Single = LazyLoading,

        /// <summary>
        /// Obsolete, use <see cref="LazyLoadingUnloadOthers"/> instead.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = "LazyLoadingUnloadOthers", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        SingleUnloadOthers = LazyLoadingUnloadOthers,

        /// <summary>
        /// Obsolete, use <see cref="EagerLoading"/> instead.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = "EagerLoading", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        AllOnStartUp = EagerLoading,

        /// <summary>
        /// Obsolete, use <see cref="EagerLoadingOnFirstUse"/> instead.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = "EagerLoadingOnFirstUse", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        AllOnFirstUse = EagerLoadingOnFirstUse
    }

    /// <summary>
    /// Item data for a tab control item.
    /// </summary>
    public class TabControlItemData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlItemData" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="content">The content.</param>
        /// <param name="contentTemplate">The content template.</param>
        /// <param name="item">The item.</param>
        public TabControlItemData(object container, object content, DataTemplate contentTemplate, object item)
        {
            Container = container;
            TabItem = container as TabItem;
            if (TabItem != null)
            {
                TabItem.Content = null;
                TabItem.ContentTemplate = null;
            }

            Content = content;
            ContentTemplate = contentTemplate;
            Item = item;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public object Container { get; private set; }

        /// <summary>
        /// Gets the tab item.
        /// </summary>
        /// <value>The tab item.</value>
        public TabItem TabItem { get; private set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; private set; }

        /// <summary>
        /// Gets the content template.
        /// </summary>
        /// <value>The content.</value>
        public DataTemplate ContentTemplate { get; private set; }

        /// <summary>
        /// The item from which it was generated.
        /// </summary>
        /// <value>The item.</value>
        public object Item { get; private set; }
    }

    /// <summary>
    /// TabControl that will not remove the tab items from the visual tree. This way, views can be re-used.
    /// </summary>
    /// <remarks>
    /// This code was originally found at http://eric.burke.name/dotnetmania/2009/04/26/22.09.28.
    /// </remarks>
    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
    public class TabControl : System.Windows.Controls.TabControl
    {
        private Panel _itemsHolder;

        private readonly ConditionalWeakTable<object, object> _wrappedContainers = new ConditionalWeakTable<object, object>();

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="TabControl"/>.class.
        /// </summary>
        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TabControl"/>.class.
        /// </summary>
        /// <remarks></remarks>
        public TabControl()
        {
            // this is necessary so that we get the initial databound selected item
            ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
            Loaded += OnTabControlLoaded;

#if SL5
            DefaultStyleKey = typeof (TabControl);
#endif

            this.SubscribeToDependencyProperty("SelectedItem", OnSelectedItemChanged);
        }

        /// <summary>
        /// Gets or sets the load tab items.
        /// <para />
        /// The default value is <see cref="LoadTabItemsBehavior.LazyLoading"/>.
        /// </summary>
        /// <value>
        /// The load tab items.
        /// </value>
        public LoadTabItemsBehavior LoadTabItems
        {
            get { return (LoadTabItemsBehavior)GetValue(LoadTabItemsProperty); }
            set { SetValue(LoadTabItemsProperty, value); }
        }

        /// <summary>
        /// Dependency property registration for the <see cref="LoadTabItems"/> property.
        /// </summary>
        public static readonly DependencyProperty LoadTabItemsProperty = DependencyProperty.Register("LoadTabItems",
            typeof(LoadTabItemsBehavior), typeof(TabControl), new PropertyMetadata(LoadTabItemsBehavior.LazyLoading,
                (sender, e) => ((TabControl)sender).OnLoadTabItemsChanged()));

        /// <summary>
        /// Gets or sets a value indicating whether this tab control uses any of the lazy loading options.
        /// </summary>
        /// <value><c>true</c> if this instance is lazy loading; otherwise, <c>false</c>.</value>
        public bool IsLazyLoading
        {
            get
            {
                var loadTabItems = LoadTabItems;
                return loadTabItems == LoadTabItemsBehavior.LazyLoading || loadTabItems == LoadTabItemsBehavior.LazyLoadingUnloadOthers;
            }
        }

        /// <summary>
        /// Called when the tab control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTabControlLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTabControlLoaded;

            InitializeItems();
        }

        /// <summary>
        /// If containers are done, generate the selected item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                ItemContainerGenerator.StatusChanged -= OnItemContainerGeneratorStatusChanged;

                InitializeItems();
            }
        }

        /// <summary>
        /// Get the ItemsHolder and generate any children.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsHolder = GetTemplateChild("PART_ItemsHolder") as Panel;

            InitializeItems();
        }

        private void OnLoadTabItemsChanged()
        {
            InitializeItems();
        }

        /// <summary>
        /// When the items change we remove any generated panel children and add any new ones as necessary
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ItemContainerGenerator.ItemsChanged"/> event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (_itemsHolder == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    _itemsHolder.Children.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                _itemsHolder.Children.Remove(cp);
                            }
                        }
                    }

                    // don't do anything with new items because we don't want to
                    // create visuals that aren't being shown
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }

            InitializeItems();
        }

        private void OnSelectedItemChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            UpdateItems();
        }

        private void InitializeItems()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            var items = Items;
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item != null)
                {
                    CreateChildContentPresenter(item);
                }
            }

            var loadAllItems = (LoadTabItems == LoadTabItemsBehavior.AllOnStartUp) || (IsLoaded && !IsLazyLoading);
            if (loadAllItems)
            {
                foreach (ContentPresenter child in _itemsHolder.Children)
                {
                    var tabControlItemData = child.Tag as TabControlItemData;
                    if (tabControlItemData != null)
                    {
                        var tabItem = tabControlItemData.TabItem;
                        if (tabItem != null)
                        {
                            ShowChildContent(child, tabControlItemData);

                            // Collapsed is hidden + not loaded
                            child.Visibility = Visibility.Collapsed;

                            if (LoadTabItems == LoadTabItemsBehavior.EagerLoading)
                            {
                                EagerLoadAllTabs();
                            }
                        }
                    }
                }
            }

            if (SelectedItem != null)
            {
                UpdateItems();
            }
        }

        private void EagerLoadAllTabs()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            foreach (ContentPresenter child in _itemsHolder.Children)
            {
                var tabControlItemData = child.Tag as TabControlItemData;
                if (tabControlItemData != null)
                {
                    var tabItem = tabControlItemData.TabItem;
                    if (tabItem != null)
                    {
                        ShowChildContent(child, tabControlItemData);
                    }
                }

                // Always start invisible, the selection will take care of visibility
                child.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateItems()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            var items = Items;
            if (items == null)
            {
                return;
            }

            if (SelectedItem != null)
            {
                if (!IsLazyLoading)
                {
                    EagerLoadAllTabs();
                }
            }

            // Show the right child first (to prevent flickering)
            var itemsToHide = new Dictionary<ContentPresenter, TabControlItemData>();

            foreach (ContentPresenter child in _itemsHolder.Children)
            {
                var tabControlItemData = child.Tag as TabControlItemData;
                if (tabControlItemData != null)
                {
                    var tabItem = tabControlItemData.TabItem;
                    if (tabItem != null && tabItem.IsSelected)
                    {
                        if (child.Content == null)
                        {
                            ShowChildContent(child, tabControlItemData);
                        }

                        child.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        itemsToHide.Add(child, tabControlItemData);
                    }
                }
            }

            // Now hide so we have prevented flickering
            foreach (var itemToHide in itemsToHide)
            {
                var child = itemToHide;

                // Note: hidden, not collapsed otherwise items will not be loaded
                child.Key.Visibility = Visibility.Hidden;

                if (LoadTabItems == LoadTabItemsBehavior.LazyLoadingUnloadOthers)
                {
                    HideChildContent(child.Key, child.Value);
                }
            }
        }

        /// <summary>
        /// Create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item">The item.</param>
        private void CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return;
            }

            object dummyObject = null;
            if (_wrappedContainers.TryGetValue(item, out dummyObject))
            {
                return;
            }

            _wrappedContainers.Add(item, new object());

            var cp = FindChildContentPresenter(item);
            if (cp != null)
            {
                return;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();

            var container = GetContentContainer(item);
            var content = GetContent(item);

            var tabItemData = new TabControlItemData(container, content, ContentTemplate, item);

            cp.Tag = tabItemData;

            if (!IsLazyLoading)
            {
                ShowChildContent(cp, tabItemData);
            }

            cp.ContentTemplateSelector = ContentTemplateSelector;
            cp.ContentStringFormat = SelectedContentStringFormat;

            _itemsHolder.Children.Add(cp);
        }

        private object GetContent(object item)
        {
            var itemAsTabItem = item as TabItem;
            if (itemAsTabItem != null)
            {
                return itemAsTabItem.Content;
            }

            return item;
        }

        private object GetContentContainer(object item)
        {
            var itemAsTabItem = item as TabItem;
            if (itemAsTabItem != null)
            {
                return itemAsTabItem;
            }

            return ItemContainerGenerator.ContainerFromItem(item);
        }

        private void ShowChildContent(ContentPresenter child, TabControlItemData tabControlItemData)
        {
            if (child.Content == null)
            {
                child.Content = tabControlItemData.Content;
            }

            if (child.ContentTemplate == null)
            {
                child.ContentTemplate = tabControlItemData.ContentTemplate;
            }

            tabControlItemData.TabItem.Content = child;
        }

        private void HideChildContent(ContentPresenter child, TabControlItemData tabControlItemData)
        {
            child.Content = null;
            child.ContentTemplate = null;

            tabControlItemData.TabItem.Content = null;
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data == null)
            {
                return null;
            }

            var dataAsTabItem = data as TabItem;
            if (dataAsTabItem != null)
            {
                data = dataAsTabItem.Content;
            }

            if (_itemsHolder == null)
            {
                return null;
            }

            var existingCp = _itemsHolder.Children.Cast<ContentPresenter>().FirstOrDefault(cp => ReferenceEquals(((TabControlItemData)cp.Tag).Item, data));
            if (existingCp != null)
            {
                return existingCp;
            }

            return null;
        }

        /// <summary>
        /// Copied from TabControl; wish it were protected in that class instead of private.
        /// </summary>
        /// <returns></returns>
        protected TabItem GetSelectedTabItem()
        {
            object selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }

            var item = selectedItem as TabItem;
            if (item == null)
            {
                item = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItem;
            }

            return item;
        }
    }
}

#endif