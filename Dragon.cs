public class Dragon : Monster
{
    public override string PlayerAttackIntroKey => "player_attack_intro";
    public override string StatsHeaderKey => "dragon_stats_header";
    public override string DefeatedNarrativeKey => "dragon_defeated_narrative";
    public override string VictoryKey => "victory";
    public override string DefeatKey => "defeat";

    // Combat picks a taunt with a roll of the die; {0} is the player's race
    public override IReadOnlyList<string> DefenseTauntKeys { get; } = new List<string>
{
    "dragon_defense_taunt_1",
    "dragon_defense_taunt_2",
    "dragon_defense_taunt_3",
    "dragon_defense_taunt_4"
};

    public override IReadOnlyList<string> DamageReplyKeys { get; } = new List<string>
{
    "dragon_damage_reply_1",
    "dragon_damage_reply_2",
    "dragon_damage_reply_3",
    "dragon_damage_reply_4"
};

    // The dragon uses a placeholder weapon object for now.
    // This can be refined later if combat requirements expand.
    public Dragon(IDie die) : base(die, "Smolderfang", new Weapon("claws", 12, "<<< claws >>>"))
    {
        // Dragon stats are initialized using Die so the class aligns better
        // with the sequence/start-game diagrams and shared repo structure.
        Strength = _die.Roll(20);
        Agility = _die.Roll(20);
        HealthPoints = _die.Roll(20);
    }
}
