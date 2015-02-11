﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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

    using NUnit.Framework;

#if !NETFX_CORE
    using Moq;
#endif

    public partial class ArgumentFacts
    {
        [TestFixture]
        public partial class TheIsNotNullMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsNotNull("param", null));
            }

            [TestCase]
            public void SucceedsForValueType()
            {
                Argument.IsNotNull("param", 1);
            }

            [TestCase]
            public void SucceedsForReferenceType()
            {
                Argument.IsNotNull("param", new object());
            }
        }

        [TestFixture]
        public partial class TheIsNotNullOrEmptyMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullStringParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", (string)null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullGuidParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", (Guid?)null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyGuidParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", Guid.Empty));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyStringParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmpty("param", string.Empty));
            }

            [TestCase]
            public void SucceedsForStringWithOnlySpaces()
            {
                Argument.IsNotNullOrEmpty("param", "  ");
            }

            [TestCase]
            public void SucceedsForStringWithCharacters()
            {
                Argument.IsNotNullOrEmpty("param", "test");
            }
        }

        [TestFixture]
        public partial class TheIsNotNullOrWhitespaceMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrWhitespace("param", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyStringParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrWhitespace("param", string.Empty));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForStringWithOnlySpacesParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrWhitespace("param", "  "));
            }

            [TestCase]
            public void SucceedsForStringWithCharacters()
            {
                Argument.IsNotNullOrWhitespace("param", "test");
            }
        }

        [TestFixture]
        public partial class TheIsNotNullOrEmptyArrayMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmptyArray("param", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyIntArrayParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmptyArray("param", new int[] { }));
            }

            [TestCase]
            public void SucceedsForFilledIntArray()
            {
                Argument.IsNotNullOrEmptyArray("param", new int[] { 1 });
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyByteArrayParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotNullOrEmptyArray("param", new byte[] { }));
            }

            [TestCase]
            public void SucceedsForFilledByteArray()
            {
                Argument.IsNotNullOrEmptyArray("param", new byte[] { 1 });
            }
        }

        [TestFixture]
        public partial class TheIsNotOutOfRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 1, 2, 3));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 3, 1, 2));
            }

            [TestCase]
            public void SucceedsForValidIntegers()
            {
                Argument.IsNotOutOfRange("param", 1, 1, 3);
                Argument.IsNotOutOfRange("param", 2, 1, 3);
                Argument.IsNotOutOfRange("param", 3, 1, 3);
            }

#if NET
            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 1d, 2d, 3d));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsNotOutOfRange("param", 3d, 1d, 2d));
            }

            [TestCase]
            public void SucceedsForValidDoubles()
            {
                Argument.IsNotOutOfRange("param", 1d, 1d, 3d);
                Argument.IsNotOutOfRange("param", 2d, 1d, 3d);
                Argument.IsNotOutOfRange("param", 3d, 1d, 3d);
            }
#endif
        }

        [TestFixture]
        public partial class TheIsMinimalMethod
        {
            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMinimal("param", 2, 3));
            }

            [TestCase]
            public void SucceedsForValidInteger()
            {
                Argument.IsMinimal("param", 3, 2);
                Argument.IsMinimal("param", 3, 3);
            }

#if NET
            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooSmallDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMinimal("param", 2d, 3d));
            }

            [TestCase]
            public void SucceedsForValidDouble()
            {
                Argument.IsMinimal("param", 3d, 2d);
                Argument.IsMinimal("param", 3d, 3d);
            }
#endif
        }

        [TestFixture]
        public partial class TheIsMaximumMethod
        {
            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeIntegerParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMaximum("param", 3, 2));
            }

            [TestCase]
            public void SucceedsForValidInteger()
            {
                Argument.IsMaximum("param", 2, 3);
                Argument.IsMaximum("param", 3, 3);
            }

#if NET
            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForTooLargeDoubleParamValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => Argument.IsMaximum("param", 3d, 2d));
            }

            [TestCase]
            public void SucceedsForValidDouble()
            {
                Argument.IsMaximum("param", 2d, 3d);
                Argument.IsMaximum("param", 3d, 3d);
            }
#endif
        }

        [TestFixture]
        public partial class TheImplementsInterfaceMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.ImplementsInterface(null, null as object, typeof(IList)));
            }

            [TestCase]
            public void SucceedsForInstanceImplementingInterface()
            {
                Argument.ImplementsInterface("myParam", new List<int>(), typeof(IList));
            }

            [TestCase]
            public void SucceedsForInstanceImplementingInterfaceGeneric()
            {
                Argument.ImplementsInterface<IList>("myParam", new List<int>());
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.ImplementsInterface("myParam", null, typeof(IList)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInterfaceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.ImplementsInterface("myParam", typeof(List<int>), null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForTypeNotImplementingInterface()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.ImplementsInterface("myParam", typeof(List<int>), typeof(INotifyPropertyChanged)));
            }

            [TestCase]
            public void SucceedsForTypeImplementingInterface()
            {
                Argument.ImplementsInterface("myParam", typeof(List<int>), typeof(IList));
            }
        }

        [TestFixture]
        public partial class TheInheritsFromMethod
        {
            [TestCase]
            public void SucceedsForTypeInheritsFrom()
            {
                Argument.InheritsFrom("myParam", typeof(CoverageExcludeAttribute), typeof(Attribute));
            }

            [TestCase]
            public void SucceedsForInstanceInheritsFrom()
            {
                Argument.InheritsFrom("myParam", new CoverageExcludeAttribute(ExcludeReason.TestCode), typeof(Attribute));
            }

            [TestCase]
            public void SucceedsForGenericInheritsFrom()
            {
                Argument.InheritsFrom<Attribute>("myParam", new CoverageExcludeAttribute(ExcludeReason.TestCode));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom(null, null as object, typeof(Exception)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom("myParam", null, typeof(Exception)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom("myParam", null as object, typeof(Exception)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullBaseType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.InheritsFrom("myParam", typeof(Exception), null));
            }
        }

        [TestFixture]
        public partial class TheIsOfTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsOfType(null, null as object, typeof(ViewModelBase)));
            }

            [TestCase]
            public void SucceedsForInstanceImplementingRequiredType()
            {
                Argument.IsOfType("myParam", new PersonViewModel(), typeof(ViewModelBase));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsOfType("myParam", null, typeof(ViewModelBase)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullNotRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsOfType("myParam", typeof(PersonViewModel), null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForTypeNotImplementingRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsOfType("myParam", typeof(PersonViewModel), typeof(ViewLocator)));
            }

            [TestCase]
            public void SucceedsForTypeImplementingRequiredType()
            {
                Argument.IsOfType("myParam", typeof(PersonViewModel), typeof(ViewModelBase));
            }
        }

        [TestFixture]
        public class TheIsNotOfTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotOfType(null, null as object, typeof(ViewModelLocator)));
            }

            [TestCase]
            public void SucceedsForInstanceNotImplementingNotRequiredType()
            {
                Argument.IsNotOfType("myParam", new ViewLocator(), typeof(ViewModelLocator));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsNotOfType("myParam", null, typeof(ViewModelLocator)));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullNotRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Argument.IsNotOfType("myParam", typeof(ViewLocator), null));
            }

            [TestCase]
            public void SucceedsForTypeNotImplementingNotRequiredType()
            {
                Argument.IsNotOfType("myParam", typeof(ViewLocator), typeof(ViewModelLocator));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForTypeImplementingNotRequiredType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotOfType("myParam", typeof(PersonViewModel), typeof(ViewModelBase)));
            }
        }

        [TestFixture]
        public class TheIsSupportedMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullMessage()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsSupported(true, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsSupported(true, string.Empty));
            }

            [TestCase]
            public void ThrowsNotSupportedExceptionForNotSupported()
            {
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => Argument.IsSupported(false, "Just not supported"));
            }

            [TestCase]
            public void SucceedsForSupported()
            {
                Argument.IsSupported(true, "Just not supported");
            }
        }

        [TestFixture]
        public partial class TheIsNotMatchMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotMatch("myParam", null, ".+"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullPattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotMatch("myParam", string.Empty, null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForValueThatMatchWithThePattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsNotMatch("myParam", "Match any single character that is not a line break character, between one and unlimited times", ".+"));
            }

            [TestCase]
            public void SucceedsForValueThatNotMatchWithThePattern()
            {
                Argument.IsNotMatch("myParam", "Match a single digit, between one and unlimited times", "\\d+");
            }
        }

        [TestFixture]
        public partial class TheIsMatchMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsMatch("myParam", null, ".+"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullPattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsMatch("myParam", string.Empty, null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForValueThatMatchWithThePattern()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsMatch("myParam", "\n", ".+"));
            }

            [TestCase]
            public void SucceedsForValueThatMatchWithThePattern()
            {
                Argument.IsMatch("myParam", "1234567890", "\\d+");
            }
        }

        [TestFixture]
        public partial class TheIsValidMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNotValid()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", false));
            }            
            
#if !NETFX_CORE
            [TestCase]
            public void ThrowsArgumentExceptionForNotValidValidator()
            {
                var validatorMock = new Mock<IValueValidator<string>>();
                validatorMock.Setup(validator => validator.IsValid(It.IsAny<string>())).Returns(false);
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", validatorMock.Object));
            }  
#endif
            
            [TestCase]
            public void ThrowsArgumentExceptionForNotValidFunc()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", () => false));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", s => s.Length > 10));
            }
            
            [TestCase]
            public void ThrowsArgumentNullExceptionIfFuncIsNull()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", (Func<string, bool>)null));
  
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionifValidatorIsNull()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Argument.IsValid("myParam", "value", (IValueValidator<string>)null));
            }
            

            [TestCase]
            public void SucceedsForValid()
            {
                Argument.IsValid("myParam", "value", true);
            }       
            
#if !NETFX_CORE
            [TestCase]
            public void SucceedsForValid_Validator()
            {
                var validatorMock = new Mock<IValueValidator<string>>();
                validatorMock.Setup(validator => validator.IsValid(It.IsAny<string>())).Returns(true);
                Argument.IsValid("myParam", "value", validatorMock.Object);
            }
#endif

            [TestCase]
            public void SucceedsForValid_Func()
            {
                Argument.IsValid("myParam", "value", () => true);
                Argument.IsValid("myParam", "value", s => s.Length < 10);
            }
        }
    }
}