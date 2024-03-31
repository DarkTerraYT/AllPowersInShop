using Il2CppAssets.Scripts.Models.Towers;

namespace AllPowersInShop.Towers;

public class TechBot : ModPowerTower
{
    public override string BaseTower => TowerType.TechBot;
    public override int Cost => AllPowersInShopMod.TechBotCost;
    protected override int Order => 6;
}