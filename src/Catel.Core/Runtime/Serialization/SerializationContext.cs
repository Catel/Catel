// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Catel.Reflection;
    using Catel.Scoping;

    /// <summary>
    /// The serialization context used to serialize and deserialize models.
    /// </summary>
    /// <typeparam name="TSerializationContextInfo">The type of the context.</typeparam>
    public class SerializationContext<TSerializationContextInfo> : Disposable, ISerializationContext<TSerializationContextInfo>
        where TSerializationContextInfo : class, ISerializationContextInfo
    {
        private ScopeManager<SerializationContextScope<TSerializationContextInfo>> _scopeManager;
        private int? _depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext{TContext}" /> class.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">The context.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        public SerializationContext(object model, Type modelType, TSerializationContextInfo context,
            SerializationContextMode contextMode, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", context);
            Argument.IsNotNull("configuration", configuration);

            Model = model;
            ModelType = modelType;
            ModelTypeName = modelType.GetSafeFullName(false);
            Context = context;
            ContextMode = contextMode;
            TypeStack = new Stack<Type>();
            Configuration = configuration;

            var scopeName = SerializationContextHelper.GetSerializationScopeName();
            _scopeManager = ScopeManager<SerializationContextScope<TSerializationContextInfo>>.GetScopeManager(scopeName, () => new SerializationContextScope<TSerializationContextInfo>());

            var contextScope = _scopeManager.ScopeObject;

            TypeStack = contextScope.TypeStack;
            ReferenceManager = contextScope.ReferenceManager;
            Contexts = contextScope.Contexts;

            var contexts = contextScope.Contexts;
            if (contexts.Count > 0)
            {
                Parent = contexts.Peek();
            }

            var serializationContextInfoParentSetter = context as ISerializationContextContainer;
            if (serializationContextInfoParentSetter is not null)
            {
                serializationContextInfoParentSetter.SetSerializationContext(this);
            }

            Initialize();
        }

        /// <summary>
        /// Gets or sets the model that needs serialization or deserialization.
        /// </summary>
        /// <value>The model.</value>
        /// <remarks>
        /// Only set the model if you know what you are doing. In most (99.9%), you want to serializer to take care of this.
        /// </remarks>
        public object Model { get; set; }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the name of the model type, which should be a cached version of <c>ModelType.GetSafeFullName(false);</c>.
        /// </summary>
        /// <value>The name of the model type.</value>
        public string ModelTypeName { get; private set; }

        /// <summary>
        /// Gets the depth of the current element being processed.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth
        {
            get
            {
                if (!_depth.HasValue)
                {
                    // Note: changed to STackCount, that's more reliable than ReferenceManager since instances
                    // can be re-used and this won't increase the depth
                    //_depth = ReferenceManager.Count;
                    var depth = TypeStack.Count;
                    if (depth > 0)
                    {
                        // The type itself is pushed to the typestack, so the depth is - 1
                        depth--;
                    }

                    _depth = depth;
                }

                return _depth.Value;
            }
        }

        /// <summary>
        /// Gets the context stack.
        /// </summary>
        /// <value>
        /// The contexts.
        /// </value>
        public Stack<ISerializationContext<TSerializationContextInfo>> Contexts { get; private set; }

        /// <summary>
        /// Gets the type stack inside the current scope.
        /// </summary>
        public Stack<Type> TypeStack { get; private set; }

        /// <summary>
        /// Gets the serialization configuration.
        /// </summary>
        public ISerializationConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the context mode.
        /// </summary>
        /// <value>The context mode.</value>
        public SerializationContextMode ContextMode { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public TSerializationContextInfo Context { get; private set; }

        /// <summary>
        /// Gets the parent context.
        /// </summary>
        /// <value>
        /// The parent context.
        /// </value>
        public ISerializationContext<TSerializationContextInfo> Parent { get; private set; }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        public ReferenceManager ReferenceManager { get; private set; }

        /// <summary>
        /// Gets or sets the serialization information.
        /// </summary>
        /// <value>The serialization information.</value>
        public SerializationInfo SerializationInfo { get; set; }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (_scopeManager is not null)
            {
                _scopeManager.Dispose();
                _scopeManager = null;
            }

            Uninitialize();
        }

        private void Initialize()
        {
            Contexts.Push(this);
            TypeStack.Push(ModelType);

            var serializable = Model as ISerializable;
            if (serializable is not null)
            {
                var registrationInfo = ReferenceManager.GetInfo(serializable, Context.ShouldAutoGenerateGraphIds(this));

                //// Note: we need to use the x.Tag instead of x.Instance.ContextMode here because we might be serializing a different thing
                //switch ((SerializationContextMode)x.Tag)
                switch (ContextMode)
                {
                    case SerializationContextMode.Serialization:
                        if (!registrationInfo.HasCalledSerializing)
                        {
                            serializable.StartSerialization();
                            registrationInfo.HasCalledSerializing = true;
                        }
                        break;

                    case SerializationContextMode.Deserialization:
                        if (!registrationInfo.HasCalledDeserializing)
                        {
                            serializable.StartDeserialization();
                            registrationInfo.HasCalledDeserializing = true;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Uninitialize()
        {
            var serializable = Model as ISerializable;
            if (serializable is not null)
            {
                var registrationInfo = ReferenceManager.GetInfo(serializable, Context.ShouldAutoGenerateGraphIds(this));

                //// Note: we need to use the x.Tag instead of x.Instance.ContextMode here because we might be serializing a different thing
                //switch ((SerializationContextMode)x.Tag)
                switch (ContextMode)
                {
                    case SerializationContextMode.Serialization:
                        if (!registrationInfo.HasCalledSerialized)
                        {
                            serializable.FinishSerialization();
                            registrationInfo.HasCalledSerialized = true;
                        }
                        break;

                    case SerializationContextMode.Deserialization:
                        if (!registrationInfo.HasCalledDeserialized)
                        {
                            serializable.FinishDeserialization();
                            registrationInfo.HasCalledDeserialized = true;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            TypeStack.Pop();
            Contexts.Pop();
        }
    }
}
