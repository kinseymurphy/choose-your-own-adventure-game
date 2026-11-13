using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MerchantTests
{
    [TestMethod]
    public void BuyMagicalArmor_WithExactly8Gold_IsPurchased()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 8 };

        PurchaseResult result = merchant.BuyMagicalArmor(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(3, player.Protection);
    }

    [TestMethod]
    public void BuyMagicalArmor_WithMoreThan8Gold_KeepsLeftoverGold()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 15 };

        PurchaseResult result = merchant.BuyMagicalArmor(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(7, player.Gold);
        Assert.AreEqual(3, player.Protection);
    }

    [TestMethod]
    public void BuyMagicalArmor_WithLessThan8Gold_IsRefusedAndNothingChanges()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 7 };

        PurchaseResult result = merchant.BuyMagicalArmor(player);

        Assert.AreEqual(PurchaseResult.NotEnoughGold, result);
        Assert.AreEqual(7, player.Gold);
        Assert.AreEqual(0, player.Protection);
    }

    [TestMethod]
    public void BuyMagicalArmor_SecondTime_IsRefusedAsAlreadyOwned()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 20 };

        merchant.BuyMagicalArmor(player);
        PurchaseResult result = merchant.BuyMagicalArmor(player);

        Assert.AreEqual(PurchaseResult.AlreadyOwned, result);
        Assert.AreEqual(12, player.Gold);
        Assert.AreEqual(3, player.Protection);
    }

    [TestMethod]
    public void BuyMagicalArmor_AlreadyOwnedWithLessThan8Gold_ReturnsAlreadyOwned()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 2, Protection = 3 };

        PurchaseResult result = merchant.BuyMagicalArmor(player);

        Assert.AreEqual(PurchaseResult.AlreadyOwned, result);
        Assert.AreEqual(2, player.Gold);
    }
}
