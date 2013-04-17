// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceOutputViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using Catel.Data;
    using Diagnostics;
    using MVVM;
    using MVVM.Services;

    /// <summary>
    /// TraceOutput view model.
    /// </summary>
    public class TraceOutputViewModel : ViewModelBase
    {
        /// <summary>
        /// The trace listener.
        /// </summary>
        private readonly OutputTraceListener _traceListener;

        /// <summary>
        /// The trace entries with all trace items.
        /// </summary>
        private readonly ObservableCollection<TraceEntry> _traceEntries = new ObservableCollection<TraceEntry>();

        /// <summary>
        /// The dispatcher service.
        /// </summary>
        private readonly IDispatcherService _dispatcherService;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOutputViewModel"/> class.
        /// </summary>
        public TraceOutputViewModel()
        {
            _dispatcherService = GetService<IDispatcherService>();

            CopyToClipboard = new Command(OnCopyToClipboardExecute, OnCopyToClipboardCanExecute);
            ClearOutput = new Command(OnClearOutputExecute);

            _traceListener = new OutputTraceListener();
            Trace.Listeners.Add(_traceListener);

            _traceListener.ActiveTraceLevel = TraceLevel.Verbose;
            _traceListener.WrittenLine += WriteLine;

            TraceEntryCollection = new ObservableCollection<TraceEntry>();
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
        /// Gets or sets the available trace levels.
        /// </summary>
        public Collection<TraceLevel> AvailableTraceLevels
        {
            get { return GetValue<Collection<TraceLevel>>(AvailableTraceLevelsProperty); }
            set { SetValue(AvailableTraceLevelsProperty, value); }
        }

        /// <summary>
        /// Register the AvailableTraceLevels property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AvailableTraceLevelsProperty = RegisterProperty("AvailableTraceLevels", typeof(Collection<TraceLevel>),
            () => new Collection<TraceLevel>(Enum<TraceLevel>.GetValues().OrderBy(x => x).ToArray()));

        /// <summary>
        /// Gets or sets the selected trace level.
        /// </summary>
        public TraceLevel SelectedTraceLevel
        {
            get { return GetValue<TraceLevel>(SelectedTraceLevelProperty); }
            set { SetValue(SelectedTraceLevelProperty, value); }
        }

        /// <summary>
        /// Register the SelectedTraceLevel property so it is known in the class.
        /// </summary>
        public static readonly PropertyData SelectedTraceLevelProperty = RegisterProperty("SelectedTraceLevel", typeof(TraceLevel), TraceLevel.Verbose,
            (sender, e) => ((TraceOutputViewModel)sender).UpdateTraceLevel());

        /// <summary>
        /// Gets or sets the collection of selected trace entries.
        /// </summary>
        public List<TraceEntry> SelectedTraceEntryCollection
        {
            get { return GetValue<List<TraceEntry>>(SelectedTraceEntryCollectionProperty); }
            set { SetValue(SelectedTraceEntryCollectionProperty, value); }
        }

        /// <summary>
        /// Register the SelectedTraceEntryCollection property so it is known in the class.
        /// </summary>
        public static readonly PropertyData SelectedTraceEntryCollectionProperty = RegisterProperty("SelectedTraceEntryCollection", typeof(List<TraceEntry>), null);

        /// <summary>
        /// Gets the list of trace entries.
        /// </summary>
        /// <remarks>
        /// Not a Catel property to prevent StackOverflow exceptions and improve speed. Change notification is not required
        /// because this property is only set in the constructor.
        /// </remarks>
        public ObservableCollection<TraceEntry> TraceEntryCollection { get; private set; }
        #endregion

        #region Commands
        /// <summary>
        /// Gets the CopyToClipboard command.
        /// </summary>
        public Command CopyToClipboard { get; private set; }

        /// <summary>
        /// Method to check whether the CopyToClipboard command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnCopyToClipboardCanExecute()
        {
            if (SelectedTraceEntryCollection == null)
            {
                return false;
            }

            if (SelectedTraceEntryCollection.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to invoke when the CopyToClipboard command is executed.
        /// </summary>
        private void OnCopyToClipboardExecute()
        {
            var text = TraceEntriesToString(SelectedTraceEntryCollection);
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text, TextDataFormat.Text);
            }
        }

        /// <summary>
        /// Gets the ClearOutput command.
        /// </summary>
        public Command ClearOutput { get; private set; }

        /// <summary>
        /// Method to invoke when the ClearOutput command is executed.
        /// </summary>
        private void OnClearOutputExecute()
        {
            TraceEntryCollection.Clear();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes a line to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="eventType">Type of the event.</param>
        private void WriteLine(string message, TraceEventType eventType)
        {
            _dispatcherService.Invoke(() =>
            {
                var traceEntry = new TraceEntry(TraceHelper.ConvertTraceEventTypeToTraceLevel(eventType), message);
                _traceEntries.Add(traceEntry);

                if (EntryMatchesLevel(traceEntry))
                {
                    TraceEntryCollection.Add(traceEntry);
                }
            });
        }

        /// <summary>
        /// Updates the trace level and rebuilds the trace list.
        /// </summary>
        private void UpdateTraceLevel()
        {
            TraceEntryCollection.Clear();

            if (SelectedTraceEntryCollection != null)
            {
                SelectedTraceEntryCollection.Clear();
            }

            foreach (var entry in _traceEntries)
            {
                if (EntryMatchesLevel(entry))
                {
                    TraceEntryCollection.Add(entry);
                }
            }
        }

        /// <summary>
        /// Determines if the given entry matches the filter tracelevel.
        /// </summary>
        /// <param name="traceEntry">The trace entry.</param>
        /// <returns>true if matches of if filter is 'Off', false if not </returns>
        private bool EntryMatchesLevel(TraceEntry traceEntry)
        {
            if (SelectedTraceLevel == TraceLevel.Off || SelectedTraceLevel == TraceLevel.Verbose)
            {
                return true;
            }

            return (traceEntry.TraceLevel <= SelectedTraceLevel);
        }

        /// <summary>
        /// Converts a list of trace entries to a string.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <returns>STring representing the trace entries.</returns>
        private string TraceEntriesToString(IEnumerable<TraceEntry> entries)
        {
            const string columnText = " | ";

            int maxTypeLength = AvailableTraceLevels.Max(c => c.ToString("G").Length);
            var rv = new StringBuilder();
            var rxMultiline = new Regex(@"(?<=(^|\n)).*", RegexOptions.Multiline | RegexOptions.Compiled);

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    string date = entry.Time.ToString(CultureInfo.CurrentUICulture);
                    string type = entry.TraceLevel.ToString("G").PadRight(maxTypeLength, ' ');
                    string datefiller = new String(' ', date.Length);
                    string typefiller = new String(' ', type.Length);

                    string message = entry.Message;
                    var matches = rxMultiline.Matches(message);

                    if (matches.Count > 0)
                    {
                        rv.AppendFormat("{0}{4}{1}{4}{2}{3}", date, type, matches[0].Value, System.Environment.NewLine, columnText);

                        if (matches.Count > 1)
                        {
                            for (int idx = 1, max = matches.Count; idx < max; idx++)
                            {
                                rv.AppendFormat("{0}{4}{1}{4}{2}{3}", datefiller, typefiller, matches[idx].Value, System.Environment.NewLine, columnText);
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