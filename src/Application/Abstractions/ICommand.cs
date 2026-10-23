namespace Application.Abstractions;

public interface ICommand : ICommand<CommandResult>
{
}

public interface ICommand<TResult>
{
}

public readonly record struct CommandResult;
