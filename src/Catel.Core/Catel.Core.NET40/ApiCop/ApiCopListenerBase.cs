// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base class for ApiCop listeners.
    /// </summary>
    public abstract class ApiCopListenerBase : IApiCopListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCopListenerBase"/> class.
        /// </summary>
        protected ApiCopListenerBase()
        {
            Grouping = ApiCopListenerGrouping.Tag;
        }

        /// <summary>
        /// Gets or sets the grouping for this listener.
        /// </summary>
        /// <value>The grouping.</value>
        public ApiCopListenerGrouping Grouping { get; set; }

        /// <summary>
        /// Writes the results of the ApiCop feature.
        /// <para />
        /// Note that this will only contain invalid results. Valid results are not written to the
        /// listeners.
        /// </summary>
        /// <param name="results">The results.</param>
        public void WriteResults(IEnumerable<IApiCopResult> results)
        {
            Argument.IsNotNull("results", results);

            BeginWriting();

            //Dictionary<string, List<IApiCopResult>> sortedResults;

            //switch (Grouping)
            //{
            //    case ApiCopListenerGrouping.Cop:
            //        sortedResults = (from result in results
            //                         orderby result.Cop.TargetType.FullName
            //                         select new KeyValuePair<string, IApiCopResult>(g.Key, g.ToList()));
            //        break;

            //    case ApiCopListenerGrouping.Rule:
            //        sortedResults = (from result in results
            //                         orderby result.Rule.Name
            //                         select result).ToList();
            //        break;

            //    case ApiCopListenerGrouping.Tag:
            //        sortedResults = (from result in results
            //                         orderby result.Tag
            //                         select result).ToList();
            //        break;

            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            //string previousGroupName = string.Empty;

            //foreach (var result in sortedResults)
            //{
            //    if (!string.Equals(previousGroupName, result.))

            //    WriteResult(result);
            //}

            //if (!string.IsNullOrEmpty(previousGroupName))
            //{
            //    EndWritingOfGroup(previousGroupName);
            //}

            EndWriting();
        }

        /// <summary>
        /// Called when the listener is about to write the results.
        /// </summary>
        protected virtual void BeginWriting()
        {
        }

        /// <summary>
        /// Begins the writing of a specific group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        protected virtual void BeginWritingOfGroup(string groupName)
        {
            
        }

        /// <summary>
        /// Writes the result to the listener target.
        /// </summary>
        /// <param name="result">The result.</param>
        protected abstract void WriteResult(IApiCopResult result);

        /// <summary>
        /// Ends the writing of a specific group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        protected virtual void EndWritingOfGroup(string groupName)
        {

        }

        /// <summary>
        /// Called when the listener has finished writing all the results.
        /// </summary>
        protected virtual void EndWriting()
        {
        }
    }
}