namespace Rokalo.Presentation.Api.Controllers.Accounts
{
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Rokalo.Application.User.Commands;
    using Rokalo.Application.User.Models;
    using Rokalo.Application.User.Responses;
    using System.Threading.Tasks;

    public class AccountsController : ApiControllerBase
    {
        public AccountsController(IMediator mediator) : base(mediator)
        {
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Register([FromBody] RegisterUserCommand request)
        {
            return await this.ProcessAsync<RegisterUserCommand, RegisterUserResponse>(request);
        }

        [AllowAnonymous]
        [HttpPost("email-confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand request)
        {
            return await this.ProcessAsync(request);
        }

        [AllowAnonymous]
        [HttpPost("resend-email-confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendConfirmationEmailCommand request)
        {
            return await this.ProcessAsync(request);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> LoginUser([FromBody] LoginCommand request)
        {
            return await this.ProcessAsync<LoginCommand, TokenResponse>(request);
        }

        [Authorize]
        [HttpPost("update-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand request)
        {
            return await this.ProcessAsync(request);
        }
    }
}
