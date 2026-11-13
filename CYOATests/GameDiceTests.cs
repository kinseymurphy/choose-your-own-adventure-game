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

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Plays a whole Session without saving and returns everything printed
    private static string Play(string input, IDie die)
    {
        Console.SetIn(new StringReader(input));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        new Game(null, die).StartGame();

        return writer.ToString();
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
}
