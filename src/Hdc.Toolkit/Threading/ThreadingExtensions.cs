using System.Diagnostics;
using System.Threading.Tasks;

namespace Hdc.Windows.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

 

    public static class ThreadingExtensions
    {
        public static TResult Dispatch<TResult>(this DispatcherObject source, Func<TResult> func)
        {
            if (source.Dispatcher.CheckAccess())
                return func();

            return (TResult)source.Dispatcher.Invoke(func);
        }

        public static TResult Dispatch<T, TResult>(this T source, Func<T, TResult> func) where T : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                return func(source);

            return (TResult)source.Dispatcher.Invoke(func, source);
        }

        public static TResult Dispatch<TSource, T, TResult>(
                this TSource source, Func<TSource, T, TResult> func, T param1) where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                return func(source, param1);

            return (TResult)source.Dispatcher.Invoke(func, source, param1);
        }

        public static TResult Dispatch<TSource, T1, T2, TResult>(
                this TSource source, Func<TSource, T1, T2, TResult> func, T1 param1, T2 param2)
                where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                return func(source, param1, param2);

            return (TResult)source.Dispatcher.Invoke(func, source, param1, param2);
        }

        public static TResult Dispatch<TSource, T1, T2, T3, TResult>(
                this TSource source, Func<TSource, T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
                where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                return func(source, param1, param2, param3);

            return (TResult)source.Dispatcher.Invoke(func, source, param1, param2, param3);
        }

        public static void Dispatch(this DispatcherObject source, Action func)
        {
            if (source.Dispatcher.CheckAccess())
                func();
            else
                source.Dispatcher.Invoke(func);
        }

        public static void Dispatch<TSource>(this TSource source, Action<TSource> func) where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                func(source);
            else
                source.Dispatcher.Invoke(func, source);
        }

        public static void Dispatch<TSource, T1>(this TSource source, Action<TSource, T1> func, T1 param1)
                where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                func(source, param1);
            else
                source.Dispatcher.Invoke(func, source, param1);
        }

        public static void Dispatch<TSource, T1, T2>(
                this TSource source, Action<TSource, T1, T2> func, T1 param1, T2 param2)
                where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                func(source, param1, param2);
            else
                source.Dispatcher.Invoke(func, source, param1, param2);
        }

        public static void Dispatch<TSource, T1, T2, T3>(
                this TSource source, Action<TSource, T1, T2, T3> func, T1 param1, T2 param2, T3 param3)
                where TSource : DispatcherObject
        {
            if (source.Dispatcher.CheckAccess())
                func(source, param1, param2, param3);
            else
                source.Dispatcher.Invoke(func, source, param1, param2, param3);
        }

        //        public static TResult Dispatch<T, TResult>(this T source, Func<T, TResult> func) where T : DispatcherObject
        //        {
        //            if (source.Dispatcher.CheckAccess())
        //                return func(source);
        //
        //            return (TResult)source.Dispatcher.Invoke(func, source);
        //        }
        public static TResult Dispatch<T1, T2, TResult>(
                this Dispatcher dispatcher, Func<T1, T2, TResult> func, T1 param1, T2 param2)
        {
            return (TResult)dispatcher.Invoke(func, param1, param2);
        }

        public static TResult Dispatch<T1, T2, T3, TResult>(
                this Dispatcher dispatcher, Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
        {
            return (TResult)dispatcher.Invoke(func, param1, param2, param3);
        }


        public static bool WaitUntilTrue(this Func<bool> func, int timeout, int timeToCheck)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                if (func())
                {
                    return true;
                }
                Thread.Sleep(timeToCheck);
            }
            return false;
        }

        public static Task SleepAsync(int millisecondsTimeout)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(millisecondsTimeout);
            });
        }

    }
}