using Il2CppAssets.Scripts.Models.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllPowersInShop.Towers
{
    internal class SuperMonkeyStorm : ModSuperMonkeyStormTowerPower
    {
        public override int Cost => AllPowersInShopMod.SuperMonkeyStormCost;

        protected override int Pierce => 9999999;
    }
}
