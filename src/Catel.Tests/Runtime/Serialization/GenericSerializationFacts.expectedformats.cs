namespace Catel.Tests.Runtime.Serialization
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using Catel.Runtime.Serialization;
    using NUnit.Framework;
    using VerifyNUnit;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public partial class ExpectedFormats
        {

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
#pragma warning disable IDISP001 // Dispose created.
                var context = new TemporaryFilesContext(name);
#pragma warning restore IDISP001 // Dispose created.
                var fileName = context.GetFile($"{serializer.GetType().Name}.dat", true);

                using (var fileStream = File.Create(fileName))
                {
                    serializer.Serialize(obj, fileStream);

                    fileStream.Flush();
                }

                Verifier.VerifyFile(fileName);
            }
        }
    }
}
