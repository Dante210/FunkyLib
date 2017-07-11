using System;

namespace funkylib
{
    public struct Exceptional<A>
    {
        internal Exception ex { get; }
        internal A value;

        public bool success => ex == null;
        public bool exception => ex != null;

        public Exceptional(Exception ex) {
            this.ex = ex;
            value = default(A);
        }

        public Exceptional(A right) {
            value = right;
            ex = null;
        }

        public static implicit operator Exceptional<A>(Exception left) => new Exceptional<A>(left);
        public static implicit operator Exceptional<A>(A right) => new Exceptional<A>(right);

        public AR fold<AR>(Func<Exception, AR> exception, Func<A, AR> success) => this.exception
            ? exception(ex)
            : success(value);

        public Unit fold(Action<Exception> exception, Action<A> success) => fold(exception.toFunc(), success.toFunc());
    }

    public static class ExceptionalExt
    {
        public static Exceptional<R> apply<A, R>(this Exceptional<Func<A, R>> @this, Exceptional<A> arg)
            => @this.fold(
                ex => ex,
                func =>
                    arg.fold(
                        ex => ex,
                        a => new Exceptional<R>(func(a))));

        public static Exceptional<RR> map<R, RR>(this Exceptional<R> @this, Func<R, RR> func) => @this.success
            ? func(@this.value)
            : new Exceptional<RR>(@this.ex);

        public static Exceptional<RR> flatMap<R, RR>(this Exceptional<R> @this, Func<R, Exceptional<RR>> func) =>
            @this.success ? func(@this.value) : new Exceptional<RR>(@this.ex);

        public static Exceptional<Unit> each<R>(this Exceptional<R> @this, Action<R> action) => map(
            @this, action.toFunc());
    }
}