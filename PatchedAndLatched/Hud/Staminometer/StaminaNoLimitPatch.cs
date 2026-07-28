using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PlayerMovement))]
    internal static class StaminaNoLimitPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("StaminaUpdate")]
        private static void PostfixStaminaUpdate(PlayerMovement __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaNoLimit!.Value) return;

            if (__instance.stamina < __instance.StaminaMax)
            {
                __instance.stamina = __instance.StaminaMax;
            }

            Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).SetStaminaValue(__instance.stamina / __instance.StaminaMax);
        }

        [HarmonyPrefix]
        [HarmonyPatch("AddStamina")]
        private static bool PrefixAddStamina(PlayerMovement __instance, float value, bool limited)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaNoLimit!.Value) return true;

            __instance.stamina += value;
            return false;
        }
    }
}
