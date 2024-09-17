namespace Rokalo.Infrastructure.Security.Services
{
    using Facebook;
    using Rokalo.Application.Contracts.Models;
    using Rokalo.Application.Contracts.Security;
    using System.Threading.Tasks;

    internal sealed class FacebookOAuthService : IFacebookOAuthService
    {
        public async Task<FacebookUserData> GetUserDataAsync(string accessToken)
        {
            var fbClient = new FacebookClient(accessToken);

            // graph API query to get current user info on facebook
            // https://developers.facebook.com/docs/graph-api/get-started
            dynamic userInfo = await fbClient.GetTaskAsync("me?fields=id,email,name");

            return new FacebookUserData
            {
                Id = Guid.Parse(userInfo.Id),
                Email = userInfo.Email,
                Name = userInfo.Name
            };
        }
    }
}
