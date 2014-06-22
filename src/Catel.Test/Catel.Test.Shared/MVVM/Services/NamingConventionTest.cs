// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamingConventionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Services
{
    using System;
    using System.Collections.Generic;
    using Catel.Services;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class NamingConventionFacts
    {
        [TestClass]
        public class TheResolveViewModelByViewNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName(null, "MyView", "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName(string.Empty, "MyView", "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullViewName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", null, "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyViewName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", string.Empty, "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", "ExampleView", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", "ExampleView", string.Empty));
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithoutConstant()
            {
                string viewName = "ExampleView";
                string convention = "ViewModels.TestViewModel";

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("ViewModels.TestViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithSingleConstant()
            {
                string viewName = "ExampleView";
                string convention = string.Format("ViewModels.{0}ViewModel", NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("ViewModels.ExampleViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithSingleConstantWithoutViewPostfix()
            {
                string viewName = "ExampleVW";
                string convention = string.Format("ViewModels.{0}ViewModel", NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("ViewModels.ExampleVWViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithMultipleConstants()
            {
                string viewName = "ExampleView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForViewEndingWithView()
            {
                string viewName = "ExampleView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForViewEndingWithControl()
            {
                string viewName = "ExampleControl";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForViewEndingWithWindow()
            {
                string viewName = "ExampleWindow";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForViewEndingWithPage()
            {
                string viewName = "ExamplePage";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleViewModel", result);
            }

            [TestMethod]
            public void ReturnsRightResultForViewEndingWithBothControlAndView()
            {
                string viewName = "ExampleControlView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.AreEqual("MyProject.MyAssembly.ViewModels.ExampleControlViewModel", result);
            }
        }

        [TestClass]
        public class TheResolveViewByViewModelNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName(null, "MyViewModel", "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyAssembly()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName(string.Empty, "MyViewModel", "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullViewModelName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", null, "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyViewModelName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", string.Empty, "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", "ExampleViewModel", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", "ExampleViewModel", string.Empty));
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithoutConstant()
            {
                string viewModelName = "ExampleViewModel";
                string convention = "/Views/TestView.xaml";

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("/Views/TestView.xaml", result);
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithSingleConstant()
            {
                string viewModelName = "ExampleViewModel";
                string convention = string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("/Views/ExampleView.xaml", result);
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithSingleConstantWithoutViewModelPostfix()
            {
                string viewModelName = "ExampleVM";
                string convention = string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("/Views/ExampleVMView.xaml", result);
            }

            [TestMethod]
            public void ReturnsRightResultForConventionWithMultipleConstants()
            {
                string viewModelName = "ExampleViewModel";
                string convention = string.Format("{0}.Views.{1}s.{1}View", NamingConvention.Assembly, NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.AreEqual("MyProject.MyAssembly.Views.Examples.ExampleView", result);
            }
        }

        [TestClass]
        public class TheResolveNamingConventionMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullConstantsWithValues()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => NamingConvention.ResolveNamingConvention(null, "MyConvention"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), string.Empty));
            }

            [TestMethod]
            public void ReturnsRightValueForMultipleConstantsWhereOnlyOneIsUsed()
            {
                var constantValues = new Dictionary<string, string>();
                constantValues.Add("[VM]", "VmValue");
                constantValues.Add("[TEST]", "TestValue");
                string convention = "/Views/[VM]View.xaml";

                string result = NamingConvention.ResolveNamingConvention(constantValues, convention);

                Assert.AreEqual("/Views/VmValueView.xaml", result);
            }

            [TestMethod]
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

        [TestClass]
        public class TheResolveNamingConventionWithValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullConstantsWithValues()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => NamingConvention.ResolveNamingConvention(null, "MyConvention", "MyValue"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), null, "MyValue"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyNamingConvention()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), string.Empty, "MyValue"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), "MyConvention", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), "MyConvention", string.Empty));
            }

            [TestMethod]
            public void ReturnsRightValueWhenUsingNoUps()
            {
                var constantValues = new Dictionary<string, string> { { "[VM]", "My" } };
                const string convention = "Catel.Views.[VM]View";
                const string value = "Catel.Examples.ViewModels.MyViewModel";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.AreEqual("Catel.Views.MyView", result);
            }

            [TestMethod]
            public void ReturnsRightValueWhenUsingSingleUp()
            {
                var constantValues = new Dictionary<string, string> { { "[VM]", "My" } };
                const string convention = "[UP].Views.[VM]View";
                const string value = "Catel.Examples.ViewModels.MyViewModel";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.AreEqual("Catel.Examples.Views.MyView", result);
            }

            [TestMethod]
            public void ReturnsRightValueWhenUsingMultipleUps()
            {
                var constantValues = new Dictionary<string, string> { { "[VW]", "MyWizard" } };
                const string convention = "[UP].[UP].ViewModels.Wizards.[VW]ViewModel";
                const string value = "Catel.Examples.Views.Wizards.MyWizardView";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.AreEqual("Catel.Examples.ViewModels.Wizards.MyWizardViewModel", result);
            }
        }

        [TestClass]
        public class TheGetParentPathMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(null, "\\"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentPath(string.Empty, "\\"));
            }

            [TestMethod]
            public void ReturnsInputWhenNoPathSeparatorIsAutomaticallyDetected()
            {
                Assert.AreEqual("pathwithoutseparator", NamingConvention.GetParentPath("pathwithoutseparator"));
            }

            [TestMethod]
            public void ReturnsInputWhenPathSeparatorIsAutomaticallyDetected()
            {
                Assert.AreEqual("Catel.Examples", NamingConvention.GetParentPath("Catel.Examples.Views"));
            }

            [TestMethod]
            public void ReturnsRightParentPathForDotSeparator()
            {
                Assert.AreEqual("Catel.Examples", NamingConvention.GetParentPath("Catel.Examples.Views", "."));
            }

            [TestMethod]
            public void ReturnsRightParentPathForSlashSeparator()
            {
                Assert.AreEqual("Catel\\Examples", NamingConvention.GetParentPath("Catel\\Examples\\Views.xaml", "\\"));
            }
        }

        [TestClass]
        public class TheGetParentSeparatorMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentSeparator(null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyPath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => NamingConvention.GetParentSeparator(string.Empty));
            }

            [TestMethod]
            public void ReturnsNullWhenNoKnownSeparatorIsUsed()
            {
                Assert.AreEqual(null, NamingConvention.GetParentSeparator("noknownseparators"));
            }

            [TestMethod]
            public void ReturnsBackSlashWhenUsed()
            {
                Assert.AreEqual("\\", NamingConvention.GetParentSeparator("Catel\\Views\\MyView.xaml"));
            }

            [TestMethod]
            public void ReturnsSlashWhenUsed()
            {
                Assert.AreEqual("/", NamingConvention.GetParentSeparator("Catel/Views/MyView.xaml"));
            }

            [TestMethod]
            public void ReturnsPipeWhenUsed()
            {
                Assert.AreEqual("|", NamingConvention.GetParentSeparator("Catel|Views|MyView.xaml"));
            }

            [TestMethod]
            public void ReturnsDotWhenUsed()
            {
                Assert.AreEqual(".", NamingConvention.GetParentSeparator("Catel.Views.MyView"));
            }
        }
    }
}