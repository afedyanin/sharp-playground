using System.Threading.Tasks.Dataflow;

namespace DataFlowSample
{
    public static class BufferBlockExample1
    {
        static public void Run()
        {

            var bufferBlock = new BufferBlock<int>();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Posting {i} Thread={Environment.CurrentManagedThreadId}");
                bufferBlock.Post(i);
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Receiving {i} Thread={Environment.CurrentManagedThreadId}");
                int result = bufferBlock.Receive();
                Console.WriteLine(result);
            }

            Console.WriteLine("Done");
        }
    }
}
