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

                Assert.IsTrue(ReferenceEquals(defaultTypeFactory, usedTypeFactory));
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

                    Assert.IsTrue(ReferenceEquals(typeFactory, usedTypeFactory));
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

                Assert.IsTrue(ReferenceEquals(defaultDependencyResolver, dependencyResolver));
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

                    Assert.IsTrue(ReferenceEquals(dependencyResolver, usedDependencyResolver));
                }
            }
        }
    }
}
