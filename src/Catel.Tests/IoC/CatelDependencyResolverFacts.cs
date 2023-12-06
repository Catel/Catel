namespace Catel.Tests.IoC
{
    using System;
    using Catel.IoC;
    using Catel.Services;
    using NUnit.Framework;

    public class CatelDependencyResolverFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNulServiceLocator()
            {
                Assert.Throws<ArgumentNullException>(() => new CatelDependencyResolver(null));
            }
        }

        [TestFixture]
        public class TheCanResolveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.Throws<ArgumentNullException>(() => dependencyResolver.CanResolve(null));
                }
            }

            [TestCase]
            public void ReturnsFalseForNonRegisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.That(dependencyResolver.CanResolve(typeof(ITestInterface)), Is.False);
                }
            }

            [TestCase]
            public void ReturnsTrueForRegisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.That(dependencyResolver.CanResolve(typeof(IMessageService)), Is.True);
                }
            }
        }

        [TestFixture]
        public class TheCanResolveMultipleMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.Throws<ArgumentNullException>(() => dependencyResolver.CanResolveMultiple(null));
                }
            }

            [TestCase]
            public void ReturnsTrueForEmptyArray()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.That(dependencyResolver.CanResolveMultiple(new Type[] { }), Is.True);
                }
            }

            [TestCase]
            public void ReturnsFalseWhenNotAllTypesCanBeResolved()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    var typesToResolve = new[] { typeof(ITestInterface), typeof(INavigationService), typeof(ITypeFactory) };

                    Assert.That(dependencyResolver.CanResolveMultiple(typesToResolve), Is.False);
                }
            }

            [TestCase]
            public void ReturnsTrueWhenAllTypesCanBeResolved()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    serviceLocator.RegisterType<INavigationService, NavigationService>();
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    var typesToResolve = new[] { typeof(IMessageService), typeof(INavigationService), typeof(ITypeFactory) };
                    Assert.That(dependencyResolver.CanResolveMultiple(typesToResolve), Is.True);
                }
            }
        }

        [TestFixture]
        public class TheResolveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.Throws<ArgumentNullException>(() => dependencyResolver.Resolve(null));
                }
            }

            [TestCase]
            public void ThrowsTypeNotRegisteredForNonRegisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.That(dependencyResolver.Resolve(typeof(ITestInterface)), Is.Null);
                }
            }

            [TestCase]
            public void ReturnsInstanceForRegisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.IsNotNull(dependencyResolver.Resolve(typeof(IMessageService)));
                }
            }
        }

        [TestFixture]
        public class TheResolveMultipleMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    Assert.Throws<ArgumentNullException>(() => dependencyResolver.ResolveMultiple(null));
                }
            }

            [TestCase]
            public void ReturnsEmptyArrayForEmptyArray()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    var resolvedObjects = dependencyResolver.ResolveMultiple(new Type[] { });
                    Assert.That(resolvedObjects.Length, Is.EqualTo(0));
                }
            }

            [TestCase]
            public void ReturnsArrayWithNullValuesForNonRegisteredTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    var typesToResolve = new[] { typeof(IMessageService), typeof(ITestInterface), typeof(ITypeFactory) };
                    var resolvedTypes = dependencyResolver.ResolveMultiple(typesToResolve);

                    Assert.IsNotNull(resolvedTypes[0] as IMessageService);
                    Assert.That(resolvedTypes[1], Is.Null);
                    Assert.IsNotNull(resolvedTypes[2] as ITypeFactory);
                }
            }

            [TestCase]
            public void ReturnsArrayWithAllValuesForRegisteredTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IMessageService, MessageService>();
                    serviceLocator.RegisterType<INavigationService, NavigationService>();
                    var dependencyResolver = serviceLocator.ResolveRequiredType<IDependencyResolver>();

                    var typesToResolve = new[] { typeof(IMessageService), typeof(INavigationService), typeof(ITypeFactory) };
                    var resolvedTypes = dependencyResolver.ResolveMultiple(typesToResolve);

                    Assert.IsNotNull(resolvedTypes[0] as IMessageService);
                    Assert.IsNotNull(resolvedTypes[1] as INavigationService);
                    Assert.IsNotNull(resolvedTypes[2] as ITypeFactory);
                }
            }
        }
    }
}
