using System;
using System.Collections.Generic;
using System.Linq;

namespace funkylib.Data
{
    public sealed class LinkedList<T>
    {
        readonly T head;
        readonly bool isEmpty;
        readonly LinkedList<T> tail;

        // the empty list
        internal LinkedList() { isEmpty = true; }

        // the non empty list
        internal LinkedList(T head, LinkedList<T> tail) {
            this.head = head;
            this.tail = tail;
        }

        // for convenience
        public T Head => fold(
            () => throw new IndexOutOfRangeException(),
            (head, _) => head);

        public LinkedList<T> Tail => fold(
            () => throw new IndexOutOfRangeException(),
            (_, tail) => tail);

        // not really required, but hey...
        public T this[int index] => fold(
            () => throw new IndexOutOfRangeException(),
            (head, tail) => index == 0 ? head : tail[index - 1]);

        public R fold<R>(Func<R> empty, Func<T, LinkedList<T>, R> cons)
            => isEmpty ? empty() : cons(head, tail);

        public IEnumerable<T> asEnumerable() {
            if (isEmpty) yield break;
            yield return head;
            foreach (var t in tail.asEnumerable()) yield return t;
        }

        public override string ToString() => fold(
            () => "{ }",
            (_, __) => $"{{ {string.Join(", ", asEnumerable().map(v => v.ToString()))} }}");
    }

    public static class LinkedList
    {
        // factory functions
        public static LinkedList<T> create<T>(T h, LinkedList<T> t) => new LinkedList<T>(h, t);

        public static LinkedList<T> create<T>(params T[] items)
            => items.Reverse().Aggregate(
                new LinkedList<T>()
                , (tail, head) => create(head, tail));

        public static int Length<T>(this LinkedList<T> @this) => @this.fold(
            () => 0,
            (t, ts) => 1 + ts.Length());

        public static LinkedList<T> add<T>(this LinkedList<T> @this, T value)
            => create(value, @this);

        public static LinkedList<T> append<T>(this LinkedList<T> @this, T value)
            => @this.fold(
                () => create(value, create<T>()),
                (head, tail) => create(head, tail.append(value)));

        public static LinkedList<A> insertAt<A>(this LinkedList<A> @this, int index, A value) =>
            index == 0
                ? create(value, @this)
                : @this.fold(
                    () => throw new IndexOutOfRangeException(),
                    (head, tail) => create(head, tail.insertAt(index - 1, value)));

        public static LinkedList<A> removeAt<A>(this LinkedList<A> @this, int index) =>
            index == 0
                ? @this.Tail
                : @this.fold(
                    () => throw new IndexOutOfRangeException(),
                    (head, tail) => create(head, tail.removeAt(index - 1)));

        public static LinkedList<A> takeWhile<A>(this LinkedList<A> @this, Func<A, bool> predicate) =>
            @this.fold(
                () => @this, (head, tail)
                    => predicate(head) ? create(head, tail.takeWhile(predicate)) : create<A>());

        public static LinkedList<A> takeWhile<A>(this IEnumerable<A> @this, Func<A, bool> predicate) {
            var temp = create<A>();
            foreach (var item in @this)
                if (predicate(item))
                    temp.add(item);
            return temp;
        }

        public static LinkedList<A> dropWhile<A>(this LinkedList<A> @this, Func<A, bool> predicate) =>
            @this.fold(() => @this, (head, tail) => predicate(head) ? tail.dropWhile(predicate) : @this);

        public static LinkedList<R> map<T, R>(this LinkedList<T> @this, Func<T, R> f)
            => @this.fold(
                () => create<R>(),
                (head, tail) => create(f(head), tail.map(f)));

        public static Unit forEach<T>(this LinkedList<T> @this, Action<T> action) {
            @this.map(action.toFunc());
            return new Unit();
        }

        public static LinkedList<R> flatmap<T, R>(this LinkedList<T> @this, Func<T, LinkedList<R>> f)
            => @this.map(f).Join();

        public static LinkedList<T> Join<T>(this LinkedList<LinkedList<T>> @this) => @this.fold(
            () => create<T>(),
            (xs, xss) => concat(xs, Join(xss)));

        public static Acc Aggregate<A, Acc>(this LinkedList<A> @this, Acc acc, Func<Acc, A, Acc> func) =>
            @this.fold(() => acc, (head, tail) => Aggregate(tail, func(acc, head), func));

        static LinkedList<T> concat<T>(LinkedList<T> l, LinkedList<T> r) => l.fold(
            () => r,
            (h, t) => create(h, concat(t, r)));
    }
}