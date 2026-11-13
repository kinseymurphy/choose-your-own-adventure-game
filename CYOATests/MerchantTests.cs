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

    [TestMethod]
    public void BuyMagicalWeapon_LongSwordWithExactly8Gold_BecomesMagicalLongSword()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 8, Weapon = new Weapon("long sword", 12, "-)=====>") };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual("magical long sword", player.Weapon.Type);
        Assert.AreEqual(16, player.Weapon.MaxDamage);
        Assert.AreEqual("-)=====>", player.Weapon.AsciiArt);
        Assert.IsTrue(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void BuyMagicalWeapon_LightningBoltSpell_BecomesMagicalLightningBoltSpell()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 8, Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~") };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual("magical lightning bolt spell", player.Weapon.Type);
        Assert.AreEqual(16, player.Weapon.MaxDamage);
        Assert.AreEqual("zap~~~~~~", player.Weapon.AsciiArt);
        Assert.IsTrue(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void BuyMagicalWeapon_Dagger_BecomesMagicalDagger()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 8, Weapon = new Weapon("dagger", 6, "-)==>") };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual("magical dagger", player.Weapon.Type);
        Assert.AreEqual(10, player.Weapon.MaxDamage);
        Assert.AreEqual("-)==>", player.Weapon.AsciiArt);
        Assert.IsTrue(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void BuyMagicalWeapon_LongBow_BecomesMagicalLongBow()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 8, Weapon = new Weapon("long bow", 8, "}    -->") };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual("magical long bow", player.Weapon.Type);
        Assert.AreEqual(12, player.Weapon.MaxDamage);
        Assert.AreEqual("}    -->", player.Weapon.AsciiArt);
        Assert.IsTrue(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void BuyMagicalWeapon_WithMoreThan8Gold_KeepsLeftoverGold()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 13, Weapon = new Weapon("dagger", 6, "-)==>") };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.Purchased, result);
        Assert.AreEqual(5, player.Gold);
    }

    [TestMethod]
    public void BuyMagicalArmorThenMagicalWeapon_With16Gold_LeavesNoGoldAndBothItems()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 16, Weapon = new Weapon("long sword", 12, "-)=====>") };

        PurchaseResult armorResult = merchant.BuyMagicalArmor(player);
        PurchaseResult weaponResult = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.Purchased, armorResult);
        Assert.AreEqual(PurchaseResult.Purchased, weaponResult);
        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(3, player.Protection);
        Assert.IsTrue(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void BuyMagicalWeapon_WithLessThan8Gold_IsRefusedAndNothingChanges()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 7, Weapon = new Weapon("dagger", 6, "-)==>") };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.NotEnoughGold, result);
        Assert.AreEqual(7, player.Gold);
        Assert.AreEqual("dagger", player.Weapon.Type);
        Assert.AreEqual(6, player.Weapon.MaxDamage);
        Assert.IsFalse(player.Weapon.IsMagical);
    }

    [TestMethod]
    public void BuyMagicalWeapon_SecondTime_IsRefusedAsAlreadyOwned()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 20, Weapon = new Weapon("long bow", 8, "}    -->") };

        merchant.BuyMagicalWeapon(player);
        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.AlreadyOwned, result);
        Assert.AreEqual(12, player.Gold);
        Assert.AreEqual("magical long bow", player.Weapon.Type);
        Assert.AreEqual(12, player.Weapon.MaxDamage);
    }

    [TestMethod]
    public void BuyMagicalWeapon_AlreadyOwnedWithLessThan8Gold_ReturnsAlreadyOwned()
    {
        Merchant merchant = new Merchant();
        Weapon weapon = new Weapon("magical dagger", 10, "-)==>") { IsMagical = true };
        Player player = new Player { Gold = 2, Weapon = weapon };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.AlreadyOwned, result);
        Assert.AreEqual(2, player.Gold);
    }

    [TestMethod]
    public void BuyMagicalWeapon_WithNoWeapon_IsRefusedAndGoldUnchanged()
    {
        Merchant merchant = new Merchant();
        Player player = new Player { Gold = 15 };

        PurchaseResult result = merchant.BuyMagicalWeapon(player);

        Assert.AreEqual(PurchaseResult.NoWeapon, result);
        Assert.AreEqual(15, player.Gold);
        Assert.IsNull(player.Weapon);
    }
}
