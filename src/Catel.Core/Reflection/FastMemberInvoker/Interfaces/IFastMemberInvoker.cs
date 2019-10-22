namespace Catel.Reflection
{
    /// <summary>
    /// Wrapper allowing fast member access as an alternative to reflection.
    /// </summary>
    public partial interface IFastMemberInvoker
    {
        bool TryGetFieldValue<TValue>(object entity, string fieldName, out TValue value);
        bool TryGetPropertyValue<TValue>(object entity, string propertyName, out TValue value);

        bool SetFieldValue<TValue>(object entity, string fieldName, TValue value);
        bool SetPropertyValue<TValue>(object entity, string propertyName, TValue value);
    }
}
