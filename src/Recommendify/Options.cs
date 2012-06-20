namespace Recommendify
{
    public class Options
    {
        public string Key { get; set; }
        public string RedisPrefix { get; set; }
        public decimal? Weight { get; set; }
        public int MaxNeighbours { get; set; }
    }
}