using System;

namespace funkylib
{
    public static class F
    {
        public static Try<A> Try<A>(Func<A> func) => () => func();
        
        public static R @using<TDisp, R>(
            TDisp disposable
            , Func<TDisp, R> f) where TDisp : IDisposable {
            using (disposable)
            {
                return f(disposable);
            }
        }

        public static Func<A, Func<B, R>> curry<A, B, R>(this Func<A, B, R> @this) { return a => b => @this(a, b); }

        public static Func<A, Func<B, Func<C, R>>> curry<A, B, C, R>(this Func<A, B, C, R> @this) {
            return a => b => c => @this(a, b, c);
        }
    }
}