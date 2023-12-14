namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    public class TestValidatorProvider : ValidatorProviderBase
    {
        protected override IValidator GetValidator(Type targetType)
        {
            ArgumentNullException.ThrowIfNull(targetType);

            return new TestValidator();
        }
    }
}