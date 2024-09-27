namespace Rokalo.Application.User.Commands
{
    using FluentValidation;
    using MediatR;
    using Rokalo.Application.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using Rokalo.Domain;
    using System;
    using Rokalo.Application.Contracts.Email;

    public record ResendConfirmationEmailCommand(string email) : IRequest;

    internal sealed class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ResendConfirmationEmailValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(u => u.email).EmailAddress().NotEmpty();
        }
    }

    internal sealed class ResendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailService emailService;

        public ResendConfirmationEmailCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
        }

        public async Task Handle(ResendConfirmationEmailCommand command, CancellationToken cancellationToken)
        {
            User? user = await this.unitOfWork.Users.GetByEmailAsync(command.email, cancellationToken);

            string newVerificationCode = Guid.NewGuid().ToString();
            
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.IsEmailVerified)
            {
                return;
            }

            user.UpdateEmailVerificationCode(newVerificationCode);

            this.unitOfWork.Users.Update(user);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            await this.emailService.SendConfirmEmailAsync(command.email, user.Id, newVerificationCode);
        }
    }
}
