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

                Assert.AreEqual(resolved, instance);
            }

            public interface IInterfaceA
            { }

            public class ClassA : IInterfaceA
            { }
        }
    }
}
