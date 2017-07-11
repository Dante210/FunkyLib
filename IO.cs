using System;

namespace funkylib
{
    public static class IO
    {
        public static readonly IO<Unit> empty = @return(() => { });

        public static IO<A> @return<A>(Func<A> fn) => new IO<A>(fn);
        public static IO<Unit> @return(Action action) => new IO<Unit>(() => {
            action();
            return new Unit();
        });
    }

    /**
     * Allows encapsulating side effects and composing them.
     */
    public struct IO<A>
    {
        readonly Func<A> fn;

        public IO(Func<A> fn) { this.fn = fn; }

        public IO<B> map<B>(Func<A, B> mapper) {
            var fn = this.fn;
            return new IO<B>(() => mapper(fn()));
        }

        public IO<B> andThen<B>(IO<B> io2) {
            var fn = this.fn;
            return new IO<B>(() => {
                fn();
                return io2.unsafeRun();
            });
        }

        public IO<B> flatMap<B>(Func<A, IO<B>> mapper) {
            var fn = this.fn;
            return new IO<B>(() => mapper(fn()).unsafeRun());
        }

        /** Runs the encapsulated side effects. */
        public A unsafeRun() => fn();
    }
}
