using FluentResults;

namespace Application.Abstractions;

public interface IDispatcher
{
    Task<Result<TResult>> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);

    Task<Result> Send(ICommand command, CancellationToken cancellationToken);

    Task<Result<TResult>> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}
