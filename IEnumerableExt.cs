﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace funkylib {
  public static class IEnumerableExt {
    public static IEnumerable<R> map<A, R>(this IEnumerable<A> @this, Func<A, R> func) {
      foreach (var item in @this) {
        yield return func(item);
      }
    }

    public static IEnumerable<R> flatMap<A, R>(this IEnumerable<A> @this, Func<A, IEnumerable<R>> func) {
      foreach (var item in @this)
        foreach (var r in func(item))
          yield return r;
    }

    public static IEnumerable<T> append<T>(this IEnumerable<T> source, params T[] ts) => source.Concat(ts);

    public static IEnumerable<A> flatten<A>(this IEnumerable<IEnumerable<A>> enumerable) {
      var temp = new List<A>();
      foreach (var variable in enumerable) {
        temp.AddRange(variable);
      }
      return temp;
    }

    public static IEnumerable<A> flatten<A>(this IEnumerable<ImmutableArray<A>> enumerable) {
      var temp = new List<A>();
      foreach (var variable in enumerable) {
        temp.AddRange(variable);
      }
      return temp;
    }
  }
}