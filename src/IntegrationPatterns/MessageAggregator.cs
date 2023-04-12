using System.Threading.Tasks.Dataflow;

namespace IntegrationPatterns
{
    public class MessageAggregator
    {
        private readonly ActionBlock<int> _numberReceiver;
        private readonly ActionBlock<string> _stringReceiver;
        private readonly ITargetBlock<(int, string)> _target;

        private int _number = -1;
        private string _term = string.Empty;
        private readonly object _locker = new();

        public MessageAggregator(ITargetBlock<(int, string)> target)
        {
            _numberReceiver = new ActionBlock<int>(HandleNumber);
            _stringReceiver = new ActionBlock<string>(HandleString);
            _target = target;
        }

        public void PostNumber(int number)
        {
            _numberReceiver.Post(number);
        }

        public void PostTerm(string term)
        {
            _stringReceiver.Post(term);
        }

        private void HandleNumber(int number)
        {
            lock (_locker)
            {
                Console.WriteLine($"cnahge number from {_number} to {number}. Thread={Environment.CurrentManagedThreadId}");
                _number = number;
                ComposeAndSend();
            }
        }

        private void HandleString(string term)
        {
            lock (_locker)
            {
                Console.WriteLine($"cnahge term from {_term} to {term}. Thread={Environment.CurrentManagedThreadId}");
                _term = term;
                ComposeAndSend();
            }
        }

        private void ComposeAndSend()
        {
            //Console.WriteLine($"Sending tuple ({_number},{_term}). Thread={Environment.CurrentManagedThreadId}");
            _target.Post((_number, _term));
        }
    }
}