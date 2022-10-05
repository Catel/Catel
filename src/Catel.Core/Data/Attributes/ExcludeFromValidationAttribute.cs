namespace Catel.Data
{
    using System;

    /// <summary>
    /// Attribute that can be used to exclude properties from validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcludeFromValidationAttribute : Attribute
    {
    }
}
