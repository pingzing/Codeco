using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace Codeco.CrossPlatform.Extensions.Reactive
{
    public static class DistinctUntilTimeoutExtension
    {
        public static IObservable<TSource> DistinctUntilTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout)
        {            
            return DistinctUntilTimeout(source, timeout, Scheduler.Default);
        }

        public static IObservable<TSource> DistinctUntilTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout, IScheduler scheduler)
        {
            return DistinctUntilTimeout(source, timeout, EqualityComparer<TSource>.Default, scheduler);
        }

        public static IObservable<TSource> DistinctUntilTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout, IEqualityComparer<TSource> comparer)
        {
            return DistinctUntilTimeout(source, timeout, comparer, Scheduler.Default);
        }

        public static IObservable<TSource> DistinctUntilTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout, IEqualityComparer<TSource> comparer, IScheduler scheduler)
        {
            var toReturn = source
                .Timestamp(scheduler)
                .StateWhere(
                    new Dictionary<TSource, DateTimeOffset>(comparer),
                    (state, item) => item.Value,
                    (state, item) => state.Where(kvp => item.Timestamp - kvp.Value < timeout)
                    .Concat(
                            !state.ContainsKey(item.Value) || item.Timestamp - state[item.Value] >= timeout
                            ? Enumerable.Repeat(new KeyValuePair<TSource, DateTimeOffset>(item.Value, item.Timestamp), 1)
                            : Enumerable.Empty<KeyValuePair<TSource, DateTimeOffset>>()
                    )
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer),
                (state, item) => !state.ContainsKey(item.Value) || item.Timestamp - state[item.Value] >= timeout
            );
            return toReturn;
        }

        public static IObservable<TResult> StateWhere<TSource, TState, TResult>(
            this IObservable<TSource> source,
            TState initialState,
            Func<TState, TSource, TResult> resultSelector,
            Func<TState, TSource, TState> stateSelector,
            Func<TState, TSource, bool> filter)
        {
            return source
                .StateSelectMany(initialState, (state, item) =>
                    filter(state, item) ? Observable.Return(resultSelector(state, item)) : Observable.Empty<TResult>(),
                stateSelector);
        }

        public static IObservable<TResult> StateSelectMany<TSource, TState, TResult>(
            this IObservable<TSource> source,
            TState initialState,
            Func<TState, TSource, IObservable<TResult>> resultSelector,
            Func<TState, TSource, TState> stateSelector)
        {
            return source
                .Scan(
                    (InitialState: initialState, ResultList: Observable.Empty<TResult>()), 
                    (state, item) => (stateSelector(state.InitialState, item), resultSelector(state.InitialState, item))
                )
                .SelectMany(t => t.ResultList);
        }
    }
}
