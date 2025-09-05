using System.Text;

static class EmailBodyBuilder
{
    public static string BuildHtmlBody(List<(string Giver, string Receiver)> assignments)
    {
        var emailBody = new StringBuilder();
        emailBody.AppendLine("<html><body>");
        emailBody.AppendLine("<h2 style='color: #d32f2f; font-family: Arial, sans-serif;'>Secret Santa Assignments:</h2>");
        emailBody.AppendLine("<br>");

        foreach (var assignment in assignments)
        {
            emailBody.AppendLine(BuildAssignmentLine(assignment));
        }

        emailBody.AppendLine("<br>");
        emailBody.AppendLine(BuildChristmasMessage());
        emailBody.AppendLine("</body></html>");

        return emailBody.ToString();
    }

    private static string BuildAssignmentLine((string Giver, string Receiver) assignment) =>
        $"<p style='font-family: Arial, sans-serif; font-size: 16px;'>" +
        $"<span style='color: red; font-weight: bold;'>{assignment.Giver}</span> " +
        $"<span style='color: black;'>is Secret Santa for</span> " +
        $"<span style='color: green; font-weight: bold;'>{assignment.Receiver}</span>" +
        $"</p>";

    private static string BuildChristmasMessage() =>
        "<p style='font-family: Arial, sans-serif; font-size: 14px; text-align: center;'>" +
        string.Join("", "Merry Christmas!".Select((c, i) =>
            c == ' ' ? "<span style='color: black;'> </span>" :
            $"<span style='color: {(i % 2 == 0 ? "red" : "green")}; font-weight: bold;'>{c}</span>")) +
        "<span style='color: black;'> Have fun and don't forget the </span>" +
        "<a href='https://www.churchofjesuschrist.org/welcome/christmas?lang=eng' style='color: #1976d2; text-decoration: underline;'>reason for the season</a>" +
        "</p>";
}
