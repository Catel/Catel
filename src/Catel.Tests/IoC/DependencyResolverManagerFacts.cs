namespace Catel.Tests.IoC
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class DependencyResolverManagerFacts
    {
        [TestFixture]
        public class TheRegisterDependencyResolverForInstanceMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var dependencyResolverManager = new DependencyResolverManager();
                    var dependencyResolver = new CatelDependencyResolver(serviceLocator);

                    Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForInstance(null, dependencyResolver));
                }
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDependencyResolver()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForInstance(new object(), null));
            }
        }

        [TestFixture]
        public class TheGetDependencyResolverForInstanceMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.GetDependencyResolverForInstance(null));
            }

            [TestCase]
            public void ReturnsRegisteredDependencyResolverForRegisteredInstance()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var dependencyResolverManager = new DependencyResolverManager();
                    var dependencyResolver = new CatelDependencyResolver(serviceLocator);
                    var myObject = new object();

                    dependencyResolverManager.RegisterDependencyResolverForInstance(myObject, dependencyResolver);

                    var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                    Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
                }
            }

            [TestCase]
            public void ReturnsRegisteredDependencyResolverForRegisteredType()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var dependencyResolverManager = new DependencyResolverManager();
                    var dependencyResolver = new CatelDependencyResolver(serviceLocator);
                    var myObject = new object();

                    dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), dependencyResolver);

                    var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                    Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
                }
            }

            [TestCase]
            public void ReturnsDefaultDependencyResolverForNonRegisteredInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var myObject = new object();

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolverManager.DefaultDependencyResolver, resolvedDependencyResolver));
            }
        }

        [TestFixture]
        public class TheRegisterDependencyResolverForTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var dependencyResolverManager = new DependencyResolverManager();
                    var dependencyResolver = new CatelDependencyResolver(serviceLocator);

                    Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForType(null, dependencyResolver));
                }
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDependencyResolver()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), null));
            }
        }

        [TestFixture]
        public class TheGetDependencyResolverForTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.GetDependencyResolverForType(null));
            }

            [TestCase]
            public void ReturnsRegisteredDependencyResolverForRegisteredType()
            {
                using (var serviceLocator = new ServiceLocator())
                {
                    var dependencyResolverManager = new DependencyResolverManager();
                    var dependencyResolver = new CatelDependencyResolver(serviceLocator);

                    dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), dependencyResolver);

                    var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                    Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
                }
            }

            [TestCase]
            public void ReturnsDefaultDependencyResolverForNonRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                Assert.IsTrue(ReferenceEquals(dependencyResolverManager.DefaultDependencyResolver, resolvedDependencyResolver));
            }

            [TestFixture]
            public class TheResolveMissingType
            {
                private interface IDummy
                {

                }

                [TestCase]
                public void ResolveRequired_Throws_TypeNotRegisteredException()
                {
                    var dependencyResolverManager = new DependencyResolverManager();

                    var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                    Assert.Throws<CatelException>(() => resolvedDependencyResolver.ResolveRequired(typeof(IDummy)));
                }

                [TestCase]
                public void Resolve_Returns_Null()
                {
                    var dependencyResolverManager = new DependencyResolverManager();

                    var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                    Assert.IsNull(resolvedDependencyResolver.Resolve(typeof(IDummy)));
                }
            }
        }
    }
}
