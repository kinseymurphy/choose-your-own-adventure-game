// Any creature the player can fight. Combat works with Monsters, so each
// subclass only supplies its stats, weapon and its own message keys.
public abstract class Monster
{
    protected readonly IDie _die;

    public string Name { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }
    public Weapon Weapon { get; set; }

    // Message keys for text that is specific to each Monster
    public abstract string PlayerAttackIntroKey { get; }
    public abstract string StatsHeaderKey { get; }
    public abstract string DefeatedNarrativeKey { get; }
    public abstract string VictoryKey { get; }
    public abstract string DefeatKey { get; }
    protected abstract IReadOnlyList<string> DefenseTauntKeys { get; }
    protected abstract IReadOnlyList<string> DamageReplyKeys { get; }

    protected Monster(IDie die, string name, Weapon weapon)
    {
        _die = die;
        Name = name;
        Weapon = weapon;
    }

    public string GetRandomDefenseTaunt(Messages messages)
    {
        int index = _die.Roll(DefenseTauntKeys.Count) - 1;
        return messages.GetMessage(DefenseTauntKeys[index]);
    }

    public string GetRandomDamageReply(Messages messages)
    {
        int index = _die.Roll(DamageReplyKeys.Count) - 1;
        return messages.GetMessage(DamageReplyKeys[index]);
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage(StatsHeaderKey));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
        Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
    }
}
