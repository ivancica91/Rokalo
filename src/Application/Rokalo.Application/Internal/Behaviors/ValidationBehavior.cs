namespace Rokalo.Application.Internal.Behaviors
{
    using FluentValidation;
    using MediatR;
    using Rokalo.Blocks.Common.Exceptions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            if (this.validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(this.validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults
                    .SelectMany(result => result.Errors)
                    .Where(failure => failure is not null)
                    .GroupBy(failure => failure.PropertyName)
                    .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());

                if (failures.Any())
                {
                    throw new ServiceValidationException(failures);
                }
            }
            

            return await next();
        }
    }
}
