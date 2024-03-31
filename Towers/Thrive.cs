using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllPowersInShop.Towers
{
    internal class Thrive : ModBoostPower
    {
        public override int Cost => AllPowersInShopMod.ThriveCost;

        protected override string PowerName => Name;
    }
}
