﻿using System;
using funkylib.common;

namespace funkylib {
  /// <summary>
  /// The `Try` type represents a computation that may either result in an exception, or return a
  /// successfully computed value. It's the same as <see cref="Exceptional{A}"/> type,
  /// but more commonly used in functional programming than Exceptional.
  /// </summary>
  public delegate Exceptional<A> Try<A>();

  public static class TryExt {
    public static Exceptional<T> run<T>(this Try<T> @try) {
      try { return @try(); }
      catch (Exception ex) { return ex; }
    }

    public static Try<R> map<A, R>(this Try<A> @this, Func<A, R> func) =>
      () => @this.run().map(func);

    public static Try<R> flatMap<A, R>(this Try<A> @this, Func<A, Try<R>> func) =>
      () => @this.run().fold(_ => _, a => func(a).run());

    static Try<R> Select<T, R>(this Try<T> @this, Func<T, R> func) => @this.map(func);

    static Try<RR> SelectMany<T, R, RR>(this Try<T> @try, Func<T, Try<R>> bind, Func<T, R, RR> project) =>
      () => @try.run().fold(
        _ => _,
        t => bind(t).run().fold<Exceptional<RR>>(ex => ex,r => project(t, r))
      );
    }
}