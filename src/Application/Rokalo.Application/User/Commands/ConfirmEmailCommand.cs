namespace Rokalo.Application.User.Commands
{
    using FluentValidation;
    using MediatR;
    using Rokalo.Application.Contracts;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Rokalo.Domain;

    public record ConfirmEmailCommand(Guid UserId, string ConfirmationCode) : IRequest;

    internal sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.ConfirmationCode).NotEmpty();
        }
    }

    internal sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ConfirmEmailCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            User user = await this.unitOfWork.Users.GetByIdSafeAsync(request.UserId, cancellationToken);
            
            if (user.EmailVerificationCode == request.ConfirmationCode)
            {
                user.ConfirmEmail();

                this.unitOfWork.Users.Update(user);

                await this.unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
