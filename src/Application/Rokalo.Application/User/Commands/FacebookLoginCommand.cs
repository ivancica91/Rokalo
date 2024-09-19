namespace Rokalo.Application.User.Commands
{
    using MediatR;
    using Rokalo.Application.Contracts;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Application.User.Models;
    using System.Threading;
    using System.Threading.Tasks;
    using Rokalo.Domain;
    using System;

    public record FacebookLoginCommand(string AccessToken) : IRequest<TokenResponse>;

    internal sealed class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommand, TokenResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenService tokenService;
        private readonly IFacebookOAuthService facebookOAuthService;

        public FacebookLoginCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IFacebookOAuthService facebookOAuthService)
        {
            this.unitOfWork = unitOfWork;
            this.tokenService = tokenService;
            this.facebookOAuthService = facebookOAuthService;
        }

        public async Task<TokenResponse> Handle(FacebookLoginCommand request, CancellationToken cancellationToken)
        {
            var fbUser = await this.facebookOAuthService.GetUserDataAsync(request.AccessToken);

            var nameParts = fbUser.Name.Split(' ', 2);

            var user = await this.unitOfWork.Users.GetByEmailAsync(fbUser.Email, cancellationToken);

            if (user is null)
            {
                user = new User(
                    Guid.NewGuid(),
                    fbUser.Email,
                    null,
                    true,
                    Guid.NewGuid().ToString()
                    );
            }

            this.unitOfWork.Users.Add(user);
            await this.unitOfWork.SaveChangesAsync(cancellationToken);

            var jwtToken = this.tokenService.GenerateJwtToken(user);
            var refreshToken = this.tokenService.GenerateRefreshToken(user);

            return new TokenResponse(jwtToken, refreshToken.Id.ToString());
        }
    }
}
