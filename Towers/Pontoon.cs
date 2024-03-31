using Il2CppAssets.Scripts.Models.Towers;

namespace AllPowersInShop.Towers;

public class Pontoon : ModPowerTower
{
    public override string BaseTower => TowerType.Pontoon;
    public override int Cost => AllPowersInShopMod.PontoonCost;
    protected override int Order => 8;
}