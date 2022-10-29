namespace Catel.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Runtime.Serialization.Json;
    using NUnit.Framework;
    using PublicApiGenerator;
    using VerifyNUnit;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Catel_Core_HasNoBreakingChanges_Async()
        {
            var assembly = typeof(Argument).Assembly;

            await PublicApiApprover.ApprovePublicApiAsync(assembly);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Catel_MVVM_HasNoBreakingChanges_Async()
        {
            var assembly = typeof(ViewModelBase).Assembly;

            await PublicApiApprover.ApprovePublicApiAsync(assembly);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Catel_Serialization_Json_HasNoBreakingChanges_Async()
        {
            var assembly = typeof(JsonSerializer).Assembly;

            await PublicApiApprover.ApprovePublicApiAsync(assembly);
        }
    }

    internal static class PublicApiApprover
    {
        public static async Task ApprovePublicApiAsync(Assembly assembly)
        {
            var publicApi = assembly.GeneratePublicApi();
            await Verifier.Verify(publicApi);
        }
    }
}
