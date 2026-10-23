using Application.Common;
using Bogus;

namespace Unit.TestData;

public static class PagedQueryTestData
{
    public sealed record SampleQuery : PagedQuery;

    private static readonly Faker<SampleQuery> Faker = new Faker<SampleQuery>("pt_BR")
        .CustomInstantiator(f => new SampleQuery
        {
            Page = f.Random.Int(1, 50),
            Size = f.Random.Int(1, 100),
            Order = f.PickRandom("data desc", "numeroVenda asc", null),
        });

    public static SampleQuery GenerateValid() => Faker.Generate();
}
