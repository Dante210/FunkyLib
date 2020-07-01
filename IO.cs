﻿using System;

 namespace funkylib {
  public static class IO {
    public static readonly IO<Unit> empty = @return(() => { });

    public static IO<A> @return<A>(Func<A> fn) => new IO<A>(fn);

    public static IO<Unit> @return(Action action) => new IO<Unit>(() => {
        action();
        return new Unit();
    });
  }

  /// <summary>
  /// Allows encapsulating side effects and composing them.
  /// </summary>
  public struct IO<A> {
    readonly Func<A> fn;
    public IO(Func<A> fn) => this.fn = fn;

    /// <summary>
    /// Runs the encapsulated side effects.
    /// </summary>
    public A unsafeRun() => fn();

    public IO<B> map<B>(Func<A, B> mapper) {
      var f = fn;
      return new IO<B>(() => mapper(f()));
    }

    public IO<B> andThen<B>(IO<B> io) {
      var f = fn;
      return new IO<B>(() => {
        f();
        return io.unsafeRun();
      });
    }

    public IO<B> flatMap<B>(Func<A, IO<B>> mapper) {
      var f = fn;
      return new IO<B>(() => mapper(f()).unsafeRun());
    }
  }
}
