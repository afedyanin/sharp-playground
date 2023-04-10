using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowSample
{
    public static class JoinBlockExample1
    {
        static public void Run()
        {
            var jBlock = new JoinBlock<int, int>();

            for (int i = 0; i < 10; i++)
            {
                jBlock.Target1.Post(i);
            }

            for (int i = -9; i < 1; i++)
            {
                jBlock.Target2.Post(i);
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(jBlock.Receive());
            }

            Console.WriteLine("Done");
        }
    }
}
