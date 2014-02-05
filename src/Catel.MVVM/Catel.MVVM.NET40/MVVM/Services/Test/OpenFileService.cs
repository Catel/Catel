// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    using System;
    using System.Collections.Generic;

#if !NET
    using System.IO;
#endif

    /// <summary>
    /// Test implementation of the <see cref="IOpenFileService"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.OpenFileService service = (Test.OpenFileService)GetService<IOpenFileService>();
    /// 
    /// // Queue the next expected result
    /// service.ExpectedResults.Add(() =>
    ///              {
    ///                service.FileName = @"c:\test.txt";
    ///                return true;
    ///              });
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class OpenFileService : IOpenFileService
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileService"/> class.
        /// </summary>
        public OpenFileService()
        {
#if !NET
            ExpectedResults = new Queue<Func<Stream[]>>();       
#else
            ExpectedResults = new Queue<Func<bool>>();
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the queue of expected results.
        /// </summary>
        /// <value>The expected results.</value>
#if NET
        public Queue<Func<bool>> ExpectedResults { get; private set; }
#else
        public Queue<Func<Stream[]>> ExpectedResults { get; private set; }
#endif

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

#if NET
        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
        /// </summary>
        /// <value><c>true</c> if extensions are added; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        public bool CheckFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool CheckPathExists { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>The index of the selected filter. The default is <c>1</c>.</value>
        public int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value><c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        public bool ValidateNames { get; set; }
#endif
        #endregion

        #region Methods
        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value><c>true</c> if this instance is multi select; otherwise, <c>false</c>.</value>
        public bool IsMultiSelect { get; set; }

        /// <summary>
        /// Gets the file names in case <see cref="IOpenFileService.IsMultiSelect"/> is <c>true</c>.
        /// </summary>
        public string[] FileNames { get; private set; }

        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if a file is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="FileName"/> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
#if !NET
        public Stream[] DetermineFile()
#else
        public bool DetermineFile()
#endif
        {
            if (ExpectedResults.Count == 0)
            {
                throw new Exception(ResourceHelper.GetString("NoExpectedResultsInQueueForUnitTest"));
            }

            return ExpectedResults.Dequeue().Invoke();
        }
        #endregion
    }
}
