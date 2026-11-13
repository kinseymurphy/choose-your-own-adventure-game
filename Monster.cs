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

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("monster_attack_intro"),
            Name,
            player.Name,
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(Weapon.AsciiArt))
            output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("monster_missed_player"), Name, player.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("monster_hit_player"), Name, player.Name));

        int defenseRoll = _die.Roll(20);

        if (defenseRoll <= player.Agility)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_monster_attack"), player.Name));
            return string.Join(Environment.NewLine, output);
        }

        int damageRoll = RollDamage();
        int damage = player.ReduceDamage(damageRoll);
        player.HealthPoints -= damage;

        if (player.HealthPoints < 0)
            player.HealthPoints = 0;

        if (player.Protection > 0)
            output.Add(string.Format(messages.GetMessage("armor_absorbed_damage"), damageRoll - damage));

        output.Add(string.Format(messages.GetMessage("monster_damage_dealt"), Name, damage));
        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (player.HealthPoints <= 0)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        return string.Join(Environment.NewLine, output);
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
