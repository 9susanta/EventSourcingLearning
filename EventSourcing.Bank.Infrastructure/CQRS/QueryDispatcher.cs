using EventSourcing.Bank.Application.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Bank.Infrastructure.CQRS
{
    public class QueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> HandleAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"No handler found for query {typeof(TQuery).Name}");

            var method = handlerType.GetMethod("HandleAsync", new[] { typeof(TQuery), typeof(CancellationToken) });
            var result = await (Task<TResult>)method.Invoke(handler, new object[] { query, cancellationToken });
            return result;
        }
    }
}
