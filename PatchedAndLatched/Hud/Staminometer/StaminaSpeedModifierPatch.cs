using HarmonyLib;
using UnityEngine;
using PatchedAndLatched;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PlayerMovement))]
    public static class StaminaSpeedModifierPatch
    {
        private static float _originalWalkSpeed;
        private static float _originalRunSpeed;
        private static float _originalStaminaDrop;
        private static bool _initialized = false;

        private const float MIN_SPEED = 0.75f;
        private const float NORMAL_SPEED = 1.0f;
        private const float MAX_SPEED = 1.15f;

        private const float STAMINA_DROP_MULTIPLIER = 1.25f; 

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void Start_Postfix(PlayerMovement __instance)
        {
            if (!PatchedAndLatchedPlugin.StaminaSpeedModifier.Value) return;
            if (_initialized) return;

            _originalWalkSpeed = __instance.walkSpeed;
            _originalRunSpeed = __instance.runSpeed;
            _originalStaminaDrop = __instance.staminaDrop;
            _initialized = true;

            __instance.staminaDrop = _originalStaminaDrop * STAMINA_DROP_MULTIPLIER;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void Update_Prefix(PlayerMovement __instance)
        {
            if (!PatchedAndLatchedPlugin.StaminaSpeedModifier.Value) return;
            if (!_initialized) return;

            float staminaRatio = __instance.stamina / __instance.StaminaMax;
            float speedMultiplier;

            if (staminaRatio >= 0.5f)
            {
                float t = (staminaRatio - 0.5f) / 0.5f;
                speedMultiplier = Mathf.Lerp(NORMAL_SPEED, MAX_SPEED, t);
            }
            else
            {
                float t = staminaRatio / 0.5f;
                speedMultiplier = Mathf.Lerp(MIN_SPEED, NORMAL_SPEED, t);
            }

            __instance.walkSpeed = _originalWalkSpeed * speedMultiplier;
            __instance.runSpeed = _originalRunSpeed * speedMultiplier;

        }
    }
}
