using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PlayerMovement))]
    internal static class StaminaNoLimitPatch
    {
        private static FieldInfo _runningField;

        static StaminaNoLimitPatch()
        {
            _runningField = AccessTools.Field(typeof(PlayerMovement), "running");
        }

        [HarmonyPrefix]
        [HarmonyPatch("StaminaUpdate")]
        private static bool PrefixStaminaUpdate(PlayerMovement __instance, float unmodifiedSpeed)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaNoLimit.Value) return true;

            var entity = __instance.Entity;
            bool running = false;
            if (_runningField != null)
                running = (bool)_runningField.GetValue(__instance);

            if (entity.InternalMovement.magnitude > 0f)
            {
                if (running && entity.RelativeToForcedVelocity.magnitude > __instance.walkSpeed * Time.deltaTime * __instance.pm.PlayerTimeScale * entity.ExternalActivity.Multiplier)
                {
                    __instance.stamina = Mathf.Max(__instance.stamina - __instance.staminaDrop * Time.deltaTime * __instance.pm.PlayerTimeScale, 0f);
                    if (__instance.stamina > 0f)
                    {
                        __instance.pm.RuleBreak("Running", 0.1f);
                    }
                }
            }
            else
            {
                __instance.stamina += __instance.staminaRise * Time.deltaTime * __instance.pm.PlayerTimeScale;
            }

            Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).SetStaminaValue(__instance.stamina / __instance.StaminaMax);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("AddStamina")]
        private static bool PrefixAddStamina(PlayerMovement __instance, float value, bool limited)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaNoLimit.Value) return true;

            __instance.stamina += value;
            return false; 
        }
    }
}
