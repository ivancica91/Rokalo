namespace Rokalo.Infrastructure.Security.Services
{
    using Rokalo.Application.Contracts.Models;
    using Rokalo.Application.Contracts.Security;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal sealed class FacebookOAuthService : IFacebookOAuthService
    {
        private readonly HttpClient httpClient;

        public FacebookOAuthService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<FacebookUserData> GetUserDataAsync(string accessToken)
        {
            // Graph API endpoint to get user data
            var url = $"https://graph.facebook.com/me?fields=id,email,name&access_token={accessToken}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error retrieving Facebook user data.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<FacebookUserData>(content);

            if (userInfo is null)
            {
                throw new Exception("Error deserializing Facebook use data");
            }

            return new FacebookUserData
            {
                Id = userInfo.Id,
                Email = userInfo.Email,
                Name = userInfo.Name
            };
        }
    }
}
