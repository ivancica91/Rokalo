namespace Rokalo.Infrastructure.Email.Configurations
{
    public class SmtpConfiguration
    {
        public const string Key = nameof(SmtpConfiguration);
        public string DisplayName { get;  set; } = default!;
        public string From { get;  set; } = default!;
        public string Host { get;  set; } = default!;
        public string Password { get; set; } = default!;
        public int Port { get; set; }
        public string UserName { get; set; } = default!;
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
    }
}
