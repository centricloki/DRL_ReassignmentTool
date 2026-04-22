using DRL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Core.Interface
{
    public interface ICacheService
    {
        // Basic operations
        bool TryGetValue<T>(string key, out T value);
        void Set<T>(string key, T value, TimeSpan expiration);
        void Remove(string key);

        // Get or create pattern (matches your existing code style)
        T GetOrCreate<T>(string key, Func<T> factory, TimeSpan expiration);

        // Prefix-based operations
        void RegisterKey(string key, string prefix);
        void RemoveByPrefix(string prefix);
        IEnumerable<string> GetKeysByPrefix(string prefix);
    }
}
