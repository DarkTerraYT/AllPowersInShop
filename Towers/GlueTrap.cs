namespace AllPowersInShop.Towers;

public class GlueTrap : ModTrackPower
{
    public override int Cost => AllPowersInShopMod.GlueTrapCost;
    protected override int Pierce => AllPowersInShopMod.GlueTrapPierce;
    protected override int Order => 3;
}