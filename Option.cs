﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace funkylib
{
    public struct Unit
    {
    }

    public struct Option
    {
        public static Option<A> Some<A>(A value) => new Some<A>(value);

        public static None None => None.none;
    }

    public struct Option<A> : IEquatable<Option<A>>, IEquatable<None>
    {
        readonly A value;
        public readonly bool isSome;
        public bool isNone => !isSome;

        internal Option(A value) {
            if (value == null) throw new ArgumentNullException();
            isSome = true;
            this.value = value;
        }
        public A _unsafe => value;
        public R fold<R>(Func<R> onNone, Func<A, R> onSome) => isSome ? onSome(value) : onNone();

        public void fold(Action onNone, Action<A> onSome) {
            if (isSome)
                onSome(value);
            else
                onNone();
        }

        /// <summary>
        ///     Calls on some action
        /// </summary>
        /// <param name="action">Action to perform</param>
        public void each(Action<A> action) {
            if (isSome) action(value);
        }


        public static implicit operator Option<A>(None _) => new Option<A>();

        public static implicit operator Option<A>(Some<A> some) => new Option<A>(some.value);

        public static implicit operator Option<A>(A value) => value == null ? Option.None : Option.Some(value);

        public bool Equals(Option<A> other) => isSome == other.isSome
            && (isNone || value.Equals(other.value));

        public bool Equals(None _) => isNone;

        public Option<C> zip<B, C>(Option<B> opt2, Func<A, B, C> mapper) => isSome && opt2.isSome
            ? Option.Some(mapper(value, opt2.value))
            : Option.None;

        public Option<D> zip<B, C, D>(Option<B> opt2, Option<C> opt3, Func<A, B, C, D> mapper) => isSome &&
            opt2.isSome && opt3.isSome
                ? Option.Some(mapper(value, opt2.value, opt3.value))
                : Option.None;

        public override string ToString() => isSome ? $"Some({value})" : "None";
        public static Option<A> operator |(Option<A> left, Option<A> right) => left.isSome ? left : right;
    }

    public struct Some<A>
    {
        internal A value;
        internal Some(A value) { this.value = value; }
    }

    public struct None
    {
        internal static readonly None none = new None();
    }

    public static class OptionExt
    {
        public static Option<A> some<A>(this A value) => new Option<A>(value);

        public static Option<R> map<A, R>(this Option<A> @this, Func<A, R> func) {
            return @this.fold(
                () => Option.None, value => Option.Some(func(value)));
        }

     public static Option<C> orElse<A,B,C>(this Option<A> @this, Func<Option<B>> optional, Func<A,C> mapThis, Func<B,C> mapAnother) {
      if (@this.isSome)
        return mapThis(@this._unsafe);
      else {
        var another = optional();
        if (another.isSome)
          return mapAnother(another._unsafe);
        else
          return Option.None;
      }
    }

    public static bool exist<A>(this Option<A> @this, Option<A> other) =>
      @this.fold(
        () => false,
        left => other.fold(
          () => false,
          right => left.Equals(right)
        )
      );

        public static Option<R> flatMap<A, R>(this Option<A> @this, Func<A, Option<R>> func) {
            return @this.fold(
                () => Option.None, func);
        }

        public static IEnumerable<A> asEnumerable<A>(this IEnumerable<Option<A>> @this) {
            var temp = new List<A>();
            foreach (var option in @this)
                option.fold(() => { }, some => temp.Add(some));
            return temp;
        }

        public static Option<R> apply<A, R>(this Option<Func<A, R>> @this, Option<A> option) => @this.fold(
            () => Option.None, func => option.fold(
                () => Option.None, a => Option.Some(func(a))));

        public static Option<Func<B, R>> apply<A, B, R>(this Option<Func<A, B, R>> @this, Option<A> option) => apply(
            @this.map(F.curry), option);
    /// <summary>
    /// Maps every collection member to option and agreggates to collection of options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="this"></param>
    /// <param name="func"></param>
    /// <returns>Option<Collection> if all collection members is some</returns>
      public static Option<IEnumerable<R>> traverse<T, R>(this IEnumerable<T> @this, Func<T, Option<R>> func) =>
        @this.Aggregate(
          Enumerable.Empty<R>().some(),
          (optResult, t) => 
            from results in optResult
            from result in func(t)
            select results.append(result)
        );

    /*LINQ*/
    public static Option<R> Select<A, R>(this Option<A> @this, Func<A, R> func) => @this.map(func);

        public static Option<RR> SelectMany<T, R, RR>
            (this Option<T> opt, Func<T, Option<R>> bind, Func<T, R, RR> project)
            => opt.fold(
                () => Option.None,
                t => bind(t).fold(
                    () => Option.None,
                    r => Option.Some(project(t, r))));

        public static Option<A> Where<A>(this Option<A> @this, Func<A, bool> predicate) =>
            @this.fold(() => Option.None, a => predicate(a) ? @this : Option.None);
    }
}