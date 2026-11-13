using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[DoNotParallelize]
public class GoblinTests
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

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 20,
            Agility = 20,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // Runs Game's private HandleGoblinEncounter with the given console input
    // and returns everything it printed.
    private string RunGoblinEncounter(string input, Goblin goblin, Messages messages)
    {
        Console.SetIn(new StringReader(input));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        MethodInfo? method = typeof(Game).GetMethod(
            "HandleGoblinEncounter",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.IsNotNull(method, "HandleGoblinEncounter method was not found.");

        method!.Invoke(new Game(), new object[] { CreateTestPlayer(), goblin, messages });

        return writer.ToString();
    }

    [TestMethod]
    public void HandleGoblinEncounter_RetreatPath_DisplaysEncounterFlowAndRetreatEnding()
    {
        Goblin goblin = new Goblin(new Die());
        Messages messages = GetMessages();

        string output = RunGoblinEncounter("r\n", goblin, messages);

        Assert.IsFalse(Regex.IsMatch(output, @"\[(west|goblin)_\w+\]"), "A goblin message key is missing:\n" + output);
        StringAssert.Contains(output, messages.GetMessage("west_path_narrative"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("goblin_intro"), goblin.Name));
        StringAssert.Contains(output, messages.GetMessage("goblin_stats_intro"));
        StringAssert.Contains(output, messages.GetMessage("goblin_stats_header"));
        StringAssert.Contains(output, messages.GetMessage("goblin_encounter_prompt"));
        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    [TestMethod]
    public void HandleGoblinEncounter_InvalidInput_ShowsInvalidAndAsksAgain()
    {
        Goblin goblin = new Goblin(new Die());
        Messages messages = GetMessages();

        string output = RunGoblinEncounter("x\nr\n", goblin, messages);

        string prompt = messages.GetMessage("goblin_encounter_prompt");
        int promptCount = Regex.Matches(output, Regex.Escape(prompt)).Count;

        StringAssert.Contains(output, messages.GetMessage("invalid"));
        Assert.AreEqual(2, promptCount, "The goblin prompt should be shown again after invalid input.");
        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    private Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void Goblin_IsAMonster()
    {
        Goblin goblin = new Goblin(new Die());

        Assert.IsInstanceOfType(goblin, typeof(Monster));
    }

    [TestMethod]
    public void NewGoblin_InitializesRequiredProperties()
    {
        Goblin goblin = new Goblin(new Die());

        Assert.AreEqual("Grubnik", goblin.Name);
        Assert.IsNotNull(goblin.Weapon);
        Assert.AreEqual("rusty axe", goblin.Weapon.Type);
        Assert.AreEqual(6, goblin.Weapon.MaxDamage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(goblin.Weapon.AsciiArt));
    }

    [TestMethod]
    public void NewGoblin_RollsStatsFrom1To12()
    {
        // Stats are random, so check many goblins to catch a d20 roll
        for (int i = 0; i < 200; i++)
        {
            Goblin goblin = new Goblin(new Die());

            Assert.IsTrue(goblin.Strength >= 1 && goblin.Strength <= 12, $"Strength {goblin.Strength}");
            Assert.IsTrue(goblin.Agility >= 1 && goblin.Agility <= 12, $"Agility {goblin.Agility}");
            Assert.IsTrue(goblin.HealthPoints >= 1 && goblin.HealthPoints <= 12, $"Health Points {goblin.HealthPoints}");
        }
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void GoblinDisplayStats_UsesLocalizedLabelsAndGoblinHeader(string language)
    {
        Goblin goblin = new Goblin(new Die());
        Messages messages = GetMessages(language);

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        goblin.DisplayStats(messages);

        string output = writer.ToString();

        StringAssert.DoesNotMatch(output, new System.Text.RegularExpressions.Regex(@"\[goblin_stats_header\]"));
        StringAssert.Contains(output, messages.GetMessage("goblin_stats_header"));
        StringAssert.Contains(output, messages.GetMessage("stats_name") + "Grubnik");
        StringAssert.Contains(output, messages.GetMessage("stats_strength"));
        StringAssert.Contains(output, messages.GetMessage("stats_agility"));
        StringAssert.Contains(output, messages.GetMessage("stats_health"));
        StringAssert.Contains(output, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(output, messages.GetMessage("stats_damage") + "6");
    }
}
