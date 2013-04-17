#pragma warning disable 1591 // 1591 = missing xml

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    /// <summary>
    /// Interface rappresenting a DotNet type
    /// </summary>
    public interface IType
    {
        string AssemblyQualifiedName { get; }
        bool IsSubclassOf(IType type);
        bool Equals(IType type);
    }
}
