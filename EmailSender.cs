using System.Net;
using System.Net.Mail;

static class EmailSender
{
    public static async Task SendAssignmentsAsync(List<(string Giver, string Receiver)> assignments)
    {
        var config = EmailConfig.LoadFromEnvironment();
        var emailBody = EmailBodyBuilder.BuildHtmlBody(assignments);

        using var client = CreateSmtpClient(config);
        var message = CreateMailMessage(config, emailBody);

        await client.SendMailAsync(message);
        Console.WriteLine($"Secret Santa assignments sent to {config.RecipientEmail}");
    }

    private static SmtpClient CreateSmtpClient(EmailConfig config)
    {
        return new SmtpClient(config.SmtpHost, config.SmtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(config.SmtpUser, config.SmtpPassword)
        };
    }

    private static MailMessage CreateMailMessage(EmailConfig config, string emailBody)
    {
        var message = new MailMessage
        {
            From = new MailAddress(config.SmtpUser),
            Subject = "Secret Santa Assignments",
            Body = emailBody,
            IsBodyHtml = true
        };

        message.To.Add(config.RecipientEmail);
        return message;
    }
}
