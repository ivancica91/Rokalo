namespace Rokalo.Application.User.Commands
{
    using FluentValidation;
    using MediatR;
    using Rokalo.Application.Contracts;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Application.Helpers;
    using Rokalo.Application.User.Models;
    using Rokalo.Blocks.Common.Exceptions;
    using System.Threading;
    using System.Threading.Tasks;

    public record LoginCommand(string Email, string Password) : IRequest<TokenResponse>;

    internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).Password();
        }
    }

    internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IPasswordHashingService hashingService;
        private readonly ITokenService tokenService;

        public LoginCommandHandler(IUnitOfWork unitOfWork,IPasswordHashingService hashingService, ITokenService tokenService)
        {
            this.unitOfWork = unitOfWork;
            this.hashingService = hashingService;
            this.tokenService = tokenService;
        }

        public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await this.unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                throw new ServiceValidationException("Invalid user credentials.");
            }

            if (user.Password == null)
            {
                throw new ServiceValidationException("Invalid user credentials.");
            }

            var pass = this.hashingService.VerifyHash(user.Password, request.Password);

            if (!pass)
            {
                throw new ServiceValidationException("Invalid user credentials.");
            }

            var jwtToken = this.tokenService.GenerateJwtToken(user);

            var refreshToken = this.tokenService.GenerateRefreshToken(user);

            var oldTokens = await this.unitOfWork.RefreshTokens.GetByUserIdAsync(user.Id, cancellationToken);

            this.unitOfWork.RefreshTokens.Delete(oldTokens);

            this.unitOfWork.RefreshTokens.Add(refreshToken);

            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            return new TokenResponse(
                jwtToken,
                refreshToken.Id.ToString());
        }
    }
}
