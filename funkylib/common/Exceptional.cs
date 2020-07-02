﻿using System;

namespace funkylib.common {
  /// <summary>
  /// <![CDATA[Either<Exception, A> analogue.]]> <see cref="Either"/>
  /// Represents a computation that may either result in an exception, or return a successfully computed value.
  /// <example>
  /// <code>
  /// <![CDATA[
  /// try {
  ///   return IO.@return(() => new Exceptional<StreamReader>(System.IO.File.OpenText(path)));
  /// }
  /// catch (Exception ex) {
  ///   return IO.@return(() => new Exceptional<StreamReader>(ex));
  /// }
  /// ]]>
  /// </code>
  /// </example>
  /// </summary>
  public struct Exceptional<A> {
    internal readonly Exception ex;
    internal readonly A value;

    public bool success => ex == null;
    public bool exception => ex != null;

    public Exceptional(Exception ex) {
      this.ex = ex;
      value = default;
    }

    public Exceptional(A right) {
      value = right;
      ex = null;
    }

    public static implicit operator Exceptional<A>(Exception left) => new Exceptional<A>(left);
    public static implicit operator Exceptional<A>(A right) => new Exceptional<A>(right);

    /// <summary>
    /// Applies `fa` if this is a `Exception` or `fb` if this is a `value`.
    /// </summary>
    /// <param name="fa">The function to apply if this is a `Exception`</param>
    /// <param name="fb">The function to apply if this is a `value`</param>
    /// <returns>The results of applying the function</returns>
    public AR fold<AR>(Func<Exception, AR> fa, Func<A, AR> fb) => exception ? fa(ex) : fb(value);

    /// <summary>
    /// Applies `fa` if this is a `Exception` or `fb` if this is a `value`.
    /// </summary>
    /// <param name="fa">The action to perform if this is a `Exception`</param>
    /// <param name="fb">The action to perform if this is a `value`</param>
    /// <returns>Unit type <see cref="Unit"/> a.k.a void</returns>
    public Unit fold(Action<Exception> fa, Action<A> fb) => fold(fa.toFunc(), fb.toFunc());
  }

  public static class ExceptionalExt {
    /// <summary>
    /// The given function is applied if this is a `value`.
    /// </summary>
    public static Exceptional<RR> map<R, RR>(this Exceptional<R> @this, Func<R, RR> func) => @this.success
        ? func(@this.value)
        : new Exceptional<RR>(@this.ex);

    /// <summary>
    /// Binds the given function across `value`.
    /// </summary>
    public static Exceptional<RR> flatMap<R, RR>(this Exceptional<R> @this, Func<R, Exceptional<RR>> func) =>
        @this.success ? func(@this.value) : new Exceptional<RR>(@this.ex);
  }
}