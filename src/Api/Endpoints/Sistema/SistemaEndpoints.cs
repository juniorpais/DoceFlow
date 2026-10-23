using Api.Results;
using Application.Abstractions;
using Application.Features.Sistema;

namespace Api.Endpoints.Sistema;

public static class SistemaEndpoints
{
    public static IEndpointRouteBuilder MapSistemaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/sistema").WithTags("Sistema");

        group.MapGet("/ping", PingAsync)
            .Produces<PingResponse>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> PingAsync(IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send(new PingQuery(), cancellationToken);
        return result.ToIResult();
    }
}
