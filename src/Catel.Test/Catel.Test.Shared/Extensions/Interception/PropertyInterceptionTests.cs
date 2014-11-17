// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyInterceptionTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Interception
{
    using Catel.IoC;
    using NUnit.Framework;

    [TestFixture]
    public class PropertyInterceptionTests
    {
        #region Fields
        private IServiceLocator _serviceLocator;
        #endregion

        #region Methods
        [SetUp]
        public void SetUp()
        {
            _serviceLocator = ServiceLocator.Default;
        }

        [TestCase]
        public void ShouldInterceptGetter()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptGetter(service => service.Name)
                           .OnBefore(invocation =>
                               {
                                   Assert.IsFalse(((ITestService) invocation.Target).WasExecuted);
                                   index++;
                               });

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            Assert.AreEqual("testValue", resolvedTestService.Name);
            Assert.IsTrue(resolvedTestService.WasExecuted);
            resolvedTestService.Name = string.Empty; // setter shouldn't be intercepted
            Assert.AreEqual(1, index);
        }

        [TestCase]
        public void ShouldInterceptSetter()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptSetter(service => service.Name)
                           .OnBefore(invocation =>
                           {
                               Assert.IsFalse(((ITestService)invocation.Target).WasExecuted);
                               index++;
                           });

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Name = string.Empty;
            Assert.AreEqual(string.Empty, resolvedTestService.Name);
            Assert.IsTrue(resolvedTestService.WasExecuted);
            Assert.AreEqual(1, index);
        }

        [TestCase]
        public void ShouldInterceptAllSetters()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptAllSetters()
                           .OnFinally(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Name = string.Empty;
            resolvedTestService.Description = string.Empty;

            Assert.AreEqual(string.Empty, resolvedTestService.Name);

            Assert.AreEqual(2, index);
        }

        [TestCase]
        public void ShouldInterceptAllGetters()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptAllGetters()
                           .OnFinally(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            var name = resolvedTestService.Name;
            Assert.AreEqual("testValue", name);

            var description = resolvedTestService.Description;

            Assert.IsNull(description);

            resolvedTestService.Name = string.Empty; // not intercepted

            Assert.AreEqual(2, index);
        }
        #endregion
    }
}

#endif