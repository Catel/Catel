// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
    /// Test implementation of the <see cref="ISaveFileService"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.SaveFileService service = (Test.SaveFileService)GetService<ISaveFileService>();
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
    public class SaveFileService : ISaveFileService
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileService"/> class.
        /// </summary>
        public SaveFileService()
        {
#if !NET
            ExpectedResults = new Queue<Func<Stream>>();       
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
        public Queue<Func<Stream>> ExpectedResults { get; private set; }
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
        public Stream DetermineFile()
#else
        public bool DetermineFile()
#endif
        {
            if (ExpectedResults.Count == 0)
            {
                throw new Exception(ResourceHelper.GetString(typeof(SaveFileService), "Exceptions", "NoExpectedResultsInQueueForUnitTest"));
            }

            return ExpectedResults.Dequeue().Invoke();
        }
        #endregion
    }
}
