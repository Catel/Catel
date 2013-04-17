// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSupport.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    /// <summary>
	/// Interface that supports file handling.
	/// </summary>
	public interface IFileSupport
	{
		#region Properties
		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		string FileName { get; set; }

#if NET
		/// <summary>
		/// Gets or sets the initial directory.
		/// </summary>
		/// <value>The initial directory.</value>
		string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }
#endif

        /// <summary>
		/// Gets or sets the filter to use when opening or saving the file.
		/// </summary>
		/// <value>The filter.</value>
		string Filter { get; set; }
		#endregion
    }
}
