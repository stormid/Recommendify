using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace Recommendify
{
    public class JaccardInputMatrix : InputMatrix
    {
        private readonly IRedisClient redisClient;
        private readonly CCMatrix ccmatrix;

        public JaccardInputMatrix(Options options, IRedisClient redisClient) : base(options)
        {
            this.redisClient = redisClient;
            ccmatrix = new CCMatrix(options, redisClient);
        }

        public override decimal Similarity(string item1, string item2)
        {
            return CalculateJaccardCached(item1, item2);
        }

        public override IEnumerable<Neighbour> SimilaritiesFor(string item1)
        {
            return CalculateSimilarities(item1);
        }

        public override void AddSet(string setId, string[] itemsIds)
        {
            ccmatrix.AddSet(setId, itemsIds);
        }

        public override void AddSingle(string setId, string itemsId, string[] otherItemIds)
        {
            ccmatrix.AddSingle(setId, itemsId, otherItemIds);
        }

        public override IEnumerable<string> AllItems()
        {
            return ccmatrix.AllItems();
        }

        public override void DeleteItem(string itemId)
        {
            ccmatrix.DeleteItem(itemId);
        }

        private IEnumerable<Neighbour> CalculateSimilarities(string item1)
        {
            return AllItems().Except(new[] {item1}).Select(item2 => new Neighbour {Id = item2, Score = Similarity(item1, item2)});
        }

        private decimal CalculateJaccardCached(string item1, string item2)
        {
            var value = ccmatrix.Matrix.Get(item1, item2);
            return value / (ccmatrix.ItemCount(item1) + ccmatrix.ItemCount(item2));
        }
    }
}