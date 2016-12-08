// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeRequestPath.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.IoC
{
    using System;
    using System.Linq;
    using System.Text;
    using Logging;

    /// <summary>
    /// A list of types in which the types will be constructed when being resolved from the <see cref="ServiceLocator" />.
    /// </summary>
    public class TypeRequestPath : ITypeRequestPath
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly TypeRequestInfo[] _typePath;

        private TypeRequestPath(TypeRequestInfo[] typePath, string name)
        {
            _typePath = typePath;
            Name = name;
        }

        /// <summary>
        /// Creates root of type request path
        /// </summary>
        /// <param name="name">Path's name</param>
        /// <returns></returns>
        public static TypeRequestPath Root(string name = null)
        {
            return new TypeRequestPath(new TypeRequestInfo[0], name);
        }

        /// <summary>
        /// Creates branch of type request path
        /// </summary>
        /// <param name="parent">Parent path</param>
        /// <param name="typeRequestInfo">Appended path item</param>
        /// <returns></returns>
        public static TypeRequestPath Branch(TypeRequestPath parent, TypeRequestInfo typeRequestInfo)
        {
            Argument.IsNotNull("parent", parent);
            Argument.IsNotNull("typeRequestInfo", typeRequestInfo);
            if (parent._typePath.Any(o => o.Equals(typeRequestInfo)))
            {
                throw Log.ErrorAndCreateException(msg => new CircularDependencyException(parent, string.Format("{0}. For more information, view the enclosed TypeRequestPath", msg)),
                    "Found a circular dependency while resolving '{0}', it is used by '{1}'", parent.FirstType, parent.LastType);
            }

            var parentTypePath = parent._typePath;
            var typePath = new TypeRequestInfo[parent.TypeCount + 1];
            Array.Copy(parentTypePath, typePath, parentTypePath.Length);
            typePath[typePath.Length - 1] = typeRequestInfo;
            return new TypeRequestPath(typePath, parent.Name);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the number of types in the type path.
        /// </summary>
        /// <value>The type count.</value>
        public int TypeCount
        {
            get { return _typePath.Length; }
        }

        /// <summary>
        /// Gets all types in the right order.
        /// </summary>
        /// <value>All types.</value>
        public TypeRequestInfo[] AllTypes
        {
            get { return _typePath; }
        }

        /// <summary>
        /// Gets the first type in the type path.
        /// </summary>
        /// <value>The first type.</value>
        public TypeRequestInfo FirstType
        {
            get
            {
                if (_typePath.Length <= 0)
                {
                    return null;
                }

                return _typePath[0];
            }
        }

        /// <summary>
        /// Gets the last type in the type path.
        /// </summary>
        /// <value>The last type.</value>
        public TypeRequestInfo LastType
        {
            get
            {
                if (_typePath.Length <= 0)
                {
                    return null;
                }
                var index = _typePath.Length - 1;
                
                return _typePath[index];
            }
        }
        
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            
            for (int i = 0; i < _typePath.Length; i++)
            {
                if (i != 0)
                {
                    stringBuilder.Append(" => ");
                }

                stringBuilder.Append(_typePath[i].Type.Name);
            }

            return stringBuilder.ToString();
        }
    }
}