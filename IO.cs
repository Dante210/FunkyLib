using System;

namespace funkylib
{
    public delegate A IO<out A>();

    public static class IOExts {
        public static IO<B> flatMap<A, B>(this IO<A> io, Func<A, IO<B>> mapper) =>
            () => mapper(io())();
    }
}