using System;

namespace funkylib {
  /// <summary>
  /// Represents a value of one of two possible types (a disjoint union).
  /// An instance of `Either` holds <see cref="Left{L}"/> or <see cref="Right{R}"/>.
  ///
  /// A common use of `Either` is as an alternative to <see cref="funkylib.Option"/> for dealing
  /// with possibly missing values.  In this usage, <see cref="Option.None"/> is replaced
  /// with a <see cref="Left{L}"/> which can contain useful information.
  /// <see cref="Right{R}"/> takes the place of <see cref="Option.Some{A}"/>.
  /// Convention dictates that `Left` is used for failure and `Right` is used for success.
  /// </summary>
  ///
  /// <example>
  /// <code>
  ///   var either = Either.Right(10);
  ///   var result = either.map(number => number * number);
  ///   result.voidFold(error => Console.Write(error), number => Console.Write(number));
  /// </code>
  /// </example>
  public static class Either {
    public static Left<L> Left<L>(L l) => new Left<L>(l);
    public static Right<R> Right<R>(R r) => new Right<R>(r);
  }

    public struct Either<L, R> {
      readonly bool isRight;
      L left { get; }
      R right { get; }

      /// <summary>
      /// Returns `Left` value of Either.
      /// Might throw an exception if `Either` holds `Right`.
      /// </summary>
      public L __unsafeGetLeft => left;

      /// <summary>
      /// Returns `Right` value of Either.
      /// Might throw an exception if `Either` holds `Left`.
      /// </summary>
      public R __unsafeGetRight => right;

      public bool isLeft => !isRight;

      Either(L left) {
        isRight = false;
        this.left = left;
        right = default;
      }

      Either(R right) {
        isRight = true;
        this.right = right;
        left = default;
      }

      /// <summary>
      /// Applies `fa` if this is a `Left` or `fb` if this is a `Right`.
      /// </summary>
      /// <param name="fa">The function to apply if this is a `Left`</param>
      /// <param name="fb">The function to apply if this is a `Right`</param>
      /// <returns>The results of applying the function</returns>
      public AR fold<AR>(Func<L, AR> fa, Func<R, AR> fb) => isLeft ? fa(left) : fb(right);

      /// <summary>
      /// Applies `fa` if this is a `Left` or `fb` if this is a `Right`.
      /// </summary>
      /// <param name="fa">The action to perform if this is a `Left`</param>
      /// <param name="fb">The action to perform if this is a `Right`</param>
      /// <returns>Unit type <see cref="Unit"/> a.k.a void</returns>
      public Unit fold(Action<L> fa, Action<R> fb) => fold(fa.toFunc(), fb.toFunc());

      public static implicit operator Either<L, R>(L left) => new Either<L, R>(left);
      public static implicit operator Either<L, R>(R right) => new Either<L, R>(right);
      public static implicit operator Either<L, R>(Left<L> left) => new Either<L, R>(left.value);
      public static implicit operator Either<L, R>(Right<R> right) => new Either<L, R>(right.value);

      public override string ToString() => fold(l => $"Left({l})", r => $"Right({r})");
    }

    public struct Left<L> {
      internal Left(L value) { this.value = value; }
      internal L value { get; }

      public override string ToString() => $"Left{value}";
    }

    public struct Right<R> {
      internal Right(R value) { this.value = value; }
      internal R value { get; }

      public override string ToString() => $"Right{value}";

      public Right<RR> map<RR>(Func<R, RR> f) => f(value).right();
      public Either<L, RR> flatMap<L, RR>(Func<R, Either<L, RR>> func) => func(value);
    }

    public static class EitherExt {
      public static Left<L> left<L>(this L @this) => new Left<L>(@this);
      public static Right<R> right<R>(this R @this) => new Right<R>(@this);

      /// <summary>
      /// The given function is applied if this is a `Right`.
      /// </summary>
      /// <returns></returns>
      public static Either<L, RR> map<L, R, RR>(this Either<L, R> @this, Func<R, RR> right) =>
        @this.fold<Either<L, RR>>(
          _ => _,
          r => right(r).right()
        );

      /// <summary>
      /// The given function is applied if this is a `Left`.
      /// </summary>
      /// <returns></returns>
      public static Either<LL, R> leftMap<L, LL, R>(this Either<L, R> @this, Func<L, LL> left)=>
        @this.fold<Either<LL, R>>(
          l => left(l).left(),
          _ => _
        );

      /// <summary>
      /// Binds the given function across `Right`.
      /// </summary>
      public static Either<L, RR> flatMap<L, R, RR>(this Either<L, R> @this, Func<R, Either<L, RR>> func) =>
        @this.fold(l => Either.Left(l), func);

      /// <summary>
      /// Returns the value from this `Right` or the given argument if this is a `Left`.
      /// </summary>
      public static Either<L, R> orElse<L, R>(this Either<L, R> @this, Func<Either<L, R>> optional) =>
        @this.isLeft ? optional() : @this;
    }
}