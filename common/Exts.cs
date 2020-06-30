using System.Collections.Immutable;

namespace funkylib.common {
  public static class Exts {
    public static Option<A> get<A>(this A[] arr, int idx) =>
      idx >= 0 && idx < arr.Length ? arr[idx].some() : Option.None;

    public static Option<A> get<A>(this ImmutableList<A> @this, int idx) =>
      idx >= 0 && idx < @this.Count ? @this[idx].some() : Option.None;
  }
}