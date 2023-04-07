using System.Collections.Concurrent;

namespace MultiplexorSample;

public class NetworkAdapter : INetworkAdapter
{
    private readonly BlockingCollection<Guid> _queue = new ();

    public bool TryEnqueueWrite(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var added = _queue.TryAdd(request.Id);
            
            return added;
        }
        catch (OperationCanceledException)
        {
            _queue.CompleteAdding();
            throw;
        }
    }

    public Task WriteAsync(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var added = _queue.TryAdd(request.Id);

            return Task.CompletedTask;
        }
        catch (OperationCanceledException)
        {
            _queue.CompleteAdding();
            throw;
        }
    }

    public Task<Response> ReadAsync(CancellationToken cancellationToken)
    {
        var guid =_queue.Take(cancellationToken);
        var response = new Response(guid);
        
        return Task.FromResult(response);
    }
}