using System.Collections.Generic;

namespace Recommendify
{
    public abstract class InputMatrix
    {
        private readonly Options options;

        public InputMatrix(Options options)
        {
            this.options = options;
        }

        public decimal Weight
        {
            get { return options.Weight.HasValue ? options.Weight.Value : 1m; }
        }

        public abstract void AddSet(string setId, string[] itemsIds);

        public abstract void AddSingle(string setId, string itemsId, string[] otherItemIds);

        public abstract decimal Similarity(string item1, string item2);

        public abstract IEnumerable<Neighbour> SimilaritiesFor(string item1);

        public abstract IEnumerable<string> AllItems();

        public abstract void DeleteItem(string itemId);
    }
}