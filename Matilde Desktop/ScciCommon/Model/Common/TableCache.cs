using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnicodeSrl.Framework;
using UnicodeSrl.Framework.Data.Interfaces;
using UnicodeSrl.Framework.Data.Model;

namespace UnicodeSrl.Scci.Model
{
    public static class TableCache
    {
        static TableCache()
        {
        }

        public static void AddToCache(string cacheID, List<object> data)
        {
            FwMemoryCache.AddOrUpdate(cacheID, data);
        }

        public static void RemoveFromCache(string cacheID)
        {
            FwMemoryCache.Remove(cacheID);
        }

        public static bool IsInTableCache(string cacheID)
        {
            dynamic value = FwMemoryCache.TryGetValue(cacheID);
            return (value != null);
        }

        public static T GetCachedRow<T>(string cacheID, Predicate<T> match)
            where T : class, new()
        {
            dynamic value = FwMemoryCache.TryGetValue(cacheID);
            if (value == null) return null;

            var enumerable = value as System.Collections.IEnumerable;
            List<object> cachedList = null;
            if (enumerable == null) return null;
            else
                cachedList = value;

            IEnumerable<T> ienCached =
                        from typed in cachedList
                        where typed is T
                        select (T)typed;

            List<T> rowsCached = ienCached.ToList();

            T result = rowsCached.Find(match);

            return result;

        }
    }
}
