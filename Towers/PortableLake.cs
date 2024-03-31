using Il2CppAssets.Scripts.Models.Towers;

namespace AllPowersInShop.Towers;

public class PortableLake : ModPowerTower
{
    public override string BaseTower => TowerType.PortableLake;
    public override int Cost => AllPowersInShopMod.PortableLakeCost;
    protected override int Order => 9;
}