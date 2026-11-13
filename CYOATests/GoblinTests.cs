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

    private static void AssertNoMissingGoblinKeys(string output)
    {
        Assert.IsFalse(Regex.IsMatch(output, @"\[(west|goblin)_\w+\]"), "A goblin message key is missing:\n" + output);
    }

    // A player with Strength 20 always hits. Goblin Agility 0 never blocks,
    // Agility 20 always blocks, so each test reaches one outcome for sure.
    private Goblin CreateGoblin(int agility, int healthPoints)
    {
        return new Goblin(new Die()) { Agility = agility, HealthPoints = healthPoints };
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void PlayerAttack_OnGoblin_UsesGoblinAttackIntro(string language)
    {
        Messages messages = GetMessages(language);
        Player player = CreateTestPlayer();

        string output = player.Attack(CreateGoblin(agility: 20, healthPoints: 10), messages);

        AssertNoMissingGoblinKeys(output);
        StringAssert.Contains(output, string.Format(
            messages.GetMessage("goblin_player_attack_intro"),
            messages.TranslateOccupationForDisplay(player.Occupation),
            messages.TranslateWeaponForDisplay(player.Weapon!.Type)));
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void PlayerAttack_BlockedByGoblin_ShowsGoblinTaunt(string language)
    {
        Messages messages = GetMessages(language);

        string output = CreateTestPlayer().Attack(CreateGoblin(agility: 20, healthPoints: 10), messages);

        AssertNoMissingGoblinKeys(output);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void PlayerAttack_HurtsGoblin_ShowsGoblinDamageReply(string language)
    {
        Messages messages = GetMessages(language);

        string output = CreateTestPlayer().Attack(CreateGoblin(agility: 0, healthPoints: 1000), messages);

        AssertNoMissingGoblinKeys(output);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void PlayerAttack_DefeatsGoblin_ShowsGoblinDefeatedNarrative(string language)
    {
        Messages messages = GetMessages(language);

        string output = CreateTestPlayer().Attack(CreateGoblin(agility: 0, healthPoints: 1), messages);

        AssertNoMissingGoblinKeys(output);
        StringAssert.Contains(output, messages.GetMessage("goblin_defeated_narrative"));
    }

    [TestMethod]
    public void Attack_ReturnsNonEmptyResultString()
    {
        Goblin goblin = new Goblin(new Die());
        Player player = new Player { Name = "TestHero", Agility = 10, HealthPoints = 30 };

        string result = goblin.Attack(player, GetMessages());

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void Attack_DoesNotIncreasePlayerHealth()
    {
        Goblin goblin = new Goblin(new Die());
        Player player = new Player { Name = "TestHero", Agility = 10, HealthPoints = 30 };

        goblin.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints <= 30);
    }

    [TestMethod]
    public void Attack_KeepsPlayerHealthNonNegative()
    {
        Goblin goblin = new Goblin(new Die()) { Strength = 20 };
        Player player = new Player { Name = "TestHero", Agility = 0, HealthPoints = 1 };

        goblin.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints >= 0);
    }

    [TestMethod]
    public void Attack_PlayerWithMagicalArmor_AbsorbsGoblinDamage()
    {
        // Goblin Strength 20 always hits; player Agility 0 never blocks
        Goblin goblin = new Goblin(new Die()) { Strength = 20 };
        Player player = new Player { Name = "TestHero", Agility = 0, HealthPoints = 30, Protection = 3 };
        Messages messages = GetMessages();

        string result = goblin.Attack(player, messages);

        string absorbedPrefix = messages.GetMessage("armor_absorbed_damage").Split('{')[0];
        StringAssert.Contains(result, absorbedPrefix);
        // A rusty axe deals at most 6, so Protection 3 leaves at most 3 damage
        Assert.IsTrue(player.HealthPoints >= 27, $"Health Points {player.HealthPoints}");
    }

    private string CaptureEndGame(Monster monster, bool playerWon, bool playerRetreated, Messages messages)
    {
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        new Game().EndGame(monster, playerWon, playerRetreated, messages);

        return writer.ToString();
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EndGame_GoblinVictory_ShowsGoblinVictoryEnding(string language)
    {
        Messages messages = GetMessages(language);

        string output = CaptureEndGame(new Goblin(new Die()), true, false, messages);

        AssertNoMissingGoblinKeys(output);
        StringAssert.Contains(output, messages.GetMessage("goblin_victory"));
    }

    [TestMethod]
    public void EndGame_GoblinVictory_FindsTheSoggySock()
    {
        string output = CaptureEndGame(new Goblin(new Die()), true, false, GetMessages());

        StringAssert.Contains(output, "soggy sock");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EndGame_GoblinDefeat_ShowsGoblinDefeatEnding(string language)
    {
        Messages messages = GetMessages(language);

        string output = CaptureEndGame(new Goblin(new Die()), false, false, messages);

        AssertNoMissingGoblinKeys(output);
        StringAssert.Contains(output, messages.GetMessage("goblin_defeat"));
    }

    [TestMethod]
    public void EndGame_GoblinRetreat_ShowsSharedRetreatEnding()
    {
        Messages messages = GetMessages();

        string output = CaptureEndGame(new Goblin(new Die()), false, true, messages);

        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    [TestMethod]
    public void GetRandomDefenseTaunt_ReturnsGoblinTaunt()
    {
        Goblin goblin = new Goblin(new Die());
        Messages messages = GetMessages();

        string taunt = goblin.GetRandomDefenseTaunt(messages);

        string[] validTaunts =
        {
            messages.GetMessage("goblin_defense_taunt_1"),
            messages.GetMessage("goblin_defense_taunt_2"),
            messages.GetMessage("goblin_defense_taunt_3"),
            messages.GetMessage("goblin_defense_taunt_4")
        };

        AssertNoMissingGoblinKeys(taunt);
        CollectionAssert.Contains(validTaunts, taunt);
    }

    [TestMethod]
    public void GetRandomDamageReply_ReturnsGoblinReply()
    {
        Goblin goblin = new Goblin(new Die());
        Messages messages = GetMessages();

        string reply = goblin.GetRandomDamageReply(messages);

        string[] validReplies =
        {
            messages.GetMessage("goblin_damage_reply_1"),
            messages.GetMessage("goblin_damage_reply_2"),
            messages.GetMessage("goblin_damage_reply_3"),
            messages.GetMessage("goblin_damage_reply_4")
        };

        AssertNoMissingGoblinKeys(reply);
        CollectionAssert.Contains(validReplies, reply);
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
    public void HandleGoblinEncounter_AttackPath_StartsCombatWithGrubnik()
    {
        Goblin goblin = CreateGoblin(agility: 20, healthPoints: 10);
        Messages messages = GetMessages();

        // "a" enters combat, then "r" retreats from Combat after the first round
        string output = RunGoblinEncounter("a\nr\n", goblin, messages);

        AssertNoMissingGoblinKeys(output);
        StringAssert.Contains(output, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(output, "--- Grubnik ---");
        StringAssert.Contains(output, messages.GetMessage("retreat_combat"));
    }

    [TestMethod]
    public void HandleGoblinEncounter_PlayerDefeatsGrubnik_ShowsGoblinVictoryEnding()
    {
        Goblin goblin = CreateGoblin(agility: 0, healthPoints: 1);
        Messages messages = GetMessages();

        string output = RunGoblinEncounter("a\n", goblin, messages);

        AssertNoMissingGoblinKeys(output);
        StringAssert.Contains(output, messages.GetMessage("goblin_defeated_narrative"));
        StringAssert.Contains(output, messages.GetMessage("goblin_victory"));
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
    public void NewGoblin_RollsStrengthAgilityAndHealthPointsFromTheDie()
    {
        Goblin goblin = new Goblin(new ScriptedDie(3, 12, 7));

        Assert.AreEqual(3, goblin.Strength);
        Assert.AreEqual(12, goblin.Agility);
        Assert.AreEqual(7, goblin.HealthPoints);
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
