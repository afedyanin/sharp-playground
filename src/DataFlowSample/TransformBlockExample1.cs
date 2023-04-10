using System.Threading.Tasks.Dataflow;

namespace DataFlowSample
{
    public static class TransformBlockExample1
    {
        static public async Task Run()
        {
            Func<int, int> fn = n => 
            {
                Console.WriteLine($"Inside function Thread={Environment.CurrentManagedThreadId}");
                Thread.Sleep(1000);
                return n * n;
            };

            var tfBlock = new TransformBlock<int, int>(fn);

            Console.WriteLine("Start Post");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Posting {i} Thread={Environment.CurrentManagedThreadId}");
                tfBlock.Post(i);
            }

            Console.WriteLine("Start Receive");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Receiving {i} Thread={Environment.CurrentManagedThreadId}");
                int result = await tfBlock.ReceiveAsync();
                Console.WriteLine(result);
            }

            Console.WriteLine($"Done Thread={Environment.CurrentManagedThreadId}");
        }
    }
}
