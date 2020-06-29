using System;

namespace funkylib
{
    public static class FuncExt
    {
        public static Func<B, R> apply<A, B, R>(this Func<A, B, R> @this, A a) => b => @this(a, b);

        public static Func<R> map<A, R>(this Func<A> @this, Func<A, R> func) => () => func(@this());
        public static Func<R> map<A, R>(this Func<A> @this, Func<A, Func<R>> g) => () => g(@this())();

        public static Func<Reader, R> map<Reader, T, R>
            (this Func<Reader, T> f, Func<T, R> g)
            => reader => g(f(reader));

        public static Func<Reader, R> Select<Reader, B, R>(this Func<Reader, B> func, Func<B, R> g) => func.map(g);

        public static Func<Reader, P> SelectMany<Reader, T, R, P>
            (this Func<Reader, T> f, Func<T, Func<Reader, R>> bind, Func<T, R, P> project)
            => reader => {
                var t = f(reader);
                var r = bind(t)(reader);
                return project(t, r);
            };
    }
}