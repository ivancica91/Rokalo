namespace Rokalo.Application.Contracts.Security
{
    public interface IPasswordHashingService
    {
        string Hash(string password);

        bool VerifyHash(string password, string providedPassword);
    }
}
