// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModelWithParsableMembers.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using System.Runtime.InteropServices;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public class TestModelWithParsableMembersWithoutAttributes : ModelBase
    {
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public Vector Vector
        {
            get { return GetValue<Vector>(VectorProperty); }
            set { SetValue(VectorProperty, value); }
        }

        /// <summary>
        /// Register the Vector property so it is known in the class.
        /// </summary>
        public static readonly PropertyData VectorProperty = RegisterProperty("Vector", typeof(Vector), null);
    }

    public class TestModelWithParsableMembersWithAttributes : ModelBase
    {
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [SerializeUsingParseAndToString]
        public Vector Vector
        {
            get { return GetValue<Vector>(VectorProperty); }
            set { SetValue(VectorProperty, value); }
        }

        /// <summary>
        /// Register the Vector property so it is known in the class.
        /// </summary>
        public static readonly PropertyData VectorProperty = RegisterProperty("Vector", typeof(Vector), null);
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector
    {
        public double X;
        public double Y;
        public double Z;
        public bool UsedParse;

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            UsedParse = false;
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return $"{X.ToString(formatProvider)} {Y.ToString(formatProvider)} {Z.ToString(formatProvider)}";
        }

        public static Vector Parse(string value, IFormatProvider formatProvider)
        {
            var splitted = value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var x = double.Parse(splitted[0], formatProvider);
            var y = double.Parse(splitted[1], formatProvider);
            var z = double.Parse(splitted[2], formatProvider);

            var vector = new Vector(x, y, z);
            vector.UsedParse = true;

            return vector;
        }
    }

    public abstract class TestModelWithParsableMembersSerializerModifierBase : SerializerModifierBase<TestModelWithParsableMembersWithoutAttributes>
    {
        private readonly bool _serializeUsingParse;

        protected TestModelWithParsableMembersSerializerModifierBase(bool serializeUsingParse)
        {
            _serializeUsingParse = serializeUsingParse;
        }

        public override bool? ShouldSerializeMemberUsingParse(MemberValue memberValue)
        {
            if (memberValue.Name == "Vector")
            {
                return _serializeUsingParse;
            }

            return base.ShouldSerializeMemberUsingParse(memberValue);
        }

        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            base.SerializeMember(context, memberValue);

            if (!_serializeUsingParse)
            {
                if (memberValue.Name == "Vector")
                {
                    var vector = (Vector) memberValue.Value;
                    memberValue.Value = $"{vector.X}|{vector.Y}|{vector.Z}";
                }
            }
        }

        public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
            base.DeserializeMember(context, memberValue);

            if (!_serializeUsingParse)
            {
                if (memberValue.Name == "Vector")
                {
                    var vectorString = (string) memberValue.Value;
                    var parsedValues = vectorString.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries);

                    memberValue.Value = new Vector(StringToObjectHelper.ToDouble(parsedValues[0]), StringToObjectHelper.ToDouble(parsedValues[1]),
                        StringToObjectHelper.ToDouble(parsedValues[2]));
                }
            }
        }
    }

    public class TestModelWithParsableMembersUsingParseSerializerModifier : TestModelWithParsableMembersSerializerModifierBase
    {
        public TestModelWithParsableMembersUsingParseSerializerModifier() 
            : base(true)
        {
        }
    }

    public class TestModelWithParsableMembersNotUsingParseSerializerModifier : TestModelWithParsableMembersSerializerModifierBase
    {
        public TestModelWithParsableMembersNotUsingParseSerializerModifier()
            : base(false)
        {
        }
    }
}