namespace Catel.Tests.Logging.Listeners
{
    using System;
    using Catel.Logging;
    using NUnit.Framework;

    public class FileLogListenerFacts
    {
        [TestFixture]
        public class TheFilePathProperty
        {
            [TestCase(@"myapp.log", @"{AppData}\CatenaLogic\Catel.Tests\myapp.log")]
            [TestCase(@"..\myapp.log", @"{AppData}\CatenaLogic\myapp.log")]
            [TestCase(@"c:\source\myapp.log", @"c:\source\myapp.log")]
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
