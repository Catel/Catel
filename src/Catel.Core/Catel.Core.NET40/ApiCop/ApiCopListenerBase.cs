// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class for ApiCop listeners.
    /// </summary>
    public abstract class ApiCopListenerBase : IApiCopListener
    {
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

            foreach (var result in results)
            {
                WriteResult(result);
            }

            EndWriting();
        }

        /// <summary>
        /// Called when the listener is about to write the results.
        /// </summary>
        protected virtual void BeginWriting()
        {
        }

        /// <summary>
        /// Writes the result to the listener target.
        /// </summary>
        /// <param name="result">The result.</param>
        protected abstract void WriteResult(IApiCopResult result);

        /// <summary>
        /// Called when the listener has finished writing all the results.
        /// </summary>
        protected virtual void EndWriting()
        {
        }
    }
}