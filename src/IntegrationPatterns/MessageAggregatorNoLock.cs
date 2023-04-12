using System.Threading.Tasks.Dataflow;

namespace IntegrationPatterns
{
    public class MessageAggregatorNoLock
    {
        private readonly ITargetBlock<(int, string)> _target;

        private int _number = -1;
        private string _term = string.Empty;

        public MessageAggregatorNoLock(ITargetBlock<(int, string)> target)
        {
            _target = target;
        }

        public void PostNumber(int number)
        {
            _number = number;
            var term = _term;
            ComposeAndSend(ref number, ref term);
        }

        public void PostTerm(string term)
        {
            _term = term;
            var number = _number;
            ComposeAndSend(ref number, ref term);
        }

        private void ComposeAndSend(ref int number, ref string term)
        {
            //Console.WriteLine($"Sending tuple ({number},{term}). Thread={Environment.CurrentManagedThreadId}");
            _target.Post((number, term));
        }
    }
}