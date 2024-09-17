namespace Rokalo.Application.User.Responses
{
    using System;

    public record RegisterUserResponse(Guid UserId, string Email);
}
