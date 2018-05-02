namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    public class TestValidatorProvider : ValidatorProviderBase
    {
        protected override IValidator GetValidator(Type targetType)
        {
            Argument.IsNotNull("targetType", targetType);

            return new TestValidator();
        }
    }
}