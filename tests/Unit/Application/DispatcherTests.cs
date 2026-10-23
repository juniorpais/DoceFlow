using Application.Abstractions;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Application;

public sealed class DispatcherTests
{
    public sealed record CriarCommand(string Nome) : ICommand<Guid>;

    public sealed record LimparCommand : ICommand;

    public sealed record ObterQuery(int Valor) : IQuery<string>;

    public sealed record SemHandlerQuery : IQuery<int>;

    public sealed class CriarHandler : ICommandHandler<CriarCommand, Guid>
    {
        public static readonly Guid Fixo = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public Task<Result<Guid>> HandleAsync(CriarCommand command, CancellationToken cancellationToken)
            => Task.FromResult(Result.Ok(Fixo));
    }

    public sealed class LimparHandler : ICommandHandler<LimparCommand, CommandResult>
    {
        public int Chamadas { get; private set; }

        public Task<Result<CommandResult>> HandleAsync(LimparCommand command, CancellationToken cancellationToken)
        {
            Chamadas++;
            return Task.FromResult(Result.Ok(default(CommandResult)));
        }
    }

    public sealed class ObterHandler : IQueryHandler<ObterQuery, string>
    {
        public Task<Result<string>> HandleAsync(ObterQuery query, CancellationToken cancellationToken)
            => Task.FromResult(Result.Ok($"valor-{query.Valor}"));
    }

    private static (Dispatcher Dispatcher, LimparHandler Limpar) CreateDispatcher()
    {
        var limpar = new LimparHandler();
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<CriarCommand, Guid>, CriarHandler>();
        services.AddSingleton<ICommandHandler<LimparCommand, CommandResult>>(limpar);
        services.AddScoped<IQueryHandler<ObterQuery, string>, ObterHandler>();
        var provider = services.BuildServiceProvider();
        return (new Dispatcher(provider), limpar);
    }

    [Fact(DisplayName = "Given registered command handler When sending command Then returns handler result")]
    public async Task Given_RegisteredCommandHandler_When_SendingCommand_Then_ReturnsHandlerResult()
    {
        var (dispatcher, _) = CreateDispatcher();

        var result = await dispatcher.Send(new CriarCommand("Brigadeiro"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(CriarHandler.Fixo);
    }

    [Fact(DisplayName = "Given registered command handler without return When sending command Then calls handler and succeeds")]
    public async Task Given_RegisteredVoidCommandHandler_When_SendingCommand_Then_CallsHandlerAndSucceeds()
    {
        var (dispatcher, limpar) = CreateDispatcher();

        var result = await dispatcher.Send(new LimparCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        limpar.Chamadas.Should().Be(1);
    }

    [Fact(DisplayName = "Given registered query handler When sending query Then returns handler result")]
    public async Task Given_RegisteredQueryHandler_When_SendingQuery_Then_ReturnsHandlerResult()
    {
        var (dispatcher, _) = CreateDispatcher();

        var result = await dispatcher.Send(new ObterQuery(7), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("valor-7");
    }

    [Fact(DisplayName = "Given no registered handler When sending query Then throws InvalidOperationException")]
    public async Task Given_NoRegisteredHandler_When_SendingQuery_Then_ThrowsInvalidOperationException()
    {
        var (dispatcher, _) = CreateDispatcher();

        var act = () => dispatcher.Send(new SemHandlerQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
