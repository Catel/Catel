// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModelWithParsableMembers.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization.TestModels
{
    using System;
    using System.Runtime.InteropServices;
    using Catel.Data;

    public class TestModelWithParsableMembers : ModelBase
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

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector
    {
        public double X;
        public double Y;
        public double Z;

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
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

            return new Vector(x, y, z);
        }
    }
}