// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Globalization;
    using System.Reflection;

#if NET
    using System.Runtime.InteropServices;
#endif

#if !NET45 && !NETFX_CORE && !PCL

    /// <summary>
    /// The type info.
    /// </summary>
    public class TypeInfo
    {
        #region Fields
        private readonly Type _type;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfo"/> class. 
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        internal TypeInfo(Type type)
        {
            Argument.IsNotNull("type", type);

            _type = type;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the current member.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing the name of this member.
        /// </returns>
        public string Name
        {
            get { return _type.Name; }
        }

#if !PCL
        /// <summary>
        /// Gets a value that identifies a metadata element.
        /// </summary>
        /// <returns>
        /// A value which, in combination with <see cref="P:System.Reflection.MemberInfo.Module"/>, uniquely identifies a metadata element.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Reflection.MemberInfo"/> represents an array method, such 
        /// as Address, on an array type whose element type is a dynamic type that has not been completed. To get a metadata token in this case, pass 
        /// the <see cref="T:System.Reflection.MemberInfo"/> object to the <c>System.Reflection.Emit.ModuleBuilder.GetMethodToken(System.Reflection.MethodInfo)</c>
        /// method; or use the <c>System.Reflection.Emit.ModuleBuilder.GetArrayMethodToken(System.Type,System.String,System.Reflection.CallingConventions,System.Type,System.Type[])</c> 
        ///  method to get the token directly, instead of using the <c>System.Reflection.Emit.ModuleBuilder.GetArrayMethod(System.Type,System.String,System.Reflection.CallingConventions,System.Type,System.Type[])</c> 
        /// method to get a <see cref="T:System.Reflection.MethodInfo"/> first.
        /// </exception>
        public int MetadataToken
        {
            get { return _type.MetadataToken; }
        }
#endif

        /// <summary>
        /// Gets DeclaredEvents.
        /// </summary>
        public EventInfo[] DeclaredEvents
        {
            get { return GetEvents(); }
        }

#if !PCL
        /// <summary>
        /// Gets a <see cref="T:System.Reflection.MemberTypes"/> value indicating that this member is a type or a nested type.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MemberTypes"/> value indicating that this member is a type or a nested type.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public MemberTypes MemberType
        {
            get { return _type.MemberType; }
        }
#endif

        /// <summary>
        /// Gets the type that declares the current nested type or generic type parameter.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing the enclosing type, if the current type is a nested type; or the generic type definition, if the current type is a type parameter of a generic type; or the type that declares the generic method, if the current type is a type parameter of a generic method; otherwise, null.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public Type DeclaringType
        {
            get { return _type.DeclaringType; }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Reflection.MethodBase"/> that represents the declaring method, if the current <see cref="T:System.Type"/> represents a type parameter of a generic method.
        /// </summary>
        /// <returns>
        /// If the current <see cref="T:System.Type"/> represents a type parameter of a generic method, a <see cref="T:System.Reflection.MethodBase"/> that represents declaring method; otherwise, null.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public MethodBase DeclaringMethod
        {
            get { return _type.DeclaringMethod; }
        }

        /// <summary>
        /// Gets the class object that was used to obtain this member. 
        /// </summary>
        /// <returns>
        /// The Type object through which this MemberInfo object was obtained. 
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public Type ReflectedType
        {
            get { return _type.ReflectedType; }
        }

#if NET

        /// <summary>
        /// Gets a <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute"/> that describes the layout of the current type.
        /// </summary>
        /// <returns>
        /// Gets a <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute"/> that describes the gross layout features of the current type.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.
        /// </exception><filterpriority>1</filterpriority>
        public StructLayoutAttribute StructLayoutAttribute
        {
            get { return _type.StructLayoutAttribute; }
        }

#endif

#if !PCL
        /// <summary>
        /// Gets the GUID associated with the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// The GUID associated with the <see cref="T:System.Type"/>.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public Guid GUID
        {
            get { return _type.GUID; }
        }

        /// <summary>
        /// Gets the module (the DLL) in which the current <see cref="T:System.Type"/> is defined.
        /// </summary>
        /// <returns>
        /// The module in which the current <see cref="T:System.Type"/> is defined.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public Module Module
        {
            get { return _type.Module; }
        }
#endif

        /// <summary>
        /// Gets the <see cref="T:System.Reflection.Assembly"/> in which the type is declared. For generic types, gets the <see cref="T:System.Reflection.Assembly"/> in which the generic type is defined.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Reflection.Assembly"/> instance that describes the assembly containing the current type. For generic types, the instance describes the assembly that contains the generic type definition, not the assembly that creates and uses a particular constructed type.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public Assembly Assembly
        {
            get { return _type.Assembly; }
        }

        /// <summary>
        /// Gets the handle for the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// The handle for the current <see cref="T:System.Type"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The .NET Compact Framework does not currently support this property.
        /// </exception><filterpriority>1</filterpriority>
        public RuntimeTypeHandle TypeHandle
        {
            get { return _type.TypeHandle; }
        }

        /// <summary>
        /// Gets the fully qualified name of the <see cref="T:System.Type"/>, including the namespace of the <see cref="T:System.Type"/> but not the assembly.
        /// </summary>
        /// <returns>
        /// The fully qualified name of the <see cref="T:System.Type"/>, including the namespace of the <see cref="T:System.Type"/> but not the assembly; or null if the current instance represents a generic type parameter, an array type, pointer type, or byref type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public string FullName
        {
            get { return _type.FullName; }
        }

        /// <summary>
        /// Gets the namespace of the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// The namespace of the <see cref="T:System.Type"/>, or null if the current instance represents a generic parameter.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string Namespace
        {
            get { return _type.Namespace; }
        }

        /// <summary>
        /// Gets the assembly-qualified name of the <see cref="T:System.Type"/>, which includes the name of the assembly from which the <see cref="T:System.Type"/> was loaded.
        /// </summary>
        /// <returns>
        /// The assembly-qualified name of the <see cref="T:System.Type"/>, which includes the name of the assembly from which the <see cref="T:System.Type"/> was loaded, or null if the current instance represents a generic type parameter.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public string AssemblyQualifiedName
        {
            get { return _type.AssemblyQualifiedName; }
        }

        /// <summary>
        /// Gets the type from which the current <see cref="T:System.Type"/> directly inherits.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> from which the current <see cref="T:System.Type"/> directly inherits, or null if the current Type represents the <see cref="T:System.Object"/> class or an interface.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Type BaseType
        {
            get { return _type.BaseType; }
        }

#if NET

        /// <summary>
        /// Gets the initializer for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.ConstructorInfo"/> containing the name of the class constructor for the <see cref="T:System.Type"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public ConstructorInfo TypeInitializer
        {
            get { return _type.TypeInitializer; }
        }
#endif
        /// <summary>
        /// Gets a value indicating whether the current <see cref="T:System.Type"/> object represents a type whose definition is nested inside the definition of another type.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is nested inside another type; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNested
        {
            get { return _type.IsNested; }
        }

        /// <summary>
        /// Gets the attributes associated with the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.TypeAttributes"/> object representing the attribute set of the <see cref="T:System.Type"/>, unless the <see cref="T:System.Type"/> represents a generic type parameter, in which case the value is unspecified. 
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public TypeAttributes Attributes
        {
            get { return _type.Attributes; }
        }

        /// <summary>
        /// Gets a combination of <see cref="T:System.Reflection.GenericParameterAttributes"/> flags that describe the covariance and special constraints of the current generic type parameter. 
        /// </summary>
        /// <returns>
        /// A bitwise combination of <see cref="T:System.Reflection.GenericParameterAttributes"/> values that describes the covariance and special constraints of the current generic type parameter.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Type"/> object is not a generic type parameter. That is, the <see cref="P:System.Type.IsGenericParameter"/> property returns false.
        /// </exception><exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.
        /// </exception><filterpriority>1</filterpriority>
        public GenericParameterAttributes GenericParameterAttributes
        {
            get { return _type.GenericParameterAttributes; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> can be accessed by code outside the assembly.
        /// </summary>
        /// <returns>
        /// true if the current <see cref="T:System.Type"/> is a public type or a public nested type such that all the enclosing types are public; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsVisible
        {
            get { return _type.IsVisible; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is not declared public.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is not declared public and is not a nested type; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNotPublic
        {
            get { return _type.IsNotPublic; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is declared public.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is declared public and is not a nested type; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsPublic
        {
            get { return _type.IsPublic; }
        }

        /// <summary>
        /// Gets a value indicating whether a class is nested and declared public.
        /// </summary>
        /// <returns>
        /// true if the class is nested and declared public; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNestedPublic
        {
            get { return _type.IsNestedPublic; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is nested and declared private.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is nested and declared private; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNestedPrivate
        {
            get { return _type.IsNestedPrivate; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is nested and visible only within its own family.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is nested and visible only within its own family; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNestedFamily
        {
            get { return _type.IsNestedFamily; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is nested and visible only within its own assembly.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is nested and visible only within its own assembly; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNestedAssembly
        {
            get { return _type.IsNestedAssembly; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is nested and visible only to classes that belong to both its own family and its own assembly.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is nested and visible only to classes that belong to both its own family and its own assembly; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNestedFamANDAssem
        {
            get { return _type.IsNestedFamANDAssem; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is nested and visible only to classes that belong to either its own family or to its own assembly.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is nested and visible only to classes that belong to its own family or to its own assembly; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsNestedFamORAssem
        {
            get { return _type.IsNestedFamORAssem; }
        }

#if !PCL
        /// <summary>
        /// Gets a value indicating whether the class layout attribute AutoLayout is selected for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the class layout attribute AutoLayout is selected for the <see cref="T:System.Type"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsAutoLayout
        {
            get { return _type.IsAutoLayout; }
        }
#endif

#if NET 

        /// <summary>
        /// Gets a value indicating whether the class layout attribute SequentialLayout is selected for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the class layout attribute SequentialLayout is selected for the <see cref="T:System.Type"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsLayoutSequential
        {
            get { return _type.IsLayoutSequential; }
        }

        /// <summary>
        /// Gets a value indicating whether the class layout attribute ExplicitLayout is selected for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the class layout attribute ExplicitLayout is selected for the <see cref="T:System.Type"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsExplicitLayout
        {
            get { return _type.IsExplicitLayout; }
        }

#endif
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is a class; that is, not a value type or interface.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is a class; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsClass
        {
            get { return _type.IsClass; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is an interface; that is, not a class or a value type.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is an interface; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsInterface
        {
            get { return _type.IsInterface; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is a value type.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is a value type; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsValueType
        {
            get { return _type.IsValueType; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is abstract and must be overridden.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is abstract; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsAbstract
        {
            get { return _type.IsAbstract; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is declared sealed.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is declared sealed; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsSealed
        {
            get { return _type.IsSealed; }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="T:System.Type"/> represents an enumeration.
        /// </summary>
        /// <returns>
        /// true if the current <see cref="T:System.Type"/> represents an enumeration; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsEnum
        {
            get { return _type.IsEnum; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> has a name that requires special handling.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> has a name that requires special handling; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsSpecialName
        {
            get { return _type.IsSpecialName; }
        }

#if !PCL
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute"/> attribute applied, indicating that it was imported from a COM type library.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsImport
        {
            get { return _type.IsImport; }
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is serializable.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is serializable; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsSerializable
        {
            get
            {
#if !NET
                return true;
#else
                return _type.IsSerializable;
#endif
            }
        }

#if !PCL
        /// <summary>
        /// Gets a value indicating whether the string format attribute AnsiClass is selected for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the string format attribute AnsiClass is selected for the <see cref="T:System.Type"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsAnsiClass
        {
            get { return _type.IsAnsiClass; }
        }

        /// <summary>
        /// Gets a value indicating whether the string format attribute UnicodeClass is selected for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the string format attribute UnicodeClass is selected for the <see cref="T:System.Type"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsUnicodeClass
        {
            get { return _type.IsUnicodeClass; }
        }

        /// <summary>
        /// Gets a value indicating whether the string format attribute AutoClass is selected for the <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the string format attribute AutoClass is selected for the <see cref="T:System.Type"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsAutoClass
        {
            get { return _type.IsAutoClass; }
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is an array.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is an array; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsArray
        {
            get { return _type.IsArray; }
        }

        /// <summary>
        /// Gets a value indicating whether the current type is a generic type.
        /// </summary>
        /// <returns>
        /// true if the current type is a generic type; otherwise, false.
        /// </returns>
        public bool IsGenericType
        {
            get { return _type.IsGenericType; }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="T:System.Type"/> represents a generic type definition, from which other generic types can be constructed.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> object represents a generic type definition; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsGenericTypeDefinition
        {
            get { return _type.IsGenericTypeDefinition; }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="T:System.Type"/> represents a type parameter in the definition of a generic type or method.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> object represents a type parameter of a generic type definition or generic method definition; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsGenericParameter
        {
            get { return _type.IsGenericParameter; }
        }

        /// <summary>
        /// Gets the position of the type parameter in the type parameter list of the generic type or method that declared the parameter, when the <see cref="T:System.Type"/> object represents a type parameter of a generic type or a generic method.
        /// </summary>
        /// <returns>
        /// The position of a type parameter in the type parameter list of the generic type or method that defines the parameter. Position numbers begin at 0.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The current type does not represent a type parameter. That is, <see cref="P:System.Type.IsGenericParameter"/> returns false. 
        /// </exception><filterpriority>2</filterpriority>
        public int GenericParameterPosition
        {
            get { return _type.GenericParameterPosition; }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="T:System.Type"/> object has type parameters that have not been replaced by specific types.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> object is itself a generic type parameter or has type parameters for which specific types have not been supplied; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool ContainsGenericParameters
        {
            get { return _type.ContainsGenericParameters; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is passed by reference.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is passed by reference; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsByRef
        {
            get { return _type.IsByRef; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is a pointer.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is a pointer; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsPointer
        {
            get { return _type.IsPointer; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is one of the primitive types.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is one of the primitive types; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsPrimitive
        {
            get { return _type.IsPrimitive; }
        }

#if !PCL
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is a COM object.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is a COM object; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsCOMObject
        {
            get { return _type.IsCOMObject; }
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the current <see cref="T:System.Type"/> encompasses or refers to another type; that is, whether the current <see cref="T:System.Type"/> is an array, a pointer, or is passed by reference.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is an array, a pointer, or is passed by reference; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool HasElementType
        {
            get { return _type.HasElementType; }
        }

#if NET 

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> can be hosted in a context.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> can be hosted in a context; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsContextful
        {
            get { return _type.IsContextful; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Type"/> is marshaled by reference.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Type"/> is marshaled by reference; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsMarshalByRef
        {
            get { return _type.IsMarshalByRef; }
        }

#endif
        /// <summary>
        /// Indicates the type provided by the common language runtime that represents this type.
        /// </summary>
        /// <returns>
        /// The underlying system type for the <see cref="T:System.Type"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Type UnderlyingSystemType
        {
            get { return _type.UnderlyingSystemType; }
        }

        /// <summary>
        /// Gets DeclaredProperties.
        /// </summary>
        public PropertyInfo[] DeclaredProperties
        {
            get { return GetProperties(); }
        }

        /// <summary>
        /// Gets DeclaredFields.
        /// </summary>
        public FieldInfo[] DeclaredFields
        {
            get { return GetFields(); }
        }

        /// <summary>
        /// Gets GenericTypeArguments
        /// </summary>
        public Type[] GenericTypeArguments { get { return this.GetGenericArguments(); } }

        /// <summary>
        /// Gets ImplementedInterfaces
        /// </summary>
        public Type[] ImplementedInterfaces
        {
            get { return this.GetInterfaces(); }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <returns></returns>
        public Type AsType()
        {
            return _type;
        }

        /// <summary>
        /// When overridden in a derived class, returns an array containing all the custom attributes.
        /// </summary>
        /// <returns>
        /// An array that contains all the custom attributes, or an array with zero elements if no attributes are defined.
        /// </returns>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes. 
        /// </param>
        /// <exception cref="T:System.InvalidOperationException">
        /// This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.
        /// </exception>
        /// <exception cref="T:System.TypeLoadException">
        /// A custom attribute type cannot be loaded. 
        /// </exception>
        public object[] GetCustomAttributes(bool inherit)
        {
            return _type.GetCustomAttributes(inherit);
        }

        /// <summary>
        /// When overridden in a derived class, returns an array of custom attributes identified by <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of custom attributes applied to this member, or an array with zero (0) elements if no attributes have been applied.
        /// </returns>
        /// <param name="attributeType">
        /// The type of attribute to search for. Only attributes that are assignable to this type are returned. 
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes. 
        /// </param>
        /// <exception cref="T:System.TypeLoadException">
        /// A custom attribute type cannot be loaded. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// If <paramref name="attributeType"/> is null.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.
        /// </exception>
        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _type.GetCustomAttributes(attributeType, inherit);
        }

        /// <summary>
        /// When overridden in a derived class, indicates whether one or more instance of <paramref name="attributeType"/> is applied to this member.
        /// </summary>
        /// <returns>
        /// true if one or more instance of <paramref name="attributeType"/> is applied to this member; otherwise false.
        /// </returns>
        /// <param name="attributeType">
        /// The Type object to which the custom attributes are applied. 
        /// </param>
        /// <param name="inherit">
        /// Specifies whether to search this member's inheritance chain to find the attributes. 
        /// </param>
        public bool IsDefined(Type attributeType, bool inherit)
        {
            return _type.IsDefined(attributeType, inherit);
        }

#if NET && !NET45

        /// <summary>
        /// Retrieves the number of type information interfaces that an object provides (either 0 or 1).
        /// </summary>
        /// <param name="pcTInfo">
        /// Points to a location that receives the number of type information interfaces provided by the object.
        /// </param>
        [CLSCompliant(false)]
        public void GetTypeInfoCount(out uint pcTInfo)
        {
            ((_Type) _type).GetTypeInfoCount(out pcTInfo);
        }

        /// <summary>
        /// Retrieves the type information for an object, which can then be used to get the type information for an interface.
        /// </summary>
        /// <param name="iTInfo">
        /// The type information to return.
        /// </param>
        /// <param name="lcid">
        /// The locale identifier for the type information.
        /// </param>
        /// <param name="ppTInfo">
        /// Receives a pointer to the requested type information object.
        /// </param>
        [CLSCompliant(false)]
        public void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            ((_Type) _type).GetTypeInfo(iTInfo, lcid, ppTInfo);
        }

        /// <summary>
        /// Maps a set of names to a corresponding set of dispatch identifiers.
        /// </summary>
        /// <param name="riid">
        /// Reserved for future use. Must be IID_NULL.
        /// </param>
        /// <param name="rgszNames">
        /// Passed-in array of names to be mapped.
        /// </param>
        /// <param name="cNames">
        /// Count of the names to be mapped.
        /// </param>
        /// <param name="lcid">
        /// The locale context in which to interpret the names.
        /// </param>
        /// <param name="rgDispId">
        /// Caller-allocated array that receives the IDs corresponding to the names.
        /// </param>
        [CLSCompliant(false)]
        public void GetIDsOfNames(ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            ((_Type) _type).GetIDsOfNames(ref riid, rgszNames, cNames, lcid, rgDispId);
        }

        /// <summary>
        /// Provides access to properties and methods exposed by an object.
        /// </summary>
        /// <param name="dispIdMember">
        /// Identifies the member.
        /// </param>
        /// <param name="riid">
        /// Reserved for future use. Must be IID_NULL.
        /// </param>
        /// <param name="lcid">
        /// The locale context in which to interpret arguments.
        /// </param>
        /// <param name="wFlags">
        /// Flags describing the context of the call.
        /// </param>
        /// <param name="pDispParams">
        /// Pointer to a structure containing an array of arguments, an array of argument DISPIDs for named arguments, and counts for the number of elements in the arrays.
        /// </param>
        /// <param name="pVarResult">
        /// Pointer to the location where the result is to be stored.
        /// </param>
        /// <param name="pExcepInfo">
        /// Pointer to a structure that contains exception information.
        /// </param>
        /// <param name="puArgErr">
        /// The index of the first argument that has an error.
        /// </param>
        [CLSCompliant(false)]
        public void Invoke(uint dispIdMember, ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            ((_Type) _type).Invoke(dispIdMember, ref riid, lcid, wFlags, pDispParams, pVarResult, pExcepInfo, puArgErr);
        }
#endif
        /// <summary>
        /// Returns a <see cref="T:System.Type"/> object that represents a pointer to the current type.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object that represents a pointer to the current type.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The invoked method is not supported in the base class.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type MakePointerType()
        {
            return _type.MakePointerType();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Type"/> object that represents the current type when passed as a ref parameter (ByRef parameter in Visual Basic).
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object that represents the current type when passed as a ref parameter (ByRef parameter in Visual Basic).
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The invoked method is not supported in the base class.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type MakeByRefType()
        {
            return _type.MakeByRefType();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Type"/> object representing a one-dimensional array of the current type, with a lower bound of zero.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing a one-dimensional array of the current type, with a lower bound of zero.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Type MakeArrayType()
        {
            return _type.MakeArrayType();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Type"/> object representing an array of the current type, with the specified number of dimensions.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing an array of the current type, with the specified number of dimensions.
        /// </returns>
        /// <param name="rank">
        /// The number of dimensions for the array. 
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// <paramref name="rank"/> is invalid. For example, 0 or negative.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The invoked method is not supported in the base class.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type MakeArrayType(int rank)
        {
            return _type.MakeArrayType(rank);
        }

#if !PCL
        /// <summary>
        /// When overridden in a derived class, invokes the specified member, using the specified binding constraints and matching the specified argument list, modifiers and culture.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> representing the return value of the invoked member.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the constructor, method, property, or field member to invoke.
        /// -or- 
        /// An empty string ("") to invoke the default member. 
        /// -or-
        /// For IDispatch members, a string representing the DispID, for example "[DispID=3]".
        /// </param>
        /// <param name="invokeAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted. The access can be one of the BindingFlags such as Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup need not be specified. If the type of lookup is omitted, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static are used. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. Note that explicitly defining a <see cref="T:System.Reflection.Binder"/> object may be requird for successfully invoking method overloads with variable arguments.
        /// </param>
        /// <param name="target">
        /// The <see cref="T:System.Object"/> on which to invoke the specified member. 
        /// </param>
        /// <param name="args">
        /// An array containing the arguments to pass to the member to invoke. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="args"/> array. A parameter's associated attributes are stored in the member's signature. 
        /// The default binder processes this parameter only when calling a COM component. 
        /// </param>
        /// <param name="culture">
        /// The <see cref="T:System.Globalization.CultureInfo"/> object representing the globalization locale to use, which may be necessary for locale-specific conversions, such as converting a numeric String to a Double.
        /// -or- 
        /// null to use the current thread's <see cref="T:System.Globalization.CultureInfo"/>. 
        /// </param>
        /// <param name="namedParameters">
        /// An array containing the names of the parameters to which the values in the <paramref name="args"/> array are passed. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="invokeAttr"/> contains CreateInstance and <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="args"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional.
        /// -or- 
        /// <paramref name="args"/> and <paramref name="modifiers"/> do not have the same length.
        /// -or- 
        /// <paramref name="invokeAttr"/> is not a valid <see cref="T:System.Reflection.BindingFlags"/> attribute.
        /// -or- 
        /// <paramref name="invokeAttr"/> does not contain one of the following binding flags: InvokeMethod, CreateInstance, GetField, SetField, GetProperty, or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains CreateInstance combined with InvokeMethod, GetField, SetField, GetProperty, or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains both GetField and SetField.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains both GetProperty and SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains InvokeMethod combined with SetField or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains SetField and <paramref name="args"/> has more than one element.
        /// -or- 
        /// The named parameter array is larger than the argument array.
        /// -or- 
        /// This method is called on a COM object and one of the following binding flags was not passed in: BindingFlags.InvokeMethod, BindingFlags.GetProperty, BindingFlags.SetProperty, BindingFlags.PutDispProperty, or BindingFlags.PutRefDispProperty.
        /// -or- 
        /// One of the named parameter arrays contains a string that is null. 
        /// </exception>
        /// <exception cref="T:System.MethodAccessException">
        /// The specified member is a class initializer. 
        /// </exception>
        /// <exception cref="T:System.MissingFieldException">
        /// The field or property cannot be found. 
        /// </exception>
        /// <exception cref="T:System.MissingMethodException">
        /// The method cannot be found.
        /// -or- 
        /// The current <see cref="T:System.Type"/> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters"/> returns true. 
        /// </exception>
        /// <exception cref="T:System.Reflection.TargetException">
        /// The specified member cannot be invoked on <paramref name="target"/>. 
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method matches the binding criteria. 
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The method represented by <paramref name="name"/> has one or more unspecified generic type parameters. That is, the method's <c>ContainsGenericParameters</c> property returns true.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            return _type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }
#endif

#if NET 

        /// <summary>
        /// Invokes the specified member, using the specified binding constraints and matching the specified argument list and culture.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> representing the return value of the invoked member.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the constructor, method, property, or field member to invoke.
        /// -or- 
        /// An empty string ("") to invoke the default member. 
        /// -or-
        /// For IDispatch members, a string representing the DispID, for example "[DispID=3]".
        /// </param>
        /// <param name="invokeAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted. The access can be one of the BindingFlags such as Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup need not be specified. If the type of lookup is omitted, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static are used. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. Note that explicitly defining a <see cref="T:System.Reflection.Binder"/> object may be requird for successfully invoking method overloads with variable arguments.
        /// </param>
        /// <param name="target">
        /// The <see cref="T:System.Object"/> on which to invoke the specified member. 
        /// </param>
        /// <param name="args">
        /// An array containing the arguments to pass to the member to invoke. 
        /// </param>
        /// <param name="culture">
        /// The <see cref="T:System.Globalization.CultureInfo"/> object representing the globalization locale to use, which may be necessary for locale-specific conversions, such as converting a numeric String to a Double.
        /// -or- 
        /// null to use the current thread's <see cref="T:System.Globalization.CultureInfo"/>. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="invokeAttr"/> contains CreateInstance and <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="args"/> is multidimensional.
        /// -or- 
        /// <paramref name="invokeAttr"/> is not a valid <see cref="T:System.Reflection.BindingFlags"/> attribute. 
        /// -or- 
        /// <paramref name="invokeAttr"/> does not contain one of the following binding flags: InvokeMethod, CreateInstance, GetField, SetField, GetProperty, or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains CreateInstance combined with InvokeMethod, GetField, SetField, GetProperty, or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains both GetField and SetField.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains both GetProperty and SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains InvokeMethod combined with SetField or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains SetField and <paramref name="args"/> has more than one element.
        /// -or- 
        /// This method is called on a COM object and one of the following binding flags was not passed in: BindingFlags.InvokeMethod, BindingFlags.GetProperty, BindingFlags.SetProperty, BindingFlags.PutDispProperty, or BindingFlags.PutRefDispProperty.
        /// -or- 
        /// One of the named parameter arrays contains a string that is null. 
        /// </exception>
        /// <exception cref="T:System.MethodAccessException">
        /// The specified member is a class initializer. 
        /// </exception>
        /// <exception cref="T:System.MissingFieldException">
        /// The field or property cannot be found. 
        /// </exception>
        /// <exception cref="T:System.MissingMethodException">
        /// The method cannot be found.
        /// -or- 
        /// The current <see cref="T:System.Type"/> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters"/> returns true. 
        /// </exception>
        /// <exception cref="T:System.Reflection.TargetException">
        /// The specified member cannot be invoked on <paramref name="target"/>. 
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method matches the binding criteria. 
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The method represented by <paramref name="name"/> has one or more unspecified generic type parameters. That is, the method's <c>ContainsGenericParameters</c> property returns true.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
        {
            return _type.InvokeMember(name, invokeAttr, binder, target, args, culture);
        }
#endif

#if !PCL
        /// <summary>
        /// Invokes the specified member, using the specified binding constraints and matching the specified argument list.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> representing the return value of the invoked member.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the constructor, method, property, or field member to invoke.
        /// -or- 
        /// An empty string ("") to invoke the default member. 
        /// -or-
        /// For IDispatch members, a string representing the DispID, for example "[DispID=3]".
        /// </param>
        /// <param name="invokeAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted. The access can be one of the BindingFlags such as Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup need not be specified. If the type of lookup is omitted, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static are used. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. Note that explicitly defining a <see cref="T:System.Reflection.Binder"/> object may be requird for successfully invoking method overloads with variable arguments.
        /// </param>
        /// <param name="target">
        /// The <see cref="T:System.Object"/> on which to invoke the specified member. 
        /// </param>
        /// <param name="args">
        /// An array containing the arguments to pass to the member to invoke. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="invokeAttr"/> contains CreateInstance and <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="args"/> is multidimensional.
        /// -or- 
        /// <paramref name="invokeAttr"/> is not a valid <see cref="T:System.Reflection.BindingFlags"/> attribute. 
        /// -or- 
        /// <paramref name="invokeAttr"/> does not contain one of the following binding flags: InvokeMethod, CreateInstance, GetField, SetField, GetProperty, or SetProperty. 
        /// -or- 
        /// <paramref name="invokeAttr"/> contains CreateInstance combined with InvokeMethod, GetField, SetField, GetProperty, or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains both GetField and SetField.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains both GetProperty and SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains InvokeMethod combined with SetField or SetProperty.
        /// -or- 
        /// <paramref name="invokeAttr"/> contains SetField and <paramref name="args"/> has more than one element.
        /// -or- 
        /// This method is called on a COM object and one of the following binding flags was not passed in: BindingFlags.InvokeMethod, BindingFlags.GetProperty, BindingFlags.SetProperty, BindingFlags.PutDispProperty, or BindingFlags.PutRefDispProperty.
        /// -or- 
        /// One of the named parameter arrays contains a string that is null. 
        /// </exception>
        /// <exception cref="T:System.MethodAccessException">
        /// The specified member is a class initializer. 
        /// </exception>
        /// <exception cref="T:System.MissingFieldException">
        /// The field or property cannot be found. 
        /// </exception>
        /// <exception cref="T:System.MissingMethodException">
        /// The method cannot be found.
        /// -or- 
        /// The current <see cref="T:System.Type"/> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters"/> returns true. 
        /// </exception>
        /// <exception cref="T:System.Reflection.TargetException">
        /// The specified member cannot be invoked on <paramref name="target"/>. 
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method matches the binding criteria. 
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The .NET Compact Framework does not currently support this method.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The method represented by <paramref name="name"/> has one or more unspecified generic type parameters. That is, the method's <c>ContainsGenericParameters</c> property returns true.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
        {
            return _type.InvokeMember(name, invokeAttr, binder, target, args);
        }
#endif

        /// <summary>
        /// Gets the number of dimensions in an <see cref="T:System.Array"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Int32"/> containing the number of dimensions in the current Type.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The functionality of this method is unsupported in the base class and must be implemented in a derived class instead. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The current Type is not an array. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int GetArrayRank()
        {
            return _type.GetArrayRank();
        }

#if NET 
        /// <summary>
        /// Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.ConstructorInfo"/> object representing the constructor that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. 
        /// </param>
        /// <param name="callConvention">
        /// The <see cref="T:System.Reflection.CallingConventions"/> object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and the stack is cleaned up. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the constructor to get.
        /// -or- 
        /// An empty array of the type <see cref="T:System.Type"/> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="types"/> array. The default binder does not process this parameter. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional.
        /// -or- 
        /// <paramref name="types"/> and <paramref name="modifiers"/> do not have the same length. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }
#endif

#if !PCL
        /// <summary>
        /// Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.ConstructorInfo"/> object representing the constructor that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the constructor to get.
        /// -or- 
        /// An empty array of the type <see cref="T:System.Type"/> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.
        /// -or- 
        /// <see cref="F:System.Type.EmptyTypes"/>. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the parameter type array. The default binder does not process this parameter. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional.
        /// -or- 
        /// <paramref name="types"/> and <paramref name="modifiers"/> do not have the same length. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetConstructor(bindingAttr, binder, types, modifiers);
        }
#endif

        /// <summary>
        /// Searches for a public instance constructor whose parameters match the types in the specified array.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.ConstructorInfo"/> object representing the public instance constructor whose parameters match the types in the parameter type array, if found; otherwise, null.
        /// </returns>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the desired constructor.
        /// -or- 
        /// An empty array of <see cref="T:System.Type"/> objects, to get a constructor that takes no parameters. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public ConstructorInfo GetConstructor(Type[] types)
        {
            return _type.GetConstructor(types);
        }

        /// <summary>
        /// Returns all the public constructors defined for the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.ConstructorInfo"/> objects representing all the public instance constructors defined for the current <see cref="T:System.Type"/>, but not including the type initializer (static constructor). If no public instance constructors are defined for the current <see cref="T:System.Type"/>, or if the current <see cref="T:System.Type"/> represents a type parameter in the definition of a generic type or generic method, an empty array of type <see cref="T:System.Reflection.ConstructorInfo"/> is returned.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public ConstructorInfo[] GetConstructors()
        {
            return _type.GetConstructors();
        }

        /// <summary>
        /// When overridden in a derived class, searches for the constructors defined for the current <see cref="T:System.Type"/>, using the specified BindingFlags.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.ConstructorInfo"/> objects representing all constructors defined for the current <see cref="T:System.Type"/> that match the specified binding constraints, including the type initializer if it is defined. Returns an empty array of type <see cref="T:System.Reflection.ConstructorInfo"/> if no constructors are defined for the current <see cref="T:System.Type"/>, if none of the defined constructors match the binding constraints, or if the current <see cref="T:System.Type"/> represents a type parameter in the definition of a generic type or generic method.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return _type.GetConstructors(bindingAttr);
        }

#if !PCL
        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the method that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the method to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. 
        /// </param>
        /// <param name="callConvention">
        /// The <see cref="T:System.Reflection.CallingConventions"/> object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and how the stack is cleaned up. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to get.
        /// -or- 
        /// An empty array of <see cref="T:System.Type"/> objects (as provided by the <see cref="F:System.Type.EmptyTypes"/> field) to get a method that takes no parameters. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="types"/> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name and matching the specified binding constraints. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
        }

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the method that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the method to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to get.
        /// -or- 
        /// An empty array of <see cref="T:System.Type"/> objects (as provided by the <see cref="F:System.Type.EmptyTypes"/> field) to get a method that takes no parameters. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="types"/> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name and matching the specified binding constraints. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the method that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        ///   The <see cref="T:System.String"/> containing the name of the method to get. 
        /// </param>
        /// <param name="types">
        ///   An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to get.
        ///   -or- 
        ///   An empty array of <see cref="T:System.Type"/> objects (as provided by the <see cref="F:System.Type.EmptyTypes"/> field) to get a method that takes no parameters. 
        /// </param>
        /// <param name="bindingAttr">
        ///   A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        ///   -or- 
        ///   Zero, to return null. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name and matching the specified binding constraints. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name, Type[] types, BindingFlags bindingAttr)
        {
            return _type.GetMethod(name, bindingAttr, null, types, null);
        }

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified argument types and modifiers.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the public method that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public method to get. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to get.
        /// -or- 
        /// An empty array of <see cref="T:System.Type"/> objects (as provided by the <see cref="F:System.Type.EmptyTypes"/> field) to get a method that takes no parameters. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="types"/> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.  
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name and specified parameters. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetMethod(name, types, modifiers);
        }
#endif

        /// <summary>
        /// Searches for the specified public method whose parameters match the specified argument types.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the public method whose parameters match the specified argument types, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public method to get. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to get.
        /// -or- 
        /// An empty array of <see cref="T:System.Type"/> objects to get a method that takes no parameters. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name and specified parameters. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null.
        /// -or- 
        /// One of the elements in <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name, Type[] types)
        {
            return _type.GetMethod(name, types);
        }

        /// <summary>
        /// Searches for the specified method, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the method that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the method to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name and matching the specified binding constraints. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
        {
            return _type.GetMethod(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the public method with the specified name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.MethodInfo"/> object representing the public method with the specified name, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public method to get. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one method is found with the specified name. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo GetMethod(string name)
        {
            return _type.GetMethod(name);
        }

        /// <summary>
        /// Returns all the public methods of the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MethodInfo"/> objects representing all the public methods defined for the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.MethodInfo"/>, if no public methods are defined for the current <see cref="T:System.Type"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public MethodInfo[] GetMethods()
        {
            return _type.GetMethods();
        }

        /// <summary>
        /// When overridden in a derived class, searches for the methods defined for the current <see cref="T:System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MethodInfo"/> objects representing all methods defined for the current <see cref="T:System.Type"/> that match the specified binding constraints.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.MethodInfo"/>, if no methods are defined for the current <see cref="T:System.Type"/>, or if none of the defined methods match the binding constraints.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return _type.GetMethods(bindingAttr);
        }

        /// <summary>
        /// Searches for the specified field, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.FieldInfo"/> object representing the field that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the data field to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _type.GetField(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the public field with the specified name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.FieldInfo"/> object representing the public field with the specified name, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the data field to get. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// This <see cref="T:System.Type"/> object is a <c>System.Reflection.Emit.TypeBuilder</c> whose <c>System.Reflection.Emit.TypeBuilder.CreateType</c> method has not yet been called. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public FieldInfo GetField(string name)
        {
            return _type.GetField(name);
        }

        /// <summary>
        /// Returns all the public fields of the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.FieldInfo"/> objects representing all the public fields defined for the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.FieldInfo"/>, if no public fields are defined for the current <see cref="T:System.Type"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public FieldInfo[] GetFields()
        {
            return _type.GetFields();
        }

        /// <summary>
        /// When overridden in a derived class, searches for the fields defined for the current <see cref="T:System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.FieldInfo"/> objects representing all fields defined for the current <see cref="T:System.Type"/> that match the specified binding constraints.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.FieldInfo"/>, if no fields are defined for the current <see cref="T:System.Type"/>, or if none of the defined fields match the binding constraints.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return _type.GetFields(bindingAttr);
        }

#if !PCL
        /// <summary>
        /// Searches for the interface with the specified name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing the interface with the specified name, implemented or inherited by the current <see cref="T:System.Type"/>, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the interface to get. For generic interfaces, this is the mangled name.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// The current <see cref="T:System.Type"/> represents a type that implements the same generic interface with different type arguments. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetInterface(string name)
        {
            return _type.GetInterface(name, false);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the specified interface, specifying whether to do a case-insensitive search for the interface name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing the interface with the specified name, implemented or inherited by the current <see cref="T:System.Type"/>, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the interface to get. For generic interfaces, this is the mangled name.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore the case of that part of <paramref name="name"/> that specifies the simple interface name (the part that specifies the namespace must be correctly cased).
        /// -or- 
        /// false to perform a case-sensitive search for all parts of <paramref name="name"/>. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// The current <see cref="T:System.Type"/> represents a type that implements the same generic interface with different type arguments. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetInterface(string name, bool ignoreCase)
        {
            return _type.GetInterface(name, ignoreCase);
        }
#endif

        /// <summary>
        /// When overridden in a derived class, gets all the interfaces implemented or inherited by the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Type"/> objects representing all the interfaces implemented or inherited by the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Type"/>, if no interfaces are implemented or inherited by the current <see cref="T:System.Type"/>.
        /// </returns>
        /// <exception cref="T:System.Reflection.TargetInvocationException">
        /// A static initializer is invoked and throws an exception. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type[] GetInterfaces()
        {
            return _type.GetInterfaces();
        }

#if NET        
        /// <summary>
        /// Returns an array of <see cref="T:System.Type"/> objects representing a filtered list of interfaces implemented or inherited by the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Type"/> objects representing a filtered list of the interfaces implemented or inherited by the current <see cref="T:System.Type"/>, or an empty array of type <see cref="T:System.Type"/> if no interfaces matching the filter are implemented or inherited by the current <see cref="T:System.Type"/>.
        /// </returns>
        /// <param name="filter">
        /// The <see cref="T:System.Reflection.TypeFilter"/> delegate that compares the interfaces against <paramref name="filterCriteria"/>. 
        /// </param>
        /// <param name="filterCriteria">
        /// The search criteria that determines whether an interface should be included in the returned array. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="filter"/> is null. 
        /// </exception>
        /// <exception cref="T:System.Reflection.TargetInvocationException">
        /// A static initializer is invoked and throws an exception. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
        {
            return _type.FindInterfaces(filter, filterCriteria);
        }
#endif
        /// <summary>
        /// Returns the <see cref="T:System.Reflection.EventInfo"/> object representing the specified public event.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Reflection.EventInfo"/> object representing the specified public event which is declared or inherited by the current <see cref="T:System.Type"/>, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of an event which is declared or inherited by the current <see cref="T:System.Type"/>. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public EventInfo GetEvent(string name)
        {
            return _type.GetEvent(name);
        }

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="T:System.Reflection.EventInfo"/> object representing the specified event, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Reflection.EventInfo"/> object representing the specified event which is declared or inherited by the current <see cref="T:System.Type"/>, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of an event which is declared or inherited by the current <see cref="T:System.Type"/>. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return _type.GetEvent(name, bindingAttr);
        }

        /// <summary>
        /// Returns all the public events that are declared or inherited by the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.EventInfo"/> objects representing all the public events which are declared or inherited by the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.EventInfo"/>, if the current <see cref="T:System.Type"/> does not have public events.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public EventInfo[] GetEvents()
        {
            return _type.GetEvents();
        }

        /// <summary>
        /// When overridden in a derived class, searches for events that are declared or inherited by the current <see cref="T:System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.EventInfo"/> objects representing all events which are declared or inherited by the current <see cref="T:System.Type"/> that match the specified binding constraints.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.EventInfo"/>, if the current <see cref="T:System.Type"/> does not have events, or if none of the events match the binding constraints.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return _type.GetEvents(bindingAttr);
        }

#if !PCL
        /// <summary>
        /// Searches for the specified property whose parameters match the specified argument types and modifiers, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the property that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the property to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <param name="binder">
        /// A <see cref="T:System.Reflection.Binder"/> object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.
        /// -or- 
        /// null, to use the <see cref="P:System.Type.DefaultBinder"/>. 
        /// </param>
        /// <param name="returnType">
        /// The return type of the property. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property to get.
        /// -or- 
        /// An empty array of the type <see cref="T:System.Type"/> (that is, Type[] types = new Type[0]) to get a property that is not indexed. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="types"/> array. The default binder does not process this parameter. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name and matching the specified binding constraints. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional.
        /// -or- 
        /// <paramref name="types"/> and <paramref name="modifiers"/> do not have the same length. 
        /// </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// An element of <paramref name="types"/> is null.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        /// <summary>
        /// Searches for the specified public property whose parameters match the specified argument types and modifiers.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the public property that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public property to get. 
        /// </param>
        /// <param name="returnType">
        /// The return type of the property. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property to get.
        /// -or- 
        /// An empty array of the type <see cref="T:System.Type"/> (that is, Type[] types = new Type[0]) to get a property that is not indexed. 
        /// </param>
        /// <param name="modifiers">
        /// An array of <see cref="T:System.Reflection.ParameterModifier"/> objects representing the attributes associated with the corresponding element in the <paramref name="types"/> array. The default binder does not process this parameter. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name and matching the specified argument types and modifiers. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional.
        /// -or- 
        /// <paramref name="modifiers"/> is multidimensional.
        /// -or- 
        /// <paramref name="types"/> and <paramref name="modifiers"/> do not have the same length. 
        /// </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// An element of <paramref name="types"/> is null.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return _type.GetProperty(name, returnType, types, modifiers);
        }
#endif

        /// <summary>
        /// Searches for the specified property, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the property that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the property to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name and matching the specified binding constraints. See Remarks.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
        {
            return _type.GetProperty(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the specified public property whose parameters match the specified argument types.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the public property whose parameters match the specified argument types, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public property to get. 
        /// </param>
        /// <param name="returnType">
        /// The return type of the property. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property to get.
        /// -or- 
        /// An empty array of the type <see cref="T:System.Type"/> (that is, Type[] types = new Type[0]) to get a property that is not indexed. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name and matching the specified argument types. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional. 
        /// </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// An element of <paramref name="types"/> is null.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
        {
            return _type.GetProperty(name, returnType, types);
        }

#if NET

        /// <summary>
        /// Searches for the specified public property whose parameters match the specified argument types.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the public property whose parameters match the specified argument types, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public property to get. 
        /// </param>
        /// <param name="types">
        /// An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property to get.
        /// -or- 
        /// An empty array of the type <see cref="T:System.Type"/> (that is, Type[] types = new Type[0]) to get a property that is not indexed. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name and matching the specified argument types. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// -or- 
        /// <paramref name="types"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="types"/> is multidimensional. 
        /// </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// An element of <paramref name="types"/> is null.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name, Type[] types)
        {
            return _type.GetProperty(name, types);
        }
#endif
        /// <summary>
        /// Searches for the public property with the specified name and return type.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the public property with the specified name, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public property to get. 
        /// </param>
        /// <param name="returnType">
        /// The return type of the property. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null, or <paramref name="returnType"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name, Type returnType)
        {
            return _type.GetProperty(name, returnType);
        }

        /// <summary>
        /// Searches for the public property with the specified name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Reflection.PropertyInfo"/> object representing the public property with the specified name, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public property to get. 
        /// </param>
        /// <exception cref="T:System.Reflection.AmbiguousMatchException">
        /// More than one property is found with the specified name. See Remarks.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo GetProperty(string name)
        {
            return _type.GetProperty(name);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the properties of the current <see cref="T:System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.PropertyInfo"/> objects representing all properties of the current <see cref="T:System.Type"/> that match the specified binding constraints.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.PropertyInfo"/>, if the current <see cref="T:System.Type"/> does not have properties, or if none of the properties match the binding constraints.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return _type.GetProperties(bindingAttr);
        }

        /// <summary>
        /// Returns all the public properties of the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.PropertyInfo"/> objects representing all public properties of the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.PropertyInfo"/>, if the current <see cref="T:System.Type"/> does not have public properties.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public PropertyInfo[] GetProperties()
        {
            return _type.GetProperties();
        }

        /// <summary>
        /// Returns the public types nested in the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Type"/> objects representing the public types nested in the current <see cref="T:System.Type"/> (the search is not recursive), or an empty array of type <see cref="T:System.Type"/> if no public types are nested in the current <see cref="T:System.Type"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Type[] GetNestedTypes()
        {
            return _type.GetNestedTypes(BindingFlagsHelper.DefaultBindingFlags);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the types nested in the current <see cref="T:System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Type"/> objects representing all the types nested in the current <see cref="T:System.Type"/> that match the specified binding constraints (the search is not recursive), or an empty array of type <see cref="T:System.Type"/>, if no nested types are found that match the binding constraints.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return _type.GetNestedTypes(bindingAttr);
        }

        /// <summary>
        /// Searches for the public nested type with the specified name.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing the public nested type with the specified name, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The string containing the name of the nested type to get. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetNestedType(string name)
        {
            return _type.GetNestedType(name, BindingFlagsHelper.DefaultBindingFlags);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the specified nested type, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing the nested type that matches the specified requirements, if found; otherwise, null.
        /// </returns>
        /// <param name="name">
        /// The string containing the name of the nested type to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return _type.GetNestedType(name, bindingAttr);
        }

        /// <summary>
        /// Searches for the public members with the specified name.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MemberInfo"/> objects representing the public members with the specified name, if found; otherwise, an empty array.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the public members to get. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] GetMember(string name)
        {
            return _type.GetMember(name);
        }

        /// <summary>
        /// Searches for the specified members, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MemberInfo"/> objects representing the public members with the specified name, if found; otherwise, an empty array.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the members to get. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return an empty array. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            return _type.GetMember(name, bindingAttr);
        }

#if !PCL
        /// <summary>
        /// Searches for the specified members of the specified member type, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MemberInfo"/> objects representing the public members with the specified name, if found; otherwise, an empty array.
        /// </returns>
        /// <param name="name">
        /// The <see cref="T:System.String"/> containing the name of the members to get. 
        /// </param>
        /// <param name="type">
        /// The <see cref="T:System.Reflection.MemberTypes"/> value to search for. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return an empty array. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="name"/> is null. 
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// A derived class must provide an implementation. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            return _type.GetMember(name, type, bindingAttr);
        }
#endif

        /// <summary>
        /// Returns all the public members of the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MemberInfo"/> objects representing all the public members of the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.MemberInfo"/>, if the current <see cref="T:System.Type"/> does not have public members.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] GetMembers()
        {
            return _type.GetMembers();
        }

        /// <summary>
        /// When overridden in a derived class, searches for the members defined for the current <see cref="T:System.Type"/>, using the specified binding constraints.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MemberInfo"/> objects representing all members defined for the current <see cref="T:System.Type"/> that match the specified binding constraints.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.MemberInfo"/>, if no members are defined for the current <see cref="T:System.Type"/>, or if none of the defined members match the binding constraints.
        /// </returns>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return _type.GetMembers(bindingAttr);
        }

        /// <summary>
        /// Searches for the members defined for the current <see cref="T:System.Type"/> whose <see cref="T:System.Reflection.DefaultMemberAttribute"/> is set.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Reflection.MemberInfo"/> objects representing all default members of the current <see cref="T:System.Type"/>.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.MemberInfo"/>, if the current <see cref="T:System.Type"/> does not have default members.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] GetDefaultMembers()
        {
            return _type.GetDefaultMembers();
        }

#if !PCL
        /// <summary>
        /// Returns a filtered array of <see cref="T:System.Reflection.MemberInfo"/> objects of the specified member type.
        /// </summary>
        /// <returns>
        /// A filtered array of <see cref="T:System.Reflection.MemberInfo"/> objects of the specified member type.
        /// -or- 
        /// An empty array of type <see cref="T:System.Reflection.MemberInfo"/>, if the current <see cref="T:System.Type"/> does not have members of type <paramref name="memberType"/> that match the filter criteria.
        /// </returns>
        /// <param name="memberType">
        /// A MemberTypes object indicating the type of member to search for. 
        /// </param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags"/> that specify how the search is conducted.
        /// -or- 
        /// Zero, to return null. 
        /// </param>
        /// <param name="filter">
        /// The delegate that does the comparisons, returning true if the member currently being inspected matches the <paramref name="filterCriteria"/> and false otherwise. You can use the FilterAttribute, FilterName, and FilterNameIgnoreCase delegates supplied by this class. The first uses the fields of FieldAttributes, MethodAttributes, and MethodImplAttributes as search criteria, and the other two delegates use String objects as the search criteria. 
        /// </param>
        /// <param name="filterCriteria">
        /// The search criteria that determines whether a member is returned in the array of MemberInfo objects.
        /// The fields of FieldAttributes, MethodAttributes, and MethodImplAttributes can be used in conjunction with the FilterAttribute delegate supplied by this class. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="filter"/> is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
        {
            return _type.FindMembers(memberType, bindingAttr, filter, filterCriteria);
        }
#endif

        /// <summary>
        /// Returns an array of <see cref="T:System.Type"/> objects that represent the constraints on the current generic type parameter. 
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Type"/> objects that represent the constraints on the current generic type parameter.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The current <see cref="T:System.Type"/> object is not a generic type parameter. That is, the <see cref="P:System.Type.IsGenericParameter"/> property returns false.
        /// </exception>
        /// <filterpriority>1</filterpriority>
        public Type[] GetGenericParameterConstraints()
        {
            return _type.GetGenericParameterConstraints();
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type"/> object representing the resulting constructed type.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> representing the constructed type formed by substituting the elements of <paramref name="typeArguments"/> for the type parameters of the current generic type.
        /// </returns>
        /// <param name="typeArguments">
        /// An array of types to be substituted for the type parameters of the current generic type.
        /// </param>
        /// <exception cref="T:System.InvalidOperationException">
        /// The current type does not represent a generic type definition. That is, <see cref="P:System.Type.IsGenericTypeDefinition"/> returns false. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="typeArguments"/> is null.
        /// -or- 
        /// Any element of <paramref name="typeArguments"/> is null. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The number of elements in <paramref name="typeArguments"/> is not the same as the number of type parameters in the current generic type definition.
        /// -or- 
        /// Any element of <paramref name="typeArguments"/> does not satisfy the constraints specified for the corresponding type parameter of the current generic type. 
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The invoked method is not supported in the base class. Derived classes must provide an implementation.
        /// </exception>
        public Type MakeGenericType(params Type[] typeArguments)
        {
            return _type.MakeGenericType(typeArguments);
        }

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="T:System.Type"/> of the object encompassed or referred to by the current array, pointer or reference type.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Type"/> of the object encompassed or referred to by the current array, pointer, or reference type, or null if the current <see cref="T:System.Type"/> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Type GetElementType()
        {
            return _type.GetElementType();
        }

        /// <summary>
        /// Returns an array of <see cref="T:System.Type"/> objects that represent the type arguments of a generic type or the type parameters of a generic type definition.
        /// </summary>
        /// <returns>
        /// An array of <see cref="T:System.Type"/> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Type[] GetGenericArguments()
        {
            return _type.GetGenericArguments();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Type"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> object representing a generic type from which the current type can be constructed.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The current type is not a generic type.  That is, <see cref="P:System.Type.IsGenericType"/> returns false. 
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The invoked method is not supported in the base class. Derived classes must provide an implementation.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public Type GetGenericTypeDefinition()
        {
            return _type.GetGenericTypeDefinition();
        }

        /// <summary>
        /// Determines whether the class represented by the current <see cref="T:System.Type"/> derives from the class represented by the specified <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the Type represented by the <paramref name="c"/> parameter and the current Type represent classes, and the class represented by the current Type derives from the class represented by <paramref name="c"/>; otherwise, false. This method also returns false if <paramref name="c"/> and the current Type represent the same class.
        /// </returns>
        /// <param name="c">
        /// The Type to compare with the current Type. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="c"/> parameter is null. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public bool IsSubclassOf(Type c)
        {
            return _type.IsSubclassOf(c);
        }

        /// <summary>
        /// Determines whether the specified object is an instance of the current <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the current Type is in the inheritance hierarchy of the object represented by <paramref name="o"/>, or if the current Type is an interface that <paramref name="o"/> supports. false if neither of these conditions is the case, or if <paramref name="o"/> is null, or if the current Type is an open generic type (that is, <see cref="P:System.Type.ContainsGenericParameters"/> returns true).
        /// </returns>
        /// <param name="o">
        /// The object to compare with the current Type. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public bool IsInstanceOfType(object o)
        {
            return _type.IsInstanceOfType(o);
        }

        /// <summary>
        /// Determines whether an instance of the current <see cref="T:System.Type"/> can be assigned from an instance of the specified Type.
        /// </summary>
        /// <returns>
        /// true if <paramref name="c"/> and the current Type represent the same type, or if the current Type is in the inheritance hierarchy of <paramref name="c"/>, or if the current Type is an interface that <paramref name="c"/> implements, or if <paramref name="c"/> is a generic type parameter and the current Type represents one of the constraints of <paramref name="c"/>. false if none of these conditions are true, or if <paramref name="c"/> is null.
        /// </returns>
        /// <param name="c">
        /// The Type to compare with the current Type. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public bool IsAssignableFrom(Type c)
        {
            return _type.IsAssignableFrom(c);
        }

        /// <summary>
        /// The is assignable from.
        /// </summary>
        /// <param name="typeInfo">
        /// The type info.
        /// </param>
        /// <returns>
        /// The is assignable from.
        /// </returns>
        public bool IsAssignableFrom(TypeInfo typeInfo)
        {
            return IsAssignableFrom(typeInfo._type);
        }

        /// <summary>
        /// Determines if the underlying system type of the current <see cref="TypeInfo"/> is the same as the underlying system type of the specified <see cref="T:System.Type"/>.
        /// </summary>
        /// <returns>
        /// true if the underlying system type of <paramref name="o"/> is the same as the underlying system type of the current <see cref="TypeInfo"/>; otherwise, false.
        /// </returns>
        /// <param name="o">
        /// The <see cref="T:System.Type"/> whose underlying system type is to be compared with the underlying system type of the current <see cref="TypeInfo"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public bool Equals(Type o)
        {
            return _type.Equals(o);
        }

        /// <summary>
        /// Determines if the underlying system type of the current <see cref="TypeInfo"/> is the same as the underlying system type of the specified <see cref="TypeInfo"/>.
        /// </summary>
        /// <param name="o">
        /// The <see cref="TypeInfo"/> whose underlying system type is to be compared with the underlying system type of the current <see cref="TypeInfo"/>. 
        /// </param>
        /// <returns>
        /// true if the underlying system type of <paramref name="o"/> is the same as the underlying system type of the current <see cref="TypeInfo"/>; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool Equals(TypeInfo o)
        {
            return Equals(o._type);
        }

#if !PCL
        /// <summary>
        /// Returns an interface mapping for the specified interface type.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Reflection.InterfaceMapping"/> object representing the interface mapping for <paramref name="interfaceType"/>.
        /// </returns>
        /// <param name="interfaceType">
        /// The <see cref="T:System.Type"/> of the interface of which to retrieve a mapping. 
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="interfaceType"/> parameter does not refer to an interface. 
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="interfaceType"/> is null. 
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The current <see cref="T:System.Type"/> represents a generic type parameter; that is, <see cref="P:System.Type.IsGenericParameter"/> is true.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The invoked method is not supported in the base class. Derived classes must provide an implementation.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            return _type.GetInterfaceMap(interfaceType);
        }
#endif
        #endregion
    }
#endif
}