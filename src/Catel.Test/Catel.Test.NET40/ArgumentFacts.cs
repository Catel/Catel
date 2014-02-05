// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Catel.MVVM;
    using Catel.Data;

    using ViewModels;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
#endif

    public partial class ArgumentFacts
    {
        [TestClass]
        public partial class TheIsNotNullMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsNotNull("param", null));
            }

            [TestMethod]
            public void SucceedsForValueType()
            {
                Argument.IsNotNull("param", 1);
            }

            [TestMethod]
            public void SucceedsForReferenceType()
            {
                Argument.IsNotNull("param", new object());
            }
        }

        [TestClass]
        public partial class TheIsNotNullOrEmptyMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullStringParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", (string)null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullGuidParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", (Guid?)null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyGuidParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", Guid.Empty));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyStringParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", string.Empty));
            }

            [TestMethod]
            public void SucceedsForStringWithOnlySpaces()
            {
                Argument.IsNotNullOrEmpty("param", "  ");
            }

            [TestMethod]
            public void SucceedsForStringWithCharacters()
            {
                Argument.IsNotNullOrEmpty("param", "test");
            }
        }

        [TestClass]
        public partial class TheIsNotNullOrWhitespaceMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrWhitespace("param", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyStringParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrWhitespace("param", string.Empty));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForStringWithOnlySpacesParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrWhitespace("param", "  "));
            }

            [TestMethod]
            public void SucceedsForStringWithCharacters()
            {
                Argument.IsNotNullOrWhitespace("param", "test");
            }
        }

        [TestClass]
        public partial class TheIsNotNullOrEmptyArrayMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmptyArray("param", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyIntArrayParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmptyArray("param", new int[] { }));
            }

            [TestMethod]
            public void SucceedsForFilledIntArray()
            {
                Argument.IsNotNullOrEmptyArray("param", new int[] { 1 });
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyByteArrayParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmptyArray("param", new byte[] { }));
            }

            [TestMethod]
            public void SucceedsForFilledByteArray()
            {
                Argument.IsNotNullOrEmptyArray("param", new byte[] { 1 });
            }
        }

        [TestClass]
        public partial class TheIsNotOutOfRangeMethod
        {
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 1, 2, 3));
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 3, 1, 2));
            }

            [TestMethod]
            public void SucceedsForValidIntegers()
            {
                Argument.IsNotOutOfRange("param", 1, 1, 3);
                Argument.IsNotOutOfRange("param", 2, 1, 3);
                Argument.IsNotOutOfRange("param", 3, 1, 3);
            }

#if NET
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 1d, 2d, 3d));
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 3d, 1d, 2d));
            }

            [TestMethod]
            public void SucceedsForValidDoubles()
            {
                Argument.IsNotOutOfRange("param", 1d, 1d, 3d);
                Argument.IsNotOutOfRange("param", 2d, 1d, 3d);
                Argument.IsNotOutOfRange("param", 3d, 1d, 3d);
            }
#endif
        }

        [TestClass]
        public partial class TheIsMinimalMethod
        {
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMinimal("param", 2, 3));
            }

            [TestMethod]
            public void SucceedsForValidInteger()
            {
                Argument.IsMinimal("param", 3, 2);
                Argument.IsMinimal("param", 3, 3);
            }

#if NET
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMinimal("param", 2d, 3d));
            }

            [TestMethod]
            public void SucceedsForValidDouble()
            {
                Argument.IsMinimal("param", 3d, 2d);
                Argument.IsMinimal("param", 3d, 3d);
            }
#endif
        }

        [TestClass]
        public partial class TheIsMaximumMethod
        {
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMaximum("param", 3, 2));
            }

            [TestMethod]
            public void SucceedsForValidInteger()
            {
                Argument.IsMaximum("param", 2, 3);
                Argument.IsMaximum("param", 3, 3);
            }

#if NET
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMaximum("param", 3d, 2d));
            }

            [TestMethod]
            public void SucceedsForValidDouble()
            {
                Argument.IsMaximum("param", 2d, 3d);
                Argument.IsMaximum("param", 3d, 3d);
            }
#endif
        }

        [TestClass]
        public partial class TheImplementsInterfaceMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.ImplementsInterface(null, null as object, typeof(IList)));
            }

            [TestMethod]
            public void SucceedsForInstanceImplementingInterface()
            {
                Argument.ImplementsInterface("myParam", new List<int>(), typeof(IList));
            }

            [TestMethod]
            public void SucceedsForInstanceImplementingInterfaceGeneric()
            {
                Argument.ImplementsInterface<IList>("myParam", new List<int>());
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.ImplementsInterface("myParam", null, typeof(IList)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInterfaceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.ImplementsInterface("myParam", typeof(List<int>), null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForTypeNotImplementingInterface()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.ImplementsInterface("myParam", typeof(List<int>), typeof(INotifyPropertyChanged)));
            }

            [TestMethod]
            public void SucceedsForTypeImplementingInterface()
            {
                Argument.ImplementsInterface("myParam", typeof(List<int>), typeof(IList));
            }
        }

        [TestClass]
        public partial class TheInheritsFromMethod
        {
            [TestMethod]
            public void SucceedsForTypeInheritsFrom()
            {
                Argument.InheritsFrom("myParam", typeof(CoverageExcludeAttribute), typeof(Attribute));
            }

            [TestMethod]
            public void SucceedsForInstanceInheritsFrom()
            {
                Argument.InheritsFrom("myParam", new CoverageExcludeAttribute(ExcludeReason.TestCode), typeof(Attribute));
            }

            [TestMethod]
            public void SucceedsForGenericInheritsFrom()
            {
                Argument.InheritsFrom<Attribute>("myParam", new CoverageExcludeAttribute(ExcludeReason.TestCode));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom(null, null as object, typeof(Exception)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom("myParam", null, typeof(Exception)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom("myParam", null as object, typeof(Exception)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullBaseType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom("myParam", typeof(Exception), null));
            }
        }

        [TestClass]
        public partial class TheIsOfTypeMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsOfType(null, null as object, typeof(ViewModelBase)));
            }

            [TestMethod]
            public void SucceedsForInstanceImplementingRequiredType()
            {
                Argument.IsOfType("myParam", new PersonViewModel(), typeof(ViewModelBase));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsOfType("myParam", null, typeof(ViewModelBase)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullNotRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsOfType("myParam", typeof(PersonViewModel), null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForTypeNotImplementingRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsOfType("myParam", typeof(PersonViewModel), typeof(ViewLocator)));
            }

            [TestMethod]
            public void SucceedsForTypeImplementingRequiredType()
            {
                Argument.IsOfType("myParam", typeof(PersonViewModel), typeof(ViewModelBase));
            }
        }

        [TestClass]
        public class TheIsNotOfTypeMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotOfType(null, null as object, typeof(ViewModelLocator)));
            }

            [TestMethod]
            public void SucceedsForInstanceNotImplementingNotRequiredType()
            {
                Argument.IsNotOfType("myParam", new ViewLocator(), typeof(ViewModelLocator));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsNotOfType("myParam", null, typeof(ViewModelLocator)));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullNotRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsNotOfType("myParam", typeof(ViewLocator), null));
            }

            [TestMethod]
            public void SucceedsForTypeNotImplementingNotRequiredType()
            {
                Argument.IsNotOfType("myParam", typeof(ViewLocator), typeof(ViewModelLocator));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForTypeImplementingNotRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotOfType("myParam", typeof(PersonViewModel), typeof(ViewModelBase)));
            }
        }

        [TestClass]
        public class TheIsSupportedMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullMessage()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsSupported(true, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsSupported(true, string.Empty));
            }

            [TestMethod]
            public void ThrowsNotSupportedExceptionForNotSupported()
            {
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => Argument.IsSupported(false, "Just not supported"));
            }

            [TestMethod]
            public void SucceedsForSupported()
            {
                Argument.IsSupported(true, "Just not supported");
            }
        }

        [TestClass]
        public partial class TheIsNotMatchMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotMatch("myParam", null, ".+"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullPattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotMatch("myParam", string.Empty, null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForValueThatMatchWithThePattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotMatch("myParam", "Match any single character that is not a line break character, between one and unlimited times", ".+"));
            }

            [TestMethod]
            public void SucceedsForValueThatNotMatchWithThePattern()
            {
                Argument.IsNotMatch("myParam", "Match a single digit, between one and unlimited times", "\\d+");
            }
        }

        [TestClass]
        public partial class TheIsMatchMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsMatch("myParam", null, ".+"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullPattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsMatch("myParam", string.Empty, null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForValueThatMatchWithThePattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsMatch("myParam", "\n", ".+"));
            }

            [TestMethod]
            public void SucceedsForValueThatMatchWithThePattern()
            {
                Argument.IsMatch("myParam", "1234567890", "\\d+");
            }
        }

        [TestClass]
        public partial class TheIsValidMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNotValid()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", false));
            }            
            
#if !NETFX_CORE
            [TestMethod]
            public void ThrowsArgumentExceptionForNotValidValidator()
            {
                var validatorMock = new Mock<IValueValidator<string>>();
                validatorMock.Setup(validator => validator.IsValid(It.IsAny<string>())).Returns(false);
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", validatorMock.Object));
            }  
#endif
            
            [TestMethod]
            public void ThrowsArgumentExceptionForNotValidFunc()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", () => false));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", s => s.Length > 10));
            }
            
            [TestMethod]
            public void ThrowsArgumentNullExceptionIfFuncIsNull()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", (Func<string, bool>)null));
  
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionifValidatorIsNull()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", (IValueValidator<string>)null));
            }
            

            [TestMethod]
            public void SucceedsForValid()
            {
                Argument.IsValid("myParam", "value", true);
            }       
            
#if !NETFX_CORE
            [TestMethod]
            public void SucceedsForValid_Validator()
            {
                var validatorMock = new Mock<IValueValidator<string>>();
                validatorMock.Setup(validator => validator.IsValid(It.IsAny<string>())).Returns(true);
                Argument.IsValid("myParam", "value", validatorMock.Object);
            }
#endif

            [TestMethod]
            public void SucceedsForValid_Func()
            {
                Argument.IsValid("myParam", "value", () => true);
                Argument.IsValid("myParam", "value", s => s.Length < 10);
            }
        }
    }
}