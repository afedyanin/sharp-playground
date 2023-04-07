namespace MultiplexorSample
{
    public interface INetworkAdapter
    {
        /// Вычитывает очередной ответ
        Task<Response> ReadAsync(CancellationToken cancellationToken);

        /// Ставит очередной запрос в очередб на отправку. false - очередь переполнена, запрос не будет отправлен
        bool TryEnqueueWrite(Request request, CancellationToken cancellationToken);

        /// Отправляет запрос
        Task WriteAsync(Request request, CancellationToken cancellationToken);
    }    
}

