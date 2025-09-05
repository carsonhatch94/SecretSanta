using DotNetEnv;
using System.Net;
using System.Net.Mail;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        Env.Load();
        var couples = LoadCouplesFromEnv();
        var participants = couples.SelectMany(c => new[] { c.Person, c.Partner }).ToList();
        var assignments = AssignSecretSanta(participants, couples);
        await SendAssignmentEmail(assignments);
    }

    static List<(string Person, string Partner)> LoadCouplesFromEnv()
    {
        var couplesString = Environment.GetEnvironmentVariable("COUPLES");

        if (string.IsNullOrEmpty(couplesString))
        {
            throw new InvalidOperationException("COUPLES environment variable not found in .env file");
        }

        return couplesString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(couple =>
            {
                var parts = couple.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    throw new FormatException($"Invalid couple format: {couple}. Expected format: 'Person:Partner'");
                }
                return (Person: parts[0].Trim(), Partner: parts[1].Trim());
            })
            .ToList();
    }

    static List<(string Giver, string Receiver)> AssignSecretSanta(List<string> participants, List<(string Person, string Partner)> couples)
    {
        var random = new Random();
        var partnerLookup = CreatePartnerLookup(couples);
        const int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var receivers = new List<string>(participants);

            // Fisher-Yates shuffle
            for (int i = receivers.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (receivers[i], receivers[j]) = (receivers[j], receivers[i]);
            }

            // Validate the entire assignment at once
            bool isValid = participants.Zip(receivers)
                .All(pair => pair.First != pair.Second &&
                            pair.Second != partnerLookup.GetValueOrDefault(pair.First));

            if (isValid)
            {
                return participants.Zip(receivers)
                    .Select(pair => (Giver: pair.First, Receiver: pair.Second))
                    .ToList();
            }
        }

        throw new InvalidOperationException("Could not generate valid Secret Santa assignments after maximum attempts");
    }

    static Dictionary<string, string> CreatePartnerLookup(List<(string Person, string Partner)> couples)
    {
        return couples.SelectMany(c => new[] {
            KeyValuePair.Create(c.Person, c.Partner),
            KeyValuePair.Create(c.Partner, c.Person)
        }).ToDictionary();
    }

    static async Task SendAssignmentEmail(List<(string Giver, string Receiver)> assignments)
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

        var emailBody = new StringBuilder();
        emailBody.AppendLine("Secret Santa Assignments:");
        emailBody.AppendLine("");
        foreach (var assignment in assignments)
        {
            emailBody.AppendLine($"{assignment.Giver} is Secret Santa for {assignment.Receiver}");
        }

        using var client = new SmtpClient(smtpHost, smtpPort);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(smtpUser, smtpPassword);

        var message = new MailMessage
        {
            From = new MailAddress(smtpUser),
            Subject = "Secret Santa Assignments",
            Body = emailBody.ToString(),
            IsBodyHtml = false
        };

        message.To.Add(recipientEmail);

        await client.SendMailAsync(message);
        Console.WriteLine($"Secret Santa assignments sent to {recipientEmail}");
    }
}