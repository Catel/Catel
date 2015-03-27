// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceOutputViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using Filters;
    using Logging;
    using MVVM;
    using Services;
    using System.Windows.Data;

    /// <summary>
    /// TraceOutput view model.
    /// </summary>
    [ObsoleteEx(Replacement = "Orc.Controls, see https://github.com/wildgums/orc.controls", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class TraceOutputViewModel : ViewModelBase
    {
        private readonly OutputLogListener _outputLogListener;

        /// <summary>
        /// The dispatcher service.
        /// </summary>
        private readonly IDispatcherService _dispatcherService;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOutputViewModel" /> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService"/> is <c>null</c>.</exception>
        public TraceOutputViewModel(IDispatcherService dispatcherService)
        {
            Argument.IsNotNull("dispatcherService", dispatcherService);

            _dispatcherService = dispatcherService;

            CopyToClipboard = new Command(OnCopyToClipboardExecute, OnCopyToClipboardCanExecute);
            ClearOutput = new Command(OnClearOutputExecute);
            ClearFilter = new Command(OnClearFilterExecute);

            _outputLogListener = new OutputLogListener();
            _outputLogListener.LogMessage += OnLogMessage;

            IgnoreCatelLogging = true;

            LogManager.AddListener(_outputLogListener);

            TraceEntries = new ObservableCollection<TraceEntry>();
            TraceEntriesSourceList = new CollectionViewSource { Source = TraceEntries };
            TraceEntriesList = TraceEntriesSourceList.View;
            Levels = Enum<LogEvent>.GetValues().OrderBy(x => x).ToList();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Trace output"; }
        }

        /// <summary>
        /// <c>true</c> if the Catel logging should be ignored.
        /// <para />
        /// The default is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Note that this is a wrapper property and does not support <see cref="INotifyPropertyChanged"/>.
        /// </remarks>
        public bool IgnoreCatelLogging
        {
            get { return _outputLogListener.IgnoreCatelLogging; }
            set { _outputLogListener.IgnoreCatelLogging = value; }
        }

        /// <summary>
        /// Gets or sets the available levels.
        /// </summary>
        public List<LogEvent> Levels { get; private set; }

        /// <summary>
        /// Gets or sets the selected level.
        /// </summary>
        [DefaultValue(LogEvent.Debug)]
        public LogEvent SelectedLevel { get; set; }

        /// <summary>
        /// Gets the list of trace entries.
        /// </summary>
        public ObservableCollection<TraceEntry> TraceEntries { get; private set; }

        /// <summary>
        /// Gets the trace entries source list.
        /// </summary>
        /// <value>The trace entries source list.</value>
        public CollectionViewSource TraceEntriesSourceList { get; private set; }

        /// <summary>
        /// Gets the trace entries list.
        /// </summary>
        /// <value>The trace entries list.</value>
        public ICollectionView TraceEntriesList { get; private set; }

        /// <summary>
        /// Gets or sets the selected trace entries.
        /// </summary>
        /// <value>The selected trace entries.</value>
        public List<TraceEntry> SelectedTraceEntries { get; set; } 

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

        #endregion

        #region Commands
        /// <summary>
        /// Gets the ClearOutput command.
        /// </summary>
        public Command ClearOutput { get; private set; }

        private void OnClearOutputExecute()
        {
            var traceEntries = TraceEntries;
            if (traceEntries != null)
            {
                traceEntries.Clear();
            }
        }

        /// <summary>
        /// Gets the clear filter command.
        /// </summary>
        /// <value>The clear filter.</value>
        public Command ClearFilter { get; private set; }

        private void OnClearFilterExecute()
        {
            Filter = string.Empty;
        }

        /// <summary>
        /// Gets the CopyToClipboard command.
        /// </summary>
        public Command CopyToClipboard { get; private set; }

        private bool OnCopyToClipboardCanExecute()
        {
            var selectedTraceEntries = SelectedTraceEntries;
            if (selectedTraceEntries == null)
            {
                return false;
            }

            if (selectedTraceEntries.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void OnCopyToClipboardExecute()
        {
            var text = TraceEntriesToString(SelectedTraceEntries);
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text, TextDataFormat.Text);
            }
        }
        #endregion

        #region Methods
        private void OnFilterChanged()
        {
            ApplyFilter();
        }

        private void OnSelectedLevelChanged()
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var traceEntriesList = TraceEntriesList;
            if (traceEntriesList != null)
            {
                var filterText = Filter.PrepareAsSearchFilter();

                TraceEntriesList.Filter = new TraceEntryFilter(filterText, SelectedLevel).MatchesObject;
            }
        }

        private void OnLogMessage(object sender, LogMessageEventArgs e)
        {
            _dispatcherService.BeginInvoke(() =>
            {
                var traceEntries = TraceEntries;
                if (traceEntries != null)
                {
                    var traceEntry = new TraceEntry(new LogEntry(e));
                    TraceEntries.Add(traceEntry);
                }
            });
        }

        /// <summary>
        /// Converts a list of trace entries to a string.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>STring representing the trace entries.</returns>
        private string TraceEntriesToString(IEnumerable<TraceEntry> entries)
        {
            const string columnText = " | ";

            int maxTypeLength = Levels.Max(c => c.ToString("G").Length);
            var rv = new StringBuilder();
            var rxMultiline = new Regex(@"(?<=(^|\n)).*", RegexOptions.Multiline | RegexOptions.Compiled);

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    string date = entry.Time.ToString(CultureInfo.CurrentUICulture);
                    string type = entry.LogEvent.ToString("G").PadRight(maxTypeLength, ' ');
                    string datefiller = new String(' ', date.Length);
                    string typefiller = new String(' ', type.Length);

                    string message = entry.Message;
                    var matches = rxMultiline.Matches(message);

                    if (matches.Count > 0)
                    {
                        rv.AppendFormat("{0}{4}{1}{4}{2}{3}", date, type, matches[0].Value, Environment.NewLine, columnText);

                        if (matches.Count > 1)
                        {
                            for (int idx = 1, max = matches.Count; idx < max; idx++)
                            {
                                rv.AppendFormat("{0}{4}{1}{4}{2}{3}", datefiller, typefiller, matches[idx].Value, Environment.NewLine, columnText);
                            }
                        }
                    }
                }
            }

            return rv.ToString();
        }
        #endregion
    }
}

#endif