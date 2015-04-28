using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumerableUtils
{
    public static class EnumerableTools
    {
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> fnRecurse)
        {
            foreach (var item in source)
            {
                yield return item;

                var seqRecurse = fnRecurse(item);

                if (seqRecurse != null)
                {
                    foreach (var itemRecurse in Traverse(seqRecurse, fnRecurse))
                    {
                        yield return itemRecurse;
                    }
                }
            }
        }

        public static ICollection<T> RecurseFilter<T>(this ICollection<T> source, Func<T, ICollection<T>> fnRecurse,
            Func<T, bool> filterFunc)
        {
            var traversal = new Stack<T>();
            source.Filter(filterFunc);
            traversal.PushAll(source);

            while (traversal.Any())
            {
                var curItem = traversal.Pop();

                var collection = fnRecurse(curItem);
                collection.Filter(filterFunc);
                traversal.PushAll(collection);
            }
            return source;
        }

        private static void PushAll<T>(this Stack<T> traversal, ICollection<T> source)
        {
            if (source == null)
            {
                return;
            }

            foreach (var item in source.Where(i => i != null))
            {
                traversal.Push(item);
            }
        }

        private static void Filter<T>(this ICollection<T> source, Func<T, bool> filterFunc)
        {
            if (source == null)
            {
                return;
            }

            foreach (var toBeRemoved in source.Where(i => !filterFunc(i)).ToList())
            {
                source.Remove(toBeRemoved);
            }
        }
    }
}