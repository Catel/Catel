// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilesHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///   Class to manage files that are required for unit tests. This class automatically deletes the files again after using them.
    /// </summary>
    internal class FilesHelper
    {
        #region Fields
        /// <summary>
        ///   List of files to delete at cleanup.
        /// </summary>
        private readonly List<string> _fileNamesToDelete = new List<string>();
        #endregion

        #region Methods
        /// <summary>
        ///   Cleans up all the mess caused by this class.
        /// </summary>
        public void CleanUp()
        {
            foreach (string fileName in _fileNamesToDelete)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception)
                {
                    // Continue
                }
            }
        }

        /// <summary>
        ///   Gets a temporary file.
        /// </summary>
        /// <returns> Filename of a new temporary file that is ready to be used. </returns>
        public string GetTempFile()
        {
            string fileName = Path.GetTempFileName();
            _fileNamesToDelete.Add(fileName);
            return fileName;
        }
        #endregion
    }
}

#endif