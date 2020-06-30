﻿using System;

namespace funkylib {
  public static class Extensions {
    public static Func<Unit> toFunc(this Action action) => () => {
      action();
      return new Unit();
    };

    public static Func<A, Unit> toFunc<A>(this Action<A> action) => a => {
      action(a);
      return new Unit();
    };
  }
}
