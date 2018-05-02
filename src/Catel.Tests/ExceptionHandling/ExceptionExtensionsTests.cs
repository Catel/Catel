// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensionsTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.ExceptionHandling
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    /// <summary>
    /// The exception extensions test.
    /// </summary>
    [TestFixture]
    public class ExceptionExtensionsFacts
    {
        #region Nested type: TheFindMethod
        /// <summary>
        /// The find method.
        /// </summary>
        [TestFixture]
        public class TheFindMethod
        {
            #region Methods
            /// <summary>
            /// Shoulds the find the specified exception.
            /// </summary>
            [TestCase]
            public void ShouldFindTheSpecifiedException()
            {
                var formatException = new FormatException();
                var argumentNullException = new ArgumentNullException("", formatException);
                var exception = new Exception("", argumentNullException);

                var foundException = exception.Find<ArgumentNullException>();

                Assert.IsNotNull(foundException);
                Assert.IsInstanceOf(typeof(ArgumentNullException), foundException);
            }

            /// <summary>
            /// Shoulds the not find the specified exception.
            /// </summary>
            [TestCase]
            public void ShouldNotFindTheSpecifiedException()
            {
                var exception = new Exception();

                var foundException = exception.Find<ArgumentNullException>();

                Assert.IsNull(foundException);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheFlattenMethod
        /// <summary>
        /// The flatten method.
        /// </summary>
        [TestFixture]
        public class TheFlattenMethod
        {
            #region Methods
            /// <summary>
            /// Shoulds flatten the exception.
            /// </summary>
            [TestCase]
            public void ShouldFlattenTheException()
            {
                var formatException = new FormatException("FormatException Message");
                var argumentNullException = new ArgumentNullException("ArgumentNullException Message", formatException);
                var exception = new Exception("Exception Message", argumentNullException);
                var exceptionFlatten = exception.Flatten();

                const string messageExpected = "Exception Message\r\nArgumentNullException Message\r\nFormatException Message\r\n";

                Assert.AreEqual(messageExpected, exceptionFlatten);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetAllInnerExceptionsMethod
        /// <summary>
        /// The getAllInnerExceptions method.
        /// </summary>
        [TestFixture]
        public class TheGetAllInnerExceptionsMethod
        {
            #region Methods
            /// <summary>
            /// Shoulds the get all inner exceptions.
            /// </summary>
            [TestCase]
            public void ShouldGetAllInnerExceptions()
            {
                var formatException = new FormatException();
                var argumentNullException = new ArgumentNullException("", formatException);
                var exception = new Exception("", argumentNullException);

                var innerExceptions = exception.GetAllInnerExceptions().ToArray();

                Assert.IsNotNull(innerExceptions);
                Assert.IsTrue(innerExceptions.Any());
                Assert.IsTrue(innerExceptions.Contains(formatException));
                Assert.IsTrue(innerExceptions.Contains(argumentNullException));
            }

            /// <summary>
            /// Shoulds not get any inner exceptions.
            /// </summary>
            [TestCase]
            public void ShouldNotGetAllInnerExceptions()
            {
                var exception = new Exception();

                var innerExceptions = exception.GetAllInnerExceptions();

                Assert.IsNotNull(innerExceptions);
                Assert.IsFalse(innerExceptions.Any());
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetLowestInnerExceptionMethod
        /// <summary>
        /// The getLowestInnerException method.
        /// </summary>
        [TestFixture]
        public class TheGetLowestInnerExceptionMethod
        {
            #region Methods
            /// <summary>
            /// Shoulds the get the lowest exception.
            /// </summary>
            [TestCase]
            public void ShouldGetTheLowestException()
            {
                var formatException = new FormatException();
                var argumentNullException = new ArgumentNullException("", formatException);
                var exception = new Exception("", argumentNullException);

                var lowsetException = exception.GetLowestInnerException();

                Assert.IsNotNull(lowsetException);
                Assert.IsInstanceOf(typeof(FormatException), lowsetException);
            }

            /// <summary>
            /// Shoulds not get the lowest exception.
            /// </summary>
            [TestCase]
            public void ShouldNotGetTheLowestException()
            {
                var exception = new Exception();

                var lowsetException = exception.GetLowestInnerException();

                Assert.IsNotNull(lowsetException);

                Assert.IsInstanceOf(typeof(Exception), lowsetException);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheToXmlMethod
        [TestFixture]
        public class TheToXmlMethod
        {
            #region Methods
            [TestCase]
            public void ShouldConvertLikeExpected()
            {
                var formatException = new FormatException("FormatException Message");

                var xmlExceptionConverted = formatException.ToXml();

                Assert.IsNotNull(xmlExceptionConverted);

                const string xlmStringExpected = "<System.FormatException>\r\n  <Message>FormatException Message</Message>\r\n</System.FormatException>";

                Assert.AreEqual(xlmStringExpected, xmlExceptionConverted.ToString());
            }
            #endregion
        }
        #endregion
    }
}