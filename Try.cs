﻿using System;
using funkylib.common;

namespace funkylib {
  public delegate Exceptional<A> Try<A>();

  public static class TryExt {
    public static Exceptional<T> run<T>(this Try<T> @try) {
      try { return @try(); }
      catch (Exception ex) { return ex; }
    }

    public static Try<R> map<A, R>(this Try<A> @this, Func<A, R> func) =>
      () => @this.run().fold<Exceptional<R>>(_ => _,a => func(a));

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