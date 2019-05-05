using System;

namespace Test_Ef_Relationship.Helpers
{
    public interface ITimeHelper
    {
        DateTimeOffset GetUtcNow();
    }

    public class TimeHelper : ITimeHelper
    {
        public DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}