// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverageExcludeAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    /// <summary>
    /// Use this enum to provide a valid reason for excluding coverage. Expand this enum 
    /// if you encounter a new type of reason. 
    /// </summary>
    internal enum ExcludeReason
    {
        /// <summary>
        /// Static singleton will only be covered in production scenarios.
        /// </summary>
        StaticSingletonWillOnlyBeCoveredInProductionScenario,

        /// <summary>
        /// Constructor will only be covered in production scenarios. 
        /// </summary>
        ConstructorWillOnlyBeCoveredInProductionScenario,

        /// <summary>
        /// Method will only be covered in production scenarios. 
        /// </summary>
        MethodWillOnlyBeCoveredInProductionScenario,

        /// <summary>
        /// Must be implemented in a future sprint.
        /// </summary>
        ToBeImplementedInFutureSprint,

        /// <summary>
        /// Property will be substituted during a test.
        /// </summary>
        PropertyWillBeSubstitutedInTest,

        /// <summary>
        /// Contains code which cannot be substituted in a test.
        /// </summary>
        ContainsCodeWhichCannotBeSubstitutedInTest,

        /// <summary>
        /// Interface will not be implemented in this class.
        /// </summary>
        InterfaceMethodWillNotBeImplementedInThisClass,

        /// <summary>
        /// This is test code, and therefore needs to be excluded.
        /// </summary>
        TestCode,

        /// <summary>
        /// Class will only be covered in production scenarios. 
        /// </summary>
        ClassWillOnlyBeCoveredInProductionScenario,

        /// <summary>
        /// This is a data type, and therefore needs to be excluded.
        /// </summary>
        DataType,

        /// <summary>
        /// This is a generated class, and therefore needs to be excluded.
        /// </summary>
        GeneratedClass,

        /// <summary>
        /// Native method will be covered in native unit tests.
        /// </summary>
        NativeMethodWillBeCoveredInNativeUnitTests,

        /// <summary>
        /// This object is deprecated, no need to test it any longer.
        /// </summary>
        Deprecated,

        /// <summary>
        /// This is debug logging, and therefore needs to be excluded.
        /// </summary>
        DebugLogging,

        /// <summary>
        /// Object is a non-used abstract implementation.
        /// </summary>
        NonUsedAbstractImplementation,

        /// <summary>
        /// Attribute is not covered by unit tests.
        /// </summary>
        Attribute
    }

    /// <summary>
    /// Use this to skip coverage for the method which is decorated with this 
    /// attribute. Use with care! 
    /// Do not put this attribute in a specific namespace.
    /// </summary>
    [CoverageExclude(ExcludeReason.TestCode)]
    internal class CoverageExcludeAttribute : System.Attribute
    {
        /// <summary>
        /// Reason why the object is excluded from coverage.
        /// </summary>
        private readonly ExcludeReason _reason;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverageExcludeAttribute"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public CoverageExcludeAttribute(ExcludeReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{base.ToString()}{System.Environment.NewLine}{_reason.ToString()}";
        }
    }
}
