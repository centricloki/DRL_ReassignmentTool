using DRL.Core.Interface;

using Microsoft.Extensions.Caching.Memory;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DRL.Core.Service
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        // ✅ Core 2.0 Compatible: Track keys by prefix using ConcurrentDictionary
        // Structure: prefix -> (key -> dummy byte value)
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _prefixIndex
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>();

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            // ✅ Core 2.0 Compatible: Remove RegisterPostEvictionCallback
            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiration,
                Priority = CacheItemPriority.Normal,
                Size = 1
                // RegisterPostEvictionCallback is NOT available in Core 2.0
            });
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            // Also remove from prefix index
            RemoveFromPrefixIndex(key);
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            if (!_cache.TryGetValue(key, out T result))
            {
                result = factory();
                Set(key, result, expiration);
            }
            return result;
        }

        public void RegisterKey(string key, string prefix)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(prefix))
                return;

            // ✅ Core 2.0: Get or create inner dictionary for this prefix
            var innerDict = _prefixIndex.GetOrAdd(prefix,
                _ => new ConcurrentDictionary<string, byte>());

            // ✅ Core 2.0: Add key to inner dictionary (value is dummy byte)
            innerDict.TryAdd(key, 0);
        }

        public void RemoveByPrefix(string prefix)
        {
            // ✅ Core 2.0 compatible TryGetValue pattern
            ConcurrentDictionary<string, byte> keys;
            if (_prefixIndex.TryGetValue(prefix, out keys))
            {
                // Get snapshot of keys to avoid modification during iteration
                var keysToRemove = keys.Keys.ToList();

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    // ✅ Core 2.0: TryRemove returns bool, not out parameter
                    byte dummy;
                    keys.TryRemove(key, out dummy);
                }

                // Optional: remove empty prefix entry
                if (keys.IsEmpty)
                {
                    ConcurrentDictionary<string, byte> removed;
                    _prefixIndex.TryRemove(prefix, out removed);
                }
            }
        }

        public IEnumerable<string> GetKeysByPrefix(string prefix)
        {
            ConcurrentDictionary<string, byte> keys;
            if (_prefixIndex.TryGetValue(prefix, out keys))
            {
                return keys.Keys.ToList(); // Return copy to avoid enumeration issues
            }
            return Enumerable.Empty<string>();
        }

        private void RemoveFromPrefixIndex(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            // Find which prefix this key belongs to (simple: last underscore)
            var lastUnderscore = key.LastIndexOf('_');
            if (lastUnderscore > 0)
            {
                var prefix = key.Substring(0, lastUnderscore + 1);

                ConcurrentDictionary<string, byte> keys;
                if (_prefixIndex.TryGetValue(prefix, out keys))
                {
                    byte dummy;
                    keys.TryRemove(key, out dummy);

                    // Clean up empty prefix entry
                    if (keys.IsEmpty)
                    {
                        ConcurrentDictionary<string, byte> removed;
                        _prefixIndex.TryRemove(prefix, out removed);
                    }
                }
            }
        }
    }
}