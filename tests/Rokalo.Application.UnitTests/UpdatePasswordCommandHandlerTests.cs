namespace Rokalo.Application.UnitTests
{
    using AutoFixture;
    using Moq;
    using Rokalo.Application.Contracts;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Application.UnitTests.Helpers.ModelBuilders;
    using Rokalo.Application.User.Commands;
    using Rokalo.Blocks.Common.Exceptions;
    using Rokalo.Domain;

    public class UpdatePasswordCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IPasswordHashingService> passwordHashingService;
        private readonly IFixture fixture;
        public UpdatePasswordCommandHandlerTests()
        {
            this.unitOfWork = new();
            this.passwordHashingService = new();
            this.fixture = new Fixture();
        }

        [Fact]
        public async Task Update_failed_with_user_not_found_exception()
        {
            // Arrange 
            var command = new UpdatePasswordCommand(this.fixture.Create<string>(), this.fixture.Create<string>(), this.fixture.Create<string>());

            this.unitOfWork.Setup(u => u.Users.GetByEmailSafeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ServiceValidationException("User not found"));

            var handler = new UpdatePasswordCommandHandler(this.unitOfWork.Object, this.passwordHashingService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ServiceValidationException>(() => handler.Handle(command, default));

            Assert.Equal("User not found", exception.Errors["General"].FirstOrDefault());
        }

        [Fact]
        public async Task Update_failed_when_new_password_is_invalid()
        {
            // Arrange 
            var command = new UpdatePasswordCommand(this.fixture.Create<string>(), this.fixture.Create<string>(), this.fixture.Create<string>());

            var user = new UserBuilder().WithPassword(this.fixture.Create<string>()).Build();

            this.unitOfWork.Setup(u => u.Users.GetByEmailSafeAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            this.passwordHashingService.Setup(v => v.VerifyHash(user.Password ?? string.Empty, command.Password))
                .Returns(false);

            var handler = new UpdatePasswordCommandHandler(this.unitOfWork.Object, this.passwordHashingService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ServiceValidationException>(() => handler.Handle(command, default));

            Assert.Equal("Invalid credentials", exception.Errors["General"].FirstOrDefault());
        }

        [Fact]
        public async Task Password_is_updated_successfully()
        {
            // Arrange 
            var command = new UpdatePasswordCommand(this.fixture.Create<string>(), this.fixture.Create<string>(), this.fixture.Create<string>());

            var user = new UserBuilder().Build();

            this.unitOfWork.Setup(u => u.Users.GetByEmailSafeAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            this.passwordHashingService.Setup(v => v.VerifyHash(user.Password ?? string.Empty, command.Password))
                .Returns(true);

            var newPassHashedPassword = this.fixture.Create<string>(); 
            
            this.passwordHashingService.Setup(v => v.Hash(command.NewPassword))
                .Returns(newPassHashedPassword);

            this.unitOfWork.Setup(v => v.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var handler = new UpdatePasswordCommandHandler(this.unitOfWork.Object, this.passwordHashingService.Object);

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.Equal(newPassHashedPassword, user.Password);

            this.unitOfWork.Verify(u => u.Users.Update(It.Is<User>(u => u == user)), Times.Once);

            this.unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
