using System;

namespace funkylib.common {
  public static class FuncExt {
    public static Func<B, R> apply<A, B, R>(this Func<A, B, R> @this, A a) => b => @this(a, b);

    public static Func<R> map<A, R>(this Func<A> @this, Func<A, R> func) => () => func(@this());
    public static Func<R> map<A, R>(this Func<A> @this, Func<A, Func<R>> g) => () => g(@this())();
    public static Func<Reader, R> map<Reader, T, R>(this Func<Reader, T> f, Func<T, R> g) => reader => g(f(reader));
  }
}