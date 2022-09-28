namespace Catel.Tests
{
    using System;

    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using Catel.Data;
    using Catel.MVVM;
    using Catel.Tests.ViewModels;

    using NUnit.Framework;
    using Moq;

    public partial class ArgumentFacts
    {
        public partial class TheIsNotNullMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotNull<object>(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParameterInvokation()
            {
                Assert.Throws<ArgumentNullException>(() => IsNotNullInvokation(null));
            }

            [TestCase]
            public void SucceedsForNotNullParameterInvokation()
            {
                IsNotNullInvokation(new object());
            }

            private void IsNotNullInvokation(object param01)
            {
                Argument.IsNotNull(() => param01);
            }
        }

        public partial class TheIsNotNullOrEmptyMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotNullOrEmpty((Expression<Func<string>>)null));
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotNullOrEmpty((Expression<Func<Guid?>>)null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyParameterInvokation()
            {
                Assert.Throws<ArgumentException>(() => IsNotNullInvokation(string.Empty));
                Assert.Throws<ArgumentException>(() => IsNotNullInvokation((string)null));
                Assert.Throws<ArgumentException>(() => IsNotNullInvokation(null as Guid?));
                Guid? param01 = Guid.Empty;
                Assert.Throws<ArgumentException>(() => IsNotNullInvokation(param01));
            }

            [TestCase]
            public void SucceedsForNotNullOrEmptyParameterInvokation()
            {
                IsNotNullInvokation("string");
                IsNotNullInvokation(Guid.NewGuid());
                IsNotNullInvokation((Guid?)Guid.NewGuid());
            }


            private void IsNotNullInvokation(string param01)
            {
                Argument.IsNotNullOrEmpty(() => param01);
            }

            private void IsNotNullInvokation(Guid? param01)
            {
                Argument.IsNotNullOrEmpty(() => param01);
            }
        }

        public partial class TheIsNotNullOrWhitespaceMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotNullOrWhitespace(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrWhitespaceInvokation()
            {
                Assert.Throws<ArgumentException>(() => IsNotNullOrWhitespaceInvokation(string.Empty));
            }

            [TestCase]
            public void SucceedsForNotNullOrWhitespaceInvokation()
            {
                IsNotNullOrWhitespaceInvokation("string");
            }

            private void IsNotNullOrWhitespaceInvokation(string param01)
            {
                Argument.IsNotNullOrWhitespace(() => param01);
            }
        }

        public partial class TheIsNotNullOrEmptyArrayMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotNullOrEmptyArray(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyArrayParameterInvokation()
            {
                Assert.Throws<ArgumentException>(() => IsNotNullOrEmptyArrayInvokation(new object[] { }));
            }

            [TestCase]
            public void SucceedsForNotNullOrEmptyArrayInvokation()
            {
                IsNotNullOrEmptyArrayInvokation(new[] { 1, 2, 3 });
            }

            private void IsNotNullOrEmptyArrayInvokation(Array param01)
            {
                Argument.IsNotNullOrEmptyArray(() => param01);
            }
        }

        public partial class TheIsNotOutOfRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotOutOfRange<double>(null, 0, 0, null));
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotOutOfRange<double>(null, 0, 0));
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotOutOfRange<int>(null, 0, 0));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForOutOfRangeInvokation()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => IsNotOutOfRangeInvokation(3, 1, 2));
                Assert.Throws<ArgumentOutOfRangeException>(() => IsNotOutOfRangeInvokation(3.0d, 1.0d, 2.0d));
                Assert.Throws<ArgumentOutOfRangeException>(() => IsNotOutOfRangeInvokation(3.0d, 1.0d, 2.0d, (d, d1, arg3) => false));
            }

            [TestCase]
            public void SucceedsForNotOutOfRangeInvokation()
            {
                IsNotOutOfRangeInvokation(1, 0, 2);
                IsNotOutOfRangeInvokation(1.0d, 0.0d, 2.0d);
                IsNotOutOfRangeInvokation(1.0d, 0.0d, 2.0d, (d, d1, d2) => true);
            }


            private void IsNotOutOfRangeInvokation(int param01, int min, int max)
            {
                Argument.IsNotOutOfRange(() => param01, min, max);
            }

            private void IsNotOutOfRangeInvokation<T>(T param01, T min, T max)
                where T : IComparable
            {
                Argument.IsNotOutOfRange(() => param01, min, max);
            }

            private void IsNotOutOfRangeInvokation<T>(T param01, T min, T max, Func<T, T, T, bool> validation)
            {
                Argument.IsNotOutOfRange(() => param01, min, max, validation);
            }
        }

        public partial class TheIsMinimalMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsMinimal<double>(null, 0, null));
                Assert.Throws<ArgumentNullException>(() => Argument.IsMinimal<double>(null, 0));
                Assert.Throws<ArgumentNullException>(() => Argument.IsMinimal<int>(null, 0));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForIsMinimalInvokation()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => IsMinimalInvokation(0, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => IsMinimalInvokation(0.0d, 1.0d));
                Assert.Throws<ArgumentOutOfRangeException>(() => IsMinimalInvokation(0.0d, 1.0d, (d, d1) => false));
            }

            [TestCase]
            public void SucceedsForIsMinimalInvokation()
            {
                IsMinimalInvokation(0, 0);
                IsMinimalInvokation(0.0d, 0.0d);
                IsMinimalInvokation(0.0d, 0.0d, (d, d1) => true);
            }


            private void IsMinimalInvokation(int param01, int min)
            {
                Argument.IsMinimal(() => param01, min);
            }

            private void IsMinimalInvokation<T>(T param01, T min)
                where T : IComparable
            {
                Argument.IsMinimal(() => param01, min);
            }

            private void IsMinimalInvokation<T>(T param01, T min, Func<T, T, bool> validation)
            {
                Argument.IsMinimal(() => param01, min, validation);
            }
        }

        public partial class TheIsMaximumMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsMaximum<double>(null, 0, null));
                Assert.Throws<ArgumentNullException>(() => Argument.IsMaximum<double>(null, 0));
                Assert.Throws<ArgumentNullException>(() => Argument.IsMaximum<int>(null, 0));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForIsMaximumInvokation()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => IsMaximumInvokation(1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => IsMaximumInvokation(1.0d, 0.0d));
                Assert.Throws<ArgumentOutOfRangeException>(() => IsMaximumInvokation(1.0d, 0.0d, (d, d1) => false));
            }

            [TestCase]
            public void SucceedsForIsMinimalInvokation()
            {
                IsMaximumInvokation(0, 0);
                IsMaximumInvokation(0.0d, 0.0d);
                IsMaximumInvokation(0.0d, 0.0d, (d, d1) => true);
            }

            private void IsMaximumInvokation(int param01, int min)
            {
                Argument.IsMaximum(() => param01, min);
            }

            private void IsMaximumInvokation<T>(T param01, T min)
                where T : IComparable
            {
                Argument.IsMaximum(() => param01, min);
            }

            private void IsMaximumInvokation<T>(T param01, T min, Func<T, T, bool> validation)
            {
                Argument.IsMaximum(() => param01, min, validation);
            }
        }

        public partial class TheImplementsInterfaceMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.ImplementsInterface<object>(null, typeof(IList)));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForImplementsInterfaceInvokation()
            {
                Assert.Throws<ArgumentException>(() => ImplementsInterfaceInvokation(new List<int>(), typeof(INotifyPropertyChanged)));
            }

            [TestCase]
            public void SucceedsForImplementsInterfaceInvokation()
            {
                ImplementsInterfaceInvokation(new List<int>(), typeof(IList));
                ImplementsInterfaceInvokation(typeof(List<int>), typeof(IList));
            }

            public void ImplementsInterfaceInvokation<T>(T param, Type interfaceType) 
                where T : class
            {
                Argument.ImplementsInterface(() => param, interfaceType);
            }
        }

        public partial class TheIsOfTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsOfType<object>(null, typeof(IList)));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsOfTypeInvokation()
            {
                Assert.Throws<ArgumentException>(() => IsOfTypeInvokation(new List<int>(), typeof(INotifyPropertyChanged)));
            }

            [TestCase]
            public void SucceedsForIsOfTypeInvokation()
            {
                IsOfTypeInvokation(new PersonViewModel(), typeof(ViewModelBase));
                IsOfTypeInvokation(typeof(PersonViewModel), typeof(ViewModelBase));
            }

            public void IsOfTypeInvokation<T>(T param, Type interfaceType) 
                where T : class
            {
                Argument.IsOfType(() => param, interfaceType);
            }
        }

        public partial class TheIsMatchMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsMatch(null, null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsMatchInvokation()
            {
                Assert.Throws<ArgumentException>(() => IsMatchInvokation("Match a single digit, between one and unlimited times", "\\d+"));
            }

            [TestCase]
            public void SucceedsForIsMatchInvokation()
            {
                IsMatchInvokation("Match any single character that is not a line break character, between one and unlimited times", ".+");
            }

            public void IsMatchInvokation(string param, string pattern)
            {
                Argument.IsMatch(() => param, pattern);
            }
        }

        public partial class TheIsNotMatchMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsNotMatch(null, null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsNotMatchInvokation()
            {
                Assert.Throws<ArgumentException>(() => IsNotMatchInvokation("Match any single character that is not a line break character, between one and unlimited times", ".+"));
            }

            [TestCase]
            public void SucceedsForIsNotMatchInvokation()
            {
                IsNotMatchInvokation("Match a single digit, between one and unlimited times", "\\d+");
            }

            public void IsNotMatchInvokation(string param, string pattern)
            {
                Argument.IsNotMatch(() => param, pattern);
            }
        }

        public partial class TheIsValidMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullExpression1()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsValid<object>(null, (Func<bool>)null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullExpression2()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsValid(null, (Func<object, bool>)null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionifValidatorIsNull2()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.IsValid(() => string.Empty, (IValueValidator<string>)null));
            }

            [TestCase]
            public void SucceedsForIsValidInvokation1()
            {
                this.IsValidInvokation("myValue", () => true);
                this.IsValidInvokation((string)null, () => true);
            }

            [TestCase]
            public void SucceedsForIsValidInvokation2()
            {
                this.IsValidInvokation("myValue", s => s.Length < 10);
                this.IsValidInvokation((string)null, s => s is null);
            }

            [TestCase]
            public void SucceedsForIsValidInvokation3()
            {
                this.IsValidInvokation("myValue", true);
                this.IsValidInvokation((string)null, true);
            }

            [TestCase]
            public void SucceedsForIsValidInvokation4()
            {
                var mock = new Mock<IValueValidator<string>>();
                mock.Setup(validator => validator.IsValid(It.IsAny<string>())).Returns(true);
                this.IsValidInvokation("myValue", mock.Object);
                this.IsValidInvokation((string)null, mock.Object);
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsValidInvokation1()
            {
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation("myValue", () => false));
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation((string)null, () => false));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsValidInvokation2()
            {
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation("myValue", s => s.Length > 10));
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation((string)null, s => s is not null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsValidInvokation3()
            {
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation("myValue", false));
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation((string)null, false));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForIsValidInvokation4()
            {
                var mock = new Mock<IValueValidator<string>>();
                mock.Setup(validator => validator.IsValid(It.IsAny<string>())).Returns(false);
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation("myValue", mock.Object));
                Assert.Throws<ArgumentException>(() => this.IsValidInvokation((string)null, mock.Object));
            }

            public void IsValidInvokation<T>(T value, Func<T, bool> validation)
            {
                Argument.IsValid(() => value, validation);
            }

            public void IsValidInvokation<T>(T value, IValueValidator<T> validator)
            {
                Argument.IsValid(() => value, validator);
            }

            public void IsValidInvokation<T>(T value, Func<bool> validation)
            {
                Argument.IsValid(() => value, validation);
            }

            public void IsValidInvokation<T>(T value, bool validation)
            {
                Argument.IsValid(() => value, validation);
            }
        }

        /*
        [TestFixture]
        public partial class TheImplementsOneOfTheInterfacesMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullExpression()
            {
                Assert.Throws<ArgumentNullException>(() => Argument.ImplementsOneOfTheInterfaces<object>(null, new[] { typeof(IList) }));
            }

            [TestCase]
             public void ThrowsArgumentExceptionForImplementsOneOfTheInterfacesInvokation()
             {
                 Assert.Throws<ArgumentException>(() => ImplementsOneOfTheInterfacesInvokation(new List<int>(), new[] { typeof(INotifyPropertyChanged) }));
             }

            [TestCase]
             public void SucceedsForImplementsOneOfTheInterfacesInvokation()
             {
                 ImplementsOneOfTheInterfacesInvokation(new List<int>(), new[] { typeof(IList) });
                 ImplementsOneOfTheInterfacesInvokation(typeof(List<int>), new[] { typeof(IList) });
             }

            public void ImplementsOneOfTheInterfacesInvokation<T>(T param, Type[] interfaceTypes) 
                where T : class
            {
                Argument.ImplementsOneOfTheInterfaces(() => param, interfaceTypes);
            }
        }*/
    }
}
