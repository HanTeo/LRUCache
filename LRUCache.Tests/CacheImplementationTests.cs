using NUnit.Framework;
using Rhino.Mocks;

namespace LRUCache.Tests
{
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
            return new LRUCache.CacheImplementation(provider, updater);
        }
    }
}
