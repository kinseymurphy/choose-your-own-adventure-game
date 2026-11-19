// CYOATests/CombatMonsterAttackTests.cs
// A Monster's attack on the player, resolved by Combat with scripted rolls (ADR 0004)
[TestClass]
[DoNotParallelize]
public class CombatMonsterAttackTests
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

    private static Messages GetMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    // Monster Strength 10, so an attack roll of 10 or less hits
    private static Monster CreateMonster(string kind)
    {
        // These rolls are only for the Monster's stats; the attack rolls come from Combat's die
        Monster monster = kind == "dragon"
            ? new Dragon(new ScriptedDie(1, 1, 1))
            : new Goblin(new ScriptedDie(1, 1, 1));
        monster.Strength = 10;
        return monster;
    }

    // Player Agility 8, so a defense roll of 8 or less blocks
    private static Player CreatePlayer(int healthPoints = 30, int protection = 0)
    {
        return new Player
        {
            Name = "Bob", Race = "Human", Occupation = "Fighter",
            Strength = 14, Agility = 8, HealthPoints = healthPoints, Protection = protection,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // Runs one Monster attack with the given rolls and returns what it printed.
    // Fails if any scripted roll is left over.
    private static string MonsterAttacks(Monster monster, Player player, params int[] rolls)
    {
        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);
        ScriptedDie die = new ScriptedDie(rolls);

        new Combat(player, monster, GetMessages(), die).MonsterAttacksPlayerSequence();

        Assert.ThrowsException<AssertFailedException>(() => die.Roll(20), "Every scripted roll should be used.");
        return writer.ToString();
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void AttackRollAboveMonsterStrength_MissesWithOneRoll(string kind)
    {
        Monster monster = CreateMonster(kind);
        Player player = CreatePlayer();

        string output = MonsterAttacks(monster, player, 11);

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(output, $"{monster.Name} missed Bob.");
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void DefenseRollAtOrBelowPlayerAgility_BlocksWithTwoRolls(string kind)
    {
        Monster monster = CreateMonster(kind);
        Player player = CreatePlayer();

        string output = MonsterAttacks(monster, player, 10, 8);

        Assert.AreEqual(30, player.HealthPoints);
        StringAssert.Contains(output, $"{monster.Name} hits Bob.");
        StringAssert.Contains(output, "Bob successfully defends the attack.");
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void UnblockedHit_ReducesHealthPointsByTheDamageRoll(string kind)
    {
        Monster monster = CreateMonster(kind);
        Player player = CreatePlayer();

        string output = MonsterAttacks(monster, player, 1, 9, 5);

        Assert.AreEqual(25, player.HealthPoints);
        StringAssert.Contains(output, $"{monster.Name} deals 5 damage.");
        StringAssert.Contains(output, "Bob's health points are now 25.");
        Assert.IsFalse(output.Contains("Magical Armor"), "No Protection, so nothing is absorbed.");
    }

    [TestMethod]
    [DataRow("dragon", 6, 3, 3)]
    [DataRow("dragon", 3, 0, 3)]
    [DataRow("dragon", 2, 0, 2)]
    [DataRow("goblin", 6, 3, 3)]
    [DataRow("goblin", 3, 0, 3)]
    [DataRow("goblin", 2, 0, 2)]
    public void Protection_IsSubtractedFromTheDamageRollButNeverBelowZero(string kind, int damageRoll, int damage, int absorbed)
    {
        Monster monster = CreateMonster(kind);
        Player player = CreatePlayer(protection: 3);

        string output = MonsterAttacks(monster, player, 1, 20, damageRoll);

        Assert.AreEqual(30 - damage, player.HealthPoints);
        StringAssert.Contains(output, $"Your Magical Armor absorbs {absorbed} damage.");
        StringAssert.Contains(output, $"{monster.Name} deals {damage} damage.");
    }

    [TestMethod]
    [DataRow("dragon")]
    [DataRow("goblin")]
    public void HitForMoreThanThePlayersHealthPoints_StopsAtZeroAndDefeatsThePlayer(string kind)
    {
        Monster monster = CreateMonster(kind);
        Player player = CreatePlayer(healthPoints: 2);

        string output = MonsterAttacks(monster, player, 1, 20, 5);

        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.Contains(output, "Bob's health points are now 0.");
        StringAssert.Contains(output, "Bob has been defeated.");
    }
}
