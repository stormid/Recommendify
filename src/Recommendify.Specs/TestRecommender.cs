using System.Collections.Generic;
using ServiceStack.Redis;

namespace Recommendify.Specs
{
    public class TestRecommender : Base
    {
        public TestRecommender(IRedisClient redisClient) : this(Recommendify.DEFAULT_MAX_NEIGHBOURS, redisClient)
        {
        }

        public TestRecommender(int maxNeighbours, IRedisClient redisClient)
            : base(maxNeighbours, "recommendify-test", redisClient)
        {
        }
    }
}