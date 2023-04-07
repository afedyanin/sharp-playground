using System.Threading.Channels;

namespace ConsoleDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Channel<int> channel = Channel.CreateUnbounded<int>();

            var writer = channel.Writer;
            var reader = channel.Reader;

            var writerLoop = Task.Run(async () =>
            {
                for(int i=0; i<100; i++)
                {
                    await writer.WriteAsync(i);
                }

                writer.Complete();

            });

            var readerLoop = Task.Run(async () =>
            {
                await foreach (var item in reader.ReadAllAsync())
                {
                    Console.WriteLine(item);
                }
            });

            await Task.WhenAll(writerLoop, readerLoop);
        }
    }
}