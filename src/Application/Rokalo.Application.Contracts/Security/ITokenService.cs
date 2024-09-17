namespace Rokalo.Application.Contracts.Security
{
    using Rokalo.Domain;

    public interface ITokenService
    {
        string GenerateJwtToken(User user);

        RefreshToken GenerateRefreshToken(User user);
    }
}
