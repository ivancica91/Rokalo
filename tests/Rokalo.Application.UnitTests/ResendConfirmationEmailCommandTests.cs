namespace Rokalo.Application.UnitTests
{
    using AutoFixture;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Rokalo.Application.Contracts;
    using Rokalo.Application.Contracts.Email;
    using Rokalo.Application.UnitTests.Helpers.ModelBuilders;
    using Rokalo.Application.User.Commands;
    using Rokalo.Domain;

    public class ResendConfirmationEmailCommandTests
    {
        private readonly IFixture fixture;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly Mock<IEmailService> emailService;

        public ResendConfirmationEmailCommandTests()
        {
            this.fixture = new Fixture();
            this.unitOfWork = new();
            this.emailService = new();
        }

        [Fact]
        public async Task Confirmation_email_cannot_be_resent_to_user_that_does_not_exist()
        {
            // Arange
            var command = new ResendConfirmationEmailCommand(this.fixture.Create<string>());

            this.unitOfWork.Setup(v => v.Users.GetByEmailAsync(command.email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

            var handler = new ResendConfirmationEmailCommandHandler(this.unitOfWork.Object, this.emailService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Confirmation_email_cannot_be_resent_to_a_user_already_verified()
        {
            // Arange
            var user = new UserBuilder()
                .IsEmailVerified(true)
                .Build();

            var command = new ResendConfirmationEmailCommand(user.Email);

            this.unitOfWork.Setup(u => u.Users.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            var handler = new ResendConfirmationEmailCommandHandler(this.unitOfWork.Object, this.emailService.Object);

            // Act
            await handler.Handle(command, default);

            // Assert
            this.unitOfWork.Verify(u => u.Users.Update(user), Times.Never());
            this.unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Never);
            this.emailService.Verify(s => s.SendConfirmEmailAsync(command.email, user.Id, user.EmailVerificationCode), Times.Never());
        }

        [Fact]
        public async Task Resend_fails_when_database_transaction_fails()
        {
            // Arrange
            var user = new UserBuilder()
                .IsEmailVerified(false)
                .Build();

            var command = new ResendConfirmationEmailCommand(user.Email);

            this.unitOfWork.Setup(u => u.Users.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            this.unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());

            var handler = new ResendConfirmationEmailCommandHandler(this.unitOfWork.Object, this.emailService.Object);

            // Act & assert
            await Assert.ThrowsAsync<DbUpdateException>(() => handler.Handle(command, default));

            this.unitOfWork.Verify(u => u.Users.Update(user), Times.Once);

            this.unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            this.emailService.Verify(s => s.SendConfirmEmailAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Confirmation_email_is_resent_successfully_when_user_is_not_verified()
        {
            // Arrange
            var user = new UserBuilder()
                .IsEmailVerified(false)
                .Build();

            var command = new ResendConfirmationEmailCommand(user.Email);

            this.unitOfWork.Setup(u => u.Users.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            var handler = new ResendConfirmationEmailCommandHandler(this.unitOfWork.Object, this.emailService.Object);

            // Act 
            await handler.Handle(command, default);

            // Assert
            this.unitOfWork.Verify(u => u.Users.Update(user), Times.Once);

            this.unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            this.emailService.Verify(s => s.SendConfirmEmailAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }
    }
}
