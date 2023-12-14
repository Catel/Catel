namespace Catel.Tests.IoC
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class ObjectExtensionsFacts
    {
        [TestFixture]
        public class TheGetTypeFactoryMethod
        {
            [TestCase]
            public void ReturnsDefaultTypeFactoryForObjectNotCreatedWithTypeFactory()
            {
                var obj = new object();
                var defaultTypeFactory = TypeFactory.Default;

#pragma warning disable IDISP001 // Dispose created.
                var usedTypeFactory = obj.GetTypeFactory();
#pragma warning restore IDISP001 // Dispose created.

                Assert.That(ReferenceEquals(defaultTypeFactory, usedTypeFactory), Is.True);
            }

            [TestCase]
            public void ReturnsTypeFactoryUsedToCreateObject()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();
                    var obj = typeFactory.CreateInstance<object>();
                    if (obj is null)
                    {
                        throw new Exception("Created object should not be null");
                    }

#pragma warning disable IDISP001 // Dispose created.
                    var usedTypeFactory = obj.GetTypeFactory();
#pragma warning restore IDISP001 // Dispose created.

                    Assert.That(ReferenceEquals(typeFactory, usedTypeFactory), Is.True);
                }
            }
        }

        [TestFixture]
        public class TheGetDependencyResolverMethod
        {
            [TestCase]
            public void ReturnsDefaultDependencyResolverForObjectNotCreatedWithTypeFactory()
            {
                var obj = new object();
                var defaultDependencyResolver = ServiceLocator.Default.ResolveRequiredType<IDependencyResolver>();
                var dependencyResolver = obj.GetDependencyResolver();

                Assert.That(ReferenceEquals(defaultDependencyResolver, dependencyResolver), Is.True);
            }

            [TestCase]
            public void ReturnsDependencyResolverUsedToCreateObject()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();
                    var typeFactory = dependencyResolver.ResolveRequired<ITypeFactory>();
                    var obj = typeFactory.CreateInstance<object>();
                    if (obj is null)
                    {
                        throw new Exception("Created object should not be null");
                    }

                    var usedDependencyResolver = obj.GetDependencyResolver();

                    Assert.That(ReferenceEquals(dependencyResolver, usedDependencyResolver), Is.True);
                }
            }
        }
    }
}
