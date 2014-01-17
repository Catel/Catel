// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilesHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;

#if SILVERLIGHT
using System.IO.IsolatedStorage;
using Catel.Windows;
#endif

#if SILVERLIGHT && !WINDOWS_PHONE
using Catel.Test.Helpers.Views;
#endif

    /// <summary>
    ///   Class to manage files that are required for unit tests. This class automatically deletes the files again after using them.
    /// </summary>
    internal class FilesHelper
    {
        #region Fields
#if SILVERLIGHT
        /// <summary>
        /// Counter to increase filenames.
        /// </summary>
        private static int _counter = 1;

        private static bool _shownWindow = false;

        private IsolatedStorageFile _userStoreForApplication;

        /// <summary>
        /// List of files to dispose at cleanup.
        /// </summary>
        private readonly List<IsolatedStorageFileStream> _filestreamsToDispose = new List<IsolatedStorageFileStream>();
#else
        /// <summary>
        ///   List of files to delete at cleanup.
        /// </summary>
        private readonly List<string> _fileNamesToDelete = new List<string>();
#endif
        #endregion

        #region Methods
        /// <summary>
        ///   Cleans up all the mess caused by this class.
        /// </summary>
        public void CleanUp()
        {
#if SILVERLIGHT
            foreach (var file in _filestreamsToDispose)
            {
                try
                {
                    file.Dispose();
                }
                catch (Exception)
                {
                    // continue
                }
            }

            _filestreamsToDispose.Clear();

            try
            {
                _userStoreForApplication.DeleteDirectory("testfiles");
                _userStoreForApplication.Dispose();
                _userStoreForApplication = null;
            }
            catch (Exception)
            {
                // Continue
            }
#else
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
#endif
        }

        /// <summary>
        ///   Gets a temporary file.
        /// </summary>
        /// <returns> Filename of a new temporary file that is ready to be used. </returns>
#if SILVERLIGHT
        public IsolatedStorageFileStream GetTempFile()
#else
        public string GetTempFile()
#endif
        {
#if SILVERLIGHT
            if (_userStoreForApplication == null)
            {
                _userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();

#if !WINDOWS_PHONE
                if (!_shownWindow)
                {
                    const int RequestedSize = 25*1024*1024; // 25 MB
                    if (_userStoreForApplication.Quota < RequestedSize)
                    {
                        var increaseQuotaWindow = new IncreaseQuotaWindow(RequestedSize);
                        increaseQuotaWindow.Show();
                    }
                }
#endif
            }

            if (!_userStoreForApplication.DirectoryExists("testfiles"))
            {
                _userStoreForApplication.CreateDirectory("testfiles");
            }

            var fileStream = _userStoreForApplication.OpenFile(string.Format("Test_{0}.tmp", _counter++), FileMode.Create);
            _filestreamsToDispose.Add(fileStream);
            return fileStream;
#else
            string fileName = Path.GetTempFileName();
            _fileNamesToDelete.Add(fileName);
            return fileName;
#endif
        }
        #endregion
    }
}