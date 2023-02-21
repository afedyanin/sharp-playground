using System.Collections.Concurrent;

namespace SharpPlaygroundDemo.Multiplexor
{
    public interface IRequestProcessor
    {
        /// Запускает обработчик. Гарантированно вызывается 1 раз при старте приложения
        Task RunAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        // При отмене CancellationToken не обязательно гарантировать то, что мы не отправим запрос на сервер, но клиент должен получить отмену задачи
        Task<Response> SendAsync(Request request, CancellationToken cancellationToken);
    }

    public class RequestProcessor : IRequestProcessor
    {
        private readonly INetworkAdapter _networkAdapter;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Response>> _waitQueue;

        public RequestProcessor(INetworkAdapter networkAdapter)
        {
            _networkAdapter = networkAdapter;
            _waitQueue = new ConcurrentDictionary<Guid, TaskCompletionSource<Response>>();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                while (true)
                {
                    var response = await _networkAdapter.ReadAsync(cancellationToken);

                    if (response == null)
                    {
                        continue;
                    }

                    if (_waitQueue.TryRemove(response.Id, out var tcs))
                    {
                        tcs.TrySetResult(response);
                    }
                }

            }
            catch (OperationCanceledException e) when (e.CancellationToken.Equals(cancellationToken))
            {
                Console.WriteLine("RequestProcessor stopped");
            }
        }

        public Task<Response> SendAsync(Request request, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Response>();
            
            if (!_waitQueue.TryAdd(request.Id, tcs))
            {
                throw new InvalidOperationException($"Request with same id has already been added: {request.Id}");
            }

            if (!_networkAdapter.TryEnqueueWrite(request, cancellationToken))
            {
                _waitQueue.Remove(request.Id, out _);
                throw new InvalidOperationException($"Failed to enqueue write for request: {request.Id}");
            }

            return tcs.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
