using System.Threading.Tasks.Dataflow;

namespace DataFlowSample
{
    public static  class BatchBlockExample1
    {
        static public void Run()
        {

            var batchBlock = new BatchBlock<int>(2);

            for (int i = 0; i < 10; i++)
            {
                batchBlock.Post(i);
            }

            for (int i = 0; i < 5; i++)
            {
                int[] result = batchBlock.Receive();
                foreach (var r in result)
                {
                    Console.Write(r + " ");
                }
                Console.Write("\n");
            }

            Console.WriteLine("Done");
        }
    }
}
