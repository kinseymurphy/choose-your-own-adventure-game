public enum PurchaseResult
{
    Purchased,
    NotEnoughGold,
    AlreadyOwned,
    NoWeapon
}

public class Merchant
{
    public const int MagicalArmorPrice = 8;
    public const int MagicalArmorProtection = 3;
    public const int MagicalWeaponPrice = 8;
    public const int MagicalWeaponDamageBonus = 4;

    public void StartEncounter(Player player, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("merchant_greeting"), player.Gold));
            Console.WriteLine(string.Format(messages.GetMessage("merchant_menu"), MagicalArmorPrice, MagicalWeaponPrice));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "1")
            {
                PurchaseResult result = BuyMagicalArmor(player);

                if (result == PurchaseResult.Purchased)
                    Console.WriteLine(string.Format(messages.GetMessage("merchant_armor_purchased"), player.Protection));
                else
                    ShowRefusal(result, messages);
            }
            else if (input == "2")
            {
                PurchaseResult result = BuyMagicalWeapon(player);

                if (result == PurchaseResult.Purchased)
                    Console.WriteLine(string.Format(
                        messages.GetMessage("merchant_weapon_purchased"),
                        messages.TranslateWeaponForDisplay(player.Weapon!.Type),
                        player.Weapon.MaxDamage));
                else
                    ShowRefusal(result, messages);
            }
            else if (input == "3")
            {
                Console.WriteLine(messages.GetMessage("merchant_farewell"));
                return;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    private void ShowRefusal(PurchaseResult result, Messages messages)
    {
        if (result == PurchaseResult.NotEnoughGold)
            Console.WriteLine(messages.GetMessage("merchant_not_enough_gold"));
        else if (result == PurchaseResult.AlreadyOwned)
            Console.WriteLine(messages.GetMessage("merchant_already_owned"));
        else if (result == PurchaseResult.NoWeapon)
            Console.WriteLine(messages.GetMessage("invalid"));
    }

    public PurchaseResult BuyMagicalArmor(Player player)
    {
        if (player.Protection > 0)
            return PurchaseResult.AlreadyOwned;

        if (player.Gold < MagicalArmorPrice)
            return PurchaseResult.NotEnoughGold;

        player.Gold -= MagicalArmorPrice;
        player.Protection = MagicalArmorProtection;
        return PurchaseResult.Purchased;
    }

    public PurchaseResult BuyMagicalWeapon(Player player)
    {
        if (player.Weapon == null)
            return PurchaseResult.NoWeapon;

        if (player.Weapon.IsMagical)
            return PurchaseResult.AlreadyOwned;

        if (player.Gold < MagicalWeaponPrice)
            return PurchaseResult.NotEnoughGold;

        player.Gold -= MagicalWeaponPrice;
        player.Weapon.IsMagical = true;
        player.Weapon.Type = "magical " + player.Weapon.Type;
        player.Weapon.MaxDamage += MagicalWeaponDamageBonus;
        return PurchaseResult.Purchased;
    }
}
