using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickRanking.Common {
    public static class TaskUtil {
        public static async Task WaitUntil(Func<bool> predicate, CancellationToken cancellationToken) {
            while(predicate() == false) {
                await Task.Yield();
            }
        }
    }
}