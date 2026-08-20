using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    public static class LightGenerationPatch
    {
        [HarmonyPatch(nameof(EnvironmentController.InitializeLighting))]
        [HarmonyPrefix]
        private static void ChangeAmbientDarkness(EnvironmentController __instance)
        {
            float darkness = PatchedAndLatchedPlugin.AmbientDarknessLevel!.Value;
            __instance.standardDarkLevel = new Color(darkness, darkness, darkness, 1f);
        }
        [HarmonyPatch(nameof(EnvironmentController.GenerateLight), typeof(Cell), typeof(Color), typeof(int), typeof(bool))]
        [HarmonyPrefix]
        private static void ModifyLightStrength(ref int strength)
        {
            float multiplier = PatchedAndLatchedPlugin.CustomLightRadiusMultiplier!.Value;

            strength = Mathf.Max(1, Mathf.RoundToInt(strength * multiplier));
        }
    }
}
