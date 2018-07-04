namespace Catel.Tests.Runtime.Serialization
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using Catel.Runtime.Serialization;
    using Catel.Tests.Data;
    using NUnit.Framework;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public partial class ExpectedFormats
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void TestBinarySerializationWithExpectedFormat(object obj, ISerializationManager serializationManager = null, [CallerMemberName]string name = null)
            {
                TestSerializationWithExpectedFormat(SerializationTestHelper.GetBinarySerializer(serializationManager), name, obj);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void TestXmlSerializationWithExpectedFormat(object obj, ISerializationManager serializationManager = null, [CallerMemberName]string name = null)
            {
                TestSerializationWithExpectedFormat(SerializationTestHelper.GetXmlSerializer(serializationManager), name, obj);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void TestJsonSerializationWithExpectedFormat(object obj, ISerializationManager serializationManager = null, [CallerMemberName]string name = null)
            {
                TestSerializationWithExpectedFormat(SerializationTestHelper.GetJsonSerializer(serializationManager), name, obj);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private void TestSerializationWithExpectedFormat(ISerializer serializer, string name, object obj)
            {
                // Note: not in using since we need the file to be available for comparison
                var context = new TemporaryFilesContext(name);
                var fileName = context.GetFile($"{serializer.GetType().Name}.dat", true);

                using (var fileStream = File.Create(fileName))
                {
                    serializer.Serialize(obj, fileStream);

                    fileStream.Flush();
                }

                Approvals.VerifyFile(fileName);
            }
        }
    }
}
