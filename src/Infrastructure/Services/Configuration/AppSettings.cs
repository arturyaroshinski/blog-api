namespace Yaroshinski.Blog.Infrastructure.Services.Configuration
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int RefreshTokenTtl { get; set; }
        public string EmailFrom { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
    }
}