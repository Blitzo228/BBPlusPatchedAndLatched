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
            if (__instance.stamina < __instance.StaminaMax)
            {
                __instance.stamina = __instance.StaminaMax;
            }

            Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).SetStaminaValue(__instance.stamina / __instance.StaminaMax);
        }
    }
}
