using System.Collections.Concurrent;

namespace MultiplexorSample
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly INetworkAdapter _networkAdapter;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Response>> _items = new ();
        private readonly BlockingCollection<Request> _requests = new();
        
        private readonly CancellationTokenSource _softStoppingCts = new();
        private readonly CancellationTokenSource _hardStoppingCts = new();
        
        private Task? _runningTask;

        public TimeSpan? RequestTimeout { get; set; }
        
        public RequestProcessor(INetworkAdapter networkAdapter)
        {
            _networkAdapter = networkAdapter;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            _runningTask = RunInnerAsync(cancellationToken);

            return _runningTask;            
        }

        public Task<Response> SendAsync(Request request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (_softStoppingCts.IsCancellationRequested)
            {
                throw new InvalidOperationException("RequestProcessor is stopping");
            }
            
            CancellationToken effectiveToken;
            
            if (RequestTimeout is not null)
            {
                var cts = new CancellationTokenSource(RequestTimeout.Value);
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token, _hardStoppingCts.Token);
                effectiveToken = linkedCts.Token;
            }
            else
            {
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _hardStoppingCts.Token);
                effectiveToken = linkedCts.Token;
            }
            
            var tcs = new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously);

            if (!_items.TryAdd(request.Id, tcs))
            {
                throw new InvalidOperationException($"Request with same id has already been added: {request.Id}");                
            }

            _requests.Add(request, cancellationToken);
            cancellationToken.Register(UnRegisterWait, new WaitContext(request.Id, effectiveToken));
            
            return tcs.Task;
        }

        private async Task RunInnerAsync(CancellationToken cancellationToken)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _hardStoppingCts.Token);

            try
            {
                await Task.WhenAll(
                    RunReadLoopAsync(linkedCts.Token),
                    RunWriteLoopAsync(linkedCts.Token));
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken.Equals(linkedCts.Token))
            {
                Console.WriteLine("RequestProcessor stopped");
            }
        }
       
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_runningTask is null)
            {
                throw new InvalidOperationException("RequestProcessor is not started");
            }

            Console.WriteLine("RequestProcessor stop requested");

            _softStoppingCts.Cancel();

            cancellationToken.Register(() => _hardStoppingCts.Cancel());

            return _runningTask;        
        }
        
        private async Task RunReadLoopAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                var response = await _networkAdapter.ReadAsync(cancellationToken);
                
                if (_items.TryRemove(response.Id, out var tcs))
                {
                    tcs.TrySetResult(response);
                }                    
            }
        }
        
        private async Task RunWriteLoopAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                var request = _requests.Take(cancellationToken);

                if (!_items.ContainsKey(request.Id))
                {
                    continue;
                }

                await _networkAdapter.WriteAsync(request, cancellationToken);
            }
        }
        
        private void UnRegisterWait(object? idObject)
        {
            var context = (WaitContext)idObject!;
            if (_items.TryRemove(context.Id, out var tcs))
            {
                tcs.TrySetCanceled(context.CancellationToken);
            }
        }

        private readonly record struct WaitContext(Guid Id, CancellationToken CancellationToken);        
    }    
}

