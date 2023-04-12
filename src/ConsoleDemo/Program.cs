using System.Threading.Channels;
using DataFlowSample;
using IntegrationPatterns;

namespace ConsoleDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Console.WriteLine("Hello, World!");
            // await MessageAggregatorExample.Run();
            await MessageAggregatorExample.RunNoLock();
        }
    }
}