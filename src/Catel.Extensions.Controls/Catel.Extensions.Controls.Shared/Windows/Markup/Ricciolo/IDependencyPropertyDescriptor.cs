#if NET

#pragma warning disable 1591 // 1591 = missing xml

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    public interface IDependencyPropertyDescriptor
    {
        bool IsAttached { get; }
    }
}

#endif