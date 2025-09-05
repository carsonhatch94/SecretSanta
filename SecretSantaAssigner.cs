static class SecretSantaAssigner
{
    private const int MaxAttempts = 100;

    public static List<(string Giver, string Receiver)> GenerateAssignments(
        List<string> participants,
        List<(string Person, string Partner)> couples)
    {
        var partnerLookup = CreatePartnerLookup(couples);
        var random = new Random();

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var shuffledReceivers = ShuffleParticipants(participants, random);

            if (IsValidAssignment(participants, shuffledReceivers, partnerLookup))
            {
                return CreateAssignments(participants, shuffledReceivers);
            }
        }

        throw new InvalidOperationException("Could not generate valid Secret Santa assignments after maximum attempts");
    }

    private static List<string> ShuffleParticipants(List<string> participants, Random random)
    {
        var receivers = new List<string>(participants);

        // Fisher-Yates shuffle
        for (int i = receivers.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (receivers[i], receivers[j]) = (receivers[j], receivers[i]);
        }

        return receivers;
    }

    private static bool IsValidAssignment(
        List<string> participants,
        List<string> receivers,
        Dictionary<string, string> partnerLookup)
    {
        return participants.Zip(receivers)
            .All(pair => pair.First != pair.Second &&
                        pair.Second != partnerLookup.GetValueOrDefault(pair.First));
    }

    private static List<(string Giver, string Receiver)> CreateAssignments(
        List<string> participants,
        List<string> receivers)
    {
        return participants.Zip(receivers)
            .Select(pair => (Giver: pair.First, Receiver: pair.Second))
            .ToList();
    }

    private static Dictionary<string, string> CreatePartnerLookup(List<(string Person, string Partner)> couples)
    {
        return couples.SelectMany(c => new[] {
            KeyValuePair.Create(c.Person, c.Partner),
            KeyValuePair.Create(c.Partner, c.Person)
        }).ToDictionary();
    }
}
