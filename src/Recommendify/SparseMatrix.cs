using System.Linq;

namespace Recommendify
{
    public class SparseMatrix
    {
        private readonly Options options;

        public SparseMatrix(Options options)
        {
            this.options = options;
        }

        public string RedisKey
        {
            get { return string.Format("{0}:{1}", options.RedisPrefix, options.Key); }
        }

        public decimal Get(string x, string y)
        {
            return KGet(Key(x, y));
        }

        public void Set(string x, string y, decimal v)
        {
            if (v == 0)
            {
                Del(x, y);
            }
            else
            {
                KSet(Key(x, y), v);
            }
        }

        public void Del(string x, string y)
        {
            KDel(Key(x, y));
        }

        public void Incr(string x, string y)
        {
            KIncr(Key(x, y));
        }

        private string Key(string x, string y)
        {
            return string.Join(":", new[] {x, y}.OrderBy(s => s));
        }

        private void KSet(string key, decimal value)
        {
            Recommendify.RedisClient.Hashes[RedisKey][key] = value.ToString();
        }

        private void KDel(string key)
        {
            Recommendify.RedisClient.Hashes[RedisKey].Remove(key);
        }

        private decimal KGet(string key)
        {
            var value = Recommendify.RedisClient.Hashes[RedisKey][key];
            if (!string.IsNullOrEmpty(value))
            {
                return decimal.Parse(value);
            }
            return 0m;
        }

        private void KIncr(string key)
        {
            Recommendify.RedisClient.Hashes[RedisKey].IncrementValue(key, 1);
        }

        internal /* I just puked a little */ void KDelAll(string[] keys)
        {
            foreach (var key in Recommendify.RedisClient.Hashes[RedisKey].Keys)
            {
                if (keys.Contains(key))
                {
                    Recommendify.RedisClient.Hashes[RedisKey].Remove(key);
                }
            }
        }
    }
}