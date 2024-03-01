namespace Test.UnitTests.Cache
{
    using FluentAssertions;
    using FluentAssertions.Execution;

    using Microsoft.Extensions.Caching.Memory;

    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;

    using static Constants.MemoryCacheAdapterConstant;

    [TestOf(typeof(MemoryCacheAdapter))]
    public class MemoryCacheAdapterTests
    {
        private IMemoryCacheAdapter cacheAdapter;

        [SetUp]
        public void SetUp()
        {
            cacheAdapter = new MemoryCacheAdapter(new MemoryCache(new MemoryCacheOptions()));
        }

        [Test]
        public void MemoryCacheAdapter_SetAndGet_UseSameObject()
        {
            // Arrange
            object setTest = new();
            cacheAdapter.Set(TEST_CACHE_KEY, setTest);

            // Act
            object getTest = cacheAdapter.Get(TEST_CACHE_KEY);

            // Assert
            using (new AssertionScope())
            {
                getTest.Should().NotBeNull();
                setTest.Equals(getTest);
            }
        }

        [Test]
        public void MemoryCacheAdapter_GetNonExistingKey_ReturnsNull()
        {
            // Arrange

            // Act
            object test = cacheAdapter.Get(TEST_CACHE_KEY);

            // Assert
            test.Should().BeNull();
        }

        [Test]
        public void MemoryCacheAdapter_SetAndTryGet_ReturnsTrueAndOutsSameObject()
        {
            // Arrange
            object setTest = new();
            cacheAdapter.Set(TEST_CACHE_KEY, setTest);

            // Act
            bool exists = cacheAdapter.TryGetValue(TEST_CACHE_KEY, out object tryGetTest);

            // Assert
            using (new AssertionScope())
            {
                exists.Should().BeTrue();
                tryGetTest.Should().NotBeNull();
                setTest.Equals(tryGetTest);
            }
        }

        [Test]
        public void MemoryCacheAdapter_TryGetsNonExistingKeyValue_ReturnsFalseAndOutsNull()
        {
            // Arrange

            // Act
            bool exists = cacheAdapter.TryGetValue(TEST_CACHE_KEY, out object test);

            // Assert
            using (new AssertionScope())
            {
                exists.Should().BeFalse();
                test.Should().BeNull();
            }
        }

        [Test]
        public void MemoryCacheAdapter_Clear_ClearsCache()
        {
            // Arrange
            cacheAdapter.Set(TEST_1_CACHE_KEY, new object());
            cacheAdapter.Set(TEST_2_CACHE_KEY, new object());

            // Act
            cacheAdapter.Clear();
            object test1 = cacheAdapter.Get(TEST_1_CACHE_KEY);
            object test2 = cacheAdapter.Get(TEST_2_CACHE_KEY);

            // Assert
            using (new AssertionScope())
            {
                test1.Should().BeNull();
                test2.Should().BeNull();
            }
        }

        [Test]
        public void MemoryCacheAdapter_CreateEntryAndGet_UseSameObject()
        {
            // Arrange
            object createEntryTest = new();
            using (var cacheEntry = cacheAdapter.CreateEntry(TEST_CACHE_KEY))
            {
                // Setting the value of the cache entry
                cacheEntry.Value = createEntryTest;

                // Setting an absolute expiration time (e.g., the entry will expire 1 day from now)
                cacheEntry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(1);

                // Setting a sliding expiration time (e.g., the entry will be removed if not accessed for 30 minutes)
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(30);

                // Setting the priority of the cache entry to high (less likely to be removed when the cache is under memory pressure)
                cacheEntry.Priority = CacheItemPriority.High;

                // Adding a post-eviction callback that will be called after the entry is evicted from the cache
                cacheEntry.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    Console.WriteLine($"Cache entry with key '{key}' was evicted. Reason: {reason}.");
                }, state: null);
            }

            // Act
            object getTest = cacheAdapter.Get(TEST_CACHE_KEY);

            // Assert
            using (new AssertionScope())
            {
                getTest.Should().NotBeNull();
                createEntryTest.Equals(getTest);
            }
        }

        [Test]
        public void MemoryCacheAdapter_Remove_RemovesItem()
        {
            // Arrange
            cacheAdapter.Set(TEST_CACHE_KEY, new object());
            cacheAdapter.Remove(TEST_CACHE_KEY);

            // Act
            object test = cacheAdapter.Get(TEST_CACHE_KEY);

            // Assert
            test.Should().BeNull();
        }

        [Test]
        public void MemoryCacheAdapter_AfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            cacheAdapter.Dispose();

            // Act
            Action action = () => cacheAdapter.Get(TEST_CACHE_KEY);

            // Assert
            action.Should().Throw<ObjectDisposedException>();
        }
    }
}
