using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using MonoMod.Backports;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;

namespace BeehiveUtilities
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [HarmonyPatch]
    class BeehiveUtilities : BaseUnityPlugin
    {
        public const string PluginGUID = "smallo.mods.beehiveutilities";
        public const string PluginName = "BeehiveUtilities";
        public const string PluginVersion = "1.1.1";

        private static ConfigEntry<bool> EnableMod;
        private static ConfigEntry<bool> ProximityCheck;
        private static ConfigEntry<bool> ShowHoneyAmount;
        private static ConfigEntry<bool> BeeStatus;
        private static ConfigEntry<bool> BiomeCheck;
        private static ConfigEntry<bool> NightCheck;
        private static ConfigEntry<bool> HoneySpawn;
        private static ConfigEntry<int> MaxHoney;
        private static ConfigEntry<double> MinsCreationTime;

        private static int creationTime = 0;

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html

        void Awake()
        {
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it

            EnableMod = Config.Bind("Mod Configuration", "Enables Mod", true, "Enables/Disables The Entire Mod");

            if (!EnableMod.Value)
            {
                return;
            }

            ProximityCheck = Config.Bind("Hive Configurations", "Disable Space Check", true, "Disables the \"Bees Need More Space\" check");
            BiomeCheck = Config.Bind("Hive Configurations", "Remove Biome Check", false, "Allows Beehives To Work In Any Biome");
            NightCheck = Config.Bind("Hive Configurations", "Remove Night Check/Rain Check", false, "Allows Beehives To Work At Night And In Rain/Storm");
            ShowHoneyAmount = Config.Bind("Hive Configurations", "Show Honey Amount", true, "Show Honey Amount Next To Beehive Name \"Honey (0/4)\" Instead Of (EMPTY) Or \"(Honey x 4)\"");
            BeeStatus = Config.Bind("Hive Configuration", "Display Bee Status On Hover", true, "Show The Bee Status On Hover Instead Of Having To Press E On The Hive, This Also Shows When There Is Honey In The Hive");
            MaxHoney = Config.Bind("Honey Configurations", "Max Honey", 4, "The maximum amount of honey a beehive can generate");
            MinsCreationTime = Config.Bind("Honey Configurations", "Minutes Per Creation", 20.0, "The Minutes It Takes A Hive To Generate 1 Honey");
            HoneySpawn = Config.Bind("Honey Configurations", "Spawn Honey In Front", false, "Spawns The Honey In Front Of The Hive, Instead Of On Top");

            creationTime = (int)(MinsCreationTime.Value * 60);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private static string LocaliseString(string text) { return Localization.instance.Localize(text); }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Beehive), "Awake")]
        public static void BeehiveAwake_Patch(Beehive __instance)
        {
            if (HoneySpawn.Value)
            {
                __instance.m_spawnPoint.localPosition = new Vector3(0.8f, 0f, 0f);
            }
            if (ProximityCheck.Value)
            {
                __instance.m_maxCover = 1000f;
            }
            else __instance.m_maxCover = 0.25f;
            if (MaxHoney.Value != __instance.m_maxHoney)
            {
                __instance.m_maxHoney = MaxHoney.Value;
            }
            else __instance.m_maxHoney = 4;
            if (creationTime != __instance.m_secPerUnit)
            {
                __instance.m_secPerUnit = creationTime;
            }
            else __instance.m_secPerUnit = 20 * 60;
            if (BiomeCheck.Value)
            {
                __instance.m_biome = Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Swamp | Heightmap.Biome.Mountain | Heightmap.Biome.Plains | Heightmap.Biome.Mistlands | Heightmap.Biome.Ocean | Heightmap.Biome.DeepNorth | Heightmap.Biome.AshLands;
            }
            else __instance.m_biome = Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Plains;
            if (NightCheck.Value)
            {
                __instance.m_effectOnlyInDaylight = false;
            }
            else __instance.m_effectOnlyInDaylight = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Beehive), "Interact")]
        public static void BeehiveInteract_Patch(Humanoid character, bool repeat, Beehive __instance)
        {
            if (BeeStatus.Value)
            {
                character.Message(MessageHud.MessageType.Center, "");
                return;
            }

            if (EnvMan.instance.IsDaylight() && !BeeStatus.Value)
            {
                character.Message(MessageHud.MessageType.Center, ProximityCheck.Value ? $"{__instance.m_happyText}" : $"{__instance.m_freespaceText}");
                return;
            }

            if (!NightCheck.Value && !EnvMan.instance.IsDaylight())
            {
                character.Message(MessageHud.MessageType.Center, __instance.m_sleepText);
                return;
            }

            if (NightCheck.Value)
            {
                if (BiomeCheck.Value && !BeeStatus.Value)
                {
                    character.Message(MessageHud.MessageType.Center, ProximityCheck.Value ? $"{__instance.m_happyText}" : $"{__instance.m_freespaceText}");
                    return;
                }
                else if (!BiomeCheck.Value && !BeeStatus.Value && ShowHoneyAmount.Value)
                {
                    character.Message(MessageHud.MessageType.Center, __instance.m_areaText);
                    return;
                }
            }

            if (!EnvMan.instance.IsDaylight() && (BeeStatus.Value || ShowHoneyAmount.Value))
            {
                character.Message(MessageHud.MessageType.Center, "");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Beehive), nameof(Beehive.GetHoverText))]
        public static string BeehiveGetHoverText_Patch(string __result, Beehive __instance)
        {
            string result = __result;

            if (!ShowHoneyAmount.Value && !BeeStatus.Value)
            {
                return result;
            }

            string statusToReplace = $"\n[<color=#F0FF33><b>$KEY_Use</b></color>] $piece_beehive_check";
            string honeyString = LocaliseString(__instance.m_honeyItem.m_itemData.m_shared.m_name);
            string empty = LocaliseString($"( $piece_container_empty )");
            string honeyCount = LocaliseString($"\n{honeyString} ( {__instance.GetHoneyLevel()} / {__instance.m_maxHoney} )");
            string hasHoney = LocaliseString($"( {honeyString} x {__instance.GetHoneyLevel()} )");

            if (BeeStatus.Value)
            {
                string status;

                if (!__instance.CheckBiome()) status = $"<color=#FF0000>$piece_beehive_area</color>";
                else if (!__instance.HaveFreeSpace()) status = $"<color=#FF0000>$piece_beehive_freespace</color>";
                else if (!EnvMan.instance.IsDaylight() && !NightCheck.Value) status = $"<color=#F0FF33>$piece_beehive_sleep</color>";
                else status = $"<color=#00FF00>$piece_beehive_happy</color>";

                result = result.Replace(LocaliseString(statusToReplace), "");
                result = result.Replace(LocaliseString($"{__instance.m_name}"), LocaliseString($"{__instance.m_name}\n{LocaliseString(status)}"));
            }

            if (result.Contains(empty))
            {
                if (ShowHoneyAmount.Value)
                {
                    return result.Replace(empty, honeyCount);
                }
            }

            if (result.Contains(hasHoney))
            {
                if (ShowHoneyAmount.Value)
                {
                    return result.Replace(hasHoney, honeyCount);
                }
            }

            return result;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Beehive), "UpdateBees")]
        public static void BeehiveUpdateBees_Patch(Beehive __instance)
        {
            if (!NightCheck.Value)
            {
                return;
            }

            bool flag = __instance.CheckBiome() && __instance.HaveFreeSpace();
            __instance.m_beeEffect.SetActive(flag);
            if (!__instance.m_nview.IsOwner() || !flag)
            {
                return;
            }

            float timeSinceLastUpdate = __instance.GetTimeSinceLastUpdate();
            float num = __instance.m_nview.GetZDO().GetFloat("product") + timeSinceLastUpdate;
            if (num > __instance.m_secPerUnit)
            {
                __instance.IncreseLevel((int)(num / __instance.m_secPerUnit));
                num = 0.0f;
            }
            __instance.m_nview.GetZDO().Set("product", num);
        }
    }
}
