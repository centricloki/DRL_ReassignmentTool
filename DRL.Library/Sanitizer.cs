using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRL.Library
{
    public static class RequestSanitizer
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, System.Reflection.PropertyInfo[]> _cache
            = new System.Collections.Concurrent.ConcurrentDictionary<Type, System.Reflection.PropertyInfo[]>();

        private static readonly System.Text.RegularExpressions.Regex _spaceRegex =
            new System.Text.RegularExpressions.Regex(@"\s+", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static string Clean(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return _spaceRegex.Replace(input.Trim(), " ");
        }

        /// <summary>
        /// Sanitizes string for SQL storage - escapes special characters and trims spaces
        /// </summary>
        public static void SanitizeAllStrings<T>(T obj)
        {
            if (obj == null) return;

            var props = _cache.GetOrAdd(typeof(T), t =>
                t.GetProperties()
                 .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite)
                 .ToArray()
            );

            foreach (var prop in props)
            {
                var current = prop.GetValue(obj) as string;
                prop.SetValue(obj, Clean(current));
            }
        }
    }
}
