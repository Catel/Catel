﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Path.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IO
{
    using System;

#if NET
    using System.IO;
    using System.Reflection;

    using Reflection;
#endif

    /// <summary>
    /// Static class that implements some path methods
    /// </summary>
    public static class Path
    {
#if NET
        /// <summary>
		/// Gets the application data directory for the company and product as defined the the assembly information of the entry assembly. 
		/// If the entry assembly is <c>null</c>, this method will fall back to the calling assembly to retrieve the information.
		/// If the folder does not exist, the folder is automatically created by this method. 
        /// <para />
		/// This method returns a value like [application data]\[company]\[product name].
		/// </summary>
		/// <returns>Directory for the application data.</returns>
		public static string GetApplicationDataDirectory()
		{
			Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

			return GetApplicationDataDirectory(assembly.Company(), assembly.Product());
		}

		/// <summary>
		/// Gets the application data directory for a specific product. If the folder does not exist, the folder is automatically created by this method.
        /// <para />
		/// This method returns a value like [application data]\[product name].
		/// </summary>
		/// <param name="productName">Name of the product.</param>
		/// <returns>Directory for the application data.</returns>
		public static string GetApplicationDataDirectory(string productName)
		{
			return GetApplicationDataDirectory(string.Empty, productName);
		}

		/// <summary>
		/// Gets the application data directory for a specific product of a specific company. If the folder does not exist, the
		/// folder is automatically created by this method.
        /// <para />
		/// This method returns a value like [application data]\[company]\[product name].
		/// </summary>
		/// <param name="companyName">Name of the company.</param>
		/// <param name="productName">Name of the product.</param>
		/// <returns>Directory for the application data.</returns>
		public static string GetApplicationDataDirectory(string companyName, string productName)
		{
            string path = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), companyName, productName);

			if (!Directory.Exists(path))
			{
			    Directory.CreateDirectory(path);
			}

			return path;
		}
#endif

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        /// <param name="path">The path to get the directory name from.</param>
        /// <returns>The directory name.</returns>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is <c>null</c> or whitespace.</exception>
        public static string GetDirectoryName(string path)
        {
            Argument.IsNotNullOrWhitespace("path", path);

            return GetParentDirectory(path);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="path">The path to get the file name from.</param>
        /// <returns>The file name.</returns>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is <c>null</c> or whitespace.</exception>
        public static string GetFileName(string path)
        {
            Argument.IsNotNullOrWhitespace("path", path);

            int lastSlashPosition = path.LastIndexOf(@"\");
            if (lastSlashPosition == -1)
            {
                return path;
            }

            return path.Remove(0, lastSlashPosition + 1);
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <param name="path">The path to get the parent directory from.</param>
        /// <returns>Parent directory of a path. If there is no parent directory, <see cref="string.Empty"/> is returned.</returns>
        /// <remarks>
        /// This method will always strip the trailing backslash from the parent.
        /// </remarks>
        public static string GetParentDirectory(string path)
        {
            string parent = string.Empty;

            if (string.IsNullOrEmpty(path))
            {
                return parent;
            }

            path = RemoveTrailingSlashes(path);
            if (!path.Contains(@"\"))
            {
                return parent;
            }

            int lastSlashPosition = path.LastIndexOf(@"\");
            parent = path.Substring(0, lastSlashPosition);

            parent = RemoveTrailingSlashes(parent);

            return parent;
        }

        /// <summary>
        /// Returns a relative path string from a full path.
        /// <para />
        /// The path to convert. Can be either a file or a directory
        /// The base path to truncate to and replace
        /// <para />
        /// Lower case string of the relative path. If path is a directory it's returned 
        /// without a backslash at the end.
        /// <para />
        /// Examples of returned values:
        ///  .\test.txt, ..\test.txt, ..\..\..\test.txt, ., ..
        /// </summary>
        /// <param name="fullPath">Full path to convert to relative path.</param>
        /// <param name="basePath">The base path (a.k.a. working directory). If this parameter is <c>null</c> or empty, the current working directory will be used.</param>
        /// <returns>Relative path.</returns>
        /// <exception cref="ArgumentException">The <paramref name="fullPath"/> is <c>null</c> or whitespace.</exception>
        public static string GetRelativePath(string fullPath, string basePath = null)
        {
            Argument.IsNotNullOrWhitespace("fullPath", fullPath);

#if !NETFX_CORE && !PCL
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Environment.CurrentDirectory;
            }
#endif

            fullPath = RemoveTrailingSlashes(fullPath.ToLower());
            basePath = RemoveTrailingSlashes(basePath.ToLower());

            // Check if the base path is really the full path (not just a subpath, for example "C:\MyTes" in "C:\MyTest")
            string fullPathWithTrailingBackslash = AppendTrailingSlash(fullPath);
            string basePathWithTrailingBackslash = AppendTrailingSlash(basePath);

            if (fullPathWithTrailingBackslash.IndexOf(basePathWithTrailingBackslash) > -1)
            {
                string result = fullPath.Replace(basePath, string.Empty);
                if (result.StartsWith("\\"))
                {
                    result = result.Remove(0, 1);
                }

                return result;
            }

            string backDirs = string.Empty;
            string partialPath = basePath;
            int index = partialPath.LastIndexOf("\\");
            while (index > 0)
            {
                partialPath = AppendTrailingSlash(partialPath.Substring(0, index));
                backDirs = backDirs + "..\\";

                if (fullPathWithTrailingBackslash.IndexOf(partialPath) > -1)
                {
                    partialPath = RemoveTrailingSlashes(partialPath);
                    fullPath = RemoveTrailingSlashes(fullPath);

                    if (fullPath == partialPath)
                    {
                        // *** Full Directory match and need to replace it all
                        return fullPath.Replace(partialPath, backDirs.Substring(0, backDirs.Length - 1));
                    }
                    else
                    {
                        // *** We're dealing with a file or a start path
                        return fullPath.Replace(partialPath + (fullPath == partialPath ? string.Empty : "\\"), backDirs);
                    }
                }

                partialPath = RemoveTrailingSlashes(partialPath);
                index = partialPath.LastIndexOf("\\", partialPath.Length - 1);
            }

            return fullPath;
        }

#if NET
        /// <summary>
        /// Returns the full path for a relative path.
        /// </summary>
        /// <param name="relativePath">Relative path to convert to a full path.</param>
        /// <param name="basePath">Base path (a.k.a. working directory).</param>
        /// <returns>Full path.</returns>
        /// <exception cref="ArgumentException">The <paramref name="relativePath"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="basePath"/> is <c>null</c> or whitespace.</exception>
        public static string GetFullPath(string relativePath, string basePath)
        {
            Argument.IsNotNullOrWhitespace("relativePath", relativePath);
            Argument.IsNotNullOrWhitespace("basePath", basePath);

            string path = System.IO.Path.Combine(basePath, relativePath);

			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

            // Get the path info (it may contain any relative path items such as ..\, but
            // now it is safe to call GetFullPath))
        	path = System.IO.Path.GetFullPath(path);

        	return path;
        }
#endif

        /// <summary>
        /// Appends a trailing backslash (\) to the path.
        /// </summary>
        /// <param name="path">Path to append the trailing backslash to.</param>
        /// <returns>Path including the trailing backslash.</returns>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is <c>null</c> or whitespace.</exception>
        public static string AppendTrailingSlash(string path)
        {
            return AppendTrailingSlash(path, '\\');
        }

        /// <summary>
        /// Appends a trailing slash (\ or /) to the path.
        /// </summary>
        /// <param name="path">Path to append the trailing slash to.</param>
        /// <param name="slash">Slash to append (\ or /).</param>
        /// <returns>Path including the trailing slash.</returns>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is <c>null</c> or whitespace.</exception>
        public static string AppendTrailingSlash(string path, char slash)
        {
            Argument.IsNotNullOrWhitespace("path", path);

            if (path[path.Length - 1] == slash)
            {
                return path;
            }

            return path + slash;
        }

#if !PCL
        /// <summary>
        /// Returns a combination of multiple paths.
        /// </summary>
        /// <param name="paths">Paths to combine.</param>
        /// <returns>Combination of all the paths passed.</returns>
        public static string Combine(params string[] paths)
        {
            string result = string.Empty;

            // Make sure we have any values
            if (paths.Length == 0)
            {
                return string.Empty;
            }

            if (paths.Length == 1)
            {
                return paths[0];
            }

            for (int i = 0; i < paths.Length; i++)
            {
                if (!string.IsNullOrEmpty(paths[i]))
                {
                    result = System.IO.Path.Combine(result, paths[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a combination of multiple urls.
        /// </summary>
        /// <param name="urls">Urls to combine.</param>
        /// <returns>Combination of all the urls passed.</returns>
        public static string CombineUrls(params string[] urls)
        {
            string result = string.Empty;

            if (urls.Length == 0)
            {
                return string.Empty;
            }

            if (urls.Length == 1)
            {
                return ReplacePathSlashesByUrlSlashes(urls[0]);
            }

            // Store first path (remove trailing slashes only since we want to support root urls)
            result = RemoveTrailingSlashes(urls[0]);

            for (int i = 1; i < urls.Length; i++)
            {
                if (!string.IsNullOrEmpty(urls[i]))
                {
                    result = RemoveTrailingSlashes(result);

                    string tempPath = RemoveStartAndTrailingSlashes(urls[i]);

                    result = Combine(result, tempPath);
                }
            }

            return ReplacePathSlashesByUrlSlashes(result);
        }
#endif

        /// <summary>
        /// Replaces path slashes (\) by url slashes (/).
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Prepared url.</returns>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is <c>null</c> or whitespace.</exception>
        internal static string ReplacePathSlashesByUrlSlashes(string value)
        {
            Argument.IsNotNullOrWhitespace("value", value);

            return value.Replace("\\", "/");
        }

        /// <summary>
        /// Removes any slashes (\ or /) at the beginning of the string.
        /// </summary>
        /// <param name="value">Value to remove the slashes from.</param>
        /// <returns>Value without slashes.</returns>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is <c>null</c> or whitespace.</exception>
        internal static string RemoveStartSlashes(string value)
        {
            Argument.IsNotNullOrWhitespace("value", value);

            while ((value.Length > 0) && ((value[0] == '\\') || (value[0] == '/')))
            {
                value = value.Remove(0, 1);
            }

            return value;
        }

        /// <summary>
        /// Removes any slashes (\ or /) at the end of the string.
        /// </summary>
        /// <param name="value">Value to remove the slashes from.</param>
        /// <returns>Value without slashes.</returns>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is <c>null</c> or whitespace.</exception>
        internal static string RemoveTrailingSlashes(string value)
        {
            Argument.IsNotNullOrWhitespace("value", value);

            while ((value.Length > 0) && ((value[value.Length - 1] == '\\') || (value[value.Length - 1] == '/')))
            {
                value = value.Remove(value.Length - 1, 1);
            }

            return value;
        }

        /// <summary>
        /// Removes any slashes (\ or /) at the beginning and end of the string.
        /// </summary>
        /// <param name="value">Value to remove the slashes from.</param>
        /// <returns>Value without trailing slashes.</returns>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is <c>null</c> or whitespace.</exception>
        internal static string RemoveStartAndTrailingSlashes(string value)
        {
            Argument.IsNotNullOrWhitespace("value", value);

            value = RemoveStartSlashes(value);
            value = RemoveTrailingSlashes(value);

            return value;
        }
    }
}
