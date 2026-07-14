using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Jumprope))]
    internal static class FasterJumpropePatch
    {
        private const float SPEED_MULTIPLIER = 1.5f;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void ModifySpeed(Jumprope __instance)
        {
            if (!PatchedAndLatchedPlugin.FasterJumpropeEnabled.Value) return;

            var ropeDelayField = Traverse.Create(__instance).Field("ropeDelay");
            var ropeTimeField = Traverse.Create(__instance).Field("ropeTime");

            if (ropeDelayField != null && ropeTimeField != null)
            {
                float currentDelay = ropeDelayField.GetValue<float>();
                float currentTime = ropeTimeField.GetValue<float>();
                ropeDelayField.SetValue(currentDelay / SPEED_MULTIPLIER);
                ropeTimeField.SetValue(currentTime / SPEED_MULTIPLIER);
            }
        }
    }
}
