// CYOATests/ScriptedDieTests.cs
[TestClass]
public class ScriptedDieTests
{
    [TestMethod]
    public void Roll_ReturnsTheScriptedRollsInOrder()
    {
        IDie die = new ScriptedDie(4, 20, 1);

        Assert.AreEqual(4, die.Roll(20));
        Assert.AreEqual(20, die.Roll(20));
        Assert.AreEqual(1, die.Roll(6));
    }

    [TestMethod]
    public void Roll_WhenTheScriptRunsOut_FailsTheTest()
    {
        IDie die = new ScriptedDie(3);
        die.Roll(20);

        AssertFailedException failure = Assert.ThrowsException<AssertFailedException>(() => die.Roll(20));
        StringAssert.Contains(failure.Message, "ran out");
    }

    [TestMethod]
    [DataRow(0, 20)]
    [DataRow(7, 6)]
    public void Roll_OutsideOneToSides_FailsTheTestNamingTheRollAndSides(int roll, int sides)
    {
        IDie die = new ScriptedDie(roll);

        AssertFailedException failure = Assert.ThrowsException<AssertFailedException>(() => die.Roll(sides));
        StringAssert.Contains(failure.Message, $"{roll}");
        StringAssert.Contains(failure.Message, $"d{sides}");
    }
}
