// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceOutputControl.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Controls
{
    using System.Linq;

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
    }
}