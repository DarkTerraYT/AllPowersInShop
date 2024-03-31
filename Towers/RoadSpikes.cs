namespace AllPowersInShop.Towers;

public class RoadSpikes : ModTrackPower
{
    public override int Cost => AllPowersInShopMod.RoadSpikesCost;
    protected override int Pierce => AllPowersInShopMod.RoadSpikesPierce;
    protected override int Order => 1;
}