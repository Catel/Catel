// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceOutputControl.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Controls;
    using IoC;
    using MVVM;

    /// <summary>
    /// Interaction logic for TraceOutputControl.xaml
    /// </summary>
    public partial class TraceOutputControl : UserControl
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOutputControl"/> class.
        /// </summary>
        public TraceOutputControl()
        {
            InitializeComponent();

            logListView.SelectionChanged += (sender, args) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.SelectedTraceEntryCollection = logListView.SelectedItems.Cast<TraceEntry>().ToList();
                }
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public new TraceOutputViewModel ViewModel
        {
            get { return (TraceOutputViewModel)base.ViewModel; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the type of the view model. If this method returns <c>null</c>, the view model type will be retrieved by naming
        /// convention using the <see cref="IViewModelLocator"/> registered in the <see cref="IServiceLocator"/>.
        /// </summary>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks></remarks>
        protected override Type GetViewModelType()
        {
            return typeof(TraceOutputViewModel);
        }

        /// <summary>
        /// Called when the <see cref="UserControl.ViewModel"/> has changed.
        /// </summary>
        /// <remarks></remarks>
        protected override void OnViewModelChanged()
        {
            if (ViewModel != null)
            {
                this.SubscribeToWeakCollectionChangedEvent(ViewModel.TraceEntryCollection, OnTraceEntryCollectionChanged);
            }
        }

        private void OnTraceEntryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom();
        }

        /// <summary>
        /// Moves the cursor down so the latest output is visible.
        /// </summary>
        private void ScrollToBottom()
        {
            bool scroll = logListView.SelectedItems.Count == 0;

            if (scroll)
            {
                if (logListView.IsVisible)
                {
                    // Get the border of the listview (first child of a listview)
                    var scrollViewer = logListView.FindVisualDescendantByType<ScrollViewer>();
                    scrollViewer.ScrollToBottom();
                }
                else
                {
                    if (ViewModel != null)
                    {
                        logListView.ScrollIntoView(ViewModel.TraceEntryCollection.Last());
                    }
                }
            }
        }
        #endregion
    }
}