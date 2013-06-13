// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeValidatorTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.Linq.Expressions;

    using Castle.DynamicProxy;

    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    using Moq;

#if NET
    using System.Threading;
#endif

    public class CompositeValidatorFacts
    {
#if NET
        [TestClass]
        public class TheValidationSequenceIsThreadSafe
        {
            [TestMethod]
            public void CompositeValidatorModificationMustWaitUntilTheEnd()
            {
                var validatorMock = new Mock<IValidator>();
                var validatorMock1 = new Mock<IValidator>();
                var validatorMock2 = new Mock<IValidator>();

                validatorMock.Setup(validator => validator.BeforeValidation(null, null, null));
                validatorMock.Setup(validator => validator.AfterValidation(null, null, null));

                validatorMock1.Setup(validator => validator.BeforeValidation(null, null, null));
                validatorMock1.Setup(validator => validator.AfterValidation(null, null, null));

                validatorMock2.Setup(validator => validator.BeforeValidation(null, null, null));
                validatorMock2.Setup(validator => validator.AfterValidation(null, null, null));

                var compositeValidator = new CompositeValidator();

                var startEvent = new AutoResetEvent(false);
                var alterEvent = new AutoResetEvent(false);
                var syncEvents = new[] { new AutoResetEvent(false), new AutoResetEvent(false) };

                // Validation loop thread
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        startEvent.WaitOne();

                        compositeValidator.BeforeValidation(null, null, null);

                        alterEvent.Set();
                        ThreadHelper.Sleep(3000);


                        compositeValidator.AfterValidation(null, null, null);
                        syncEvents[0].Set();
                    });

                // Alter validator composition thread.
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {

                        compositeValidator.Add(validatorMock.Object);

                        compositeValidator.Add(validatorMock1.Object);

                        startEvent.Set();
                        alterEvent.WaitOne();

                        // Try add a validator during a validation loop execution to the composition.

                        compositeValidator.Add(validatorMock2.Object);

                        syncEvents[1].Set();
                    });
                
                syncEvents[0].WaitOne(TimeSpan.FromSeconds(10));
                syncEvents[1].WaitOne(TimeSpan.FromSeconds(10));

                validatorMock.Verify(validator => validator.BeforeValidation(null, null, null), Times.Exactly(1));
                validatorMock.Verify(validator => validator.AfterValidation(null, null, null), Times.Exactly(1));

                validatorMock1.Verify(validator => validator.BeforeValidation(null, null, null), Times.Exactly(1));
                validatorMock1.Verify(validator => validator.AfterValidation(null, null, null), Times.Exactly(1));

                validatorMock2.Verify(validator => validator.BeforeValidation(null, null, null), Times.Never());
                validatorMock2.Verify(validator => validator.AfterValidation(null, null, null), Times.Never());
            }
        }
#endif

        [TestClass]
        public class TheAddMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidator()
            {
                var compositeValidator = new CompositeValidator();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeValidator.Add(null));
            }

            [TestMethod]
            public void AddsNonExistingValidator()
            {
                var compositeValidator = new CompositeValidator();

                var validatorMock = new Mock<IValidator>();
                var validator = validatorMock.Object;

                compositeValidator.Add(validator);

                Assert.IsTrue(compositeValidator.Contains(validator));
            }

            [TestMethod]
            public void PreventsAddingDuplicateValidator()
            {
                var compositeValidator = new CompositeValidator();

                var validatorMock = new Mock<IValidator>();
                var validator = validatorMock.Object;

                compositeValidator.Add(validator);

                Assert.IsTrue(compositeValidator.Contains(validator));

                compositeValidator.Add(validator);

                Assert.IsTrue(compositeValidator.Contains(validator));

                compositeValidator.Remove(validator);

                Assert.IsFalse(compositeValidator.Contains(validator));
            }
        }

        [TestClass]
        public class TheRemoveMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidator()
            {
                var compositeValidator = new CompositeValidator();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeValidator.Remove(null));
            }

            [TestMethod]
            public void RemovesExistingValidator()
            {
                var compositeValidator = new CompositeValidator();

                var validatorMock = new Mock<IValidator>();
                var validator = validatorMock.Object;

                compositeValidator.Add(validator);

                Assert.IsTrue(compositeValidator.Contains(validator));

                compositeValidator.Remove(validator);

                Assert.IsFalse(compositeValidator.Contains(validator));
            }

            [TestMethod]
            public void DoesNotRemoveNonExistingValidator()
            {
                var compositeValidator = new CompositeValidator();

                var validatorMock = new Mock<IValidator>();
                var validator = validatorMock.Object;

                Assert.IsFalse(compositeValidator.Contains(validator));

                compositeValidator.Remove(validator);

                Assert.IsFalse(compositeValidator.Contains(validator));
            }
        }

        [TestClass]
        public class TheContainsMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidator()
            {
                var compositeValidator = new CompositeValidator();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeValidator.Contains(null));
            }

            [TestMethod]
            public void ReturnsTrueForExistingValidator()
            {
                var compositeValidator = new CompositeValidator();

                var validatorMock = new Mock<IValidator>();
                var validator = validatorMock.Object;

                compositeValidator.Add(validator);

                Assert.IsTrue(compositeValidator.Contains(validator));
            }

            [TestMethod]
            public void ReturnsFalseForNonExistingValidator()
            {
                var compositeValidator = new CompositeValidator();

                var validatorMock = new Mock<IValidator>();
                var validator = validatorMock.Object;

                Assert.IsFalse(compositeValidator.Contains(validator));
            }
        }

        [TestClass]
        public class TheBeforeValidationMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.BeforeValidation(null, null, null), () => compositeValidator.BeforeValidation(null, null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.BeforeValidation(null, null, null),
                    () => compositeValidator.BeforeValidation(null, null, null));
            }
        }

        [TestClass]
        public class TheBeforeValidateFieldsMethod
        {

            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.BeforeValidateFields(null, null), () => compositeValidator.BeforeValidateFields(null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.BeforeValidateFields(null, null),
                    () => compositeValidator.BeforeValidateFields(null, null));
            }
        }

        [TestClass]
        public class TheBeforeValidateBusinessRulesMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.BeforeValidateBusinessRules(null, null), () => compositeValidator.BeforeValidateBusinessRules(null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.BeforeValidateBusinessRules(null, null),
                    () => compositeValidator.BeforeValidateBusinessRules(null, null));
            }
        }

        [TestClass]
        public class TheValidateFieldsMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.ValidateFields(null, null), () => compositeValidator.ValidateFields(null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.ValidateFields(null, null),
                    () => compositeValidator.ValidateFields(null, null));
            }
        }

        [TestClass]
        public class TheValidateBusinessRulesMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.ValidateBusinessRules(null, null), () => compositeValidator.ValidateBusinessRules(null, null));
            }


            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.ValidateBusinessRules(null, null),
                    () => compositeValidator.ValidateBusinessRules(null, null));
            }
        }

        [TestClass]
        public class TheAfterValidateFieldsMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.AfterValidateFields(null, null), () => compositeValidator.AfterValidateFields(null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.AfterValidateFields(null, null),
                    () => compositeValidator.AfterValidateFields(null, null));
            }
        }

        [TestClass]
        public class TheAfterValidateBusinessRulesMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.AfterValidateBusinessRules(null, null), () => compositeValidator.AfterValidateBusinessRules(null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.AfterValidateBusinessRules(null, null),
                    () => compositeValidator.AfterValidateBusinessRules(null, null));
            }
        }

        [TestClass]
        public class TheAfterValidationMethod
        {
            [TestMethod]
            public void ThrowsExceptionIfAnyValidatorDoes()
            {
                var compositeValidator = new CompositeValidator();
                TestCompositeRethrowException<ArgumentNullException>(compositeValidator, validator => validator.AfterValidation(null, null, null), () => compositeValidator.AfterValidation(null, null, null));
            }

            [TestMethod]
            public void CallsMethodOnRegisteredValidatorsCorrectly()
            {
                var compositeValidator = new CompositeValidator();

                TestCompositeValidator(compositeValidator, validator => validator.AfterValidation(null, null, null),
                    () => compositeValidator.AfterValidation(null, null, null));
            }
        }

        /// <summary>
        /// Creates a composite validator with 2 validators and checks whether the method are called.
        /// </summary>
        /// <param name="compositeValidator">The composite validator.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        private static void TestCompositeValidator(CompositeValidator compositeValidator, Expression<Action<IValidator>> expression, Action actionToExecute)
        {
            var validator1Mock = new Mock<IValidator>();
            validator1Mock.Setup(expression);
            var validator1 = validator1Mock.Object;

            var validator2Mock = new Mock<IValidator>();
            validator2Mock.Setup(expression);
            var validator2 = validator2Mock.Object;

            compositeValidator.Add(validator1);
            compositeValidator.Add(validator2);

            actionToExecute();

            validator1Mock.Verify(expression, Times.Once());
            validator2Mock.Verify(expression, Times.Once());
        }

        private static void TestCompositeRethrowException<TException>(CompositeValidator compositeValidator, Expression<Action<IValidator>> expression, Action actionToExecute) where TException : Exception, new()
        {
            var validator1Mock = new Mock<IValidator>();
            validator1Mock.Setup(expression);
            var validator1 = validator1Mock.Object;

            var validator2Mock = new Mock<IValidator>();
            validator2Mock.Setup(expression).Throws(new TException());
            var validator2 = validator2Mock.Object;

            compositeValidator.Add(validator1);
            compositeValidator.Add(validator2);

            ExceptionTester.CallMethodAndExpectException<TException>(actionToExecute);

            validator1Mock.Verify(expression, Times.Once());
            validator2Mock.Verify(expression, Times.Once());
        }
    }
}