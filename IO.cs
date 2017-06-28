using System;

namespace funkylib
{
    //Why??
//    public class IO
//    {
//        public static readonly IO<Unit> empty = a(() => { });
//
//        public static IO<A> a<A>(Func<A> fn) => new IO<A>(fn);
//        public static IO<Unit> a(Action action) => new IO<Unit>(() => {
//            action();
//            return new Unit();
//        });
//    }

    public struct IO<A>
    {
        readonly Func<A> func;
        public IO(Func<A> func) { this.func = func; }
        public A __unsafePerformIO() => func();

        public IO<B> flatMap<B>(Func<A, IO<B>> mapper) {
            var fn = this.func;
            return new IO<B>(() => mapper(fn()).__unsafePerformIO());
        }
    }
}