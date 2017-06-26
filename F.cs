﻿using System;

namespace funkylib
{
    public static class F
    {
        public static Try<A> Try<A>(Func<A> func) => () => func();

        public static Func<A, Func<B, R>> curry<A, B, R>(this Func<A, B, R> @this) => a => b => @this(a, b);

        public static Func<A, Func<B, Func<C, R>>> curry<A, B, C, R>(this Func<A, B, C, R> @this) =>
            a => b => c => @this(a, b, c);
    }
}