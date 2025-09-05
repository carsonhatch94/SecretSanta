using System;
using System.Collections.Generic;
using System.Linq;
using DotNetEnv;

class Program
{
    static void Main(string[] args)
    {
        Env.Load();
        var couples = LoadCouplesFromEnv();

        // Create list of all participants
        var participants = couples.SelectMany(c => new[] { c.Person, c.Partner }).ToList();

        // Shuffle and assign Secret Santa
        var assignments = AssignSecretSanta(participants, couples);

        // Display results
        Console.WriteLine("Secret Santa Assignments:");
        foreach (var assignment in assignments)
        {
            Console.WriteLine($"{assignment.Giver} is Secret Santa for {assignment.Receiver}");
        }
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
}