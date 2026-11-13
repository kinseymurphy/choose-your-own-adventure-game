// CYOATests/GameDiceTests.cs
// Drives the whole game with a scripted die (ADR 0004) and scripted console input.
[TestClass]
[DoNotParallelize]
public class GameDiceTests
{
    private const int TimeoutMs = 10000;

    // Game creates the dragon (three d20 rolls) and then the goblin (three d12 rolls)
    private static readonly int[] DragonStats = { 15, 9, 18 };
    private static readonly int[] GoblinStats = { 3, 12, 7 };

    // Creates an English Human Fighter named Bob and proceeds to the path choice.
    // Character creation's rolls stay outside the dice seam.
    private const string CreateBob = "human\nBob\nfighter\nroll\nroll\nroll\nroll\n2\n";

    private TextReader? _originalIn;
    private TextWriter? _originalOut;
    private string _folder = "";

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        _folder = Path.Combine(Path.GetTempPath(), "cyoa-dice-" + Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);

        if (Directory.Exists(_folder))
            Directory.Delete(_folder, true);
    }

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Plays a whole Session and returns everything printed. Without a store nothing is saved.
    private static string Play(string input, IDie die, ProfileStore? store = null)
    {
        Console.SetIn(new StringReader(input));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        new Game(store, die).StartGame();

        return writer.ToString();
    }

    // Picks the only saved Profile from the profile menu, then takes the path and attacks
    private static string PickProfileAndAttack(string path) => "1\n1\n1\n" + path + "\na\n";

    private static void AssertEveryRollUsed(IDie die)
    {
        Assert.ThrowsException<AssertFailedException>(() => die.Roll(20), "Every scripted roll should be used.");
    }

    private static string StatsBlock(Messages messages, string headerKey, int[] stats)
    {
        return string.Join(Environment.NewLine,
            messages.GetMessage(headerKey),
            messages.GetMessage("stats_name") + (headerKey == "dragon_stats_header" ? "Smolderfang" : "Grubnik"),
            messages.GetMessage("stats_strength") + stats[0],
            messages.GetMessage("stats_agility") + stats[1],
            messages.GetMessage("stats_health") + stats[2]);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void WestPath_ShowsGrubnikWithTheScriptedStats()
    {
        Messages messages = GetMessages("English");
        IDie die = new ScriptedDie(DragonStats.Concat(GoblinStats).ToArray());

        string output = Play("1\n1\n" + CreateBob + "w\nr\n", die);

        StringAssert.Contains(output, StatsBlock(messages, "goblin_stats_header", GoblinStats));
        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void NorthPath_ShowsSmolderfangWithTheScriptedStats()
    {
        Messages messages = GetMessages("English");
        IDie die = new ScriptedDie(DragonStats.Concat(GoblinStats).ToArray());

        string output = Play("1\n1\n" + CreateBob + "n\nr\n", die);

        StringAssert.Contains(output, StatsBlock(messages, "dragon_stats_header", DragonStats));
        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    // Bob (Strength 14, Agility 8) is wounded by Smolderfang (Strength 15, Agility 9, Health Points 18),
    // then wins. The saved Profile keeps its rolled Health Points (ADR 0003).
    [TestMethod]
    [Timeout(TimeoutMs)]
    public void NorthDeadEnd_WoundedVictory_EndsWithTheDragonVictoryAndKeepsTheSavedHealthPoints()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(new Player
        {
            Name = "Bob", Race = "Human", Occupation = "Fighter",
            Strength = 14, Agility = 8, HealthPoints = 30, Gold = 5,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        });
        string savedFile = File.ReadAllText(Path.Combine(_folder, "profiles.json"));
        Messages messages = GetMessages("English");
        IDie die = new ScriptedDie(DragonStats.Concat(GoblinStats).Concat(new[]
        {
            1, 20, 12, 2,   // round 1: Bob hits for 12, dragon at 6, reply 2
            1, 20, 6,       // dragon hits Bob for 6
            2, 20, 7        // round 2: Bob hits for 7, dragon at -1
        }).ToArray());

        string output = Play(PickProfileAndAttack("n") + "a\n", die, store);

        StringAssert.Contains(output, "Bob's health points are now 24.");
        StringAssert.EndsWith(output, messages.GetMessage("victory") + Environment.NewLine);
        AssertEveryRollUsed(die);
        Assert.AreEqual(savedFile, File.ReadAllText(Path.Combine(_folder, "profiles.json")));
    }

    // Grubnik (Strength 3, Agility 12, Health Points 7) defeats Ana in the first round.
    // The saved Profile keeps its rolled Health Points, Gold, Protection and Weapon (ADR 0003).
    [TestMethod]
    [Timeout(TimeoutMs)]
    public void WestDeadEnd_Defeat_EndsWithTheGoblinDefeatAndKeepsTheSavedProfile()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(new Player
        {
            Name = "Ana", Race = "Elf", Occupation = "Archer",
            Strength = 2, Agility = 6, HealthPoints = 5, Gold = 9,
            Weapon = new Weapon("long bow", 8, "}    -->")
        });
        string savedFile = File.ReadAllText(Path.Combine(_folder, "profiles.json"));
        Messages messages = GetMessages("English");
        IDie die = new ScriptedDie(DragonStats.Concat(GoblinStats).Concat(new[]
        {
            20,             // round 1: Ana misses
            1, 20, 6        // Grubnik hits Ana for 6, Ana at 0
        }).ToArray());

        string output = Play(PickProfileAndAttack("w"), die, store);

        StringAssert.Contains(output, "Ana's health points are now 0.");
        StringAssert.EndsWith(output, messages.GetMessage("goblin_defeat") + Environment.NewLine);
        AssertEveryRollUsed(die);
        Assert.AreEqual(savedFile, File.ReadAllText(Path.Combine(_folder, "profiles.json")));
    }
}
