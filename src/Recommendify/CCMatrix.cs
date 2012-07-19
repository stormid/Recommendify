using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace Recommendify
{
    public class CCMatrix
    {
        private readonly Options options;
        private readonly IRedisClient redisClient;
        public SparseMatrix Matrix { get; private set; }

        public CCMatrix(Options options, IRedisClient redisClient)
        {
            this.options = options;
            this.redisClient = redisClient;
            Matrix = new SparseMatrix(
                    new Options
                    {
                        Key = string.Format("{0}:{1}", options.Key, "ccmatrix"),
                        MaxNeighbours = options.MaxNeighbours,
                        RedisPrefix = options.RedisPrefix,
                        Weight = options.Weight
                    }, redisClient);
        }

        public string RedisKey()
        {
            return string.Format("{0}:{1}", options.RedisPrefix, options.Key);
        }

        public string RedisKey(string append)
        {
            return string.Format("{0}:{1}:{2}", options.RedisPrefix, options.Key, append);
        }

        public void AddSet(string setId, string[] itemIds)
        {
            foreach (var itemId in itemIds)
            {
                ItemCountIncr(itemId);
            }

            foreach (var pair in AllPairs(itemIds))
            {
                var values = pair.Split(':');
                Matrix.Incr(values[0], values[1]);
            }
        }

        public void AddSingle(string setId, string itemId, string[] otherItemIds)
        {
            ItemCountIncr(itemId);
            foreach (var otherItemId in otherItemIds)
            {
                Matrix.Incr(itemId, otherItemId);
            }
        }

        public IEnumerable<string> AllItems()
        {
            return redisClient.Hashes[RedisKey("items")].Keys;
        }

        public void DeleteItem(string itemId)
        {
            redisClient.Hashes[RedisKey("items")].Remove(itemId);
            Matrix.KDelAll(new[] { itemId });
        }

        private IEnumerable<string> AllPairs(string[] keys)
        {
            return keys.SelectMany(x =>
                keys.Except(new[] { x }).Select(y =>
                    string.Join(":", new[] { x, y }.OrderBy(s => s))
                )
            ).Distinct();
        }

        private void ItemCountIncr(string key)
        {
            redisClient.Hashes[RedisKey("items")].IncrementValue(key, 1);
        }

        public int ItemCount(string key)
        {
            return int.Parse(redisClient.Hashes[RedisKey("items")][key]);
        }
    }
}