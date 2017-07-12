using System;
using System.Collections.Generic;
using System.Linq;

namespace funkylib
{
    public static class IEnumerableExt
    {
        public static IEnumerable<R> apply<A, R>(this IEnumerable<Func<A, R>> @this, A a) {
            return @this.Select(func => func(a));
        }

        public static IEnumerable<Func<B, R>> apply<A, B, R>(this IEnumerable<Func<A, B, R>> @this, A a) => apply(
            @this.Select(F.curry), a);

        public static IEnumerable<R> flatmap<A, R>(this IEnumerable<A> @this, Func<A, IEnumerable<R>> func) {
            foreach (var item in @this) foreach (var r in func(item)) yield return r;
        }

        public static IEnumerable<R> map<A, R>(this IEnumerable<A> @this, Func<A, R> func) {
            foreach (var item in @this) {
                yield return func(item);
            }
        }
      public static IEnumerable<T> append<T>(this IEnumerable<T> source
        , params T[] ts) => source.Concat(ts);
    
  }
}