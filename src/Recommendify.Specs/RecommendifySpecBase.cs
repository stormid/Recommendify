using Machine.Specifications;
using ServiceStack.Redis;

namespace Recommendify.Specs
{
    public abstract class RecommendifySpecBase
    {
        protected static IRedisClient RedisClient;

        protected static void EstablishRedisClient()
        {
            RedisClient = new RedisClient(RedisSetup.RedisConnectionInfo.Host, RedisSetup.RedisConnectionInfo.Port);
            FlushRedis(RedisClient);
        }

        protected static void FlushRedis(IRedisClient redisClient)
        {
            foreach (var key in redisClient.SearchKeys("recommendify-test*"))
            {
                redisClient.Remove(key);
            }
        }

        Cleanup after = () => FlushRedis(RedisClient);
    }
}