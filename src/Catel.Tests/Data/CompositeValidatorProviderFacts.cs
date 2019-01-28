// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeValidatorProviderTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Tests.Data
{
    using System;

    using Catel.Data;
    using Catel.MVVM;

    using NUnit.Framework;

    public class CompositeValidatorProviderFacts
    {
        [TestFixture]
        public class TheAddMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidator()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(
                    () => compositeValidatorProvider.Add(null));
            }
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidator()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(
                    () => compositeValidatorProvider.Remove(null));
            }

            [TestCase]
            public void EliminatesAnAlreadyAddedValidator()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();
                var validatorProviderMock1 = new Moq.Mock<IValidatorProvider>();
                var validatorProviderMock2 = new Moq.Mock<IValidatorProvider>();
                
                compositeValidatorProvider.Add(validatorProviderMock1.Object);
                compositeValidatorProvider.Add(validatorProviderMock2.Object);
                
                compositeValidatorProvider.Remove(validatorProviderMock1.Object);
                Assert.IsFalse(compositeValidatorProvider.Contains(validatorProviderMock1.Object));
            }
        }

        [TestFixture]
        public class GetValidatorGenericMethod
        {
            public class FooViewModel : ViewModelBase
            {
                 
            }

            [TestCase]
            public void RetrieveTheRightValidatorComposition()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();
                var validatorMock1 = new Moq.Mock<IValidator>();
                var validatorMock2 = new Moq.Mock<IValidator>();

                var validatorProviderMock1 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock1.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(validatorMock1.Object);

                var validatorProviderMock2 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock2.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(validatorMock2.Object);

                compositeValidatorProvider.Add(validatorProviderMock1.Object);
                compositeValidatorProvider.Add(validatorProviderMock2.Object);

                IValidator validator = (compositeValidatorProvider as IValidatorProvider).GetValidator<FooViewModel>();

                Assert.IsInstanceOf(typeof(CompositeValidator), validator);
                ((CompositeValidator)validator).Contains(validatorMock1.Object);
                ((CompositeValidator)validator).Contains(validatorMock2.Object);
            }

            [TestCase]
            public void RetrieveTheRightSingleValidator()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();
                var validatorMock1 = new Moq.Mock<IValidator>();

                var validatorProviderMock1 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock1.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(validatorMock1.Object);

                var validatorProviderMock2 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock2.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(default(IValidator));

                compositeValidatorProvider.Add(validatorProviderMock1.Object);
                compositeValidatorProvider.Add(validatorProviderMock2.Object);

                IValidator validator = (compositeValidatorProvider as IValidatorProvider).GetValidator<FooViewModel>();

                Assert.AreEqual(validator, validatorMock1.Object);
            }
        }

        [TestFixture]
        public class GetValidatorMethod
        {
            public class FooViewModel : ViewModelBase
            {

            }

            [TestCase]
            public void RetrieveTheRightValidatorComposition()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();
                var validatorMock1 = new Moq.Mock<IValidator>();
                var validatorMock2 = new Moq.Mock<IValidator>();

                var validatorProviderMock1 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock1.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(validatorMock1.Object);
                
                var validatorProviderMock2 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock2.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(validatorMock2.Object);

                compositeValidatorProvider.Add(validatorProviderMock1.Object);
                compositeValidatorProvider.Add(validatorProviderMock2.Object);

                IValidator validator = (compositeValidatorProvider as IValidatorProvider).GetValidator(typeof(FooViewModel));

                Assert.IsInstanceOf(typeof(CompositeValidator), validator);
                ((CompositeValidator)validator).Contains(validatorMock1.Object);
                ((CompositeValidator)validator).Contains(validatorMock2.Object);
            }

            [TestCase]
            public void RetrieveTheRightSingleValidator()
            {
                var compositeValidatorProvider = new CompositeValidatorProvider();
                var validatorMock1 = new Moq.Mock<IValidator>();

                var validatorProviderMock1 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock1.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(validatorMock1.Object);

                var validatorProviderMock2 = new Moq.Mock<IValidatorProvider>();
                validatorProviderMock2.Setup(provider => provider.GetValidator(typeof(FooViewModel))).Returns(default(IValidator));

                compositeValidatorProvider.Add(validatorProviderMock1.Object);
                compositeValidatorProvider.Add(validatorProviderMock2.Object);

                IValidator validator = (compositeValidatorProvider as IValidatorProvider).GetValidator(typeof(FooViewModel));

                Assert.AreEqual(validator, validatorMock1.Object);
            }
        }
    }
}

#endif
