﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace funkylib {
  public static class IEnumerableExt {
    /// <summary>
    /// Builds a new collection by applying a function to all elements of this IEnumerable.
    /// </summary>
    /// <param name="this">Collection to apply the function</param>
    /// <param name="func">Function to apply to all elements</param>
    /// <returns>New collection with applied function</returns>
    public static IEnumerable<R> map<A, R>(this IEnumerable<A> @this, Func<A, R> func) {
      foreach (var item in @this) {
        yield return func(item);
      }
    }

    /// <summary>
    /// Builds a new collection by applying a function to all elements of this collection
    /// and using the elements of the resulting collections.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// static IEnumerable<String> getWords(IEnumerable<String> lines) => lines.flatMap(line => line.Split());
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="this">Collection to apply the function</param>
    /// <param name="func">Function to apply to all elements</param>
    /// <returns>
    /// a new collection resulting from applying the given collection-valued function func
    /// to each element of this collection and concatenating the results
    /// </returns>
    public static IEnumerable<R> flatMap<A, R>(this IEnumerable<A> @this, Func<A, IEnumerable<R>> func) {
      foreach (var item in @this)
        foreach (var r in func(item))
          yield return r;
    }

    /// <summary>
    /// Concatenates two sequences, it's the same as `Concat`
    /// </summary>
    /// <param name="source">The first sequence to concatenate</param>
    /// <param name="ts">The second sequence to concatenate</param>
    /// <returns>An IEnumerable that contains the concatenated elements of the two input sequences.</returns>
    public static IEnumerable<T> append<T>(this IEnumerable<T> source, params T[] ts) => source.Concat(ts);

    /// <summary>
    /// Flattens out <![CDATA[IEnumerable<IEnumerable<A>>]]> to <![CDATA[IEnumerable<A>]]>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// static IEnumerable<int> getFlat() {
    ///   var toFlatten = ImmutableList.Create(
    ///     ImmutableList.Create(1, 2),
    ///     ImmutableList.Create(3, 4)
    ///   );
    ///   return toFlatten.flatten();
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="enumerable">Collection to flatten</param>
    /// <returns>Flattened out collection</returns>
    public static IEnumerable<A> flatten<A>(this IEnumerable<IEnumerable<A>> enumerable) {
      var temp = new List<A>();
      foreach (var variable in enumerable) {
        temp.AddRange(variable);
      }
      return temp;
    }

    /// <inheritdoc cref="flatten{A}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{A}})"/>
    public static IEnumerable<A> flatten<A>(this IEnumerable<ImmutableArray<A>> enumerable) {
      var temp = new List<A>();
      foreach (var variable in enumerable) {
        temp.AddRange(variable);
      }
      return temp;
    }

    public static Option<A> head<A>(this IEnumerable<A> enumerable) {
      foreach (var a in enumerable) return new Option<A>(a);
      return Option.None;
    }
  }
}