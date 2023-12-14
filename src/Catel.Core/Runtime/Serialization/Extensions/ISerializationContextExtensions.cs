namespace Catel.Runtime.Serialization
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods for the serialization context.
    /// </summary>
    public static class ISerializationContextExtensions
    {
        /// <summary>
        /// Tries to find the parent type in the object graph.
        /// </summary>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="maxLevels">The maximum number of levels to check. If <c>-1</c>, will check all up to the root.</param>
        /// <returns>The type or <c>null</c> of the type is not found.</returns>
        public static Type? FindParentType(this ISerializationContext serializationContext, Func<Type, bool> predicate, int maxLevels = -1)
        {
            var typeStack = serializationContext.TypeStack.ToList();
            typeStack.Reverse();

            if (maxLevels == -1)
            {
                maxLevels = typeStack.Count;
            }

            for (var i = 0; i < maxLevels && i < typeStack.Count; i++)
            {
                var type = typeStack[i];
                if (predicate(type))
                {
                    return type;
                }
            }

            return null;
        }
    }
}
