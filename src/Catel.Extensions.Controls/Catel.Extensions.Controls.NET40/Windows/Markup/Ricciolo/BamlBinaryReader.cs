#pragma warning disable 1591 // 1591 = missing xml

using System;
using System.IO;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    internal class BamlBinaryReader : BinaryReader
    {
        // Methods
        public BamlBinaryReader(Stream stream)
            : base(stream)
        {
        }

        public virtual double ReadCompressedDouble()
        {
            switch (this.ReadByte())
            {
                case 1:
                    return 0;

                case 2:
                    return 1;

                case 3:
                    return -1;

                case 4:
                    {
                        double num = this.ReadInt32();
                        return (num * 1E-06);
                    }
                case 5:
                    return this.ReadDouble();
            }
            throw new NotSupportedException();
        }

        public int ReadCompressedInt32()
        {
            return base.Read7BitEncodedInt();
        }
    }
}