// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFileApiCopListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop.Listeners
{
    using System.IO;

    /// <summary>
    /// <see cref="IApiCopListener"/> implementation which writes all results to a text file.
    /// <para />
    /// If no <c>FileStream</c> is available in the target platform, this will write to a memory stream.
    /// </summary>
    public class TextFileApiCopListener : TextApiCopListenerBase
    {
        private Stream _fileStream;
        private StreamWriter _textWriter;

#if NET
        private readonly string _fileName;
#endif

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextFileApiCopListener"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public TextFileApiCopListener(string fileName)
        {
            Argument.IsNotNull("fileName", fileName);

#if NET
            _fileName = fileName;
#endif
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes the line the to final output.
        /// </summary>
        /// <param name="line">The line.</param>
        protected override void WriteLine(string line)
        {
            _textWriter.WriteLine(line);
        }

        /// <summary>
        /// Called when the listener is about to write the results.
        /// </summary>
        protected override void BeginWriting()
        {
#if NET
            _fileStream = new FileStream(_fileName, FileMode.Create, FileAccess.Write);
#else
            _fileStream = new MemoryStream();
#endif
            _textWriter = new StreamWriter(_fileStream);

            base.BeginWriting();
        }

        /// <summary>
        /// Called when the listener has finished writing all the results.
        /// </summary>
        protected override void EndWriting()
        {
            base.EndWriting();

            _textWriter.Dispose();
            _textWriter = null;

            _fileStream.Dispose();
            _fileStream = null;
        }
        #endregion
    }
}
