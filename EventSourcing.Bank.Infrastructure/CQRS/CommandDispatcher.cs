using EventSourcing.Bank.Application.CQRS.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Bank.Infrastructure.CQRS
{
    public class CommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(TCommand));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"No handler found for command {typeof(TCommand).Name}");

            var method = handlerType.GetMethod("HandleAsync", new[] { typeof(TCommand), typeof(CancellationToken) });
            await (Task)method.Invoke(handler, new object[] { command, cancellationToken });
        }

        public async Task<TResult> HandleAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResult>
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResult));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"No handler found for command {typeof(TCommand).Name}");

            var method = handlerType.GetMethod("HandleAsync", new[] { typeof(TCommand), typeof(CancellationToken) });
            var result = await (Task<TResult>)method.Invoke(handler, new object[] { command, cancellationToken });
            return result;
        }
    }
}
