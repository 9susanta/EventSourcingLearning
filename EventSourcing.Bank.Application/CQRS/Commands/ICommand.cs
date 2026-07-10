namespace EventSourcing.Bank.Application.CQRS.Commands
{
    public interface ICommand
    {
    }

    public interface ICommand<out TResult> : ICommand
    {
    }
}
