using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPlaygroundDemo.Multiplexor
{
    public record Request(Guid Id); // Ещё какие-то поля

    public record Response(Guid Id); // Ещё какие-то поля

    public interface INetworkAdapter
    {
        /// Вычитывает очередной ответ
        Task<Response> ReadAsync(CancellationToken cancellationToken);

        /// Ставит очередной запрос в очередь на отправку. false - очередь переполнена, запрос не будет отправлен
        bool TryEnqueueWrite(Request request, CancellationToken cancellationToken);

        /// Отправляет запрос <summary>
        Task WriteAsync(Request request, CancellationToken cancellationToken);
    }
}
