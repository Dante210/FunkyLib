﻿using System;
 using System.Collections.Generic;
using System.Linq;

namespace funkylib {
  public struct Unit { }

  /// <summary>
  /// Represents optional values. Instances of `Option` are either an instance of Some or None.
  /// The most idiomatic way to use an $option instance is to treat it
  /// as a collection or monad and use `map`,`flatMap`, or `@foreach`:
  /// </summary>
  public struct Option {
    public static Option<A> Some<A>(A value) => new Some<A>(value);
    public static None None => None.none;
  }

  public struct Option<A> : IEquatable<Option<A>>, IEquatable<None> {
    readonly A value;
    public readonly bool isSome;

    /// <summary>
    /// returns true if the option is None, false otherwise.
    /// </summary>
    public bool isNone => !isSome;

    internal Option(A value) {
      if (value == null) throw new ArgumentNullException();
      isSome = true;
      this.value = value;
    }

    public A _unsafe => value;

    /// <summary>
    /// Returns the result of applying `onSome` to this option's
    /// value if the option is nonempty.  Otherwise, evaluates
    /// expression `onNone`
    /// </summary>
    /// <param name="onNone">The expression to evaluate if empty</param>
    /// <param name="onSome">The function to apply if nonempty.</param>
    public R fold<R>(Func<R> onNone, Func<A, R> onSome) => isSome ? onSome(value) : onNone();

    public void fold(Action onNone, Action<A> onSome) {
      if (isSome) onSome(value);
      else onNone();
    }

    /// <summary>
    /// Apply the given procedure $f to the option's value, if it is nonempty. Otherwise, do nothing.
    /// </summary>
    public void @foreach(Action<A> action) {
      if (isSome) action(value);
    }

    public static implicit operator Option<A>(None _) => new Option<A>();
    public static implicit operator Option<A>(Some<A> some) => new Option<A>(some.value);
    public static implicit operator Option<A>(A value) => value == null ? Option.None : Option.Some(value);

    public bool Equals(Option<A> other) =>
      isSome == other.isSome && (isNone || value.Equals(other.value));

    public bool Equals(None _) => isNone;

    /// <summary>
    /// Returns a `Some` formed from this option and another option
    /// by combining the corresponding elements into a single option by applying given `mapper` function,
    /// if either of the two options is empty, `None` is returned.
    /// </summary>
    /// <param name="that">Second option</param>
    /// <param name="mapper">Function to apply with both option's values</param>
    public Option<C> zip<B, C>(Option<B> that, Func<A, B, C> mapper) =>
      isSome && that.isSome
        ? Option.Some(mapper(value, that.value))
        : Option.None;

    /// <summary>
    /// Returns a `Some` formed from three different options
    /// by combining the corresponding elements into a single option by applying given `mapper` function,
    /// if either of the three options is empty, `None` is returned.
    /// </summary>
    /// <param name="opt2">Second option</param>
    /// <param name="opt3">Third option</param>
    /// <param name="mapper">Function to apply with all option's values</param>
    public Option<D> zip<B, C, D>(Option<B> opt2, Option<C> opt3, Func<A, B, C, D> mapper) =>
      isSome && opt2.isSome && opt3.isSome
        ? Option.Some(mapper(value, opt2.value, opt3.value))
        : Option.None;

    public override string ToString() => isSome ? $"Some({value})" : "None";
    public static Option<A> operator |(Option<A> left, Option<A> right) => left.isSome ? left : right;
  }

  public struct Some<A> {
    internal A value;
    internal Some(A value) { this.value = value; }
  }

  public struct None {
    internal static readonly None none = new None();
  }

  public static class OptionExt {
    public static Option<A> some<A>(this A value) => new Option<A>(value);

    public static Option<R> map<A, R>(this Option<A> @this, Func<A, R> func) =>
      @this.fold(() => Option.None, value => Option.Some(func(value)));

    public static Option<C> orElse<A,B,C>(
      this Option<A> @this, Func<Option<B>> optional, Func<A,C> mapThis, Func<B,C> mapAnother
    ) {
      if (@this.isSome) return mapThis(@this._unsafe);

      var another = optional();
      return another.isSome ? (Option<C>) mapAnother(another._unsafe) : Option.None;
    }

    public static bool exist<A>(this Option<A> @this, Option<A> other) =>
      @this.fold(
        () => false,
        left => other.fold(
          () => false,
          right => left.Equals(right)
        )
      );

    public static Option<R> flatMap<A, R>(this Option<A> @this, Func<A, Option<R>> func) =>
      @this.fold(() => Option.None, func);

    public static IEnumerable<A> asEnumerable<A>(this IEnumerable<Option<A>> @this) {
      var temp = new List<A>();
      foreach (var option in @this)
        option.fold(() => { }, some => temp.Add(some));
      return temp;
    }

    /// <summary>
    /// Maps every collection member to option and aggregates to collection of options.
    /// </summary>
    public static Option<IEnumerable<R>> traverse<T, R>(this IEnumerable<T> @this, Func<T, Option<R>> func) =>
      @this.Aggregate(
        Enumerable.Empty<R>().some(),
        (optResult, t) =>
          from results in optResult
          from result in func(t)
          select results.append(result)
      );

    static Option<RR> SelectMany<T, R, RR>
        (this Option<T> opt, Func<T, Option<R>> bind, Func<T, R, RR> project)
        => opt.fold(
            () => Option.None,
            t => bind(t).fold(
                () => Option.None,
                r => Option.Some(project(t, r)))
            );

    public static Option<A> find<A>(this Option<A> @this, Func<A, bool> predicate) =>
      @this.fold(() => Option.None, a => predicate(a) ? @this : Option.None);
  }
}