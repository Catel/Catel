// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System.Collections.Generic;

    /// <summary>
    /// The command line helper
    /// </summary>
    public static class CommandLineHelper
    {
        #region Methods
        /// <summary>
        /// Parse a command line arguments.
        /// </summary>
        /// <param name="arguments">A command line like argument string</param>
        /// <returns>An array of string</returns>
        public static string[] Parse(string arguments)
        {
            var argumentList = new List<string>();

            string current = string.Empty;
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == '\"')
                {
                    int j = i + 1;
                    while (j < arguments.Length && arguments[j] != '\"')
                    {
                        current += arguments[j++];
                    }

                    i = j;
                }
                else if (arguments[i] == ' ')
                {
                    if (current != string.Empty)
                    {
                        argumentList.Add(current);
                        current = string.Empty;
                    }
                }
                else
                {
                    current += arguments[i];
                }
            }

            if (current != string.Empty)
            {
                argumentList.Add(current);
            }

            return argumentList.ToArray();
        }
        #endregion
    }
}