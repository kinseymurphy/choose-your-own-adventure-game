// CYOATests/ScriptedDie.cs
// Returns the listed rolls in order, so a test decides every roll (ADR 0004).
// Fails the test if it runs out or a roll is impossible for the die being rolled.
public class ScriptedDie : IDie
{
    private readonly Queue<int> _rolls;

    public ScriptedDie(params int[] rolls)
    {
        _rolls = new Queue<int>(rolls);
    }

    public int Roll(int sides)
    {
        if (_rolls.Count == 0)
            Assert.Fail($"The scripted die ran out of rolls (asked for a d{sides}).");

        int roll = _rolls.Dequeue();

        if (roll < 1 || roll > sides)
            Assert.Fail($"Scripted roll {roll} is impossible on a d{sides}.");

        return roll;
    }
}
