using FluentValidation;

namespace Application.Common;

public abstract class PagedQueryValidator<T> : AbstractValidator<T>
    where T : PagedQuery
{
    protected PagedQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
    }
}
