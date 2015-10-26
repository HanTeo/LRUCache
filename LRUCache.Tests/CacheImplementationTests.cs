using NUnit.Framework;
using Rhino.Mocks;

namespace LRUCache.Tests
{
    /*
    Key features
·         The cache should implement the same IProvider interface so we can easily integrate it into existing code

·         It should populate on demand

·         It should keep no more than 10 entries in memory at any time. When the 11th distinct key is requested, the least recently queried key/value pair should be evicted from the cache

Please bear in mind the following:
·         The cache should be safe when called by multiple threads (Lock where there is shared writes)

·         If two or more threads request the same key, only one call to the underlying IProvider implementation should be made

·         Ideally, the locking should be fine-grained, so if two threads request different keys, the retrieval should operate in parallel

·         The provider is not 100% reliable, it can fail on occasion with an exception
    */


    [TestFixture]
    public class CacheImplementationTests
    {
        private MockRepository _mocker;

        [SetUp]
        public void Setup()
        {
            _mocker = new MockRepository();
        }

        [Test]
        public void test_simple_get()
        {
            var provider = _mocker.StrictMock<IProvider>();
            var updater = _mocker.StrictMock<IUpdater>();

            Expect.Call(provider.GetData("key")).Repeat.Once().Return("value");
            Expect.Call(() => updater.AddSubscription("key")).Repeat.Once();

            _mocker.ReplayAll();

            var cache = MakeCache(provider, updater);

            Assert.AreEqual("value", cache.GetData("key"));

            _mocker.VerifyAll();
        }

        [Test]
        public void test_simple_get_twice_with_update()
        {
            var provider = _mocker.StrictMock<IProvider>();
            var updater = _mocker.StrictMock<IUpdater>();

            Expect.Call(provider.GetData("key")).Repeat.Once().Return("value");
            Expect.Call(() => updater.AddSubscription("key")).Repeat.Once();

            _mocker.ReplayAll();

            var cache = MakeCache(provider, updater);

            Assert.AreEqual("value", cache.GetData("key"));

            //simulate update to data
            ((IUpdateCallback) cache).Updated("key", "value2");

            Assert.AreEqual("value2", cache.GetData("key"));

            _mocker.VerifyAll();
        }

        private IProvider MakeCache(IProvider provider, IUpdater updater)
        {
            return new CacheImplementation(provider, updater);
        }
    }
}
