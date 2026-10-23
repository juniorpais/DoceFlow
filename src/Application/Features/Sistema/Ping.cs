using Application.Abstractions;
using FluentResults;

namespace Application.Features.Sistema;

public sealed record PingQuery : IQuery<PingResponse>;

public sealed record PingResponse(string Mensagem, DateTime ServidorUtc);

public sealed class PingHandler(IDateTimeProvider clock) : IQueryHandler<PingQuery, PingResponse>
{
    public Task<Result<PingResponse>> HandleAsync(PingQuery query, CancellationToken cancellationToken)
        => Task.FromResult(Result.Ok(new PingResponse("pong", clock.UtcNow)));
}
