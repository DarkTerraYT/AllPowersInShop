using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllPowersInShop.Towers
{
    internal class MonkeyBoost : ModBoostPower
    {
        public override int Cost => AllPowersInShopMod.MonkeyBoostCost;

        protected override string PowerName => Name;
    }
}
