// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Load behavior of the tabs in the <see cref="TabControl"/>.
    /// </summary>
    public enum LoadTabItemsBehavior
    {
        /// <summary>
        /// Loads the current tab and unloads the others.
        /// </summary>
        SingleUnloadOthers,

        /// <summary>
        /// Load the current tab.
        /// </summary>
        Single,

        /// <summary>
        /// Load all when a tab is used for the first time.
        /// </summary>
        AllOnFirstUse,

        /// <summary>
        /// Load all as soon as the control is loaded.
        /// </summary>
        AllOnStartUp,
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
        public TabControlItemData(object container, object content)
        {
            Container = container;
            TabItem = container as TabItem;
            if (TabItem != null)
            {
                TabItem.Content = null;
            }

            Content = content;
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
        }

        /// <summary>
        /// Gets or sets the load tab items.
        /// <para />
        /// The default value is <see cref="LoadTabItemsBehavior.Single"/>.
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
            typeof(LoadTabItemsBehavior), typeof(TabControl), new PropertyMetadata(LoadTabItemsBehavior.Single));

        /// <summary>
        /// Called when the tab control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTabControlLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTabControlLoaded;

            if (LoadTabItems == LoadTabItemsBehavior.AllOnStartUp)
            {
                UpdateItems();
            }
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

                UpdateItems();
            }
        }

        /// <summary>
        /// Get the ItemsHolder and generate any children.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsHolder = GetTemplateChild("PART_ItemsHolder") as Panel;

            UpdateItems();
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
                            ContentPresenter cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                _itemsHolder.Children.Remove(cp);
                            }
                        }
                    }

                    // don't do anything with new items because we don't want to
                    // create visuals that aren't being shown

                    UpdateItems();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }
        }

        /// <summary>
        /// Update the visible child in the ItemsHolder.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            UpdateItems();
        }

        /// <summary>
        /// Generate a ContentPresenter for every item.
        /// </summary>
        private void UpdateItems()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            var selectedTabItem = GetSelectedTabItem();
            IEnumerable source = null;
            var unvisible = Visibility.Hidden;

            switch (LoadTabItems)
            {
                case LoadTabItemsBehavior.Single:
                    source = new[] { selectedTabItem };
                    break;

                case LoadTabItemsBehavior.SingleUnloadOthers:
                case LoadTabItemsBehavior.AllOnFirstUse:
                case LoadTabItemsBehavior.AllOnStartUp:
                    source = Items;
                    unvisible = Visibility.Collapsed;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var item in source)
            {
                // Generate a ContentPresenter if necessary
                if (item != null)
                {
                    CreateChildContentPresenter(item);
                }
            }

            // Show the right child
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
                            child.Content = tabControlItemData.Content;
                        }

                        child.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (LoadTabItems == LoadTabItemsBehavior.SingleUnloadOthers)
                        {
                            child.Content = null;
                        }
                        else
                        {
                            child.Visibility = unvisible;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            object dummyObject = null;
            if (_wrappedContainers.TryGetValue(item, out dummyObject))
            {
                return null;
            }

            _wrappedContainers.Add(item, new object());

            var cp = FindChildContentPresenter(item);
            if (cp != null)
            {
                return cp;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();

            var container = GetContentContainer(item);
            var content = GetContent(item);
            var tabItemData = new TabControlItemData(container, content);

            cp.Tag = tabItemData;
            cp.Content = content;
            cp.ContentTemplate = ContentTemplate;
            cp.ContentTemplateSelector = ContentTemplateSelector;
            cp.ContentStringFormat = SelectedContentStringFormat;
            cp.Visibility = Visibility.Collapsed;

            _itemsHolder.Children.Add(cp);

            return cp;
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

            return _itemsHolder.Children.Cast<ContentPresenter>().FirstOrDefault(cp => cp.Content == data);
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