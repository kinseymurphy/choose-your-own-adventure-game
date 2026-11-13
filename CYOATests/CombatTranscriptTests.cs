// CYOATests/CombatTranscriptTests.cs
// Characterization tests (ADR 0004): whole fights, played with scripted dice and
// scripted input, must print exactly the approved transcripts in CombatTranscripts/.
// The transcripts were produced from the combat code before it moved into Combat
// and checked by hand. Never regenerate them to make a test pass.
[TestClass]
[DoNotParallelize]
public class CombatTranscriptTests
{
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);
    }

    private sealed class Fight
    {
        public required Player Player { get; init; }
        public required Func<IDie, Monster> CreateMonster { get; init; }
        // Every roll in order, starting with the Monster's three stat rolls
        public required int[] Rolls { get; init; }
        public required string Input { get; init; }
    }

    // Bob (Strength 14, Agility 8) beats Smolderfang (Strength 10, Agility 9, Health Points 15)
    private static Fight DragonVictory() => new Fight
    {
        Player = new Player
        {
            Name = "Bob", Race = "Human", Occupation = "Fighter",
            Strength = 14, Agility = 8, HealthPoints = 30, Gold = 5,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        },
        CreateMonster = die => new Dragon(die),
        Rolls = new[]
        {
            10, 9, 15,              // dragon stats
            18, 12,                 // round 1: Bob misses; dragon misses
            5, 4, 2, 3, 15, 7,      // round 2: dragon blocks, taunt 2; dragon hits Bob for 7
            7, 16, 9, 3, 4, 2,      // round 3: Bob hits for 9, reply 3; Bob blocks
            1, 20, 12               // round 4: Bob hits for 12, dragon at -6
        },
        Input = "a\na\na\n"
    };

    // Ana (Strength 2, Agility 6, Protection 3) loses to Grubnik (Strength 11, Agility 5, Health Points 12)
    private static Fight GoblinDefeat() => new Fight
    {
        Player = new Player
        {
            Name = "Ana", Race = "Elf", Occupation = "Archer",
            Strength = 2, Agility = 6, HealthPoints = 8, Gold = 0, Protection = 3,
            Weapon = new Weapon("long bow", 8, "}    -->")
        },
        CreateMonster = die => new Goblin(die),
        Rolls = new[]
        {
            11, 5, 12,              // goblin stats
            20, 15,                 // round 1: Ana misses; Grubnik misses
            19, 4, 3,               // round 2: Ana misses; Ana blocks
            1, 10, 5, 4, 6, 18, 2,  // round 3: Ana hits for 5, reply 4; Protection absorbs all 2
            17, 2, 9, 6,            // round 4: Ana misses; 6 less 3 Protection, Ana at 5
            13, 1, 11, 6,           // round 5: Ana misses; Ana at 2
            14, 5, 12, 6            // round 6: Ana misses; Ana at 0 (not -1)
        },
        Input = "a\na\na\na\na\n"
    };

    // The first round happens before the prompt, an invalid answer plays another round, then Bob retreats
    private static Fight DragonRetreat() => new Fight
    {
        Player = DragonVictory().Player,
        CreateMonster = die => new Dragon(die),
        Rolls = new[]
        {
            10, 9, 15,              // dragon stats
            6, 19, 4, 1, 20,        // round 1: Bob hits for 4, reply 1; dragon misses
            18, 13                  // round 2: both miss
        },
        Input = "x\nr\n"
    };

    // A Magical Weapon's damage roll is a d16, so a 16 is possible
    private static Fight MagicalWeaponVictory() => new Fight
    {
        Player = new Player
        {
            Name = "Bob", Race = "Human", Occupation = "Fighter",
            Strength = 14, Agility = 8, HealthPoints = 30, Gold = 0, Protection = 3,
            Weapon = new Weapon("magical long sword", 16, "-)=====>") { IsMagical = true }
        },
        CreateMonster = die => new Dragon(die),
        Rolls = new[] { 10, 9, 15, 2, 20, 16 },
        Input = ""
    };

    private static Fight GetFight(string name) => name switch
    {
        "dragon-victory" => DragonVictory(),
        "goblin-defeat" => GoblinDefeat(),
        "dragon-retreat" => DragonRetreat(),
        "magical-weapon-victory" => MagicalWeaponVictory(),
        _ => throw new ArgumentException(name)
    };

    [TestMethod]
    [DataRow("dragon-victory", "English")]
    [DataRow("dragon-victory", "Spanish")]
    [DataRow("dragon-victory", "French")]
    [DataRow("dragon-victory", "Italian")]
    [DataRow("goblin-defeat", "English")]
    [DataRow("goblin-defeat", "Spanish")]
    [DataRow("goblin-defeat", "French")]
    [DataRow("goblin-defeat", "Italian")]
    [DataRow("dragon-retreat", "English")]
    [DataRow("magical-weapon-victory", "English")]
    public void Fight_PrintsTheApprovedTranscript(string fightName, string language)
    {
        Fight fight = GetFight(fightName);
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        ScriptedDie die = new ScriptedDie(fight.Rolls);
        Monster monster = fight.CreateMonster(die);

        Console.SetIn(new StringReader(fight.Input));
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        new Combat(fight.Player, monster, messages, die).StartCombat();

        AssertMatchesApprovedTranscript($"{fightName}.{language.ToLower()}", writer.ToString());
        Assert.ThrowsException<AssertFailedException>(() => die.Roll(20), "Every scripted roll should be used.");
    }

    private static void AssertMatchesApprovedTranscript(string name, string actual)
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "CombatTranscripts");
        string approvedPath = Path.Combine(folder, name + ".approved.txt");
        string received = Normalize(actual);

        string approved = File.Exists(approvedPath) ? Normalize(File.ReadAllText(approvedPath)) : "";
        if (approved == received)
            return;

        Directory.CreateDirectory(folder);
        string receivedPath = Path.Combine(folder, name + ".received.txt");
        File.WriteAllText(receivedPath, received);
        Assert.Fail($"Combat output differs from CombatTranscripts/{name}.approved.txt. Received output written to {receivedPath}");
    }

    private static string Normalize(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}
