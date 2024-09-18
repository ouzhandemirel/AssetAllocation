using FluentValidation;
using MediatR;

namespace AssetAllocation.Api;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext<TRequest>(request);
        var Failures = _validators
            .Select(v => v.Validate(validationContext))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (Failures.Count > 0)
        {
            string[] errors = Failures.Select(f => f.ErrorMessage).ToArray();
            throw new ValidationException(errors);
        }

                var aba = next.GetInvocationList().ToList();
        
        return await next();
    }
}
