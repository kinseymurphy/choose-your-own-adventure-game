public class Combat
{
    public Player player { get; set; }
    public Monster monster { get; set; }
    public Messages messages { get; set; }

    public bool PlayerRetreated { get; private set; }

    // Every combat roll will go through this die (ADR 0004)
    private readonly IDie _die;

    public Combat(Player player, Monster monster, Messages messages, IDie die)
    {
        this.player = player;
        this.monster = monster;
        this.messages = messages;
        _die = die;
        PlayerRetreated = false;
    }

    public bool StartCombat()
    {
        Console.WriteLine(GetCombatStatsDisplay());

        while (true)
        {
            PlayerAttacksMonsterSequence();

            if (monster.HealthPoints <= 0)
                return true;

            MonsterAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            Console.WriteLine(messages.GetMessage("attack_prompt"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "r" || input == "retreat")
            {
                PlayerRetreated = true;
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }

            if (input == "a" || input == "attack")
                continue;

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    public string GetCombatStatsDisplay()
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.AddRange(player.GetCombatStatLines(messages));

        output.Add("");
        output.Add(string.Format(messages.GetMessage("combat_monster_header"), monster.Name));
        output.Add(messages.GetMessage("stats_name") + monster.Name);
        output.Add(messages.GetMessage("stats_strength") + monster.Strength);
        output.Add(messages.GetMessage("stats_agility") + monster.Agility);
        output.Add(messages.GetMessage("stats_health") + monster.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(monster.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + monster.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }

    public void PlayerAttacksMonsterSequence()
    {
        if (player.Weapon == null)
        {
            Console.WriteLine("You have no weapon to attack with.");
            return;
        }

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage(monster.PlayerAttackIntroKey),
            messages.TranslateOccupationForDisplay(player.Occupation),
            messages.TranslateWeaponForDisplay(player.Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(player.Weapon.AsciiArt))
            output.Add(player.Weapon.AsciiArt);

        // Monsters have no Protection
        AttackResult attack = ResolveAttack(player.Strength, monster.Agility, player.Weapon.MaxDamage, 0);

        if (!attack.Hit)
        {
            output.Add(string.Format(messages.GetMessage("player_missed_monster"), monster.Name));
            Console.WriteLine(string.Join(Environment.NewLine, output));
            return;
        }

        output.Add(string.Format(messages.GetMessage("player_hit_monster"), monster.Name));

        if (attack.Blocked)
        {
            output.Add(string.Format(messages.GetMessage("monster_defended_player_attack"), monster.Name));
            output.Add(string.Format(
                messages.GetMessage(PickKey(monster.DefenseTauntKeys)),
                messages.TranslateRaceForDisplay(player.Race)
            ));
            Console.WriteLine(string.Join(Environment.NewLine, output));
            return;
        }

        // The Monster's Health Points stop at 0, like the player's
        monster.HealthPoints = Math.Max(0, monster.HealthPoints - attack.Damage);

        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), attack.Damage));
        output.Add(string.Format(messages.GetMessage("monster_health_now"), monster.Name, monster.HealthPoints));

        if (monster.HealthPoints <= 0)
            output.Add(messages.GetMessage(monster.DefeatedNarrativeKey));
        else
            output.Add(messages.GetMessage(PickKey(monster.DamageReplyKeys)));

        Console.WriteLine(string.Join(Environment.NewLine, output));
    }

    // Picks one of a Monster's taunts or damage replies with a roll of the die
    private string PickKey(IReadOnlyList<string> keys)
    {
        return keys[_die.Roll(keys.Count) - 1];
    }

    public void MonsterAttacksPlayerSequence()
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("monster_attack_intro"),
            monster.Name,
            player.Name,
            messages.TranslateWeaponForDisplay(monster.Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(monster.Weapon.AsciiArt))
            output.Add(monster.Weapon.AsciiArt);

        AttackResult attack = ResolveAttack(monster.Strength, player.Agility, monster.Weapon.MaxDamage, player.Protection);

        if (!attack.Hit)
        {
            output.Add(string.Format(messages.GetMessage("monster_missed_player"), monster.Name, player.Name));
            Console.WriteLine(string.Join(Environment.NewLine, output));
            return;
        }

        output.Add(string.Format(messages.GetMessage("monster_hit_player"), monster.Name, player.Name));

        if (attack.Blocked)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_monster_attack"), player.Name));
            Console.WriteLine(string.Join(Environment.NewLine, output));
            return;
        }

        // The player's Health Points stop at 0
        player.HealthPoints = Math.Max(0, player.HealthPoints - attack.Damage);

        if (player.Protection > 0)
            output.Add(string.Format(messages.GetMessage("armor_absorbed_damage"), attack.DamageRoll - attack.Damage));

        output.Add(string.Format(messages.GetMessage("monster_damage_dealt"), monster.Name, attack.Damage));
        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (player.HealthPoints <= 0)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        Console.WriteLine(string.Join(Environment.NewLine, output));
    }

    // What one attack did: missed, blocked, or hit for Damage (the DamageRoll less any Protection)
    private readonly record struct AttackResult(bool Hit, bool Blocked, int DamageRoll, int Damage);

    // The attack sequence both directions share (ADR 0004). The rolls are always in this order:
    // attack d20, then defense d20 only on a hit, then damage only when not blocked.
    private AttackResult ResolveAttack(int attackerStrength, int defenderAgility, int damageSides, int protection)
    {
        if (_die.Roll(20) > attackerStrength)
            return new AttackResult(false, false, 0, 0);

        if (_die.Roll(20) <= defenderAgility)
            return new AttackResult(true, true, 0, 0);

        int damageRoll = _die.Roll(damageSides);
        return new AttackResult(true, false, damageRoll, Math.Max(0, damageRoll - protection));
    }
}