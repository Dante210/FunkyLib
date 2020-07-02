﻿using System.Collections.Immutable;

namespace funkylib.common {
  public static class Exts {
    /// <summary>
    /// Same as example below, but if index references non existing element code does not throw exception.
    /// <para><c>var arr = new [](1,2,3); return arr[idx];</c></para>
    /// </summary>
    /// <param name="arr">Array which holds the elements</param>
    /// <param name="idx">Element index to get</param>
    /// <returns>Some if element found, None otherwise</returns>
    public static Option<A> get<A>(this A[] arr, int idx) =>
      idx >= 0 && idx < arr.Length ? arr[idx].some() : Option.None;

    /// <inheritdoc cref="get{A}(A[],int)"/>
    public static Option<A> get<A>(this ImmutableList<A> @this, int idx) =>
      idx >= 0 && idx < @this.Count ? @this[idx].some() : Option.None;
  }
}