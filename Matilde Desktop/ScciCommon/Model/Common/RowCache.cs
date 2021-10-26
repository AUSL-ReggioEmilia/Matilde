using System;
using System.Linq;
using System.Collections.Generic;
using UnicodeSrl.Framework.Data.Interfaces;
using UnicodeSrl.Framework.Data.Model;


namespace UnicodeSrl.Scci.Model
{
    public static class RowCache
    {
        private static readonly object LockObject = new object();

        private static readonly List<IFwModelRow> cache = new List<IFwModelRow>();

        public static T GetCachedRow<T>(Predicate<T> match, IFwModelRow elseSelectObj = null)
            where T : class, IFwModelRow, new()
        {

            lock (RowCache.LockObject)
            {
                IEnumerable<T> ienCached =
                            from typed in cache
                            where typed is T
                            select (T)typed;

                List<T> rowsCached = ienCached.ToList();

                T result = rowsCached.Find(match);

                if ((result == default(T)) && (elseSelectObj != null))
                {
                    bool exist = elseSelectObj.TrySelect();

                    if (exist)
                    {
                        RowCache.AddRow(elseSelectObj);
                        return (T)elseSelectObj;
                    }
                }

                return result;
            }

        }

        public static void AddRow(IFwModelRow row)
        {
            lock (RowCache.LockObject)
            {
                cache.Add(row);
            }
        }

        public static void RemoveCachedRow<T>(Predicate<T> match)
                where T : class, IFwModelRow, new()
        {

            lock (RowCache.LockObject)
            {
                T row = GetCachedRow(match);

                if (row != null)
                    cache.Remove(row);

            }

        }

        public static void RemoveCachedRow(IFwModelRow row)
        {
            lock (RowCache.LockObject)
            {
                cache.Remove(row);
            }
        }
    }
}
