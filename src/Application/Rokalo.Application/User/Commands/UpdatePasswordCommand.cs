namespace Rokalo.Application.User.Commands
{
    using FluentValidation;
    using MediatR;
    using Rokalo.Application.Contracts;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Application.Helpers;
    using Rokalo.Blocks.Common.Exceptions;
    using System.Threading;
    using System.Threading.Tasks;

    public record UpdatePasswordCommand(string Email, string Password, string NewPassword) : IRequest;

    internal sealed class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator()
        {
            this.RuleFor(r => r.Email).NotEmpty();

            this.RuleFor(r => r.Password).Password();

            this.RuleFor(r => r.NewPassword).Password();

            this.RuleFor(r => r.Password).NotEqual(r => r.NewPassword).WithMessage("New password cannot match old password.");
        }
    }

    internal sealed class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPasswordHashingService passwordHashingService;
        public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHashingService passwordHashingService)
        {
            this.unitOfWork = unitOfWork;
            this.passwordHashingService = passwordHashingService;
        }

        public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await this.unitOfWork.Users.GetByEmailSafeAsync(request.Email, cancellationToken);

            if (user.Password is null)
            {
                throw new ServiceValidationException("User has no password set.");
            }

            var isOldPassValid = this.passwordHashingService.VerifyHash(user.Password, request.Password);

            if (!isOldPassValid)
            {
                throw new ServiceValidationException("Invalid credentials");
            }

            var newPassHash = passwordHashingService.Hash(request.NewPassword);

            user.UpdatePassword(newPassHash);

            this.unitOfWork.Users.Update(user);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
