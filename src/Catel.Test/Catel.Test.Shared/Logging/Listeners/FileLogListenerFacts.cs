// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogListenerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Logging.Listeners
{
    using System;
    using System.Reflection;
    using Catel.Logging;
    using Catel.Reflection;
    using NUnit.Framework;

    public class FileLogListenerFacts
    {
        [TestFixture]
        public class TheFilePathProperty
        {
            [TestCase(@"log.txt", @"{AppData}\CatenaLogic\Catel.Test\log.txt")]
            [TestCase(@"..\log.txt", @"{AppData}\CatenaLogic\log.txt")]
            [TestCase(@"c:\source\log.txt", @"c:\source\log.txt")]
            public void ReturnsRightFullPath(string path, string expectedPath)
            {
                var assembly = GetType().Assembly;

                var fileLogListener = new FileLogListener(path, 25000, assembly);
                fileLogListener.FilePath = path;

                var returnedPath = fileLogListener.FilePath;
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                returnedPath = returnedPath.Replace(appDataPath, "{AppData}");

                Assert.AreEqual(expectedPath, returnedPath);
            }
        }
    }
}

#endif