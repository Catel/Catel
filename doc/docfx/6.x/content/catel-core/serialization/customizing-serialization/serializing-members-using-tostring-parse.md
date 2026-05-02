---
title: "Serializing members using ToString / Parse" 
description: ""
weight: 30
---
Sometimes types (classes or structs) don't implement a proper serialization mechanism. If they support proper *ToString(IFormatProvider)* and *Parse(string, IFormatProvider)* methods, there is no need to create a custom *SerializerModifier* to serialize these types. To let the serializers take care of this automatically, at least one of the following options must be true:

1.  The member is decorated using the *SerializeUsingParseAndToString* attribute
2.  The container class has a *SerializerModifier* that returns *true* in the *ShouldSerializeMemberUsingParse* method

{{% notice info %}}
Note that decorating a member that does not implement proper *ToString(IFormatProvider)* and *Parse(string, IFormatProvider)* methods is useless, the serialization engine will ignore these types
{{% /notice %}}

For example, the class below is an excellent usage example of when to use this technique:

```
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

            var vector = new Vector(x, y, z);
            return vector;
        }
    }
```

