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
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName(null, "MyView", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyAssembly()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName(string.Empty, "MyView", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullViewName()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", null, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyViewName()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", string.Empty, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", "ExampleView", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", "ExampleView", string.Empty));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithoutConstant()
            {
                string viewName = "ExampleView";
                string convention = "ViewModels.TestViewModel";

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("ViewModels.TestViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstant()
            {
                string viewName = "ExampleView";
                string convention = string.Format("ViewModels.{0}ViewModel", NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("ViewModels.ExampleViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstantWithoutViewPostfix()
            {
                string viewName = "ExampleVW";
                string convention = string.Format("ViewModels.{0}ViewModel", NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("ViewModels.ExampleVWViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithMultipleConstants()
            {
                string viewName = "ExampleView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.ViewModels.ExampleViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithView()
            {
                string viewName = "ExampleView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.ViewModels.ExampleViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithControl()
            {
                string viewName = "ExampleControl";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.ViewModels.ExampleViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithWindow()
            {
                string viewName = "ExampleWindow";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.ViewModels.ExampleViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithPage()
            {
                string viewName = "ExamplePage";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.ViewModels.ExampleViewModel"));
            }

            [TestCase]
            public void ReturnsRightResultForViewEndingWithBothControlAndView()
            {
                string viewName = "ExampleControlView";
                string convention = string.Format("{0}.ViewModels.{1}ViewModel", NamingConvention.Assembly, NamingConvention.ViewName);

                string result = NamingConvention.ResolveViewModelByViewName("MyProject.MyAssembly", viewName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.ViewModels.ExampleControlViewModel"));
            }
        }

        [TestFixture]
        public class TheResolveViewByViewModelNameMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullAssembly()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName(null, "MyViewModel", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyAssembly()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName(string.Empty, "MyViewModel", "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullViewModelName()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", null, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyViewModelName()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", string.Empty, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", "ExampleViewModel", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", "ExampleViewModel", string.Empty));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithoutConstant()
            {
                string viewModelName = "ExampleViewModel";
                string convention = "/Views/TestView.xaml";

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.That(result, Is.EqualTo("/Views/TestView.xaml"));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstant()
            {
                string viewModelName = "ExampleViewModel";
                string convention = string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.That(result, Is.EqualTo("/Views/ExampleView.xaml"));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithSingleConstantWithoutViewModelPostfix()
            {
                string viewModelName = "ExampleVM";
                string convention = string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.That(result, Is.EqualTo("/Views/ExampleVMView.xaml"));
            }

            [TestCase]
            public void ReturnsRightResultForConventionWithMultipleConstants()
            {
                string viewModelName = "ExampleViewModel";
                string convention = string.Format("{0}.Views.{1}s.{1}View", NamingConvention.Assembly, NamingConvention.ViewModelName);

                string result = NamingConvention.ResolveViewByViewModelName("MyProject.MyAssembly", viewModelName, convention);

                Assert.That(result, Is.EqualTo("MyProject.MyAssembly.Views.Examples.ExampleView"));
            }
        }

        [TestFixture]
        public class TheResolveNamingConventionMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullConstantsWithValues()
            {
                Assert.Throws<ArgumentNullException>(() => NamingConvention.ResolveNamingConvention(null, "MyConvention"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullNamingConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyNamingConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), string.Empty));
            }

            [TestCase]
            public void ReturnsRightValueForMultipleConstantsWhereOnlyOneIsUsed()
            {
                var constantValues = new Dictionary<string, string>();
                constantValues.Add("[VM]", "VmValue");
                constantValues.Add("[TEST]", "TestValue");
                string convention = "/Views/[VM]View.xaml";

                string result = NamingConvention.ResolveNamingConvention(constantValues, convention);

                Assert.That(result, Is.EqualTo("/Views/VmValueView.xaml"));
            }

            [TestCase]
            public void ReturnsRightValueForMultipleConstantsWhereAllAreUsed()
            {
                var constantValues = new Dictionary<string, string>();
                constantValues.Add("[VM]", "VmValue");
                constantValues.Add("[TEST]", "TestValue");
                string convention = "/Views/[TEST]/[VM]View.xaml";

                string result = NamingConvention.ResolveNamingConvention(constantValues, convention);

                Assert.That(result, Is.EqualTo("/Views/TestValue/VmValueView.xaml"));
            }
        }

        [TestFixture]
        public class TheResolveNamingConventionWithValueMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullConstantsWithValues()
            {
                Assert.Throws<ArgumentNullException>(() => NamingConvention.ResolveNamingConvention(null, "MyConvention", "MyValue"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullNamingConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), null, "MyValue"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyNamingConvention()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), string.Empty, "MyValue"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullValue()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), "MyConvention", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyValue()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.ResolveNamingConvention(new Dictionary<string, string>(), "MyConvention", string.Empty));
            }

            [TestCase]
            public void ReturnsRightValueWhenUsingNoUps()
            {
                var constantValues = new Dictionary<string, string> { { "[VM]", "My" } };
                const string convention = "Catel.Views.[VM]View";
                const string value = "Catel.Examples.ViewModels.MyViewModel";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.That(result, Is.EqualTo("Catel.Views.MyView"));
            }

            [TestCase]
            public void ReturnsRightValueWhenUsingSingleUp()
            {
                var constantValues = new Dictionary<string, string> { { "[VM]", "My" } };
                const string convention = "[UP].Views.[VM]View";
                const string value = "Catel.Examples.ViewModels.MyViewModel";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.That(result, Is.EqualTo("Catel.Examples.Views.MyView"));
            }

            [TestCase]
            public void ReturnsRightValueWhenUsingMultipleUps()
            {
                var constantValues = new Dictionary<string, string> { { "[VW]", "MyWizard" } };
                const string convention = "[UP].[UP].ViewModels.Wizards.[VW]ViewModel";
                const string value = "Catel.Examples.Views.Wizards.MyWizardView";

                var result = NamingConvention.ResolveNamingConvention(constantValues, convention, value);

                Assert.That(result, Is.EqualTo("Catel.Examples.ViewModels.Wizards.MyWizardViewModel"));
            }
        }

        [TestFixture]
        public class TheGetParentPathMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullPath()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.GetParentPath(null));
                Assert.Throws<ArgumentException>(() => NamingConvention.GetParentPath(null, "\\"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyPath()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.GetParentPath(string.Empty));
                Assert.Throws<ArgumentException>(() => NamingConvention.GetParentPath(string.Empty, "\\"));
            }

            [TestCase]
            public void ReturnsInputWhenNoPathSeparatorIsAutomaticallyDetected()
            {
                Assert.That(NamingConvention.GetParentPath("pathwithoutseparator"), Is.EqualTo("pathwithoutseparator"));
            }

            [TestCase]
            public void ReturnsInputWhenPathSeparatorIsAutomaticallyDetected()
            {
                Assert.That(NamingConvention.GetParentPath("Catel.Examples.Views"), Is.EqualTo("Catel.Examples"));
            }

            [TestCase]
            public void ReturnsRightParentPathForDotSeparator()
            {
                Assert.That(NamingConvention.GetParentPath("Catel.Examples.Views", "."), Is.EqualTo("Catel.Examples"));
            }

            [TestCase]
            public void ReturnsRightParentPathForSlashSeparator()
            {
                Assert.That(NamingConvention.GetParentPath("Catel\\Examples\\Views.xaml", "\\"), Is.EqualTo("Catel\\Examples"));
            }
        }

        [TestFixture]
        public class TheGetParentSeparatorMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullPath()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.GetParentSeparator(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyPath()
            {
                Assert.Throws<ArgumentException>(() => NamingConvention.GetParentSeparator(string.Empty));
            }

            [TestCase]
            public void ReturnsDefaultWhenNoKnownSeparatorIsUsed()
            {
                Assert.That(NamingConvention.GetParentSeparator("noknownseparators"), Is.EqualTo("."));
            }

            [TestCase]
            public void ReturnsBackSlashWhenUsed()
            {
                Assert.That(NamingConvention.GetParentSeparator("Catel\\Views\\MyView.xaml"), Is.EqualTo("\\"));
            }

            [TestCase]
            public void ReturnsSlashWhenUsed()
            {
                Assert.That(NamingConvention.GetParentSeparator("Catel/Views/MyView.xaml"), Is.EqualTo("/"));
            }

            [TestCase]
            public void ReturnsPipeWhenUsed()
            {
                Assert.That(NamingConvention.GetParentSeparator("Catel|Views|MyView.xaml"), Is.EqualTo("|"));
            }

            [TestCase]
            public void ReturnsDotWhenUsed()
            {
                Assert.That(NamingConvention.GetParentSeparator("Catel.Views.MyView"), Is.EqualTo("."));
            }
        }
    }
}
