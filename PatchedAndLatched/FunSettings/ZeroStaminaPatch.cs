using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PlayerMovement))]
    internal static class ZeroStaminaPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("StaminaUpdate")]
        private static bool PrefixStaminaUpdate(PlayerMovement __instance, float unmodifiedSpeed)
        {
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
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void PrefixStart(PlayerMovement __instance)
        {
            __instance.stamina = 0f;
        }
    }
}
