namespace AllPowersInShop.Towers;

public class CamoTrap : ModTrackPower
{
    public override int Cost => AllPowersInShopMod.CamoTrapCost;
    protected override int Pierce => AllPowersInShopMod.CamoTrapPierce;
    protected override int Order => 4;
}