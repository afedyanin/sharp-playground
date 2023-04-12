using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace IntegrationPatterns
{
    public static class MessageAggregatorExample
    {
        public static async Task Run()
        {
            var actionBlock = new ActionBlock<(int, string)>(item =>
            {
                Console.WriteLine($"Receving tuple ({item.Item1},{item.Item2}). Thread={Environment.CurrentManagedThreadId}");
            });

            var ma = new MessageAggregator(actionBlock);

            var t1 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(10);
                    ma.PostNumber(i);
                }
            });

            var t2 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    ma.PostTerm($"T-{i}");
                    await Task.Delay(20);
                }
            });

            await Task.WhenAll(t1, t2);

            Console.WriteLine("Done.");
        }
        public static async Task RunNoLock()
        {
            var actionBlock = new ActionBlock<(int, string)>(item =>
            {
                Console.WriteLine($"Receving tuple ({item.Item1},{item.Item2}). Thread={Environment.CurrentManagedThreadId}");
            });

            var ma = new MessageAggregatorNoLock(actionBlock);

            var t1 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(10);
                    ma.PostNumber(i);
                }
            });

            var t2 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    ma.PostTerm($"T-{i}");
                    await Task.Delay(20);
                }
            });

            await Task.WhenAll(t1, t2);

            Console.WriteLine("Done.");
        }

        private static void RunInternal()
        {
            // TODO: Implement this
        }
    }
}
