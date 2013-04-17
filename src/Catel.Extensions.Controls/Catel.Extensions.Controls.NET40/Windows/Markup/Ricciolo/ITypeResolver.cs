#pragma warning disable 1591 // 1591 = missing xml

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    public interface ITypeResolver
    {
        IType GetTypeByAssemblyQualifiedName(string name);
        IDependencyPropertyDescriptor GetDependencyPropertyDescriptor(string name, IType ownerType, IType targetType);
    }
}
