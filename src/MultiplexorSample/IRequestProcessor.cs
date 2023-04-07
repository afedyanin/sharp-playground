namespace MultiplexorSample
{
    public interface IRequestProcessor
    {
        /// Запускает обработчик. Гарантированно вызывается 1 раз при старте приложения
        Task RunAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        // При отмене CancellationToken не обязательно гарантировать то, что мы не отправим запрос на сервер, но клиент должен получить отмену задачи
        Task<Response> SendAsync(Request request, CancellationToken cancellationToken);
    }    
}

