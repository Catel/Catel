// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextApiCopListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel.Reflection;

    /// <summary>
    /// <see cref="IApiCopListener"/> implementation which writes all results to a text file.
    /// </summary>
    public abstract class TextApiCopListenerBase : ApiCopListenerBase
    {
        private const string DocumentationUrl = "https://catelproject.atlassian.net/wiki/display/CTL/ApiCop";

        private DateTime _startTime;
        private DateTime _endTime;

        /// <summary>
        /// Called when the listener is about to write the results.
        /// </summary>
        protected override void BeginWriting()
        {
            _startTime = DateTime.Now;

            base.BeginWriting();

            var assembly = AssemblyHelper.GetEntryAssembly();

            WriteLine("****************************************************************");
            WriteLine(string.Empty);
            WriteLine("ApiCop (r) results of '{0}' v{1}", assembly.Title(), assembly.Version());
            WriteLine("  recorded on {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            WriteLine(string.Empty);
            WriteLine("To ignore rules, call ApiCopManager.IgnoredRules.Add([rulename]);");
            WriteLine(string.Empty);
            WriteLine("For more information about ApiCop, visit the website:");
            WriteLine("  {0}", DocumentationUrl);
            WriteLine(string.Empty);
            WriteLine("****************************************************************");
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes the summary, called before any groups are written.
        /// </summary>
        /// <param name="results">The results.</param>
        protected override void WriteSummary(IEnumerable<IApiCopResult> results)
        {
            if (results.Count() == 0)
            {
                WriteLine("No results found which is a good sign, congratulations!");
            }
        }

        /// <summary>
        /// Begins the writing of a specific group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        protected override void BeginWritingOfGroup(string groupName)
        {
            base.BeginWritingOfGroup(groupName);

            WriteLine("================================================================");
            WriteLine("{0}", groupName.ToUpper());
            WriteLine("================================================================");
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes the result to the listener target.
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void WriteResult(IApiCopResult result)
        {
            WriteLine("Cop TargetType: {0}", result.Cop.TargetType);
            WriteLine("Rule: {0} ({1})", result.Rule.Name, result.Rule.Level);

            if (!string.IsNullOrWhiteSpace(result.Rule.Url))
            {
                WriteLine("For more information about this rule, visit {0}", result.Rule.Url);
            }

            WriteLine(string.Empty);

            WriteLine("{0}", result.Rule.GetResultAsText(result.Tag));
            WriteLine(string.Empty);
            WriteLine("----------------------------------------------------------------");
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Ends the writing of a specific group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        protected override void EndWritingOfGroup(string groupName)
        {
            // No group footer yet

            base.EndWritingOfGroup(groupName);
        }

        /// <summary>
        /// Called when the listener has finished writing all the results.
        /// </summary>
        protected override void EndWriting()
        {
            _endTime = DateTime.Now;

            var duration = _endTime - _startTime;

            WriteLine("****************************************************************");
            WriteLine("End of ApiCop (r) results, generation took '{0}'", duration.ToString(@"hh\:mm\:ss\.fff"));
            WriteLine("****************************************************************");
            WriteLine(string.Empty);

            base.EndWriting();
        }

        /// <summary>
        /// Writes the line with formatting arguments.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        protected void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// Writes the line the to final output.
        /// </summary>
        /// <param name="line">The line.</param>
        protected abstract void WriteLine(string line);
    }
}