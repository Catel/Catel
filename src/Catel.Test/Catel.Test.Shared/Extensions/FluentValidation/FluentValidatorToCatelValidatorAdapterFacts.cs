// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorToCatelValidatorAdapterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.FluentValidation
{
    using System;
    using System.Collections.Generic;

    using Catel.Data;
    using Catel.Test.Extensions.FluentValidation.Validators;

    using NUnit.Framework;

    /// <summary>
    /// The fluent validator to catel validator adapter test.
    /// </summary>
    public class FluentValidatorToCatelValidatorAdapterFacts
    {
        /// <summary>
        /// The the from generic method.
        /// </summary>
        [TestFixture]
        public class TheFromGenericMethod
        {
            #region Public Methods and Operators

            /// <summary>
            /// The creates the adapter validator from a collection with a single validator type.
            /// </summary>
            [TestCase]
            public void CreatesTheAdapterValidatorFromACollectionWithASingleValidatorType()
            {
                IValidator validator = FluentValidatorToCatelValidatorAdapter.From<PersonViewModelValidatorWarnings>();
                Assert.IsInstanceOf(typeof(FluentValidatorToCatelValidatorAdapter), validator);
            }

            #endregion
        }

        /// <summary>
        /// The the from method.
        /// </summary>
        [TestFixture]
        public class TheFromMethod
        {
            #region Public Methods and Operators

            /// <summary>
            /// The from helper method creates validator from a collection with a single validator type element.
            /// </summary>
            [TestCase]
            public void CreatesACompositeValidatorFromACollectionOfValidatorType()
            {
                IValidator validator =
                    FluentValidatorToCatelValidatorAdapter.From(
                        new List<Type> { typeof(PersonViewModelValidatorWarnings), typeof(PersonViewModelValidator) });
                Assert.IsInstanceOf(typeof(CompositeValidator), validator);
            }

            /// <summary>
            /// The from helper method creates validator from a collection with a single validator type element.
            /// </summary>
            [TestCase]
            public void CreatesTheAdapterValidatorFromACollectionWithASingleValidatorType()
            {
                IValidator validator =
                    FluentValidatorToCatelValidatorAdapter.From(
                        new List<Type> { typeof(PersonViewModelValidatorWarnings) });
                Assert.IsInstanceOf(typeof(FluentValidatorToCatelValidatorAdapter), validator);
            }

            /// <summary>
            /// The from helper method must throw argument exception if the list is empty.
            /// </summary>
            [TestCase]
            public void ThrowsArgumentExceptionIfTheListIsEmpty()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => FluentValidatorToCatelValidatorAdapter.From(new List<Type>()));
            }

            #endregion
        }
    }
}

#endif