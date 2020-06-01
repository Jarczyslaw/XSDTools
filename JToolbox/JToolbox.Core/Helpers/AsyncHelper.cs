using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JToolbox.Core.Helpers
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory taskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func)
            => taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static Task ForEach<TItem>(IEnumerable<TItem> input, Func<TItem, CancellationToken, Task> handler)
        {
            return ForEach(input, handler, CancellationToken.None);
        }

        public static async Task ForEach<TItem>(IEnumerable<TItem> input, Func<TItem, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var item in input)
            {
                tasks.Add(handler(item, cancellationToken));
            }
            await Task.WhenAll(tasks);
        }

        public static Task<IEnumerable<KeyValuePair<TItem, TResult>>> ForEachWithResult<TItem, TResult>
            (IEnumerable<TItem> input, Func<TItem, CancellationToken, Task<TResult>> handler)
        {
            return ForEachWithResult(input, handler, CancellationToken.None);
        }

        public static async Task<IEnumerable<KeyValuePair<TItem, TResult>>> ForEachWithResult<TItem, TResult>
            (IEnumerable<TItem> input, Func<TItem, CancellationToken, Task<TResult>> handler, CancellationToken cancellationToken)
        {
            var tasks = new List<KeyValuePair<TItem, Task<TResult>>>();
            foreach (var item in input)
            {
                tasks.Add(new KeyValuePair<TItem, Task<TResult>>(item, handler(item, cancellationToken)));
            }
            await Task.WhenAll(tasks.Select(p => p.Value));
            return tasks.Select(p => new KeyValuePair<TItem, TResult>(p.Key, p.Value.Result)).ToList();
        }

        public static Task<T> AsyncCallback<T>(Action<Action<T>> methodWithCallback)
        {
            var tcs = new TaskCompletionSource<T>();
            try
            {
                methodWithCallback(t => tcs.SetResult(t));
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }
    }
}