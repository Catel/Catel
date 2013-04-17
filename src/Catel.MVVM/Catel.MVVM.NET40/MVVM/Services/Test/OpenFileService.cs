// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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
#endif

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }
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
                throw new Exception(ResourceHelper.GetString(typeof(OpenFileService), "Exceptions", "NoExpectedResultsInQueueForUnitTest"));
            }

            return ExpectedResults.Dequeue().Invoke();
        }
        #endregion
    }
}
