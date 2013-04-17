// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorToCatelValidatorAdapterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.FluentValidation
{
    using System;
    using System.Collections.Generic;

    using Catel.Data;
    using Catel.Test.Extensions.FluentValidation.Validators;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// The fluent validator to catel validator adapter test.
    /// </summary>
    public class FluentValidatorToCatelValidatorAdapterFacts
    {
        /// <summary>
        /// The the from generic method.
        /// </summary>
        [TestClass]
        public class TheFromGenericMethod
        {
            #region Public Methods and Operators

            /// <summary>
            /// The creates the adapter validator from a collection with a single validator type.
            /// </summary>
            [TestMethod]
            public void CreatesTheAdapterValidatorFromACollectionWithASingleValidatorType()
            {
                IValidator validator = FluentValidatorToCatelValidatorAdapter.From<PersonViewModelValidatorWarnings>();
                Assert.IsInstanceOfType(validator, typeof(FluentValidatorToCatelValidatorAdapter));
            }

            #endregion
        }

        /// <summary>
        /// The the from method.
        /// </summary>
        [TestClass]
        public class TheFromMethod
        {
            #region Public Methods and Operators

            /// <summary>
            /// The from helper method creates validator from a collection with a single validator type element.
            /// </summary>
            [TestMethod]
            public void CreatesACompositeValidatorFromACollectionOfValidatorType()
            {
                IValidator validator =
                    FluentValidatorToCatelValidatorAdapter.From(
                        new List<Type> { typeof(PersonViewModelValidatorWarnings), typeof(PersonViewModelValidator) });
                Assert.IsInstanceOfType(validator, typeof(CompositeValidator));
            }

            /// <summary>
            /// The from helper method creates validator from a collection with a single validator type element.
            /// </summary>
            [TestMethod]
            public void CreatesTheAdapterValidatorFromACollectionWithASingleValidatorType()
            {
                IValidator validator =
                    FluentValidatorToCatelValidatorAdapter.From(
                        new List<Type> { typeof(PersonViewModelValidatorWarnings) });
                Assert.IsInstanceOfType(validator, typeof(FluentValidatorToCatelValidatorAdapter));
            }

            /// <summary>
            /// The from helper method must throw argument exception if the list is empty.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentExceptionIfTheListIsEmpty()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => FluentValidatorToCatelValidatorAdapter.From(new List<Type>()));
            }

            #endregion
        }
    }
}