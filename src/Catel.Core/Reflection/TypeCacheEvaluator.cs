namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class TypeCacheEvaluator
    {
        static TypeCacheEvaluator()
        {
            AssemblyEvaluators = new List<Func<Assembly, bool>>();
            TypeEvaluators = new List<Func<Assembly, Type, bool>>();

            AssemblyEvaluators.Add(new Func<Assembly, bool>(x => x.FullName?.Contains(".resources.") ?? false));
        }

        /// <summary>
        /// Gets the evaluators used to determine whether a specific assembly should be ignored.
        /// </summary>
        /// <value>The should ignore assembly function.</value>
        public static List<Func<Assembly, bool>> AssemblyEvaluators { get; private set; }

        /// <summary>
        /// Gets the evaluators used to determine whether a specific type should be ignored.
        /// </summary>
        /// <value>The should ignore assembly function.</value>
        public static List<Func<Assembly, Type, bool>> TypeEvaluators { get; private set; }
    }
}
