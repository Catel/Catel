// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPleaseWaitServiceExtensionsTests.cs" company="Catel development team">
//   Copyright (c) 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Services
{
    using System;
    using Catel.Services;
    using NUnit.Framework;

    [TestFixture, RequiresSTA]
    class IPleaseWaitServiceExtensionsTests
    {
        private IPleaseWaitService _target;
        private IPleaseWaitService Target
        {
            get { return _target ?? (_target = new PleaseWaitService(FakeLanguageService)); }
            set { _target = value; }
        }

        private ILanguageService FakeLanguageService { get; set; }

        /// <summary>
        /// Use Test_Initialize to run code before running each test.
        /// </summary>
        [SetUp]
        public void Test_Initialize()
        {
            // TODO: use mocking framework to stub out the ILanguageService
            FakeLanguageService = new LanguageService();
        }

        /// <summary>
        /// Use Test_Cleanup to run code after each test has run.
        /// </summary>
        [TearDown]
        public void Test_Cleanup()
        {
            Target = null;
        }
        
        [Test]
        public void PushInScope_CodeThrowsException_Hides()
        {
            // ARRANGE
            Assert.AreEqual(0, Target.ShowCounter);

            // ACT
            try
            {
                using (Target.PushInScope())
                {
                    Assert.AreEqual(1, Target.ShowCounter);
                    throw new ArgumentException();
                }
            }
            catch (ArgumentException)
            {
            }

            // ASSERT
            Assert.AreEqual(0, Target.ShowCounter);
        }

        [Test]
        public void PushInScope_WithStatus_CodeThrowsException_Hides()
        {
            // ARRANGE
            Assert.AreEqual(0, Target.ShowCounter);
            
            // ACT
            try
            {
                using (Target.PushInScope("Loading..."))
                {
                    Assert.AreEqual(1, Target.ShowCounter);
                    throw new ArgumentException();
                }
            }
            catch (ArgumentException)
            {
            }

            // ASSERT
            Assert.AreEqual(0, Target.ShowCounter);
        }
    }
}
