using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllPowersInShop.Towers
{
    internal class CashDrop : ModCashDropTowerPower
    {
        public override int Cost => AllPowersInShopMod.CashDropCost;

        protected override int Pierce => 1;
    }
}
