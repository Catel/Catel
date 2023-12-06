namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Reflection;
    using NUnit.Framework;

    public class TypeCacheFacts
    {
        [TestFixture]
        public class AssemblyLoading
        {
            [TestCase]
            public void IgnoresResourceAssemblies()
            {
                var allAssemblies = TypeCache.InitializedAssemblies;

                var resourceAssemblies = allAssemblies.Where(x => x.ToLower().Contains(".resources.") && !x.ToLower().Contains("resourcemanager")).ToList();

                Assert.That(resourceAssemblies.Count, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheGetTypeMethod
        {
            [TestCase("System.String", false, typeof(string))]
            [TestCase("System.String", true, typeof(string))]
            [TestCase("sYstEm.StRing", false, null)]
            [TestCase("sYstEm.StRing", true, typeof(string))]
            public void ReturnsTypeFromMsCorlib(string typeName, bool ignoreCase, Type? expectedType)
            {
                var type = TypeCache.GetType(typeName, ignoreCase);

                Assert.That(type, Is.EqualTo(expectedType));
            }

            [TestCase("System.Uri", false, typeof(Uri))]
            [TestCase("System.Uri", true, typeof(Uri))]
            [TestCase("sYstEm.uRi", false, null)]
            [TestCase("sYstEm.uRi", true, typeof(Uri))]
            public void ReturnsTypeFromSystem(string typeName, bool ignoreCase, Type? expectedType)
            {
                var type = TypeCache.GetType(typeName, ignoreCase);

                Assert.That(type, Is.EqualTo(expectedType));
            }

            [TestCase("System.Lazy`1", false, typeof(Lazy<>))]
            [TestCase("System.Lazy`1", true, typeof(Lazy<>))]
            [TestCase("sYstEm.lAzY`1", false, null)]
            [TestCase("sYstEm.lAzY`1", true, typeof(Lazy<>))]
            public void ReturnsTypeFromSystemCore(string typeName, bool ignoreCase, Type? expectedType)
            {
                var type = TypeCache.GetType(typeName, ignoreCase);

                Assert.That(type, Is.EqualTo(expectedType));
            }

            [TestCase]
            public void ReturnsTypeForLateBoundGenericTypeMultipleTimes()
            {
                var type = TypeCache.GetType("System.Collections.Generic.List`1[[System.Int32]]");
                Assert.That(type, Is.EqualTo(typeof(List<int>)));

                var type2 = TypeCache.GetType("System.Collections.Generic.List`1[[System.Int32]]");
                Assert.That(type2, Is.EqualTo(typeof(List<int>)));
            }

            [TestCase("Catel.Tests.Reflection.TypeCacheFacts[]", typeof(TypeCacheFacts[]))]
            [TestCase("System.Collections.Generic.List`1[[Catel.Tests.Reflection.TypeCacheFacts]]", typeof(List<TypeCacheFacts>))]
            public void ReturnsRightType(string typeName, Type expectedType)
            {
                var type = TypeCache.GetType(typeName);

                Assert.That(type, Is.EqualTo(expectedType));
            }
        }

        [TestFixture]
        public class TheGetTypesImplementingInterfaceMethod
        {
            public interface IMySpecialInterface
            {

            }

            public class MySpecialClass : IMySpecialInterface
            {

            }

            [TestCase]
            public void ReturnsRightTypes()
            {
                var interfaces = TypeCache.GetTypesImplementingInterface(typeof(IMySpecialInterface));

                Assert.That(interfaces.Length, Is.EqualTo(1));
                Assert.That(interfaces[0], Is.EqualTo(typeof(MySpecialClass)));
            }
        }

        [Test, Explicit]
        public void Does_Not_Enter_In_A_Deadlock()
        {
            TypeCache.InitializeTypes();

            Task<Type>[] tasks = { LoadABAsync(), LoadACAsync() };

            // ReSharper disable once CoVariantArrayConversion
            Assert.That(Task.WaitAll(tasks, 5000), Is.True);

            Assert.IsNotNull(tasks[0].Result);
            Assert.IsNotNull(tasks[1].Result);

            var typeB = TypeCache.GetType("A.AB, A");
            var typeC = TypeCache.GetType("A.AC, A");

            Assert.IsNotNull(typeB);
            Assert.IsNotNull(typeC);
        }

        private static Task<Type> LoadACAsync()
        {
            return Task.Run(() => Type.GetType("A.AC, A, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }

        private static Task<Type> LoadABAsync()
        {
            return Task.Run(() => Type.GetType("A.AB, A, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }
    }
}
