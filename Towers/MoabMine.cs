namespace AllPowersInShop.Towers;

public class MoabMine : ModTrackPower
{
    public override int Cost => AllPowersInShopMod.MoabMineCost;
    protected override int Pierce => AllPowersInShopMod.MoabMinePierce;
    protected override int Order => 2;
}