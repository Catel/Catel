// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodInterceptionTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Interception
{
    using System;
    using System.Reflection;
    using Catel.Interception.Utilities;
    using Catel.IoC;
    using Catel.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodInterceptionTests
    {
        #region Fields
        private IServiceLocator _serviceLocator;
        #endregion

        #region Methods
        [TestInitialize]
        public void Initialization()
        {
            _serviceLocator = ServiceLocator.Default;
        }

        [TestMethod]
        public void ShouldCallbackOnBefore()
        {
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Perform())
                           .OnBefore(invocation => Assert.IsFalse(((ITestService) invocation.Target).WasExecuted));

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Perform();

            Assert.IsTrue(resolvedTestService.WasExecuted);
        }

        [TestMethod]
        public void ShouldCallbackOnAfter()
        {
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Perform())
                           .OnAfter(invocation => Assert.IsTrue(((ITestService) invocation.Target).WasExecuted));

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Perform();

            Assert.IsTrue(resolvedTestService.WasExecuted);
        }

        [TestMethod]
        public void ShouldCallbackOnCatchAndOnFinally()
        {
            var index = 0;

            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Fail())
                           .OnCatch(exception => exception.GetType().IsAssignableFromEx(typeof (InvalidOperationException)))
                           .OnFinally(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Fail();

            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void ShouldCallbackOnBeforeOnFinallyAndOnAfter()
        {
            var index = 0;

            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Perform())
                           .OnBefore(invocation =>
                               {
                                   index++;
                                   Assert.IsFalse(((ITestService) invocation.Target).WasExecuted);
                               })
                           .OnFinally(() => index++)
                           .OnAfter(invocation =>
                               {
                                   index++;
                                   Assert.IsTrue(((ITestService) invocation.Target).WasExecuted);
                               });

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Perform();

            Assert.AreEqual(3, index);
        }

        [TestMethod]
        public void ShouldCallbackOnReturnAndReplaceReturnValue()
        {
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Return())
                           .OnReturn((invocation, returnedValue) => returnedValue.Equals(1) ? -1 : -2);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Perform();

            Assert.AreEqual(-1, resolvedTestService.Return());
            Assert.IsTrue(resolvedTestService.WasExecuted);
        }

        [TestMethod]
        public void CanCallbackOnFinallyIfMethodThrows()
        {
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Fail())
                           .OnFinally(invocation => Assert.IsTrue(((ITestService) invocation.Target).WasExecuted));

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            ExceptionTester.CallMethodAndExpectException<TargetInvocationException>(resolvedTestService.Fail);
            Assert.IsTrue(resolvedTestService.WasExecuted);
        }

        [TestMethod]
        public void ShouldInterceptAllMembersExceptWhichTagged()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptAllMembers()
                           .OnFinally(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            var name = resolvedTestService.Name;
            resolvedTestService.Description = "anyValue";
            var description = resolvedTestService.Description;
            resolvedTestService.Name = name;
            resolvedTestService.TaggedPerform();
            var result = resolvedTestService.Perform<string>(description);

            Assert.IsNotNull(result);
            Assert.AreEqual("anyValue", result);
            Assert.AreEqual(5, index);
        }

        [TestMethod]
        public void ShouldInterceptAllMethodsGettersAndSetters()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptAll()
                           .OnFinally(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            var name = resolvedTestService.Name;
            resolvedTestService.Description = "anyValue";
            var description = resolvedTestService.Description;
            resolvedTestService.Name = name;
            var result = resolvedTestService.Perform<string>(description);

            Assert.IsNotNull(result);
            Assert.AreEqual("anyValue", result);
            Assert.AreEqual(5, index);
        }

        [TestMethod]
        public void ShouldInterceptGenericMethods()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Perform<int>(It.Any<int>()))
                           .OnBefore(invocation =>
                               {
                                   Assert.IsFalse(((ITestService) invocation.Target).WasExecuted);
                                   index++;
                               })
                           .And()
                           .InterceptMethod(service => service.Perform<string>(It.Any<string>()))
                           .OnAfter(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Perform<int>(1); // intercepted
            resolvedTestService.Perform<string>(string.Empty); // intercepted
            resolvedTestService.Perform(2d); // not intercepted
            resolvedTestService.Perform(It.Any<object>()); // not intercepted

            Assert.IsTrue(resolvedTestService.WasExecuted);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void ShouldInterceptMultipleMembersUsingFluentInstruction()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Perform())
                           .OnBefore(() => Assert.AreEqual(0, index))
                           .OnAfter(() => index++)
                           .And()
                           .InterceptGetter(service => service.Name)
                           .OnBefore(() => Assert.AreEqual(1, index))
                           .OnAfter(() => index++)
                           .And()
                           .InterceptSetter(service => service.Description)
                           .OnBefore(() => Assert.AreEqual(2, index))
                           .OnAfter(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            // intercepted
            resolvedTestService.Perform();
            var name = resolvedTestService.Name;
            resolvedTestService.Description = string.Empty;

            // not intercpted
            resolvedTestService.Return();
            resolvedTestService.Name = name;
            var description = resolvedTestService.Description;

            Assert.AreEqual(string.Empty, description);
            Assert.AreEqual(3, index);
        }

        [TestMethod]
        public void ShouldInterceptOverloadedMethods()
        {
            var value = false;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethod(service => service.Perform(It.Any<string>()))
                           .OnBefore(() => value = true);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            resolvedTestService.Perform(-1); // not intercepted
            Assert.IsFalse(value);
            resolvedTestService.Perform(string.Empty); // intercepted
            Assert.IsTrue(value);
        }

        [Ignore]
        [TestMethod]
        public void ShouldInterceptMethodUsingPredicate()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptWhere(invocation => invocation.Name == "Perform")
                           .OnBefore(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            // intercepted
            resolvedTestService.Perform();
            resolvedTestService.Perform<int>(-1);
            resolvedTestService.Perform(-1);
            resolvedTestService.Perform(string.Empty);

            // not intercepted
            resolvedTestService.Name = string.Empty;
            resolvedTestService.Return();

            Assert.AreEqual(4, index);
        }

        [TestMethod]
        public void ShouldInterceptManyMethodsWithOneCallback()
        {
            var index = 0;
            _serviceLocator.ConfigureInterceptionForType<ITestService, TestService>()
                           .InterceptMethods(
                               service => service.Return(),
                               service => service.Perform<int>(It.Any<int>()),
                               service => service.Perform())
                           .OnBefore(() => index++);

            var resolvedTestService = _serviceLocator.ResolveType<ITestService>();

            // intercepted
            resolvedTestService.Perform();
            resolvedTestService.Return();
            resolvedTestService.Perform<int>(-1);

            // not intercepted
            resolvedTestService.Name = string.Empty;
            resolvedTestService.Perform(string.Empty);

            Assert.AreEqual(3, index);
        }
        #endregion
    }
}