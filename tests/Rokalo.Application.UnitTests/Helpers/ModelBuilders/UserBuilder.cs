namespace Rokalo.Application.UnitTests.Helpers.ModelBuilders
{
    using AutoFixture;
    using Rokalo.Domain;

    public class UserBuilder
    {
        private Guid id = Guid.NewGuid();
        private string email = "default@test.com";
        private string? password = "defaultPassword";
        private bool isEmailVerified = true;
        private string emailVerificationCode = Guid.NewGuid().ToString();

        private readonly IFixture fixture = new Fixture();

        public UserBuilder WithId(Guid id)
        {
            this.id = id;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            this.email = email;
            return this;
        } 
        
        public UserBuilder WithPassword(string password)
        {
            this.password = password;
            return this;
        }

        public UserBuilder WithEmailVerificationCode(string code)
        {
            this.emailVerificationCode = code;
            return this;
        }

        public UserBuilder IsEmailVerified(bool isVerified)
        {
            this.isEmailVerified = isVerified;
            return this;
        }

        public User Build()
        {
            return new User(id, email, password, isEmailVerified, emailVerificationCode);
        }
    }
}
