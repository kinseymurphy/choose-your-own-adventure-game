[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(0, player.Protection);

        Assert.IsNull(player.Weapon);
    }

    [TestMethod]
    public void ReduceDamage_WithNoProtection_ReturnsDamageUnchanged()
    {
        Player player = new Player { Protection = 0 };

        Assert.AreEqual(7, player.ReduceDamage(7));
    }

    [TestMethod]
    public void ReduceDamage_WithDamageGreaterThanProtection_ReturnsReducedDamage()
    {
        Player player = new Player { Protection = 3 };

        Assert.AreEqual(4, player.ReduceDamage(7));
    }

    [TestMethod]
    public void ReduceDamage_WithDamageEqualToProtection_ReturnsZero()
    {
        Player player = new Player { Protection = 3 };

        Assert.AreEqual(0, player.ReduceDamage(3));
    }

    [TestMethod]
    public void ReduceDamage_WithDamageLessThanProtection_ReturnsZeroNotNegative()
    {
        Player player = new Player { Protection = 3 };

        Assert.AreEqual(0, player.ReduceDamage(1));
    }
}
