// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextApiCopListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    /// <summary>
    /// <see cref="IApiCopListener"/> implementation which writes all results to a text file.
    /// </summary>
    public class TextApiCopListener : ApiCopListenerBase
    {
        /// <summary>
        /// Called when the listener is about to write the results.
        /// </summary>
        protected override void BeginWriting()
        {
            base.BeginWriting();

            // TODO: Create text fix
        }

        /// <summary>
        /// Writes the result to the listener target.
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void WriteResult(IApiCopResult result)
        {
            var text = string.Empty;

            

        }

        /// <summary>
        /// Called when the listener has finished writing all the results.
        /// </summary>
        protected override void EndWriting()
        {
            // TODO: Close text file

            base.EndWriting();
        }
    }
}