// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Windows.Helpers
{
    using System.Linq;

    using Catel.Windows;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineHelper_Facts
    {
        #region Nested type: TheParseMethod
        [TestFixture]
        public class The_Parse_Method
        {
            #region Methods
            [TestCase]
            public void Ignores_The_Starting_Spaces()
            {
                var arguments = CommandLineHelper.Parse("   install \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln\" -outputDirectory \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\lib\" -source https://nuget.org/api/v2/");
                Assert.AreEqual("install", arguments[0]);
            }

            [TestCase]
            public void Ignores_The_Trailing_Spaces()
            {
                var arguments = CommandLineHelper.Parse("   install \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln\" -outputDirectory \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\lib\" -source https://nuget.org/api/v2/      ");
                Assert.AreEqual("https://nuget.org/api/v2/", arguments[5]);
            }

            [TestCase]
            public void Parse_Quoted_Arguments()
            {
                var arguments = CommandLineHelper.Parse("install \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln\" -outputDirectory \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\lib\" -source https://nuget.org/api/v2/");
                Assert.AreEqual("c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln", arguments[1]);
            }
            
            [TestCase]
            public void Parse_The_Last_Quoted_Argument_Even_When_The_Quote_Mark_Is_Not_Closed()
            {
                var arguments = CommandLineHelper.Parse("install \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln");
                Assert.AreEqual("c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln", arguments[1]);
            }

            [TestCase]
            public void Parse_The_Right_Count_Of_Arguments()
            {
                var arguments = CommandLineHelper.Parse("install \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\src\\Catel.sln\" -outputDirectory \"c:\\Users\\alexander.fernandez\\Documents\\Visual Studio 2012\\Projects\\Catel\\lib\" -source https://nuget.org/api/v2/");
                Assert.AreEqual(6, arguments.Count());
            }
            #endregion
        }
        #endregion
    }
}

#endif