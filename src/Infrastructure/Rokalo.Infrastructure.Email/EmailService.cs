namespace Rokalo.Infrastructure.Email
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using Rokalo.Application.Contracts.Email;
    using Rokalo.Infrastructure.Email.Configurations;
    using System;
    using System.Threading.Tasks;

    internal sealed class EmailService : IEmailService
    {
        private readonly SmtpConfiguration smtpConfig;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EmailService(IOptions<SmtpConfiguration> smtpConfig, IHttpContextAccessor httpContextAccessor)
        {
            this.smtpConfig = smtpConfig.Value;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task SendConfirmEmailAsync(string email, Guid userId, string code)
        {
            var msg = new MimeMessage();

            msg.From.Add(MailboxAddress.Parse(this.smtpConfig.UserName));

            msg.To.Add(MailboxAddress.Parse(email));

            msg.Subject = "Confirmation Email";

            var bodyBuilder = new BodyBuilder();

            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "SendEmailConfirmation.html");

            using (StreamReader reader = File.OpenText(templatePath))
            {
                bodyBuilder.HtmlBody = reader.ReadToEnd();
            }

            bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("{{user}}", email);

            var request = httpContextAccessor.HttpContext.Request;

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var confirmationLink = $"{baseUrl}/v1/accounts/email-confirmation?userId={userId}&confirmationCode={code}";

            bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace("{{link}}", confirmationLink);

            msg.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(this.smtpConfig.Host, this.smtpConfig.Port, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(this.smtpConfig.UserName, this.smtpConfig.Password);

            await smtp.SendAsync(msg);

            await smtp.DisconnectAsync(true);
        }
    }
}
