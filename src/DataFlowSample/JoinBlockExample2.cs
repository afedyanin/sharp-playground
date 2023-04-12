using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowSample
{
    public static  class JoinBlockExample2
    {
        static public void Run()
        {
            var jBlock = new BatchedJoinBlock<int, int>(1);

            jBlock.Target1.Post(1);
            jBlock.Target2.Post(1);
            // (1,1)
            var item = jBlock.Receive();

            jBlock.Target1.Post(2);
            jBlock.Target1.Post(3);
            jBlock.Target2.Post(2);
            // (2,1)
            item = jBlock.Receive();
            // (3,2)
            item = jBlock.Receive();

            Console.WriteLine("Done");
        }

    }
}
