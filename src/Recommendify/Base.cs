using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ServiceStack.Redis;

namespace Recommendify
{
    public abstract class Base
    {
        private readonly IRedisClient redisClient;
        public SimilarityMatrix SimilarityMatrix { get; private set; }
        public IDictionary<string, InputMatrix> InputMatrices { get; private set; }
        public int MaxNeighbours { get; set; }

        public Base(IRedisClient redisClient) : this(Recommendify.DEFAULT_MAX_NEIGHBOURS, Recommendify.DEFAULT_REDIS_PREFIX, redisClient) { }

        public Base(int maxNeighbours, string redisPrefix, IRedisClient redisClient)
        {
            this.redisClient = redisClient;
            MaxNeighbours = maxNeighbours;
            InputMatrices = InitializeMatrices();
            SimilarityMatrix = new SimilarityMatrix(
                    new Options {Key = "similarities", MaxNeighbours = MaxNeighbours, RedisPrefix = redisPrefix},
                    redisClient);
        }

        protected abstract IDictionary<string, InputMatrix> InitializeMatrices();

        public InputMatrix this[string key]
        {
            get { return InputMatrices[key]; }
        }

        public IEnumerable<string> AllItems()
        {
            return InputMatrices.SelectMany(x => x.Value.AllItems()).Distinct();
        }

        public IEnumerable<Neighbour> For(string itemId)
        {
            return SimilarityMatrix[itemId].Select(x => new Neighbour { Id = x.Key, Score = x.Value }).OrderByDescending(x => x.Score);
        }

        public void Process()
        {
            foreach (var item in AllItems())
            {
                ProcessItem(item);
            }
        }

        private void ProcessItem(string itemId)
        {
            foreach (var inputMatrix in InputMatrices.Values)
            {
                var neighbours = inputMatrix.SimilaritiesFor(itemId);
                foreach (var neighbour in neighbours)
                {
                    neighbour.Score = neighbour.Score*inputMatrix.Weight;
                }
                SimilarityMatrix.Update(itemId, neighbours);
            }
            SimilarityMatrix.CommitItem(itemId);
        }

        public void DeleteItem(string itemId)
        {
            foreach (var inputMatrix in InputMatrices.Values)
            {
                inputMatrix.DeleteItem(itemId);
            }
        }
    }
}