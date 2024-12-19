﻿namespace Catel.Tests.IO
{
    using System;
    using System.IO;
    using System.Reflection;

    using Catel.Reflection;

    using NUnit.Framework;

    using Path = Catel.IO.Path;

    /// <summary>
    ///   Summary description for PathTest
    /// </summary>
    [TestFixture]
    public class PathTest
    {
        private string _testDirectory;

        [SetUp]
        public void Initialize()
        {
            // Determine test directory
            _testDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "PathTest");

            // Delete directory, than create it
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }

            Directory.CreateDirectory(_testDirectory);
        }

        [TearDown]
        public void CleanUp()
        {
            // Delete test directory
            if (!string.IsNullOrEmpty(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        //[TestCase]
        //public void GetApplicationData_EntryAssembly()
        //{
        //    string expected = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        //                                   Assembly.GetExecutingAssembly().Company(), Assembly.GetExecutingAssembly().Product());

        //    string result = Path.GetApplicationDataDirectory();

        //    Assert.AreEqual(expected, result);
        //}

        [TestCase]
        public void GetApplicationDataDirectory_AppOnly()
        {
            string expected = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                           Assembly.GetExecutingAssembly().Product());

            string result = Path.GetApplicationDataDirectory(Assembly.GetExecutingAssembly().Product());

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void GetApplicationDataDirectory_CompanyAndApp()
        {
            string expected = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                           Assembly.GetExecutingAssembly().Company(), Assembly.GetExecutingAssembly().Product());

            string result = Path.GetApplicationDataDirectory(Assembly.GetExecutingAssembly().Company(), Assembly.GetExecutingAssembly().Product());

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void GetApplicationDataDirectoryForAllUsers_AppOnly()
        {
            string expected = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                           Assembly.GetExecutingAssembly().Product());

            string result = Path.GetApplicationDataDirectoryForAllUsers(Assembly.GetExecutingAssembly().Product());

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase]
        public void GetApplicationDataDirectoryForAllUsers_CompanyAndApp()
        {
            string expected = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                           Assembly.GetExecutingAssembly().Company(), Assembly.GetExecutingAssembly().Product());

            string result = Path.GetApplicationDataDirectoryForAllUsers(Assembly.GetExecutingAssembly().Company(), Assembly.GetExecutingAssembly().Product());

            Assert.That(result, Is.EqualTo(expected));
        }

        #region GetDirectoryName
        [TestCase]
        public void GetDirectoryName_NormalDirectory()
        {
            string result = Path.GetDirectoryName(@"C:\ParentDirectory\ChildDirectory");

            Assert.That(result, Is.EqualTo(@"C:\ParentDirectory"));
        }

        [TestCase]
        public void GetDirectoryName_RootDirectory()
        {
            string result = Path.GetDirectoryName(@"C:\");

            Assert.That(result, Is.EqualTo(string.Empty));
        }
        #endregion

        #region GetFileName
        [TestCase]
        public void GetFileName_WithDirectory()
        {
            // Declare variables
            string input = @"C:\WINDOWS\notepad.exe";
            string expectedOutput = "notepad.exe";

            // Call method
            string output = Path.GetFileName(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }

        [TestCase]
        public void GetFileName_WithoutDirectory()
        {
            // Declare variables
            string input = @"notepad.exe";
            string expectedOutput = "notepad.exe";

            // Call method
            string output = Path.GetFileName(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }

        [TestCase]
        public void GetFileName_EmptyInput()
        {
            Assert.Throws<ArgumentException>(() => Path.GetFileName(null));
            Assert.Throws<ArgumentException>(() => Path.GetFileName(string.Empty));
        }
        #endregion

        #region GetParentDirectory
        [TestCase]
        public void GetParentDirectory_File()
        {
            // Declare variables
            string input = @"C:\MyPathThatDoesntExist\MyDirectory\MyFile.txt";
            string expectedOutput = @"C:\MyPathThatDoesntExist\MyDirectory";

            // Call method
            string output = Path.GetParentDirectory(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }

        [TestCase]
        public void GetParentDirectory_DirectoryEndingWithSlash()
        {
            // Declare variables
            string input = @"C:\MyPathThatDoesntExist\MyDirectory\";
            string expectedOutput = @"C:\MyPathThatDoesntExist";

            // Call method
            string output = Path.GetParentDirectory(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }

        [TestCase]
        public void GetParentDirectory_DirectoryNotEndingWithSlash()
        {
            // Declare variables
            string input = @"C:\MyPathThatDoesntExist\MyDirectory";
            string expectedOutput = @"C:\MyPathThatDoesntExist";

            // Call method
            string output = Path.GetParentDirectory(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }

        [TestCase]
        public void GetParentDirectory_InvalidInput()
        {
            // Declare variables
            string input = @"abse";
            string expectedOutput = string.Empty;

            // Call method
            string output = Path.GetParentDirectory(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }

        [TestCase]
        public void GetParentDirectory_EmptyInput()
        {
            // Declare variables
            string input = string.Empty;
            string expectedOutput = string.Empty;

            // Call method
            string output = Path.GetParentDirectory(input);

            // Check result
            Assert.That(output, Is.EqualTo(expectedOutput));
        }
        #endregion

        #region GetRelativePath
        [TestCase]
        public void GetRelativePath_Root()
        {
            // Declare variables
            string file = @"C:\Windows\notepad.exe";
            string path = @"C:\";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_SingleDirectory()
        {
            // Declare variables
            string file = @"C:\Windows\notepad.exe";
            string path = @"C:\Windows\";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_SingleDirectoryNotEndingWithSlash()
        {
            // Declare variables
            string file = @"C:\Windows\notepad.exe";
            string path = @"C:\Windows";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_DeeperDirectory()
        {
            // Declare variables
            string file = @"C:\Windows\notepad.exe";
            string path = @"C:\Windows\Temp\";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"..\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_SameDirectoryLevelWithDifferentName()
        {
            // Declare variables
            string file = @"C:\Windows\MyTest\MyFile.exe";
            string path = @"C:\Windows\MyTes";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"..\MyTest\MyFile.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_EmptyBasePath()
        {
            // Get current directory
            string currentWorkingDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = @"C:\Windows\System32";

            // Declare variables
            string file = @"C:\Windows\MyTest\MyFile.exe";

            // Call method
            string relative = Path.GetRelativePath(file);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"..\MyTest\MyFile.exe".ToLower()));

            // Restore current working directory
            Environment.CurrentDirectory = currentWorkingDirectory;
        }

        [TestCase]
        public void GetRelativePath_DeepTree()
        {
            // Declare variables
            string file = @"C:\Windows\Level1_\Level2_";
            string path = @"C:\Windows\Level1\Level2";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"..\..\Level1_\Level2_".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_HigherDirectory()
        {
            // Declare variables
            string file = @"C:\Windows\";
            string path = @"C:\Windows\Level1\Level2";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"..\..".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_DifferentBaseDirectory()
        {
            // Declare variables
            string file = @"C:\Windows\notepad.exe";
            string path = @"C:\MyTest\";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"..\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_DifferentRoot()
        {
            // Declare variables
            string file = @"C:\Windows\notepad.exe";
            string path = @"D:\Windows\";

            // Call method
            string relative = Path.GetRelativePath(file, path);

            // Validate
            Assert.That(relative.ToLower(), Is.EqualTo(@"C:\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetRelativePath_InvalidInput()
        {
            Assert.Throws<ArgumentException>(() => Path.GetRelativePath(null, @"C:\test\"));
        }
        #endregion

        #region GetFullPath
        [TestCase]
        public void GetFullPath_FromRootDirectory()
        {
            // Declare variables
            string file = @"Windows\notepad.exe";
            string path = @"C:\";

            // Call method
            string full = Path.GetFullPath(file, path);

            // Validate
            Assert.That(full.ToLower(), Is.EqualTo(@"C:\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetFullPath_FileNameOnly()
        {
            // Declare variables
            string file = @"notepad.exe";
            string path = @"C:\Windows\";

            // Call method
            string full = Path.GetFullPath(file, path);

            // Validate
            Assert.That(full.ToLower(), Is.EqualTo(@"C:\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetFullPath_FileNameOnlyAndDirectoryWithoutTrailingSlash()
        {
            // Declare variables
            string file = @"notepad.exe";
            string path = @"C:\Windows";

            // Call method
            string full = Path.GetFullPath(file, path);

            // Validate
            Assert.That(full.ToLower(), Is.EqualTo(@"C:\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetFullPath_RelativeDotsDirectory()
        {
            // Declare variables
            string file = @"..\notepad.exe";
            string path = @"C:\Windows\Temp\";

            // Call method
            string full = Path.GetFullPath(file, path);

            // Validate
            Assert.That(full.ToLower(), Is.EqualTo(@"C:\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetFullPath_RelativeDotsAndNameDirectory()
        {
            // Declare variables
            string file = @"..\Windows\notepad.exe";
            string path = @"C:\Program Files\";

            // Call method
            string full = Path.GetFullPath(file, path);

            // Validate
            Assert.That(full.ToLower(), Is.EqualTo(@"C:\Windows\notepad.exe".ToLower()));
        }

        [TestCase]
        public void GetFullPath_NoBasePath()
        {
            // Declare variables
            string file = @"..\Windows\notepad.exe";
            string path = string.Empty;

            // Set current environment path
            string oldEnvironmentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = @"C:\Program Files\";

            Assert.Throws<ArgumentException>(() => Path.GetFullPath(file, path));
        }
        #endregion

        #region AppendTrailing
        [TestCase]
        public void AppendTrailingSlash_EmptyValue()
        {
            Assert.Throws<ArgumentException>(() => Path.AppendTrailingSlash(null));
            Assert.Throws<ArgumentException>(() => Path.AppendTrailingSlash(string.Empty));
        }

        [TestCase]
        public void AppendTrailingBackslash_WithoutTrailingBackslash()
        {
            // Declare variables
            string path = @"C:\Windows";

            // Call method
            string result = Path.AppendTrailingSlash(path);

            // Validate
            Assert.That(result, Is.EqualTo(@"C:\Windows\"));
        }

        [TestCase]
        public void AppendTrailingSlash_WithoutTrailingSlash()
        {
            // Declare variables
            string path = @"http://www.catenalogic.com";

            // Call method
            string result = Path.AppendTrailingSlash(path, '/');

            // Validate
            Assert.That(result, Is.EqualTo(@"http://www.catenalogic.com/"));
        }

        [TestCase]
        public void AppendTrailingBackslash_WithTrailingBackslash()
        {
            // Declare variables
            string path = @"C:\Windows\";

            // Call method
            string result = Path.AppendTrailingSlash(path);

            // Validate
            Assert.That(result, Is.EqualTo(@"C:\Windows\"));
        }

        [TestCase]
        public void AppendTrailingSlash_WithTrailingSlash()
        {
            // Declare variables
            string path = @"http://www.catenalogic.com/";

            // Call method
            string result = Path.AppendTrailingSlash(path, '/');

            // Validate
            Assert.That(result, Is.EqualTo(@"http://www.catenalogic.com/"));
        }
        #endregion

        #region RemoveStartSlashes
        [TestCase]
        public void RemoveStartSlashes_EmptyInput()
        {
            Assert.Throws<ArgumentException>(() => Path.RemoveStartSlashes(null));
            Assert.Throws<ArgumentException>(() => Path.RemoveStartSlashes(string.Empty));
        }

        [TestCase]
        public void RemoveStartSlashes_StartingWithSlash()
        {
            string result = Path.RemoveStartSlashes(@"\withStartSlash");

            Assert.That(result, Is.EqualTo(@"withStartSlash"));
        }

        [TestCase]
        public void RemoveStartSlashes_NotStartingWithSlash()
        {
            string result = Path.RemoveStartSlashes(@"withoutStartSlash");

            Assert.That(result, Is.EqualTo(@"withoutStartSlash"));
        }
        #endregion

        #region RemoveTrailingSlashes
        [TestCase]
        public void RemoveTrailingSlashes_EmptyInput()
        {
            Assert.Throws<ArgumentException>(() => Path.RemoveTrailingSlashes(null));
            Assert.Throws<ArgumentException>(() => Path.RemoveTrailingSlashes(string.Empty));
        }

        [TestCase]
        public void RemoveTrailingSlashes_EndingWithSlash()
        {
            string result = Path.RemoveTrailingSlashes(@"withEndingSlash\");

            Assert.That(result, Is.EqualTo(@"withEndingSlash"));
        }

        [TestCase]
        public void RemoveTrailingSlashes_NotRemoveTrailingSlashes_EndingWithSlash()
        {
            string result = Path.RemoveTrailingSlashes(@"withoutEndingSlash");

            Assert.That(result, Is.EqualTo(@"withoutEndingSlash"));
        }
        #endregion
    }
}
