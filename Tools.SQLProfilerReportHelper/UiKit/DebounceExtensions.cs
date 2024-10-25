using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.SQLProfilerReportHelper.UiKit
{
    public static class DebounceExtensions
    {
        public static Action<T> Debounce<T>(this Action<T> action, int milliseconds = 300)
        {
            CancellationTokenSource cancelTokenSource = null;

            return arg =>
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource = new CancellationTokenSource();

                Task.Delay(milliseconds, cancelTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompleted && !t.IsFaulted)
                        {
                            action(arg);
                        }
                    }, TaskScheduler.Default);
            };
        }
    }
}
