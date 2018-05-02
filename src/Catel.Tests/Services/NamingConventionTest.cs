// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamingConventionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Catel.Services;

    using NUnit.Framework;

    public class NamingConventionFacts
    {
        [TestFixture]
        public class TheResolveViewModelByViewNameMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName(null, "MyView", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName(string.Empty, "MyView", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullViewName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", null, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyViewName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", string.Empty, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", "ExampleView", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", "ExampleView", string.Empty));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithoutConstant()
            {
                string viewName = "ExampleView";
                string convention = "ViewModels.TestViewModel";

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("ViewModels.TestViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstant()
            {
                string viewName = "ExampleView";
                string convention = string.Format("ViewModels.{0}ViewModel", NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("ViewModels.ExampleViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstantWithoutViewPostfix()
            {
                string viewName = "ExampleVW";
                string convention = string.Format("ViewModels.{0}ViewModel", NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("ViewModels.ExampleVWViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithMultipleConstants()
            {
                string viewName = "ExampleView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithView()
            {
                string viewName = "ExampleView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithControl()
            {
                string viewName = "ExampleControl";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithWindow()
            {
                string viewName = "ExampleWindow";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithPage()
            {
                string viewName = "ExamplePage";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithBothControlAndView()
            {
                string viewName = "ExampleControlView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleControlViewModel", result);
            }
        }

        [TestFixture]
        public class TheResolveViewByViewModelNameMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName(null, "MyViewModel", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName(string.Empty, "MyViewModel", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullViewModelName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", null, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyViewModelName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", string.Empty, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", "ExampleViewModel", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", "ExampleViewModel", string.Empty));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithoutConstant()
            {
                string viewModelName = "ExampleViewModel";
                string convention = "/Views/TestView.xaml";

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("/Views/TestView.xaml", result);
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstant()
            {
                string viewModelName = "ExampleViewModel";
                string convention = string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("/Views/ExampleView.xaml", result);
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstantWithoutViewModelPostfix()
            {
                string viewModelName = "ExampleVM";
                string convention = string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("/Views/ExampleVMView.xaml", result);
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithMultipleConstants()
            {
                string viewModelName = "ExampleViewModel";
                string convention = string.Format("{0}.Views.{1}s.{1}View", NamingConvention.Assembly, NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("MyProject.MyAssembly.Views.Examples.ExampleView", result);
            }
        }

        [TestFixture]
        public class TheResolveNamingConventionMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullConstantsWithValues()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => NamingConvention.ResolveNamingConvention(null, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), string.Empty));
            }

            [TestCase]
            public void ReturnsRightValueForMultipleConstantsWhereOnlyOneIsUsed()
            {
                var constantValues = new Dictionary<string, string>();
                constantValues.Add("[VM]", "VmValue");
                constantValues.Add("[TEST]", "TestValue");
                string convention = "/Views/[VM]View.xaml";

                string result = NamingConvention.ResolveNamingConvention(constantValues, convention);

                Assert.AreEqual("/Views/VmValueView.xaml", result);
            }

            [TestCase]
            public void ReturnsRightValueForMultipleConstantsWhereAllAreUsed()
            {
                var constantValues = new Dictionary<string, string>();
                constantValues.Add("[VM]", "VmValue");
                constantValues.Add("[TEST]", "TestValue");
                string convention = "/Views/[TEST]/[VM]View.xaml";

                string result = NamingConvention.ResolveNamingConvention(constantValues, convention);

                Assert.AreEqual("/Views/TestValue/VmValueView.xaml", result);
            }
        }

        [TestFixture]
        public class TheResolveNamingConventionWithValueMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullConstantsWithValues()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => NamingConvention.ResolveNamingConvention(null, "MyConvention", "MyValue"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), null, "MyValue"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), string.Empty, "MyValue"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), "MyConvention", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), "MyConvention", string.Empty));
            }

            [TestCase]
            public void ReturnsRightValueWhenUsingNoUps()
            {
                var constantValues = new Dictionary<string, string> { { "[VM]", "My" } };
                const string convention = "Catel.Views.[VM]View";
                const string value = "Catel.Examples.ViewModels.MyViewModel";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.AreEqual("Catel.Views.MyView", result);
            }

            [TestCase]
            public void ReturnsRightValueWhenUsingSingleUp()
            {
                var constantValues = new Dictionary<string, string> { { "[VM]", "My" } };
                const string convention = "[UP].Views.[VM]View";
                const string value = "Catel.Examples.ViewModels.MyViewModel";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.AreEqual("Catel.Examples.Views.MyView", result);
            }

            [TestCase]
            public void ReturnsRightValueWhenUsingMultipleUps()
            {
                var constantValues = new Dictionary<string, string> { { "[VW]", "MyWizard" } };
                const string convention = "[UP].[UP].ViewModels.Wizards.[VW]ViewModel";
                const string value = "Catel.Examples.Views.Wizards.MyWizardView";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.AreEqual("Catel.Examples.ViewModels.Wizards.MyWizardViewModel", result);
            }
        }

        [TestFixture]
        public class TheGetParentPathMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(null, "\\"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(string.Empty, "\\"));
            }

            [TestCase]
            public void ReturnsInputWhenNoPathSeparatorIsAutomaticallyDetected()
            {
                Assert.AreEqual("pathwithoutseparator", NamingConvention.GetParentPath("pathwithoutseparator"));
            }

            [TestCase]
            public void ReturnsInputWhenPathSeparatorIsAutomaticallyDetected()
            {
                Assert.AreEqual("Catel.Examples", NamingConvention.GetParentPath("Catel.Examples.Views"));
            }

            [TestCase]
            public void ReturnsRightParentPathForDotSeparator()
            {
                Assert.AreEqual("Catel.Examples", NamingConvention.GetParentPath("Catel.Examples.Views", "."));
            }

            [TestCase]
            public void ReturnsRightParentPathForSlashSeparator()
            {
                Assert.AreEqual("Catel\\Examples", NamingConvention.GetParentPath("Catel\\Examples\\Views.xaml", "\\"));
            }
        }

        [TestFixture]
        public class TheGetParentSeparatorMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentSeparator(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentSeparator(string.Empty));
            }

            [TestCase]
            public void ReturnsNullWhenNoKnownSeparatorIsUsed()
            {
                Assert.AreEqual(null, NamingConvention.GetParentSeparator("noknownseparators"));
            }

            [TestCase]
            public void ReturnsBackSlashWhenUsed()
            {
                Assert.AreEqual("\\", NamingConvention.GetParentSeparator("Catel\\Views\\MyView.xaml"));
            }

            [TestCase]
            public void ReturnsSlashWhenUsed()
            {
                Assert.AreEqual("/", NamingConvention.GetParentSeparator("Catel/Views/MyView.xaml"));
            }

            [TestCase]
            public void ReturnsPipeWhenUsed()
            {
                Assert.AreEqual("|", NamingConvention.GetParentSeparator("Catel|Views|MyView.xaml"));
            }

            [TestCase]
            public void ReturnsDotWhenUsed()
            {
                Assert.AreEqual(".", NamingConvention.GetParentSeparator("Catel.Views.MyView"));
            }
        }
    }
}