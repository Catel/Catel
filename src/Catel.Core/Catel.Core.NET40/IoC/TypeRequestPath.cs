// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeRequestPath.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logging;
    using Reflection;

    /// <summary>
    /// A list of types in which the types will be constructed when being resolved from the <see cref="ServiceLocator" />.
    /// </summary>
    public class TypeRequestPath : ITypeRequestPath
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly List<TypeRequestInfo> _typePath = new List<TypeRequestInfo>();
        
        private bool _isValid = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRequestPath"/> class.
        /// </summary>
        /// <param name="typeRequestInfo">The type request info.</param>
        /// <param name="ignoreValueTypes">If set to <c>true</c>, this type path will ignore value types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeRequestInfo" /> is <c>null</c>.</exception>
        public TypeRequestPath(TypeRequestInfo typeRequestInfo, bool ignoreValueTypes = true)
            : this(new [] { typeRequestInfo }, ignoreValueTypes) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRequestPath" /> class.
        /// </summary>
        /// <param name="typeRequestInfos">The type requests already in the path.</param>
        /// <param name="ignoreValueTypes">If set to <c>true</c>, this type path will ignore value types.</param>
        /// <exception cref="ArgumentException">The <paramref name="typeRequestInfos" /> is <c>null</c> or an empty array.</exception>
        public TypeRequestPath(TypeRequestInfo[] typeRequestInfos, bool ignoreValueTypes = true)
        {
            Argument.IsNotNullOrEmptyArray("typeRequestInfos", typeRequestInfos);

            IgnoreValueTypes = ignoreValueTypes;

            foreach (var typeRequestInfo in typeRequestInfos)
            {
                PushType(typeRequestInfo, false, false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether value types should be ignored in the path.
        /// </summary>
        /// <value><c>true</c> if value types should be ignored; otherwise, <c>false</c>.</value>
        public bool IgnoreValueTypes { get; private set; }

        /// <summary>
        /// Gets the number of types in the type path.
        /// </summary>
        /// <value>The type count.</value>
        public int TypeCount
        {
            get { return _typePath.Count; }
        }

        /// <summary>
        /// Gets all types in the right order.
        /// </summary>
        /// <value>All types.</value>
        public TypeRequestInfo[] AllTypes
        {
            get { return _typePath.ToArray(); }
        }

        /// <summary>
        /// Gets the first type in the type path.
        /// </summary>
        /// <value>The first type.</value>
        public TypeRequestInfo FirstType
        {
            get { return _typePath[0]; }
        }

        /// <summary>
        /// Gets the last type in the type path.
        /// </summary>
        /// <value>The last type.</value>
        public TypeRequestInfo LastType
        {
            get { return _typePath[_typePath.Count - 1]; }
        }

        /// <summary>
        /// Gets a value indicating whether this path is valid, which means that the same type does not occur multiple times.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                if (!_isValid)
                {
                    return false;
                }

                foreach (var typeRequestInfo in _typePath)
                {
                    if (_typePath.Count(x => x.Equals(typeRequestInfo)) > 1)
                    {
                        _isValid = false;
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            
            for (int i = 0; i < _typePath.Count; i++)
            {
                if (i != 0)
                {
                    stringBuilder.Append(" => ");
                }

                stringBuilder.Append(_typePath[i].Type.Name);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Marks the type as not created and removes all the history until this type from the request path.
        /// </summary>
        /// <param name="typeRequestInfo">The type request info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeRequestInfo"/> is <c>null</c>.</exception>
        public void MarkTypeAsNotCreated(TypeRequestInfo typeRequestInfo)
        {
            Argument.IsNotNull("typeRequestInfo", typeRequestInfo);

            for (int i = _typePath.Count - 1; i >= 0; i--)
            {
                if (_typePath[i] == typeRequestInfo)
                {
                    _typePath.RemoveRange(i, _typePath.Count - i);
                    return;
                }
            }
        }

        /// <summary>
        /// Marks the type as created and removes all the history until this type from the request path.
        /// </summary>
        /// <param name="typeRequestInfo">The type request info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeRequestInfo"/> is <c>null</c>.</exception>
        public void MarkTypeAsCreated(TypeRequestInfo typeRequestInfo)
        {
            Argument.IsNotNull("typeRequestInfo", typeRequestInfo);

            for (int i = _typePath.Count - 1; i >= 0; i--)
            {
                if (_typePath[i] == typeRequestInfo)
                {
                    _typePath.RemoveRange(i, _typePath.Count - i);
                    return;
                }
            }
        }

        /// <summary>
        /// Pops the last added type from the type path.
        /// </summary>
        /// <exception cref="InvalidOperationException">The path reaches zero types which is not allowed.</exception>
        public void PopType()
        {
            if (_typePath.Count <= 1)
            {
                string error = string.Format("This call to Pop() would result in an empty TypeRequestPath which is not allowed");
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            _typePath.RemoveAt(_typePath.Count - 1);
        }

        /// <summary>
        /// Pushes the type to the type path.
        /// </summary>
        /// <param name="typeRequestInfo">The type request info.</param>
        /// <param name="throwExceptionForDuplicateTypes">If set to <c>true</c>, this method will throw a <see cref="CircularDependencyException"/> for duplicate types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeRequestInfo"/> is <c>null</c>.</exception>
        /// <exception cref="CircularDependencyException">The type is already in the type path and <paramref name="throwExceptionForDuplicateTypes"/> is <c>true</c>.</exception>
        public void PushType(TypeRequestInfo typeRequestInfo, bool throwExceptionForDuplicateTypes)
        {
            PushType(typeRequestInfo, throwExceptionForDuplicateTypes, IgnoreValueTypes);
        }

        /// <summary>
        /// Adds the type to the type path.
        /// </summary>
        /// <param name="typeRequestInfo">The type request info.</param>
        /// <param name="throwExceptionForDuplicateTypes">If set to <c>true</c>, this method will throw a <see cref="CircularDependencyException" /> for duplicate types.</param>
        /// <param name="ignoreValueTypes">If set to <c>true</c>, value types will be ignored.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeRequestInfo" /> is <c>null</c>.</exception>
        /// <exception cref="CircularDependencyException">The type is already in the type path and <paramref name="throwExceptionForDuplicateTypes" /> is <c>true</c>.</exception>
        private void PushType(TypeRequestInfo typeRequestInfo, bool throwExceptionForDuplicateTypes, bool ignoreValueTypes)
        {
            Argument.IsNotNull("typeRequestInfo", typeRequestInfo);

            if (ignoreValueTypes)
            {
                if (typeRequestInfo.Type.IsValueTypeEx())
                {
                    return;
                }
            }

            var lastTypeRequest = _typePath.LastOrDefault();
            if (lastTypeRequest == typeRequestInfo)
            {
                Log.Debug("Requesting type {0} twice after eachother, ignoring second request", typeRequestInfo);
                return;
            }

            bool alreadyContainsType = _typePath.Contains(typeRequestInfo);

            _typePath.Add(typeRequestInfo);

            if (throwExceptionForDuplicateTypes)
            {
                if (alreadyContainsType)
                {
                    string error = string.Format("Found a circular dependency while resolving '{0}', it is used by '{1}'", FirstType, _typePath[_typePath.Count - 2]);
                    Log.Error(error);
                    throw new CircularDependencyException(this, string.Format("{0}. For more information, view the enclosed TypeRequestPath", error));
                }
            }
        }
    }
}