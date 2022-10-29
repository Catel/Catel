namespace Catel.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Catel.MVVM;
    using Catel.Runtime.Serialization.Json;
    using NUnit.Framework;
    using PublicApiGenerator;
    using VerifyNUnit;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Catel_Core_HasNoBreakingChanges()
        {
            var assembly = typeof(Argument).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Catel_MVVM_HasNoBreakingChanges()
        {
            var assembly = typeof(ViewModelBase).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Catel_Serialization_Json_HasNoBreakingChanges()
        {
            var assembly = typeof(JsonSerializer).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }

    internal static class PublicApiApprover
    {
        public static void ApprovePublicApi(Assembly assembly)
        {
            var publicApi = assembly.GeneratePublicApi();
            Verifier.Verify(publicApi);
        }
    }
}
