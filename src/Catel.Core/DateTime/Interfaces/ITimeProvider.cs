namespace Catel
{
    using System;

    public interface ITimeProvider
    {
        DateTimeOffset GetLocalNow();

        DateTimeOffset GetUtcNow();
    }
}
