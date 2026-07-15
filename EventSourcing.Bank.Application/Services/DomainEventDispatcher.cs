using System.Threading.Tasks;
using EventSourcing.Bank.Domain.Events;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Bank.Application.Services
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }

    public class DomainEventDispatcher
    {
        private readonly IEnumerable<object> _handlers;

        public DomainEventDispatcher(IEnumerable<object> handlers)
        {
            _handlers = handlers;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            
            var handlers = _handlers.Where(h => handlerType.IsInstanceOfType(h)).ToList();

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                if (method != null)
                {
                    var task = (Task)method.Invoke(handler, new object[] { domainEvent });
                    if (task != null)
                    {
                        await task;
                    }
                }
            }
        }
    }
}
