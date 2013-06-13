// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
#if !NET
    using System.IO;
#endif

    /// <summary>
	/// Interface for the Save File service.
	/// </summary>
	public interface ISaveFileService : IFileSupport
	{
#if !NET
		/// <summary>
		/// Determines the filename of the file what will be used.
		/// </summary>
		/// <returns>The <see cref="Stream"/> of the file or <c>null</c> if no file was selected by the user.</returns>
		/// <remarks>
		/// If this method returns a valid <see cref="Stream"/> object, the <see cref="IFileSupport.FileName"/> property will be filled 
		/// with the safe filename. This can be used for display purposes only.
		/// </remarks>
		Stream DetermineFile();
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="IFileSupport.FileName"/> property will be filled with the filenames. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        bool DetermineFile();
#endif
	}
}
