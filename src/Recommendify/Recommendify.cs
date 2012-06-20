using ServiceStack.Redis;

namespace Recommendify
{
    public class Recommendify
    {
        public const int DEFAULT_MAX_NEIGHBOURS = 50;

        internal static IRedisClient RedisClient { get; private set; }
    }
}