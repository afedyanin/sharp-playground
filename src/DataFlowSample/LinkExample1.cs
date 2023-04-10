using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowSample
{
    public static class LinkExample1
    {
        static public void Run()
        {
            var bufferBlock = new BufferBlock<int>();
            var printBlock = new ActionBlock<int>(
            n => Console.WriteLine(n)
            );

            for (int i = 0; i < 10; i++)
            {
                bufferBlock.Post(i);
            }

            bufferBlock.LinkTo(printBlock);

            Console.WriteLine("Done");
        }
    }
}
