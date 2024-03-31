using Il2CppAssets.Scripts.Models.Towers;

namespace AllPowersInShop.Towers;

public class BananaFarmer : ModPowerTower
{
    public override string BaseTower => TowerType.BananaFarmer;
    public override int Cost => AllPowersInShopMod.BananaFarmerCost;
    protected override int Order => 5;
}