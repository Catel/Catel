// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using ApprovalTests.Namers;
    using Catel.MVVM;
    using Catel.Runtime.Serialization.Json;
    using NUnit.Framework;
    using PublicApiGenerator;

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
            var writer = new ApprovalTextWriter(publicApi, "cs");
            var approvalNamer = new AssemblyPathNamer(assembly.Location);
            Approvals.Verify(writer, approvalNamer, Approvals.GetReporter());
        }
    }

    internal class AssemblyPathNamer : UnitTestFrameworkNamer
    {
        private readonly string _name;

        public AssemblyPathNamer(string assemblyPath)
        {
            _name = Path.GetFileNameWithoutExtension(assemblyPath);
        }

        public override string Name
        {
            get { return _name; }
        }
    }
}
