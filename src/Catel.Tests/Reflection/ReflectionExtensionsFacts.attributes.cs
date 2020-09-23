// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensionsFacts.attributes.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// Required since we are testing obsolete attribute retrievals
#pragma warning disable 612

namespace Catel.Tests.Reflection
{
    using System;
    using System.Reflection;
    using Catel.Reflection;
    using NUnit.Framework;

    public partial class ReflectionExtensionsFacts
    {
        public class ClassWithoutAttributeDecorations
        {
            public string Property { get; set; }
        }

        [Obsolete]
        public class ClassWithAttributeDecorations
        {
            [Obsolete]
            public string Property { get; set; }
        }

        public class TheGetAttributeMethod
        {
            [TestCase(typeof(ClassWithoutAttributeDecorations), typeof(ObsoleteAttribute), false)]
            [TestCase(typeof(ClassWithAttributeDecorations), typeof(ObsoleteAttribute), true)]
            public void ReturnsAttributeForTypes(Type type, Type expectedAttributeType, bool isNotNull)
            {
                var attribute = type.GetAttribute(expectedAttributeType);

                if (isNotNull)
                {
                    Assert.IsNotNull(attribute);
                }
                else
                {
                    Assert.IsNull(attribute);
                }
            }

            [TestCase(typeof(ClassWithoutAttributeDecorations), typeof(ObsoleteAttribute), false)]
            [TestCase(typeof(ClassWithAttributeDecorations), typeof(ObsoleteAttribute), true)]
            public void ReturnsAttributeForMembers(Type type, Type expectedAttributeType, bool isNotNull)
            {
                var member = type.GetPropertyEx("Property");
                var attribute = member.GetAttribute(expectedAttributeType);

                if (isNotNull)
                {
                    Assert.IsNotNull(attribute);
                }
                else
                {
                    Assert.IsNull(attribute);
                }
            }
        }

        [TestFixture]
        public class TheTryGetAttributeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyInfo()
            {
                ObsoleteAttribute attribute;
                Assert.Throws<ArgumentNullException>(() => ((MemberInfo)null).TryGetAttribute(out attribute));
            }
        }
    }
}