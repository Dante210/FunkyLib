using System;

namespace funkylib
{
    public delegate A IO<out A>();

    public static class IO
    {
        public static readonly IO<Unit> empty = @return(() => { });

        public static IO<A> @return<A>(Func<A> fn) { return new IO<A>(fn); }

        public static IO<Unit> @return(Action action) {
            return () => {
                action();
                return new Unit();
            };
        }
    }

    public static class IOExts
    {
        public static IO<B> flatMap<A, B>(this IO<A> io, Func<A, IO<B>> mapper) { return () => mapper(io())(); }
    }
}