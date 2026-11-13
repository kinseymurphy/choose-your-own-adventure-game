public class Goblin : Monster
{
    public override string PlayerAttackIntroKey => "goblin_player_attack_intro";
    public override string StatsHeaderKey => "goblin_stats_header";
    public override string DefeatedNarrativeKey => "goblin_defeated_narrative";
    public override string VictoryKey => "goblin_victory";
    public override string DefeatKey => "goblin_defeat";

    protected override IReadOnlyList<string> DefenseTauntKeys { get; } = new List<string>
{
    "goblin_defense_taunt_1",
    "goblin_defense_taunt_2",
    "goblin_defense_taunt_3",
    "goblin_defense_taunt_4"
};

    protected override IReadOnlyList<string> DamageReplyKeys { get; } = new List<string>
{
    "goblin_damage_reply_1",
    "goblin_damage_reply_2",
    "goblin_damage_reply_3",
    "goblin_damage_reply_4"
};

    // The goblin is weaker than the dragon: d12 stats and a small axe.
    public Goblin(Die die) : base(die, "Grubnik", new Weapon("rusty axe", 6, "  /|\n | |==========\n  \\|"))
    {
        Strength = _die.Roll(12);
        Agility = _die.Roll(12);
        HealthPoints = _die.Roll(12);
    }
}
