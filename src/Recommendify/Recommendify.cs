using ServiceStack.Redis;

namespace Recommendify
{
    public class Recommendify
    {
        public const int DEFAULT_MAX_NEIGHBOURS = 50;
        public const string DEFAULT_REDIS_PREFIX = "recommendify";
    }
}