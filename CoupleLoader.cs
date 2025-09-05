static class CoupleLoader
{
    public static List<(string Person, string Partner)> LoadFromEnvironment()
    {
        var couplesString = Environment.GetEnvironmentVariable("COUPLES");

        if (string.IsNullOrEmpty(couplesString))
        {
            throw new InvalidOperationException("COUPLES environment variable not found in .env file");
        }

        return couplesString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseCouple)
            .ToList();
    }

    private static (string Person, string Partner) ParseCouple(string couple)
    {
        var parts = couple.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            throw new FormatException($"Invalid couple format: {couple}. Expected format: 'Person:Partner'");
        }
        return (Person: parts[0].Trim(), Partner: parts[1].Trim());
    }
}
