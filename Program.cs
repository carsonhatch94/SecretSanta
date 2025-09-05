using DotNetEnv;

class Program
{
    static async Task Main(string[] args)
    {
        Env.Load();
        var couples = CoupleLoader.LoadFromEnvironment();
        var participants = couples.SelectMany(c => new[] { c.Person, c.Partner }).ToList();
        var assignments = SecretSantaAssigner.GenerateAssignments(participants, couples);
        await EmailSender.SendAssignmentsAsync(assignments);
    }
}