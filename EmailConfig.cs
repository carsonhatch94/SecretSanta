public record EmailConfig(string SmtpHost, int SmtpPort, string SmtpUser, string SmtpPassword, string RecipientEmail)
{
    public static EmailConfig LoadFromEnvironment()
    {
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
        var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        var recipientEmail = Environment.GetEnvironmentVariable("RECIPIENT_EMAIL");

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) ||
            string.IsNullOrEmpty(smtpPassword) || string.IsNullOrEmpty(recipientEmail))
        {
            throw new InvalidOperationException("Email configuration missing. Please set SMTP_HOST, SMTP_USER, SMTP_PASSWORD, and RECIPIENT_EMAIL in .env file");
        }

        return new EmailConfig(smtpHost, smtpPort, smtpUser, smtpPassword, recipientEmail);
    }
}