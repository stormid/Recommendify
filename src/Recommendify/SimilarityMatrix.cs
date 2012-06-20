using System;
using System.Collections.Generic;
using System.Linq;

namespace Recommendify
{
    public class SimilarityMatrix
    {
        private IDictionary<string, IDictionary<string, decimal>> writeQueue;
        private Options options;

        public SimilarityMatrix(Options options)
        {
            this.options = options;
            writeQueue = new Dictionary<string, IDictionary<string, decimal>>();
        }

        public string RedisKey()
        {
            return string.Format("{0}:{1}", options.RedisPrefix, options.Key);
        }

        public string RedisKey(string append)
        {
            return string.Format("{0}:{1}:{2}", options.RedisPrefix, options.Key, append);
        }

        public int MaxNeighbours
        {
            get { return options.MaxNeighbours > 0 ? options.MaxNeighbours : Recommendify.DEFAULT_MAX_NEIGHBOURS; }
        }

        public void Update(string itemId, IEnumerable<Neighbour> neighbours)
        {
            if (!writeQueue.ContainsKey(itemId))
            {
                writeQueue[itemId] = new Dictionary<string, decimal>();
            }

            foreach (var neighbour in neighbours)
            {
                if (writeQueue[itemId].ContainsKey(neighbour.Id))
                {
                    writeQueue[itemId][neighbour.Id] += neighbour.Score;
                }
                else
                {
                    writeQueue[itemId][neighbour.Id] = neighbour.Score;
                }
            }
        }

        public IDictionary<string, decimal> this [string itemId]
        {
            get
            {
                if (writeQueue.ContainsKey(itemId))
                {
                    return writeQueue[itemId];
                }
                return RetrieveItem(itemId);
            }
        }

        public void CommitItem(string itemId)
        {
            var serialized = SerializeItem(itemId);
            Recommendify.RedisClient.Hashes[RedisKey()][itemId] = serialized;
            writeQueue.Remove(itemId);
        }

        public IDictionary<string, decimal> RetrieveItem(string itemId)
        {
            var data = Recommendify.RedisClient.Hashes[RedisKey()][itemId];

            if (string.IsNullOrEmpty(data))
            {
                return new Dictionary<string, decimal>();
            }

            return data.Split('|').Select(x => x.Split(':')).Where(x => decimal.Parse(x[1]) > 0).ToDictionary(x => x[0], x => decimal.Parse(x[1]));
        }

        private string SerializeItem(string itemId, int maxPrecision = 5)
        {
            return string.Join("|", writeQueue[itemId]
                .OrderByDescending(x => x.Value)
                .Take(MaxNeighbours)
                .Select(x => x.Value > 0 ? string.Format("{0}:{1}", x.Key, Math.Round(x.Value, maxPrecision)) : string.Empty)
                .Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}