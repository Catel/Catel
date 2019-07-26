// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInstantiatedEventArgsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.IoC.EventArgs
{
    using Catel.IoC;
    using NUnit.Framework;

    public class TypeInstantiatedEventArgsFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void TypeInstantiatedEventArgsHasValidInstance()
            {
                object instance = null;
                var serviceLocator = new ServiceLocator();
                serviceLocator.TypeInstantiated += (s, e) => instance = e.Instance;
                serviceLocator.RegisterType<IInterfaceA, ClassA>();

                var resolved = serviceLocator.ResolveType<IInterfaceA>();

                Assert.IsTrue(ReferenceEquals(resolved, instance));
            }

            [TestCase]
            public void TypeInstantiatedEventIsRaisedWhenRegisteringSingletonUsingCreateServiceFunc()
            {
                int numCalls = 0;
                bool typeInstantiatedCalled = false;
                var serviceLocator = new ServiceLocator();
                serviceLocator.TypeInstantiated += (s, e) => typeInstantiatedCalled = true;

                IInterfaceA createServiceFunc(ServiceLocatorRegistration reg)
                {
                    numCalls++;
                    return new ClassA();
                }

                serviceLocator.RegisterType<IInterfaceA>(createServiceFunc, RegistrationType.Singleton);

                var resolved = serviceLocator.ResolveType<IInterfaceA>();

                Assert.IsTrue(typeInstantiatedCalled);
                Assert.AreEqual(1, numCalls);
                Assert.IsInstanceOf(typeof(ClassA), resolved);
            }

            [TestCase]
            public void TypeInstantiatedEventIsRaisedWhenRegisteringTransientUsingCreateServiceFunc()
            {
                int numCalls = 0;
                bool typeInstantiatedCalled = false;
                var serviceLocator = new ServiceLocator();
                serviceLocator.TypeInstantiated += (s, e) => typeInstantiatedCalled = true;

                IInterfaceA createServiceFunc(ServiceLocatorRegistration reg)
                {
                    numCalls++;
                    return new ClassA();
                }

                serviceLocator.RegisterType<IInterfaceA>(createServiceFunc, RegistrationType.Transient);

                var resolved = serviceLocator.ResolveType<IInterfaceA>();
                var resolved1 = serviceLocator.ResolveType<IInterfaceA>();

                Assert.IsTrue(typeInstantiatedCalled);
                Assert.AreEqual(2, numCalls);
                Assert.IsInstanceOf(typeof(ClassA), resolved);
            }

            [TestCase]
            public void TypeInstantiatedEventIsRaisedWhenRegisteringWithoutCreateServiceFunc()
            {
                var serviceLocator = new ServiceLocator();
                bool typeInstantiatedCalled = false;
                serviceLocator.TypeInstantiated += (s, e) => typeInstantiatedCalled = true;

                serviceLocator.RegisterType<IInterfaceA, ClassA>();

                var resolved = serviceLocator.ResolveType<IInterfaceA>();

                Assert.IsTrue(typeInstantiatedCalled);
                Assert.IsInstanceOf(typeof(ClassA), resolved);
            }

            public interface IInterfaceA
            { }

            public class ClassA : IInterfaceA
            { }
        }
    }
}
