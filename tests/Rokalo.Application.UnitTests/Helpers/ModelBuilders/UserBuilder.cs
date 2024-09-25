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
            this.id = this.fixture.Create<Guid>();
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            this.email = this.fixture.Create<string>();
            return this;
        } 
        
        public UserBuilder WithPassword(string password)
        {
            this.password = this.fixture.Create<string>();
            return this;
        }

        public UserBuilder WithEmailVerificationCode(string code)
        {
            this.emailVerificationCode = this.fixture.Create<Guid>().ToString();
            return this;
        }

        public UserBuilder IsEmailVerified(bool isVerified)
        {
            this.isEmailVerified = this.fixture.Create<bool>();
            return this;
        }

        public User Build()
        {
            return new User(id, email, password, isEmailVerified, emailVerificationCode);
        }
    }
}
