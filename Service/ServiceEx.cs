using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public static class ServiceEx
    {
        /// <summary>
        /// Extension method which waits for the concurrent queue to dequeue.
        /// </summary>
        public static T Dequeue<T>(this ConcurrentQueue<T> queue)
        {
            T request;
            if (queue.TryDequeue(out request))
                return request;
            
            // If we don't have any messages, wait a bit and continue
            Thread.Sleep(50);
            return default(T);
        }
    }
}
