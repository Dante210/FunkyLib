using System;

namespace funkylib
{
    public static class Either
    {
        public static Left<L> Left<L>(L l) => new Left<L>(l);
        public static Right<R> Right<R>(R r) => new Right<R>(r);
    }

    public struct Either<L, R>
    {
        internal L left { get; }
        internal R right { get; }
        bool isRight { get; }
        bool isLeft => !isRight;

        internal Either(L left) {
            isRight = false;
            this.left = left;
            right = default(R);
        }

        internal Either(R right) {
            isRight = true;
            this.right = right;
            left = default(L);
        }

        public static implicit operator Either<L, R>(L left) => new Either<L, R>(left);
        public static implicit operator Either<L, R>(R right) => new Either<L, R>(right);
        public static implicit operator Either<L, R>(Left<L> left) => new Either<L, R>(left.value);
        public static implicit operator Either<L, R>(Right<R> right) => new Either<L, R>(right.value);

        public AR fold<AR>(Func<L, AR> left, Func<R, AR> right) => isLeft ? left(this.left) : right(this.right);

        public Unit fold(Action<L> left, Action<R> right) => fold(left.toFunc(), right.toFunc());
        public override string ToString() => fold(l => $"Left({l})", r => $"Right({r})");
    }

    public struct Left<L>
    {
        internal Left(L value) { this.value = value; }
        internal L value { get; }
        public override string ToString() => $"Left{value}";
    }

    public struct Right<R>
    {
        internal Right(R value) { this.value = value; }
        internal R value { get; }
        public override string ToString() => $"Right{value}";

        public Right<RR> map<L, RR>(Func<R, RR> f) => f(value).right(); //?????????
        public Either<L, RR> flatMap<L, RR>(Func<R, Either<L, RR>> func) => func(value);
    }

    public static class EitherExt
    {
        public static Left<L> left<L>(this L @this) => new Left<L>(@this);
        public static Right<R> right<R>(this R @this) => new Right<R>(@this);

        public static Either<L, RR> flatMap<L, R, RR>(this Either<L, R> @this, Func<R, Either<L, RR>> func) =>
            @this.fold(l => Either.Left(l), func);


        public static Either<LL, RR> map<L, LL, R, RR>
            (this Either<L, R> @this, Func<L, LL> left, Func<R, RR> right)
            => @this.fold<Either<LL, RR>>(
                l => left(l).left(),
                r => right(r).right());

    }
}