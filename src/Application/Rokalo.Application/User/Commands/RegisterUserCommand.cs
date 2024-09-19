namespace Rokalo.Application.User.Commands
{
    using FluentValidation;
    using MediatR;
    using Rokalo.Application.Contracts;
    using Rokalo.Application.Contracts.Email;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Application.Helpers;
    using Rokalo.Application.User.Responses;
    using Rokalo.Blocks.Common.Exceptions;
    using Rokalo.Domain;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public record RegisterUserCommand(
        string Email,
        string? Password,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string MobileNumber,
        string Address,
        int? CityId) : IRequest<RegisterUserResponse>;

    internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        private const int MinLength = 0;
        private const int MaxNameLength = 50;
        private const int MaxPhoneNumberLength = 20;
        private const int MaxAddressLength = 100;

        private readonly IUnitOfWork unitOfWork;

        public RegisterUserCommandValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(u => u.Password).Password();
            RuleFor(u => u.FirstName).Length(MinLength, MaxNameLength);
            RuleFor(u => u.LastName).Length(MinLength, MaxNameLength);
            RuleFor(u => u.PhoneNumber).Length(MinLength, MaxPhoneNumberLength);
            RuleFor(u => u.MobileNumber).Length(MinLength, MaxPhoneNumberLength);
            RuleFor(u => u.Address).Length(MinLength, MaxAddressLength);
            RuleFor(u => u.CityId).NotNull(); // TODO add must be valid city id, where is that city id?
            RuleFor(u => u.Email).EmailAddress()
                .MustAsync(IsUniqueEmail)
                .WithMessage("Email is already in use.");
        }

        private async Task<bool> IsUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            return user is null;
        }
    }

    internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPasswordHashingService hashingService;
        private readonly IEmailService emailService;

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHashingService hashingService, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.hashingService = hashingService;
            this.emailService = emailService;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Password is null)
            {
                throw new ServiceValidationException("Invalid user credentials.");
            }

            string hashedPassword = this.hashingService.Hash(request.Password);

            User user = new (
                Guid.NewGuid(),
                request.Email,
                hashedPassword,
                false,
                Guid.NewGuid().ToString()
                );

            this.unitOfWork.Users.Add(user);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            await this.emailService.SendConfirmEmailAsync(user.Email, user.Id, user.EmailVerificationCode);

            return new RegisterUserResponse(user.Id, user.Email);
        }
    }
}
