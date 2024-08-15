using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickRanking.Common {
    public static class TaskExtension {
        public static void Forget(this Task task) {
            _ = TryTaskAsync(task);
        }

        private static async Task TryTaskAsync(Task task) {
            try {
                await task;
            }
            catch(OperationCanceledException) {}
            catch(Exception e) {
                Debug.LogError(e);
                throw;
            }
        }
    }
}