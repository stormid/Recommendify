using Machine.Specifications;
using ServiceStack.Redis;

namespace Recommendify.Specs
{
    public class BaseSpec
    {
        [Subject(typeof(Base), "configuration")]
        public class when_max_neighbours_is_not_configured : RecommendifySpecBase
        {
            private static TestRecommender recommender;

            Establish context = () => EstablishRedisClient();

            Because of = () => recommender = new TestRecommender(RedisClient);

            It should_return_default_max_neighbours = () => recommender.MaxNeighbours.ShouldEqual(50);
        }

        [Subject(typeof(Base), "configuration")]
        public class when_max_neighbours_is_configured : RecommendifySpecBase
        {
            private static TestRecommender recommender;

            Establish context = () => EstablishRedisClient();

            Because of = () => recommender = new TestRecommender(23, RedisClient);

            It should_remember_max_neighbours = () => recommender.MaxNeighbours.ShouldEqual(23);
        }

        [Subject(typeof (Base), "configuration")]
        public class when_adding_input_matrix_by_key : RecommendifySpecBase
        {
            private static TestRecommender recommender;

            Establish context = () =>
            {
                EstablishRedisClient();
                recommender = new TestRecommender(RedisClient);
            };

            Because of = () => recommender.InputMatrix("TestInput",
                                                       new JaccardInputMatrix(
                                                           new Options { Key = "TestItems", RedisPrefix = recommender.RedisPrefix, Weight = 5m },
                                                           RedisClient));

            It should_add_key = () => recommender.InputMatrices.Keys.ShouldContain("TestInput");
        }

        [Subject(typeof(Base), "configuration")]
        public class when_retrieving_input_matrix : RecommendifySpecBase
        {
            private static TestRecommender recommender;

            Establish context = () =>
            {
                EstablishRedisClient();
                recommender = new TestRecommender(RedisClient);
            };

            Because of = () => recommender.InputMatrix("TestInput",
                                                       new JaccardInputMatrix(
                                                           new Options { Key = "TestItems", RedisPrefix = recommender.RedisPrefix, Weight = 5m },
                                                           RedisClient));

            It should_retrieve_input_matrix_by_key = () => recommender["TestInput"].ShouldNotBeNull();
        }
    }
}