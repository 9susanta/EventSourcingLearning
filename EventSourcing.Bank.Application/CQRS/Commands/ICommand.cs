using System;

namespace EventSourcing.Bank.Application.CQRS.Commands
{
    public interface ICommand
    {
        Guid CommandId { get; set; }
    }

    public interface ICommand<out TResult> : ICommand
    {
    }
}
