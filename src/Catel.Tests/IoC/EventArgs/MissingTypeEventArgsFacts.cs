// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingTypeEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.IoC.EventArgs
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class MissingTypeEventArgsFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInterfaceType()
            {
                Assert.Throws<ArgumentNullException>(() => new MissingTypeEventArgs(null));
            }

            [TestCase]
            public void SetsValuesCorrectly()
            {
                var eventArgs = new MissingTypeEventArgs(typeof(ITestInterface));

                Assert.AreEqual(typeof(ITestInterface), eventArgs.InterfaceType);
            }

            [TestCase]
            public void CanResolveTransientByMissingTypeEventArgsWithTag()
            {
                const string Tag = "TAG";
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.MissingType += (s, e) =>
                    {
                        var tagString = (string)e.Tag;
                        Type instanceType = (tagString == Tag) ? typeof(ClassATag) : typeof(ClassA);
                        e.ImplementingType = instanceType;
                        e.RegistrationType = RegistrationType.Transient;
                    };

                    var classA = serviceLocator.ResolveType<IInterfaceA>();
                    var classATag = serviceLocator.ResolveType<IInterfaceA>(Tag);

                    Assert.IsInstanceOf(typeof(ClassA), classA);
                    Assert.IsInstanceOf(typeof(ClassATag), classATag);
                }
            }

            [TestCase]
            public void ResolvedInstancesAreNotSameForTransientWhenResolvedUsingMissingTypeEventWithTag()
            {
                const string Tag = "TAG";
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.MissingType += (s, e) =>
                    {
                        var tagString = (string)e.Tag;
                        Type instanceType = (tagString == Tag) ? typeof(ClassATag) : typeof(ClassA);
                        e.ImplementingType = instanceType;
                        e.RegistrationType = RegistrationType.Transient;
                    };

                    var classATag = serviceLocator.ResolveType<IInterfaceA>(Tag);
                    var classATag2 = serviceLocator.ResolveType<IInterfaceA>(Tag);

                    Assert.AreNotSame(classATag, classATag2);
                }
            }

            [TestCase]
            public void CanResolveSingletonByMissingTypeEventArgsWithTag()
            {
                const string Tag = "TAG";
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.MissingType += (s, e) =>
                    {
                    var tagString = (string)e.Tag;
                    IInterfaceA instance = (tagString == Tag) ? new ClassATag() as IInterfaceA : new ClassA() as IInterfaceA;
                    e.ImplementingInstance = instance;
                    e.RegistrationType = RegistrationType.Singleton;
                    };

                    var classA = serviceLocator.ResolveType<IInterfaceA>();
                    var classATag = serviceLocator.ResolveType<IInterfaceA>(Tag);

                    Assert.IsInstanceOf(typeof(ClassA), classA);
                    Assert.IsInstanceOf(typeof(ClassATag), classATag);
                }
            }

            [TestCase]
            public void ResolvedInstancesAreSameForSingletonWhenResolvedUsingMissingTypeEventWithTag()
            {
                const string Tag = "TAG";
                using (var serviceLocator = new ServiceLocator())
                {
                    serviceLocator.MissingType += (s, e) =>
                    {
                    var tagString = (string)e.Tag;
                    IInterfaceA instance = (tagString == Tag) ? new ClassATag() as IInterfaceA : new ClassA() as IInterfaceA;
                    e.ImplementingInstance = instance;
                    e.RegistrationType = RegistrationType.Singleton;
                    };

                    var classATag = serviceLocator.ResolveType<IInterfaceA>(Tag);
                    var classATag2 = serviceLocator.ResolveType<IInterfaceA>(Tag);

                    Assert.AreSame(classATag, classATag2);
                }
            }

            public interface IInterfaceA
            { }

            public class ClassA : IInterfaceA
            { }

            public class ClassATag : IInterfaceA
            { }
        }
    }
}
