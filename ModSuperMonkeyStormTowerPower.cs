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

namespace AllPowersInShop;

public abstract class ModSuperMonkeyStormTowerPower : ModPowerTower
{
    protected abstract int Pierce { get; }

    public override string BaseTower => TowerType.PortableLake;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        var powerModel = Game.instance.model.GetPowerWithName(Name);

        towerModel.footprint = new CircleFootprintModel("", 0, true, false, true);
        towerModel.radiusSquared = 9;
        towerModel.radius = 3;
        towerModel.range = 0;
        towerModel.showPowerTowerBuffs = false;

        towerModel.AddBehavior<CreateEffectOnPlaceModel>(new("CreateEffectOnExpireModel", powerModel.GetBehavior<CreateEffectOnPowerModel>().effectModel));

        var assetId = powerModel.GetBehavior<CreateSoundOnPowerModel>().sound.assetId;
        var createSoundOnTowerPlaceModel = towerModel.GetBehavior<CreateSoundOnTowerPlaceModel>();
        createSoundOnTowerPlaceModel.sound1.assetId = assetId;
        createSoundOnTowerPlaceModel.sound2.assetId = assetId;
        createSoundOnTowerPlaceModel.heroSound1 = new SoundModel("", CreateAudioSourceReference(""));
        createSoundOnTowerPlaceModel.heroSound2 = new SoundModel("", CreateAudioSourceReference(""));

        towerModel.display = towerModel.GetBehavior<DisplayModel>().display = CreatePrefabReference("");

        var projectile = Game.instance.model.GetTower("DartMonkey").GetWeapon().projectile.Duplicate();

        projectile.GetBehavior<TravelStraitModel>().lifespan = 1f;
        projectile.GetBehavior<TravelStraitModel>().speed = 0;
        projectile.radius = 99999;
        projectile.pierce = 9999999999999999999;
        projectile.GetDamageModel().damage = 2000;
        projectile.display = new("");

        towerModel.AddBehavior(new CreateProjectileOnTowerDestroyModel("", projectile,
            new SingleEmissionModel("", null), true, false));

        towerModel.RemoveBehaviors<SlowBloonsZoneModel>();
        towerModel.RemoveBehaviors<SavedSubTowerModel>();
    }

    internal static void OnUpdate()
    {
        if (InGame.instance == null) return;

        var inputManager = InGame.instance.inputManager;
        var towerModel = inputManager?.towerModel;

        if (towerModel != null && inputManager!.inPlacementMode && towerModel.GetModTower() is ModSuperMonkeyStormTowerPower)
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
            if (forTowerModel.GetModTower() is ModSuperMonkeyStormTowerPower trackPower)
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
            if (tm.GetModTower() is ModSuperMonkeyStormTowerPower)
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
            if (__instance.towerModel?.GetModTower() is ModSuperMonkeyStormTowerPower trackPower &&
                (!InGame.instance.IsCoop || __instance.owner == Game.instance.GetNkGI().PeerID) &&
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                ShopMenu.instance.GetTowerButtonFromBaseId(trackPower.Id).gameObject
                    .GetComponent<TowerPurchaseButton>()
                    .ButtonActivated();
            }
        }
    }
    
    [HarmonyPatch(typeof(Tower), nameof(Tower.OnPlace))]
    public class Tower_OnPlace
    {
        [HarmonyPrefix]
        public static void Prefix(Tower __instance)
        {
            if(__instance.towerModel?.GetModTower() is ModSuperMonkeyStormTowerPower trackPower)
            {
                __instance.worth = 0;
                __instance.SellTower();
            }
        }
    }
}