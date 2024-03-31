using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Powers.Effects;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Track;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using UnityEngine;
using UnityEngine.UI;
using CreateEffectOnExpireModel = Il2CppAssets.Scripts.Models.Towers.Behaviors.CreateEffectOnExpireModel;
using Vector2 = Il2CppAssets.Scripts.Simulation.SMath.Vector2;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Powers;

namespace AllPowersInShop;

public abstract class ModBoostPower : ModPowerTower
{
    public override string BaseTower => TowerType.PortableLake;

    protected abstract string PowerName { get; }

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        var powerModel = Game.instance.model.GetPowerWithName(PowerName);

        towerModel.footprint = new CircleFootprintModel("", 0, true, false, true);
        towerModel.radiusSquared = 0;
        towerModel.radius = 0;
        towerModel.range = 99999999;
        towerModel.showPowerTowerBuffs = false;

        towerModel.GetBehavior<CreateEffectOnPlaceModel>().effectModel = powerModel.GetBehavior<CreateEffectOnPowerModel>().effectModel;
        towerModel.GetBehavior<CreateEffectOnPlaceModel>().effectModel.lifespan = 15;

        var assetId = powerModel.GetBehavior<CreateSoundOnPowerModel>().sound.assetId;
        var createSoundOnTowerPlaceModel = towerModel.GetBehavior<CreateSoundOnTowerPlaceModel>();
        createSoundOnTowerPlaceModel.sound1.assetId = assetId;
        createSoundOnTowerPlaceModel.sound2.assetId = assetId;
        createSoundOnTowerPlaceModel.heroSound1 = new SoundModel("", CreateAudioSourceReference(""));
        createSoundOnTowerPlaceModel.heroSound2 = new SoundModel("", CreateAudioSourceReference(""));

        towerModel.display = towerModel.GetBehavior<DisplayModel>().display = CreatePrefabReference("");
        if (PowerName == "MonkeyBoost")
        {
            towerModel.AddBehavior<RateSupportModel>(new("RateAreaBuffModel_", powerModel.GetBehavior<MonkeyBoostModel>().rateScale, false, "MonkeyBoost", true, 99, null, "", ""));

            towerModel.AddBehavior<TowerExpireModel>(new("TowerExpireModel_", powerModel.GetBehavior<MonkeyBoostModel>().duration, 9999999, false, true));
        }
        else if (PowerName == "Thrive")
        {
            towerModel.AddBehavior<CashIncreaseModel>(new("CashIncreaseModel_", 0, powerModel.GetBehavior<ThriveModel>().cashScale));

            towerModel.AddBehavior<TowerExpireModel>(new("TowerExpireModel_", 9999999999999999999, (int)powerModel.GetBehavior<CreateEffectOnPowerModel>().effectModel.lifespan, true, false));
        }

        towerModel.RemoveBehaviors<SlowBloonsZoneModel>();
        towerModel.RemoveBehaviors<SavedSubTowerModel>();
    }

    internal static void OnUpdate()
    {
        if (InGame.instance == null) return;

        var inputManager = InGame.instance.inputManager;
        var towerModel = inputManager?.towerModel;

        if (towerModel != null && inputManager!.inPlacementMode && towerModel.GetModTower() is ModBoostPower)
        {
            var map = InGame.instance.UnityToSimulation.simulation.Map;
            InGameObjects.instance.IconUpdate(InputManager.GetCursorPosition(),
                map.CanPlace(new Vector2(inputManager.cursorPositionWorld), towerModel));
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.EnterPlacementMode), typeof(TowerModel),
        typeof(InputManager.PositionDelegate), typeof(ObjectId), typeof(bool))]
    internal class InputManager_EnterPlacementMode
    {
        [HarmonyPostfix]
        internal static void Postfix(TowerModel forTowerModel)
        {
            if (forTowerModel.GetModTower() is ModBoostPower trackPower)
            {
                var image = ShopMenu.instance
                    .GetTowerButtonFromBaseId(trackPower.Id)
                    .gameObject.transform
                    .Find("Icon").GetComponent<Image>();

                InGameObjects.instance.IconUpdate(new UnityEngine.Vector2(-3000, 0), false);
                InGameObjects.instance.IconStart(image.sprite);
            }
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.ExitPlacementMode))]
    internal class InputManager_ExitPlacementMode
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            if (InGameObjects.instance.powerIcon != null)
            {
                InGameObjects.instance.IconEnd();
            }
        }
    }

    [HarmonyPatch(typeof(Map), nameof(Map.CanPlace))]
    internal class Map_CanPlace
    {
        [HarmonyPostfix]
        internal static void Patch(Map __instance, ref bool __result, Vector2 at, TowerModel tm)
        {
            if (tm.GetModTower() is ModBoostPower)
            {
                var map = InGame.instance.UnityToSimulation.simulation.Map;
                __result = map.GetAreaAtPoint(at)?.areaModel?.type == AreaType.land || map.GetAreaAtPoint(at)?.areaModel?.type == AreaType.track || map.GetAreaAtPoint(at)?.areaModel?.type == AreaType.water || map.GetAreaAtPoint(at)?.areaModel?.type == AreaType.ice || map.GetAreaAtPoint(at)?.areaModel?.type == AreaType.unplaceable;
            }
        }
    }

    [HarmonyPatch(typeof(Tower), nameof(Tower.OnDestroy))]
    public class Tower_OnDestroy
    {
        [HarmonyPrefix]
        public static void Prefix(Tower __instance)
        {
            if (__instance.towerModel?.GetModTower() is ModBoostPower trackPower &&
                (!InGame.instance.IsCoop || __instance.owner == Game.instance.GetNkGI().PeerID) &&
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                ShopMenu.instance.GetTowerButtonFromBaseId(trackPower.Id).gameObject
                    .GetComponent<TowerPurchaseButton>()
                    .ButtonActivated();
            }
        }
    }
}