using System;
using System.Collections.Generic;
using System.Linq;

namespace funkylib.Data
{
    public sealed class FunkyList<T>
    {
        readonly T head;
        readonly bool isEmpty;
        readonly FunkyList<T> tail;

        // the empty list
        internal FunkyList() { isEmpty = true; }

        // the non empty list
        internal FunkyList(T head, FunkyList<T> tail) {
            this.head = head;
            this.tail = tail;
        }

        // for convenience
        public T Head => Match(
            () => throw new IndexOutOfRangeException(),
            (head, _) => head);

        public FunkyList<T> Tail => Match(
            () => throw new IndexOutOfRangeException(),
            (_, tail) => tail);

        // not really required, but hey...
        public T this[int index] => Match(
            () => throw new IndexOutOfRangeException(),
            (head, tail) => index == 0 ? head : tail[index - 1]);

        public R Match<R>(Func<R> Empty, Func<T, FunkyList<T>, R> Cons)
            => isEmpty ? Empty() : Cons(head, tail);

        public IEnumerable<T> AsEnumerable() {
            if (isEmpty) yield break;
            yield return head;
            foreach (var t in tail.AsEnumerable()) yield return t;
        }

        public override string ToString() => Match(
            () => "{ }",
            (_, __) => $"{{ {string.Join(", ", AsEnumerable().map(v => v.ToString()))} }}");
    }

    public static class FunkyList
    {
        // factory functions
        public static FunkyList<T> List<T>(T h, FunkyList<T> t) => new FunkyList<T>(h, t);

        public static FunkyList<T> List<T>(params T[] items)
            => items.Reverse().Aggregate(
                new FunkyList<T>()
                , (tail, head) => List(head, tail));

        public static int Length<T>(this FunkyList<T> @this) => @this.Match(
            () => 0,
            (t, ts) => 1 + ts.Length());

        public static FunkyList<T> Add<T>(this FunkyList<T> @this, T value)
            => List(value, @this);

        public static FunkyList<T> Append<T>(this FunkyList<T> @this, T value)
            => @this.Match(
                () => List(value, List<T>()),
                (head, tail) => List(head, tail.Append(value)));

        public static FunkyList<A> insertAt<A>(this FunkyList<A> @this, int index, A value) =>
            index == 0
                ? List(value, @this)
                : @this.Match(
                    () => throw new IndexOutOfRangeException(),
                    (head, tail) => List(head, tail.insertAt(index - 1, value)));

        public static FunkyList<A> removeAt<A>(this FunkyList<A> @this, int index) =>
            index == 0
                ? @this.Tail
                : @this.Match(
                    () => throw new IndexOutOfRangeException(),
                    (head, tail) => List(head, tail.removeAt(index - 1)));

        public static FunkyList<A> takeWhile<A>(this FunkyList<A> @this, Func<A, bool> predicate) =>
            @this.Match(
                () => @this, (head, tail)
                    => predicate(head) ? List(head, tail.takeWhile(predicate)) : List<A>());

        public static FunkyList<A> takeWhile<A>(this IEnumerable<A> @this, Func<A, bool> predicate) {
            var temp = List<A>();
            foreach (var item in @this)
                if (predicate(item))
                    temp.Add(item);
            return temp;
        }

        public static FunkyList<A> dropWhile<A>(this FunkyList<A> @this, Func<A, bool> predicate) =>
            @this.Match(() => @this, (head, tail) => predicate(head) ? tail.dropWhile(predicate) : @this);

        public static FunkyList<R> Map<T, R>(this FunkyList<T> @this, Func<T, R> f)
            => @this.Match(
                () => List<R>(),
                (head, tail) => List(f(head), tail.Map(f)));

        public static Unit ForEach<T>(this FunkyList<T> @this, Action<T> action) {
            @this.Map(action.toFunc());
            return new Unit();
        }

        public static FunkyList<R> Bind<T, R>(this FunkyList<T> @this, Func<T, FunkyList<R>> f)
            => @this.Map(f).Join();

        public static FunkyList<T> Join<T>(this FunkyList<FunkyList<T>> @this) => @this.Match(
            () => List<T>(),
            (xs, xss) => concat(xs, Join(xss)));

        public static Acc Aggregate<A, Acc>(this FunkyList<A> @this, Acc acc, Func<Acc, A, Acc> func) =>
            @this.Match(() => acc, (head, tail) => Aggregate(tail, func(acc, head), func));

        static FunkyList<T> concat<T>(FunkyList<T> l, FunkyList<T> r) => l.Match(
            () => r,
            (h, t) => List(h, concat(t, r)));
    }
}