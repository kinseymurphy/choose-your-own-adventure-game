// CYOATests/CombatPlayerAttackTests.cs
// The player's attack on a Monster, resolved by Combat with scripted rolls (ADR 0004)
[TestClass]
[DoNotParallelize]
public class CombatPlayerAttackTests
{
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
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

    // Monster Agility 9, so a defense roll of 9 or less blocks
    private static Monster CreateMonster(string kind, int healthPoints = 20)
    {
        // These rolls are only for the Monster's stats; the attack rolls come from Combat's die
        Monster monster = kind == "dragon"
            ? new Dragon(new ScriptedDie(1, 1, 1))
            : new Goblin(new ScriptedDie(1, 1, 1));
        monster.Agility = 9;
        monster.HealthPoints = healthPoints;
        return monster;
    }

    // Player Strength 10, so an attack roll of 10 or less hits
    private static Player CreatePlayer(Weapon? weapon = null)
    {
        return new Player
        {
            Name = "Ana", Race = "Elf", Occupation = "Thief",
            Strength = 10, Agility = 8, HealthPoints = 30,
            Weapon = weapon ?? new Weapon("dagger", 6, "-)==>")
        };
    }

    // Runs one player attack with the given rolls and returns what it printed.
    // Fails if any scripted roll is left over.
    private static string PlayerAttacks(Player player, Monster monster, string language, params int[] rolls)
    {
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);
        ScriptedDie die = new ScriptedDie(rolls);

        new Combat(player, monster, GetMessages(language), die).PlayerAttacksMonsterSequence();

        Assert.ThrowsException<AssertFailedException>(() => die.Roll(20), "Every scripted roll should be used.");
        return writer.ToString();
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void AttackRollAbovePlayerStrength_MissesWithOneRoll(string kind)
    {
        Monster monster = CreateMonster(kind);

        string output = PlayerAttacks(CreatePlayer(), monster, "English", 11);

        Assert.AreEqual(20, monster.HealthPoints);
        StringAssert.Contains(output, $"You missed {monster.Name}.");
    }

    [TestMethod]
    [DataRow("dragon", "English")]
    [DataRow("dragon", "Spanish")]
    [DataRow("dragon", "French")]
    [DataRow("dragon", "Italian")]
    [DataRow("goblin", "English")]
    [DataRow("goblin", "Spanish")]
    [DataRow("goblin", "French")]
    [DataRow("goblin", "Italian")]
    public void EachMonster_UsesItsOwnAttackIntroAndTheWeaponArt(string kind, string language)
    {
        Messages messages = GetMessages(language);
        Monster monster = CreateMonster(kind);

        string output = PlayerAttacks(CreatePlayer(), monster, language, 11);

        string intro = string.Format(
            messages.GetMessage(kind == "dragon" ? "player_attack_intro" : "goblin_player_attack_intro"),
            messages.TranslateOccupationForDisplay("Thief"),
            messages.TranslateWeaponForDisplay("dagger"));
        StringAssert.StartsWith(output, intro + Environment.NewLine + "-)==>" + Environment.NewLine);
    }

    // Same rule as the Monster's attack: blank art isn't printed
    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void WeaponWithBlankArt_PrintsNoArtLine(string kind)
    {
        Messages messages = GetMessages("English");
        Monster monster = CreateMonster(kind);

        string output = PlayerAttacks(CreatePlayer(new Weapon("dagger", 6, " ")), monster, "English", 11);

        string intro = string.Format(
            messages.GetMessage(monster.PlayerAttackIntroKey),
            messages.TranslateOccupationForDisplay("Thief"),
            messages.TranslateWeaponForDisplay("dagger"));
        Assert.AreEqual(intro + Environment.NewLine + $"You missed {monster.Name}." + Environment.NewLine, output);
    }

    [TestMethod]
    [DataRow("dragon", "English")]
    [DataRow("dragon", "Spanish")]
    [DataRow("dragon", "French")]
    [DataRow("dragon", "Italian")]
    [DataRow("goblin", "English")]
    [DataRow("goblin", "Spanish")]
    [DataRow("goblin", "French")]
    [DataRow("goblin", "Italian")]
    public void Block_ShowsTheScriptedTauntWithTheTranslatedRace(string kind, string language)
    {
        Messages messages = GetMessages(language);
        Monster monster = CreateMonster(kind);

        string output = PlayerAttacks(CreatePlayer(), monster, language, 10, 9, 2);

        Assert.AreEqual(20, monster.HealthPoints);
        StringAssert.Contains(output, string.Format(messages.GetMessage("monster_defended_player_attack"), monster.Name));
        StringAssert.Contains(output, string.Format(messages.GetMessage($"{kind}_defense_taunt_2"), messages.TranslateRaceForDisplay("Elf")));
    }

    [TestMethod]
    [DataRow("dragon", "English")]
    [DataRow("dragon", "Spanish")]
    [DataRow("dragon", "French")]
    [DataRow("dragon", "Italian")]
    [DataRow("goblin", "English")]
    [DataRow("goblin", "Spanish")]
    [DataRow("goblin", "French")]
    [DataRow("goblin", "Italian")]
    public void DamagingHit_ReducesMonsterHealthAndShowsTheScriptedReply(string kind, string language)
    {
        Messages messages = GetMessages(language);
        Monster monster = CreateMonster(kind);

        string output = PlayerAttacks(CreatePlayer(), monster, language, 1, 10, 6, 3);

        Assert.AreEqual(14, monster.HealthPoints);
        StringAssert.Contains(output, string.Format(messages.GetMessage("player_damage_dealt"), 6));
        StringAssert.Contains(output, string.Format(messages.GetMessage("monster_health_now"), monster.Name, 14));
        StringAssert.EndsWith(output, messages.GetMessage($"{kind}_damage_reply_3") + Environment.NewLine);
    }

    [TestMethod]
    [DataRow("dragon", "English")]
    [DataRow("dragon", "Spanish")]
    [DataRow("dragon", "French")]
    [DataRow("dragon", "Italian")]
    [DataRow("goblin", "English")]
    [DataRow("goblin", "Spanish")]
    [DataRow("goblin", "French")]
    [DataRow("goblin", "Italian")]
    public void FinalHit_StopsMonsterHealthAtZeroAndShowsItsDefeatedNarrative(string kind, string language)
    {
        Messages messages = GetMessages(language);
        Monster monster = CreateMonster(kind, healthPoints: 4);

        // A hit for 6 against 4 Health Points; no reply roll after a defeating hit
        string output = PlayerAttacks(CreatePlayer(), monster, language, 1, 10, 6);

        Assert.AreEqual(0, monster.HealthPoints);
        StringAssert.Contains(output, string.Format(messages.GetMessage("monster_health_now"), monster.Name, 0));
        StringAssert.EndsWith(output, messages.GetMessage(monster.DefeatedNarrativeKey) + Environment.NewLine);
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void MagicalWeapon_ItsMaxDamageIsTheDamageRollsSides(string kind)
    {
        Weapon magicalDagger = new Weapon("magical dagger", 10, "-)==>") { IsMagical = true };
        Monster monster = CreateMonster(kind);

        PlayerAttacks(CreatePlayer(magicalDagger), monster, "English", 1, 10, 10, 1);

        Assert.AreEqual(10, monster.HealthPoints);
        Assert.ThrowsException<AssertFailedException>(
            () => PlayerAttacks(CreatePlayer(magicalDagger), CreateMonster(kind), "English", 1, 10, 11, 1),
            "A d10 can't roll 11.");
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void NoWeapon_PrintsTheEnglishTextAndUsesNoRolls(string kind)
    {
        Player player = CreatePlayer();
        player.Weapon = null;
        Monster monster = CreateMonster(kind);

        string output = PlayerAttacks(player, monster, "Spanish");

        Assert.AreEqual("You have no weapon to attack with." + Environment.NewLine, output);
        Assert.AreEqual(20, monster.HealthPoints);
    }
}
