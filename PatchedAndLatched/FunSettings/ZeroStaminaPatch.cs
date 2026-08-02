using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PlayerMovement))]
    internal static class ZeroStaminaPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("StaminaUpdate")]
        private static bool PrefixStaminaUpdate(PlayerMovement __instance, float unmodifiedSpeed)
        {
            if (!PatchedAndLatchedPlugin.ZeroStaminaEnabled.Value) return true;

            __instance.stamina = 0f;
            var hud = Singleton<CoreGameManager>.Instance?.GetHud(__instance.pm.playerNumber);
            if (hud != null)
                hud.SetStaminaValue(0f);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("AddStamina")]
        private static bool PrefixAddStamina(PlayerMovement __instance, float value, bool limited)
        {
            if (!PatchedAndLatchedPlugin.ZeroStaminaEnabled.Value) return true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void PrefixStart(PlayerMovement __instance)
        {
            if (!PatchedAndLatchedPlugin.ZeroStaminaEnabled.Value) return;
            __instance.stamina = 0f;
        }
    }
}