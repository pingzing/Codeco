using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.Extensions
{
    public static class SynchronizationContextExtensions
    {
        /// <summary>
        /// Returns a reference to the UI thread. Useful when using Reactive Extensions.
        /// </summary>
        /// <returns></returns>
        public static async Task<SynchronizationContext> GetUIThreadAsync()
        {
            TaskCompletionSource<bool> done = new TaskCompletionSource<bool>();
            SynchronizationContext context = null;
            Device.BeginInvokeOnMainThread(() =>
            {
                context = SynchronizationContext.Current;
                done.SetResult(true);
            });

            await done.Task;
            return context;
        }
    }
}
