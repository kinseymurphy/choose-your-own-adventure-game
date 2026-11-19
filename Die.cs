using System;

// The dice seam (ADR 0004): the game rolls a random Die, tests script their rolls
public interface IDie
{
    // Returns a whole number from 1 to sides
    int Roll(int sides);
}

public class Die : IDie
{
    private readonly Random _random = new Random();

    public int Roll(int sides)
    {
        return _random.Next(1, sides + 1);
    }
}
