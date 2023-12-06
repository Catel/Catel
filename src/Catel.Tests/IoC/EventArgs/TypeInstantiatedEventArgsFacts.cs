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
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.TypeInstantiated += (s, e) => instance = e.Instance;
                    serviceLocator.RegisterType<IInterfaceA, ClassA>();

                    var resolved = serviceLocator.ResolveType<IInterfaceA>();

                    Assert.That(ReferenceEquals(resolved, instance), Is.True);
                }
            }

            [TestCase]
            public void TypeInstantiatedEventIsRaisedWhenRegisteringSingletonUsingCreateServiceFunc()
            {
                int numCalls = 0;
                bool typeInstantiatedCalled = false;
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.TypeInstantiated += (s, e) => typeInstantiatedCalled = true;

                    serviceLocator.RegisterType<IInterfaceA>(createServiceFunc, RegistrationType.Singleton);

                    var resolved = serviceLocator.ResolveType<IInterfaceA>();

                    Assert.That(typeInstantiatedCalled, Is.True);
                    Assert.That(numCalls, Is.EqualTo(1));
                    Assert.That(resolved, Is.InstanceOf(typeof(ClassA)));
                }

                IInterfaceA createServiceFunc(ITypeFactory tf, ServiceLocatorRegistration reg)
                {
                    numCalls++;
                    return new ClassA();
                }
            }

            [TestCase]
            public void TypeInstantiatedEventIsRaisedWhenRegisteringTransientUsingCreateServiceFunc()
            {
                int numCalls = 0;
                bool typeInstantiatedCalled = false;
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.TypeInstantiated += (s, e) => typeInstantiatedCalled = true;

                    serviceLocator.RegisterType<IInterfaceA>(createServiceFunc, RegistrationType.Transient);

                    var resolved = serviceLocator.ResolveType<IInterfaceA>();
                    var resolved1 = serviceLocator.ResolveType<IInterfaceA>();

                    Assert.That(typeInstantiatedCalled, Is.True);
                    Assert.That(numCalls, Is.EqualTo(2));
                    Assert.That(resolved, Is.InstanceOf(typeof(ClassA)));
                }

                IInterfaceA createServiceFunc(ITypeFactory tf, ServiceLocatorRegistration reg)
                {
                    numCalls++;
                    return new ClassA();
                }
            }

            [TestCase]
            public void TypeInstantiatedEventIsRaisedWhenRegisteringWithoutCreateServiceFunc()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    bool typeInstantiatedCalled = false;
                    serviceLocator.TypeInstantiated += (s, e) => typeInstantiatedCalled = true;

                    serviceLocator.RegisterType<IInterfaceA, ClassA>();

                    var resolved = serviceLocator.ResolveType<IInterfaceA>();

                    Assert.That(typeInstantiatedCalled, Is.True);
                    Assert.That(resolved, Is.InstanceOf(typeof(ClassA)));
                }
            }

            public interface IInterfaceA
            { }

            public class ClassA : IInterfaceA
            { }
        }
    }
}
