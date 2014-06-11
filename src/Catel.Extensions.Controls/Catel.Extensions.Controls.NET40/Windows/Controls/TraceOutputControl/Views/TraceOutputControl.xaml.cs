// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceOutputControl.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Controls
{
    using System.Linq;
    using System.Windows;
    using MVVM.Views;

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
                var vm = ViewModel as TraceOutputViewModel;
                if (vm != null)
                {
                    vm.SelectedTraceEntries = logListView.SelectedItems.Cast<TraceEntry>().ToList();
                }
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the Catel logging should be ignored.
        /// </summary>
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public bool IgnoreCatelLogging
        {
            get { return (bool)GetValue(IgnoreCatelLoggingProperty); }
            set { SetValue(IgnoreCatelLoggingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IgnoreCatelLogging.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IgnoreCatelLoggingProperty =
            DependencyProperty.Register("IgnoreCatelLogging", typeof(bool), typeof(TraceOutputControl), new PropertyMetadata(true));
        #endregion
    }
}