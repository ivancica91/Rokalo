namespace Rokalo.Application.Contracts.Security
{
    using Rokalo.Application.Contracts.Models;
    using System.Threading.Tasks;

    public interface IFacebookOAuthService
    {
        Task<FacebookUserData> GetUserDataAsync(string accessToken);
    }
}
